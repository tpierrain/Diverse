using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diverse
{
    /// <summary>
    /// Fuzz instance of Types.
    /// Uses a <see cref="GenerationContext"/> to track the ancestry chain of types
    /// during recursive generation, preventing stack overflow and geometric explosion
    /// for self-referencing or mutually-referencing types.
    /// </summary>
    internal class TypeFuzzer : IFuzzTypes
    {
        private const int DefaultMaxCollectionSize = 5;
        private const int DefaultMaxDepthForRecursiveTypes = 2;

        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiates a <see cref="TypeFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public TypeFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Generates an instance of a type T.
        /// </summary>
        /// <returns>An instance of type T with some fuzzed properties.</returns>
        public T GenerateInstanceOf<T>()
        {
            var context = new GenerationContext(DefaultMaxDepthForRecursiveTypes, DefaultMaxCollectionSize);
            var result = GenerateInstance(typeof(T), context);

            if (result == null)
            {
                return default(T);
            }

            return (T)result;
        }

        /// <summary>
        /// Generates an instance of an <see cref="Enum"/> type.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Enum"/></typeparam>
        /// <returns>An random value of the specified <see cref="Enum"/> type.</returns>
        public T GenerateEnum<T>()
        {
            var enumValues = Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(_fuzzer.Random.Next(0, enumValues.Length));
        }

        /// <summary>
        /// Core generation method. Routes the type to the appropriate generation strategy:
        /// enums, collections, BCL primitives, or constructor-based generation with cycle detection.
        /// </summary>
        private object GenerateInstance(Type type, GenerationContext context)
        {
            // 1. Enums
            if (type.IsEnum)
            {
                return FuzzEnumValue(type);
            }

            // 2. Collections (IEnumerable<T>)
            if (type.IsEnumerable())
            {
                return GenerateListOf(type, context);
            }

            // 3. BCL types covered by dedicated fuzzers (int, string, DateTime, etc.)
            if (type.IsCoveredByAFuzzer())
            {
                return FuzzBclType(Type.GetTypeCode(type));
            }

            // 4. Complex/custom types: cycle detection to prevent stack overflow
            if (context.ShouldStopRecursingFor(type))
            {
                return null;
            }

            var constructor = type.GetConstructorWithBiggestNumberOfParameters();
            if (constructor == null)
            {
                // Interface, abstract class, or type with no accessible constructor
                return null;
            }

            context.PushType(type);
            try
            {
                if (constructor.IsEmpty())
                {
                    return InstantiateAndFuzzViaPropertiesWhenTheyHaveSetters(constructor, context, type);
                }

                try
                {
                    return InstantiateViaConstructor(constructor, context);
                }
                catch (Exception)
                {
                    return TryOtherConstructorsUntilOneWorks(type, context);
                }
            }
            finally
            {
                context.PopType();
            }
        }

        private object FuzzBclType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return _fuzzer.HeadsOrTails();

                case TypeCode.Int32:
                    return _fuzzer.GenerateInteger();

                case TypeCode.Int64:
                    return _fuzzer.GenerateLong();

                case TypeCode.Decimal:
                    return _fuzzer.GeneratePositiveDecimal();

                case TypeCode.String:
                    return _fuzzer.GenerateFirstName();

                case TypeCode.DateTime:
                    return _fuzzer.GenerateDateTime();

                default:
                    return null;
            }
        }

        private object InstantiateViaConstructor(ConstructorInfo constructor, GenerationContext context)
        {
            var parameters = PrepareFuzzedParametersForThisConstructor(constructor, context);
            return constructor.Invoke(parameters);
        }

        private object InstantiateAndFuzzViaPropertiesWhenTheyHaveSetters(ConstructorInfo constructor,
            GenerationContext context, Type type, object instance = null)
        {
            if (instance == null)
            {
                instance = constructor.Invoke(new object[0]);
            }

            var propertyInfos = type.GetProperties().Where(prop => prop.CanWrite);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyValue = GenerateInstance(propertyInfo.PropertyType, context);
                propertyInfo.SetValue(instance, propertyValue);
            }

            return instance;
        }

        private object[] PrepareFuzzedParametersForThisConstructor(ConstructorInfo constructor, GenerationContext context)
        {
            var parameters = new List<object>();
            var parameterInfos = constructor.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                parameters.Add(GenerateInstance(parameterInfo.ParameterType, context));
            }

            return parameters.ToArray();
        }

        private object TryOtherConstructorsUntilOneWorks(Type type, GenerationContext context)
        {
            // Some constructors are complicated to use (e.g. those accepting abstract classes as input)
            // Try other constructors until it works
            var constructors = type.GetConstructorsOrderedByNumberOfParametersDesc().Skip(1);
            foreach (var constructorInfo in constructors)
            {
                try
                {
                    return InstantiateViaConstructor(constructorInfo, context);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // We couldn't use any of its constructors. Return null (degraded mode).
            return null;
        }

        private IEnumerable GenerateListOf(Type type, GenerationContext context)
        {
            var typeGenericTypeArguments = type.GenericTypeArguments;
            var elementType = typeGenericTypeArguments.Single();

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(typeGenericTypeArguments);

            // Instantiate a List<elementType>
            var list = Activator.CreateInstance(constructedListType) as IList;

            // Use context-aware collection size (reduced for recursive types)
            var collectionSize = context.GetCollectionSizeFor(elementType);
            for (var i = 0; i < collectionSize; i++)
            {
                list.Add(GenerateInstance(elementType, context));
            }

            return list;
        }

        private object FuzzEnumValue(Type enumType)
        {
            var enumValues = Enum.GetValues(enumType);
            return enumValues.GetValue(_fuzzer.Random.Next(0, enumValues.Length));
        }
    }
}
