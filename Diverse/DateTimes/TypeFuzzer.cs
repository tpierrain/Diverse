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

        public T GenerateInstance<T>()
        {
            Type genericType = typeof(T);
            T instance = (T)Activator.CreateInstance(genericType);

            IEnumerable<PropertyInfo> stringProperties = GetWritableStringProperties(genericType);
            
            foreach (var stringPropertyInfo in stringProperties)
            {
                SetProperty<T>(instance, stringPropertyInfo);
            }

            return instance;
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