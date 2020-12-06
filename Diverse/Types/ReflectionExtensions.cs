using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Diverse
{
    /// <summary>
    /// Extension methods related to the usage of Reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets a value indicating whether a given <see cref="Type"/> is already covered by the lib for Fuzzing.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns><b>true</b> if the <see cref="Type"/> is already covered by the lib for Fuzzing, <b>false</b> otherwise.</returns>
        public static bool IsCoveredByAFuzzer(this Type type)
        {
            return Types.CoveredByAFuzzer.Contains(type);
        }

        /// <summary>
        /// Gets a value indicating whether a given <see cref="Type"/> is <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns><b>true</b> if the <see cref="Type"/> is a <see cref="IEnumerable{T}"/> instance, <b>false</b> otherwise.</returns>
        public static bool IsEnumerable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        /// <summary>
        /// Gets a value indicated whether a given <see cref="ConstructorInfo"/> has no parameter defined.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> to check absence of parameters for.</param>
        /// <returns><b>true</b> if the <see cref="ConstructorInfo"/> has no parameter defined, <b>false</b> otherwise.</returns>
        public static bool IsEmpty(this ConstructorInfo constructor)
        {
            return constructor.GetParameters().Length == 0;
        }

        /// <summary>
        /// Gets the constructor with the biggest number of Parameters of a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The considered <see cref="Type"/>.</param>
        /// <returns>The <see cref="ConstructorInfo"/> which has the biggest number of Parameters defined for this <see cref="Type"/>.</returns>
        public static ConstructorInfo GetConstructorWithBiggestNumberOfParameters(this Type type)
        {
            var constructors = type.GetConstructorsOrderedByNumberOfParametersDesc().ToArray();

            if (!constructors.Any())
            {
                return null;
            }

            return constructors.First();
        }

        /// <summary>
        /// Gets all the constructors of a <see cref="Type"/> ordered by their number of parameters desc.
        /// </summary>
        /// <param name="type">The considered <see cref="Type"/>.</param>
        /// <returns>All the constructors of a <see cref="Type"/> ordered by their number of parameters desc.</returns>
        public static IEnumerable<ConstructorInfo> GetConstructorsOrderedByNumberOfParametersDesc(this Type type)
        {
            var constructors = ((System.Reflection.TypeInfo)type).DeclaredConstructors;

            return constructors.OrderByDescending(c => c.GetParameters().Length);
        }
    }
}