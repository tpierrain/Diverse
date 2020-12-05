using System;
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
            if (recursionLevel > 500)
            {
                return default(T);
            }

            var genericType = typeof(T);
            var constructors = ((System.Reflection.TypeInfo) genericType).DeclaredConstructors;
            var constructor = GetConstructorWithBiggestNumberOfParameters(constructors);

            var constructorParameters = PrepareFuzzedParametersForThisConstructor(constructor, recursionLevel);
            var instance = constructor.Invoke(constructorParameters);

            return (T)instance;
        }

        private object[] PrepareFuzzedParametersForThisConstructor(ConstructorInfo constructor, int recursionLevel)
        {
            var parameters = new List<object>();
            var parameterInfos = constructor.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                var type = parameterInfo.ParameterType;

                if (type.IsEnum)
                {
                    parameters.Add(FuzzEnumValue(type));
                    continue;
                }

                // Default .NET types
                parameters.Add(FuzzAnyDotNetType(Type.GetTypeCode(type), type, ++recursionLevel));
            }

            return parameters.ToArray();
        }

        private object FuzzAnyDotNetType(TypeCode typeCode, Type type, int recursionLevel)
        {
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
                    result = GenerateInstanceOf(type, recursionLevel);
                    break;
            }

            return result;
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

        private static ConstructorInfo GetConstructorWithBiggestNumberOfParameters(IEnumerable<ConstructorInfo> constructors)
        {
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

        private static IEnumerable<PropertyInfo> GetWritableStringProperties(Type genericType)
        {
            return genericType.GetProperties().Where(prop => prop.PropertyType == typeof(String) && prop.CanWrite);
        }

        private void SetProperty<T>(T instance, PropertyInfo stringPropertyInfo)
        {
            var firstName = _fuzzer.GenerateFirstName();
            if(stringPropertyInfo.Name.Contains("FirstName"))
            {
                stringPropertyInfo.SetValue(instance, firstName);
            }
            else if(stringPropertyInfo.Name.Contains("LastName"))
            {
                stringPropertyInfo.SetValue(instance, _fuzzer.GenerateLastName(firstName));
            }
        }
    }
}