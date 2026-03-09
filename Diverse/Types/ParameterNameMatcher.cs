using System;

namespace Diverse
{
    /// <summary>
    /// Infers the appropriate fuzzer to use based on a parameter or property name.
    /// Matches are case-insensitive and ordered by priority (most specific first).
    /// Returns null when no pattern matches, letting the caller fall back to type-based generation.
    /// </summary>
    internal class ParameterNameMatcher
    {
        private readonly IFuzz _fuzzer;

        public ParameterNameMatcher(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Tries to generate a value based on the parameter/property name and type.
        /// </summary>
        /// <param name="name">The parameter or property name (e.g., "customerEmail", "age").</param>
        /// <param name="type">The CLR type of the parameter or property.</param>
        /// <param name="context">The fuzzing context containing previously generated values for this constructor.</param>
        /// <returns>A generated value if a pattern matched, or null if no match (caller should use type-based fallback).</returns>
        public object TryGenerateFromName(string name, Type type, ConstructorFuzzingContext context)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.String:
                    return TryGenerateString(name, context);

                case TypeCode.Int32:
                    return TryGenerateInt(name);

                case TypeCode.Decimal:
                    return TryGenerateDecimal(name);

                default:
                    return null;
            }
        }

        private object TryGenerateString(string name, ConstructorFuzzingContext context)
        {
            // Priority 1: email / mail
            if (EqualsOrEndsWith(name, "email") || EqualsOrEndsWith(name, "mail"))
            {
                context.TryGet("firstName", out var firstNameObj);
                context.TryGet("lastName", out var lastNameObj);
                return _fuzzer.GenerateEMail(firstNameObj as string, lastNameObj as string);
            }

            // Priority 2: password / pwd
            if (EqualsOrEndsWith(name, "password") || EqualsOrEndsWith(name, "pwd"))
            {
                return _fuzzer.GeneratePassword();
            }

            // Priority 3: lastname / surname
            if (EqualsOrEndsWith(name, "lastname") || EqualsOrEndsWith(name, "surname"))
            {
                if (context.TryGet("firstName", out var firstNameObj) && firstNameObj is string firstName)
                {
                    return _fuzzer.GenerateLastName(firstName);
                }

                // lastName appears before firstName in the constructor: generate an internal firstName for continent coherence
                var internalFirstName = _fuzzer.GenerateFirstName();
                return _fuzzer.GenerateLastName(internalFirstName);
            }

            // Priority 4: firstname / givenname
            if (EqualsOrEndsWith(name, "firstname") || EqualsOrEndsWith(name, "givenname"))
            {
                return _fuzzer.GenerateFirstName();
            }

            // Priority 5: name (but not firstname/lastname/givenname/surname — already handled above)
            if (EqualsOrEndsWith(name, "name"))
            {
                return _fuzzer.GenerateFirstName();
            }

            // Priority 6: description / text / comment / message / title / label / summary
            if (EqualsOrEndsWith(name, "description") ||
                EqualsOrEndsWith(name, "text") ||
                EqualsOrEndsWith(name, "comment") ||
                EqualsOrEndsWith(name, "message") ||
                EqualsOrEndsWith(name, "title") ||
                EqualsOrEndsWith(name, "label") ||
                EqualsOrEndsWith(name, "summary"))
            {
                return _fuzzer.GenerateSentence();
            }

            return null;
        }

        private object TryGenerateInt(string name)
        {
            if (string.Equals(name, "age", StringComparison.OrdinalIgnoreCase))
            {
                return _fuzzer.GenerateAge();
            }

            return null;
        }

        private object TryGenerateDecimal(string name)
        {
            if (EqualsOrEndsWith(name, "price") ||
                EqualsOrEndsWith(name, "amount") ||
                EqualsOrEndsWith(name, "cost") ||
                EqualsOrEndsWith(name, "total"))
            {
                return _fuzzer.GeneratePositiveDecimal(0.01m, 9999.99m);
            }

            return null;
        }

        private static bool EqualsOrEndsWith(string name, string suffix)
        {
            return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }
    }
}
