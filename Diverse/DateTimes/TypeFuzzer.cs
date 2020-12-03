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
        private readonly IProvideCorePrimitivesToFuzzer _fuzzerPrimitives;
        private readonly IFuzzPersons _personFuzzer;

        /// <summary>
        /// Instantiates a <see cref="TypeFuzzer"/>.
        /// </summary>
        /// <param name="fuzzerPrimitives">Instance of <see cref="IProvideCorePrimitivesToFuzzer"/> to use.</param>
        /// <param name="personFuzzer">Instance of <see cref="IFuzzPersons"/> to use.</param>
        public TypeFuzzer(IProvideCorePrimitivesToFuzzer fuzzerPrimitives, IFuzzPersons personFuzzer)
        {
            _fuzzerPrimitives = fuzzerPrimitives;
            _personFuzzer = personFuzzer;
        }

        /// <summary>
        /// Generates an instance of a type T.
        /// </summary>
        /// <returns>An instance of type T with some fuzzed properties.</returns>
        public T GenerateInstance<T>()
        {
            var genericType = typeof(T);
            var instance = (T)Activator.CreateInstance(genericType);

            var stringProperties = GetWritableStringProperties(genericType);
            
            foreach (var stringPropertyInfo in stringProperties)
            {
                SetProperty<T>(instance, stringPropertyInfo);
            }

            return instance;
        }

        /// <summary>
        /// Generates an instance of an <see cref="Enum"/> type.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="Enum"/></typeparam>
        /// <returns>An random value of the specified <see cref="Enum"/> type.</returns>
        public T GenerateEnum<T>()
        {
            var enumValues = Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(_fuzzerPrimitives.Random.Next(0, enumValues.Length));
        }

        private static IEnumerable<PropertyInfo> GetWritableStringProperties(Type genericType)
        {
            return genericType.GetProperties().Where(prop => prop.PropertyType == typeof(String) && prop.CanWrite);
        }

        private void SetProperty<T>(T instance, PropertyInfo stringPropertyInfo)
        {
            var firstName = _personFuzzer.GenerateFirstName();
            if(stringPropertyInfo.Name.Contains("FirstName"))
            {
                stringPropertyInfo.SetValue(instance, firstName);
            }
            else if(stringPropertyInfo.Name.Contains("LastName"))
            {
                stringPropertyInfo.SetValue(instance, _personFuzzer.GenerateLastName(firstName));
            }
        }
    }
}