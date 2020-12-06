using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diverse.Reflection
{
    /// <summary>
    /// Extension methods related to the usage of Reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Indicates whether or not a given <see cref="Type"/> is already covered by the lib for Fuzzing.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns><b>true</b> if the <see cref="Type"/> is already covered by the lib for Fuzzing, <b>false</b> otherwise.</returns>
        public static bool IsCoveredByAFuzzer(this Type type)
        {
            return Types.CoveredByAFuzzer.Contains(type);
        }

        /// <summary>
        /// Gets a value indicated whether or not a <see cref="ConstructorInfo"/> has no parameter defined.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to check absence of parameters for.</param>
        /// <returns><b>true</b> if the <see cref="ConstructorInfo"/> has no parameter defined, <b>false</b> otherwise.</returns>
        public static bool IsEmpty(this ConstructorInfo constructor)
        {
            return constructor.GetParameters().Length == 0;
        }

        public static ConstructorInfo GetConstructorWithBiggestNumberOfParameters(this Type type)
        {
            var constructors = type.GetConstructorsOrderedByNumberOfParametersDesc();

            if (!constructors.Any())
            {
                return null;
            }

            return constructors.First();
        }

        public static IEnumerable<ConstructorInfo> GetConstructorsOrderedByNumberOfParametersDesc(this Type type)
        {
            var constructors = ((System.Reflection.TypeInfo)type).DeclaredConstructors;

            return constructors.OrderByDescending(c => c.GetParameters().Length);
        }

    }
}