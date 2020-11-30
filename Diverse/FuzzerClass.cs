using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diverse
{
    public class FuzzerClass
    {
        public T GenerateInstance<T>(Generators generators)
        {
            Type genericType = typeof(T);
            T instance = (T)Activator.CreateInstance(genericType);

            IEnumerable<PropertyInfo> stringProperties = GetWritableStringProperties(genericType);
            
            foreach (var stringPropertyInfo in stringProperties)
            {
                SetProperty<T>(generators, instance, stringPropertyInfo);
            }

            return instance;
        }

        private static IEnumerable<PropertyInfo> GetWritableStringProperties(Type genericType)
        {
            return genericType.GetProperties().Where(prop => prop.PropertyType == typeof(String) && prop.CanWrite);
        }

        private void SetProperty<T>(Generators generators, T instance, PropertyInfo stringPropertyInfo)
        {
            string firstName = generators.NameGenerator();
            if(stringPropertyInfo.Name.Contains("FirstName"))
            {
                stringPropertyInfo.SetValue(instance, firstName);
            }
            else if(stringPropertyInfo.Name.Contains("LastName"))
            {
                stringPropertyInfo.SetValue(instance, generators.LastNameGenerator(firstName));
            }
        }
    }
}