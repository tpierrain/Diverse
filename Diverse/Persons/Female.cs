using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Information related to female persons.
    /// </summary>
    public static class Female
    {
        public static ContextualizedFirstName[] ContextualizedFirstNames => _contextualizedFirstNames;

        private static string[] _firstNames = null;

        public static string[] FirstNames
        {
            get
            {
                if (_firstNames == null)
                {
                    _firstNames = Female.ContextualizedFirstNames
                                            .Where(x => !string.IsNullOrWhiteSpace(x.FirstName))
                                            .Select(m => m.FirstName)
                                            .ToArray();
                }

                return _firstNames;
            }
        }

        private static readonly ContextualizedFirstName[] _contextualizedFirstNames = new ContextualizedFirstName[]
        {
            new ContextualizedFirstName("Aaliyah", Continent.Africa),
            new ContextualizedFirstName("Alyssa", Continent.Africa),
            new ContextualizedFirstName("Bintu", Continent.Africa),
            new ContextualizedFirstName("Jemila", Continent.Africa),
            new ContextualizedFirstName("Jendayi", Continent.Africa),
            new ContextualizedFirstName("Keisha", Continent.Africa),
            new ContextualizedFirstName("Laila", Continent.Africa),
            new ContextualizedFirstName("Abeba", Continent.Africa),
            new ContextualizedFirstName("Makeba", Continent.Africa),
            new ContextualizedFirstName("Malika", Continent.Africa),
            new ContextualizedFirstName("Nzinga", Continent.Africa),
            new ContextualizedFirstName("Nakeisha", Continent.Africa),
            new ContextualizedFirstName("Nichelle", Continent.Africa),
            new ContextualizedFirstName("Zahara", Continent.Africa),
            new ContextualizedFirstName("Zuri", Continent.Africa),

            new ContextualizedFirstName("Rim", Continent.Africa),
            new ContextualizedFirstName("Nora", Continent.Africa),
            new ContextualizedFirstName("Sarah", Continent.Africa),
            new ContextualizedFirstName("Fatima", Continent.Africa),
            new ContextualizedFirstName("Noûr", Continent.Africa),
            new ContextualizedFirstName("Louna", Continent.Africa),
            new ContextualizedFirstName("Maïssa", Continent.Africa),
            new ContextualizedFirstName("Kenza", Continent.Africa),
            new ContextualizedFirstName("Selma", Continent.Africa),
            new ContextualizedFirstName("Jasmine", Continent.Africa),
            new ContextualizedFirstName("Janna", Continent.Africa),
            new ContextualizedFirstName("Mériem", Continent.Africa),
            new ContextualizedFirstName("Zayneb", Continent.Africa),
            new ContextualizedFirstName("Khadija", Continent.Africa),

            new ContextualizedFirstName("Solveig", Continent.Antartica),
            new ContextualizedFirstName("Freja", Continent.Antartica),
            new ContextualizedFirstName("Kirsten", Continent.Antartica),
            new ContextualizedFirstName("Mette", Continent.Antartica),
            new ContextualizedFirstName("Liv", Continent.Antartica),
            new ContextualizedFirstName("Erika", Continent.Antartica),
            new ContextualizedFirstName("Maja", Continent.Antartica),
            new ContextualizedFirstName("Ida", Continent.Antartica),
            new ContextualizedFirstName("Anouk", Continent.Antartica),
            new ContextualizedFirstName("Johanna", Continent.Antartica),
            new ContextualizedFirstName("Mathilda", Continent.Antartica),
            new ContextualizedFirstName("Sigrid", Continent.Antartica),
            new ContextualizedFirstName("Anya", Continent.Antartica),
            
            
            new ContextualizedFirstName("Kyeol", Continent.Asia),
            new ContextualizedFirstName("Ja-kyung", Continent.Asia),
            new ContextualizedFirstName("Jeong-ja", Continent.Asia),
            new ContextualizedFirstName("Ji-yeon", Continent.Asia),
            new ContextualizedFirstName("Jung-sook", Continent.Asia),
            new ContextualizedFirstName("Diễm Hạnh", Continent.Asia),
            new ContextualizedFirstName("Duong Liễu", Continent.Asia),
            new ContextualizedFirstName("Mai Lan", Continent.Asia),
            new ContextualizedFirstName("Mây", Continent.Asia),
            new ContextualizedFirstName("My Lê", Continent.Asia),
            new ContextualizedFirstName("Tuong Vi", Continent.Asia),
            new ContextualizedFirstName("Vân", Continent.Asia),
            new ContextualizedFirstName("Xuân", Continent.Asia),

            new ContextualizedFirstName("Zhang", Continent.Asia),
            new ContextualizedFirstName("Mei", Continent.Asia),
            new ContextualizedFirstName("Lin", Continent.Asia),
            new ContextualizedFirstName("Wen", Continent.Asia),
            new ContextualizedFirstName("Xian", Continent.Asia),
            new ContextualizedFirstName("Jiao", Continent.Asia),
            new ContextualizedFirstName("Lan", Continent.Asia),
            new ContextualizedFirstName("Chan", Continent.Asia),
            new ContextualizedFirstName("Li", Continent.Asia),
            new ContextualizedFirstName("Avigail", Continent.Asia),
            new ContextualizedFirstName("Bayla", Continent.Asia),
            new ContextualizedFirstName("Esther", Continent.Asia),
            
            new ContextualizedFirstName("Malka", Continent.Asia),
            new ContextualizedFirstName("Meirav", Continent.Asia),
            new ContextualizedFirstName("Yael", Continent.Asia),


            new ContextualizedFirstName("Louise", Continent.Europe),
            new ContextualizedFirstName("Audrey", Continent.Europe),
            new ContextualizedFirstName("Laurence", Continent.Europe),
            new ContextualizedFirstName("Christine", Continent.Europe),
            new ContextualizedFirstName("Marie", Continent.Europe),
            new ContextualizedFirstName("Nathalie", Continent.Europe),
            new ContextualizedFirstName("Sophie", Continent.Europe),
            new ContextualizedFirstName("Karine", Continent.Europe),
            new ContextualizedFirstName("Eve", Continent.Europe),
            new ContextualizedFirstName("Noëlla", Continent.Europe),
            new ContextualizedFirstName("Sandrine", Continent.Europe),
            new ContextualizedFirstName("Marie-Hélène", Continent.Europe),
            new ContextualizedFirstName("Iris", Continent.Europe),
            
            new ContextualizedFirstName("Maya", Continent.Europe),
            new ContextualizedFirstName("Maëla", Continent.Europe),
            new ContextualizedFirstName("", Continent.Europe),
            new ContextualizedFirstName("", Continent.Europe),
            new ContextualizedFirstName("", Continent.Europe),
            new ContextualizedFirstName("", Continent.Europe),
            new ContextualizedFirstName("", Continent.Europe),
            new ContextualizedFirstName("", Continent.Europe),
            
            new ContextualizedFirstName("Nancy", Continent.NorthAmerica),
            new ContextualizedFirstName("Rebecca", Continent.NorthAmerica),
            new ContextualizedFirstName("Julie", Continent.NorthAmerica),
            new ContextualizedFirstName("Vanessa", Continent.NorthAmerica),
            new ContextualizedFirstName("Darlene", Continent.NorthAmerica),
            new ContextualizedFirstName("Naomi", Continent.NorthAmerica),
            new ContextualizedFirstName("Shayna", Continent.NorthAmerica),
            new ContextualizedFirstName("Sarah", Continent.NorthAmerica),
            new ContextualizedFirstName("", Continent.NorthAmerica),
            new ContextualizedFirstName("", Continent.NorthAmerica),
            new ContextualizedFirstName("", Continent.NorthAmerica),
            new ContextualizedFirstName("", Continent.NorthAmerica),
            new ContextualizedFirstName("", Continent.NorthAmerica),
            new ContextualizedFirstName("", Continent.NorthAmerica),
            
            new ContextualizedFirstName("Penelope", Continent.SouthAmerica),
            new ContextualizedFirstName("Sierra", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            new ContextualizedFirstName("", Continent.SouthAmerica),
            
            
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),
            new ContextualizedFirstName("", Continent.Australia),

        };
    }
}