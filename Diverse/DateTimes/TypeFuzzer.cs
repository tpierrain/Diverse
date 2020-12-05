using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diverse.DateTimes
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

            var genericType = typeof(T);
            
            var constructor = GetConstructorWithBiggestNumberOfParameters(genericType);
            if (IsEmptyConstructor(constructor))
            {
                var instance = InstantiateAndFuzzViaPropertiesWhenTheyHaveSetters<T>(constructor, recursionLevel, genericType);
                return instance;
            }
            else
            {
                var instance = InstantiateAndFuzzViaConstructorWithBiggestNumberOfParameters<T>(constructor, recursionLevel);
                return instance;
            }
        }

        private T InstantiateAndFuzzViaConstructorWithBiggestNumberOfParameters<T>(ConstructorInfo constructor, int recursionLevel)
        {
            var constructorParameters = PrepareFuzzedParametersForThisConstructor(constructor, recursionLevel);
            var instance = constructor.Invoke(constructorParameters);
            return (T)instance;
        }

        private T InstantiateAndFuzzViaPropertiesWhenTheyHaveSetters<T>(ConstructorInfo constructor, int recursionLevel, Type genericType)
        {
            var instance = constructor.Invoke(new object[0]);

            var propertyInfos = genericType.GetProperties().Where(prop => prop.CanWrite);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                var propertyValue = FuzzAnyDotNetType(Type.GetTypeCode(propertyType), propertyType, recursionLevel);

                propertyInfo.SetValue(instance, propertyValue);
            }

            return (T)instance;
        }

        private static bool IsEmptyConstructor(ConstructorInfo constructor)
        {
            return constructor.GetParameters().Length == 0;
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

            if (IsEnumerable(type))
            {
                return GenerateListOf(type, recursionLevel);
            }

            object result;
            switch (typeCode)
            {
                case TypeCode.Int32:
                    result = _fuzzer.GenerateInteger();
                    break;

                case TypeCode.Decimal:
                    result = _fuzzer.GeneratePositiveDecimal();
                    break;

                case TypeCode.String:
                    result = _fuzzer.GenerateFirstName();
                    break;
               
                default:
                    // is it an IEnumerable?
                    result = GenerateInstanceOf(type, recursionLevel);

                    break;
            }

            return result;
        }

        private static bool IsEnumerable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
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
            var methodInfo = ((System.Reflection.TypeInfo) typeof(TypeFuzzer)).DeclaredMethods.Single(m =>
                m.IsGenericMethod && m.IsPrivate && m.Name.Contains(privateMethodName));
            var generic = methodInfo.MakeGenericMethod(typeOfT);
            
            // private T GenerateInstanceOf<T>(int recursionLevel)
            var result = generic.Invoke(this, parameters);
            
            return result;
        }

        private static ConstructorInfo GetConstructorWithBiggestNumberOfParameters(Type genericType)
        {
            var constructors = ((System.Reflection.TypeInfo)genericType).DeclaredConstructors;
            return constructors.OrderByDescending(c => c.GetParameters().Length).First();
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