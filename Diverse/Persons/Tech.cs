using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Information related to tech things.
    /// </summary>
    internal static class Tech
    {
        /// <summary>
        /// Gets a list of domain names for emails (e.g.: protonmail.com, yahoo.fr, etc).
        /// </summary>
        public static string[] EmailDomainNames => _domainNames.Distinct().ToArray();

        private static readonly string[] _domainNames = new string[]
        {
            "kolab.com", "protonmail.com", "gmail.com", "yahoo.fr", "42skillz.com", "gmail.com", "ibm.com",
            "gmail.com", "yopmail.com", "microsoft.com", "gmail.com", "aol.com"
        };
    }
}