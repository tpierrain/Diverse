using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Diverse
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Remove diacritics (accents etc) from a given <see cref="string"/>.
        /// </summary>
        /// <param name="text">The <see cref="string"/> you want to remove diacritics from.</param>
        /// <returns>The string without accents/diacritic.</returns>
        public static string RemoveDiacritics(this string text)
        {
            // from https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Upper the first char of a word.
        /// </summary>
        /// <param name="word">The word to upper.</param>
        /// <returns>The word with the first letter capitalized.</returns>
        public static string FirstCharToUpper(this string word)
        {
            switch (word)
            {
                case null:
                    throw new ArgumentNullException(nameof(word));
                case "":
                    throw new ArgumentException($"{nameof(word)} cannot be empty", nameof(word));
                default:
                    return word.First().ToString().ToUpper() + word.Substring(1);
            }
        }
    }
}