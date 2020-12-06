using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diverse
{
    /// <summary>
    /// Fuzz instance of Types.
    /// </summary>
    public class TypeFuzzer : IFuzzTypes
    {
        private const int MaxRecursionAllowedWhileFuzzing = 400;
        private const int MaxCountToFuzzInLists = 5;
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
            var instance = GenerateInstanceOf<T>(0);

            return instance;
        }

        private T GenerateInstanceOf<T>(int recursionLevel)
        {
            recursionLevel++;
            if (recursionLevel > MaxRecursionAllowedWhileFuzzing)
            {
                return default(T);
            }

            var type = typeof(T);

            var constructor = type.GetConstructorWithBiggestNumberOfParameters();
            if (constructor == null || type.IsCoveredByAFuzzer()) // the case with some BCL types
            {
                var instance = FuzzAnyDotNetType(Type.GetTypeCode(type), type, recursionLevel);
                return (T)instance;
            }

            if (constructor.IsEmpty())
            {
                var instance = InstantiateAndFuzzViaPropertiesWhenTheyHaveSetters<T>(constructor, recursionLevel, type);
                return instance;
            }
            else
            {
                try
                {
                    return InstantiateAndFuzzViaConstructorWithBiggestNumberOfParameters<T>(constructor, recursionLevel);
                }
                catch (Exception)
                {
                    return InstantiateAndFuzzViaOtherConstructorIteratingOnAllThemUntilItWorks<T>(recursionLevel, type);
                }
            }
        }

        private T InstantiateAndFuzzViaConstructorWithBiggestNumberOfParameters<T>(ConstructorInfo constructor, int recursionLevel)
        {
            var constructorParameters = PrepareFuzzedParametersForThisConstructor(constructor, recursionLevel);
            var instance = constructor.Invoke(constructorParameters);
            return (T)instance;
        }

        private T InstantiateAndFuzzViaPropertiesWhenTheyHaveSetters<T>(ConstructorInfo constructor, int recursionLevel,
            Type genericType, object instance = null)
        {
            if (instance == null)
            {
                instance = constructor.Invoke(new object[0]);
            }

            var propertyInfos = genericType.GetProperties().Where(prop => prop.CanWrite);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                var propertyValue = FuzzAnyDotNetType(Type.GetTypeCode(propertyType), propertyType, recursionLevel);

                propertyInfo.SetValue(instance, propertyValue);
            }

            return (T)instance;
        }

        private object[] PrepareFuzzedParametersForThisConstructor(ConstructorInfo constructor, int recursionLevel)
        {
            var parameters = new List<object>();
            var parameterInfos = constructor.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                var type = parameterInfo.ParameterType;

                // Default .NET types
                parameters.Add(FuzzAnyDotNetType(Type.GetTypeCode(type), type, recursionLevel));
            }

            return parameters.ToArray();
        }

        private object FuzzAnyDotNetType(TypeCode typeCode, Type type, int recursionLevel)
        {
            if (type.IsEnum)
            {
                return FuzzEnumValue(type);
            }

            if (type.IsEnumerable())
            {
                return GenerateListOf(type, recursionLevel);
            }

            object result;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    result = _fuzzer.HeadsOrTails();
                    break;

                case TypeCode.Int32:
                    result = _fuzzer.GenerateInteger();
                    break;

                case TypeCode.Int64:
                    result = _fuzzer.GenerateLong();
                    break;

                case TypeCode.Decimal:
                    result = _fuzzer.GeneratePositiveDecimal();
                    break;

                case TypeCode.String:
                    result = _fuzzer.GenerateFirstName();
                    break;

                case TypeCode.DateTime:
                    result = _fuzzer.GenerateDateTime();
                    break;

                default:
                    // is it an IEnumerable?
                    result = GenerateInstanceOf(type, recursionLevel);
                    break;
            }

            return result;
        }

        private T InstantiateAndFuzzViaOtherConstructorIteratingOnAllThemUntilItWorks<T>(int recursionLevel, Type type)
        {
            T instance;
            // Some constructor are complicated to use (e.g. those accepting abstract classes as input)
            // Try other constructors until it works
            var constructors = type.GetConstructorsOrderedByNumberOfParametersDesc().Skip(1);
            foreach (var constructorInfo in constructors)
            {
                try
                {
                    instance = InstantiateAndFuzzViaConstructorWithBiggestNumberOfParameters<T>(constructorInfo, recursionLevel);
                    return instance;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // We couldn't use any of its Constructor. Let's return a default instance (degraded mode)
            instance = default(T);

            return instance;
        }

        private IEnumerable GenerateListOf(Type type, int recursionLevel)
        {
            var typeGenericTypeArguments = type.GenericTypeArguments;

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(typeGenericTypeArguments);

            // Instantiates a collection of ...
            var list = Activator.CreateInstance(constructedListType) as IList;

            // Add 5 elements of this type
            for (var i = 0; i < MaxCountToFuzzInLists; i++)
            {
                list.Add(GenerateInstanceOf(typeGenericTypeArguments.Single(), recursionLevel));
            }

            return list;
        }

        private object GenerateInstanceOf(Type type, int recursionLevel)
        {
            return CallPrivateGenericMethod(type, nameof(GenerateInstanceOf), new object[] { recursionLevel });
        }

        private object CallPrivateGenericMethod(Type typeOfT, string privateMethodName, object[] parameters)
        {
            var methodInfo = ((TypeInfo) typeof(TypeFuzzer)).DeclaredMethods.Single(m =>
                m.IsGenericMethod && m.IsPrivate && m.Name.Contains(privateMethodName));
            var generic = methodInfo.MakeGenericMethod(typeOfT);
            
            // private T GenerateInstanceOf<T>(int recursionLevel)
            var result = generic.Invoke(this, parameters);
            
            return result;
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

        private object FuzzEnumValue(Type enumType)
        {
            var enumValues = Enum.GetValues(enumType);
            return enumValues.GetValue(_fuzzer.Random.Next(0, enumValues.Length));
        }
    }
}