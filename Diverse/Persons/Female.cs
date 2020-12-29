using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Information related to female persons.
    /// </summary>
    public static class Female
    {
        /// <summary>
        /// Gets all the <see cref="ContextualizedFirstName"/> instances possibly used by the lib.
        /// </summary>
        public static ContextualizedFirstName[] ContextualizedFirstNames => _contextualizedFirstNames;

        private static string[] _firstNames = null;

        /// <summary>
        /// Gets all the male first names possibly used by the lib.
        /// </summary>
        public static string[] FirstNames
        {
            get
            {
                if (_firstNames == null)
                {
                    _firstNames = Female.ContextualizedFirstNames
                        .Where(x => !string.IsNullOrWhiteSpace(x.FirstName))
                        .Select(m => m.FirstName)
                        .Distinct()
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

            new ContextualizedFirstName("Solveig", Continent.Antarctica),
            new ContextualizedFirstName("Freja", Continent.Antarctica),
            new ContextualizedFirstName("Kirsten", Continent.Antarctica),
            new ContextualizedFirstName("Mette", Continent.Antarctica),
            new ContextualizedFirstName("Liv", Continent.Antarctica),
            new ContextualizedFirstName("Erika", Continent.Antarctica),
            new ContextualizedFirstName("Maja", Continent.Antarctica),
            new ContextualizedFirstName("Ida", Continent.Antarctica),
            new ContextualizedFirstName("Anouk", Continent.Antarctica),
            new ContextualizedFirstName("Johanna", Continent.Antarctica),
            new ContextualizedFirstName("Mathilda", Continent.Antarctica),
            new ContextualizedFirstName("Sigrid", Continent.Antarctica),
            new ContextualizedFirstName("Anya", Continent.Antarctica),
            
            
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
            
            new ContextualizedFirstName("Kamini", Continent.Asia),
            new ContextualizedFirstName("Akshara", Continent.Asia),
            new ContextualizedFirstName("Amruta", Continent.Asia),
            new ContextualizedFirstName("Deepa", Continent.Asia),
            new ContextualizedFirstName("Ishani", Continent.Asia),
            new ContextualizedFirstName("Lajita", Continent.Asia),
            new ContextualizedFirstName("Rajeshri", Continent.Asia),
            new ContextualizedFirstName("Suhani", Continent.Asia),
            new ContextualizedFirstName("Indu", Continent.Asia),
            new ContextualizedFirstName("Vasatika", Continent.Asia),
            new ContextualizedFirstName("Trisha", Continent.Asia),

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
            
            new ContextualizedFirstName("Beata", Continent.Europe),
            new ContextualizedFirstName("Maya", Continent.Europe),
            new ContextualizedFirstName("Maëla", Continent.Europe),
            new ContextualizedFirstName("Marija", Continent.Europe),
            new ContextualizedFirstName("Tanja", Continent.Europe),
            new ContextualizedFirstName("Anneke", Continent.Europe),
            new ContextualizedFirstName("Antje", Continent.Europe),
            new ContextualizedFirstName("Heike", Continent.Europe),
            new ContextualizedFirstName("Sofie", Continent.Europe),
            
            new ContextualizedFirstName("Nancy", Continent.NorthAmerica),
            new ContextualizedFirstName("Rebecca", Continent.NorthAmerica),
            new ContextualizedFirstName("Julie", Continent.NorthAmerica),
            new ContextualizedFirstName("Vanessa", Continent.NorthAmerica),
            new ContextualizedFirstName("Darlene", Continent.NorthAmerica),
            new ContextualizedFirstName("Naomi", Continent.NorthAmerica),
            new ContextualizedFirstName("Shayna", Continent.NorthAmerica),
            new ContextualizedFirstName("Sarah", Continent.NorthAmerica),
            new ContextualizedFirstName("Emily", Continent.NorthAmerica),
            new ContextualizedFirstName("Shana", Continent.NorthAmerica),
            new ContextualizedFirstName("Zoe", Continent.NorthAmerica),
            new ContextualizedFirstName("Chloe", Continent.NorthAmerica),
            new ContextualizedFirstName("Ella", Continent.NorthAmerica),
            new ContextualizedFirstName("Hailey", Continent.NorthAmerica),
            new ContextualizedFirstName("Karen", Continent.NorthAmerica),
            
            new ContextualizedFirstName("Penelope", Continent.SouthAmerica),
            new ContextualizedFirstName("Sierra", Continent.SouthAmerica),
            new ContextualizedFirstName("Dhalia", Continent.SouthAmerica),
            new ContextualizedFirstName("Francesca", Continent.SouthAmerica),
            new ContextualizedFirstName("Valentina", Continent.SouthAmerica),
            new ContextualizedFirstName("Ximena", Continent.SouthAmerica),
            new ContextualizedFirstName("Isabella", Continent.SouthAmerica),
            new ContextualizedFirstName("Renata", Continent.SouthAmerica),
            new ContextualizedFirstName("Valeria", Continent.SouthAmerica),
            new ContextualizedFirstName("Andrijana", Continent.SouthAmerica),
            new ContextualizedFirstName("Camelia", Continent.SouthAmerica),
            new ContextualizedFirstName("Emanuele", Continent.SouthAmerica),
            new ContextualizedFirstName("Maria Eduarda", Continent.SouthAmerica),
            new ContextualizedFirstName("Beatriz", Continent.SouthAmerica),
            new ContextualizedFirstName("Romina", Continent.SouthAmerica),
            
            
            new ContextualizedFirstName("Abigail", Continent.Australia),
            new ContextualizedFirstName("Addison", Continent.Australia),
            new ContextualizedFirstName("Alinta", Continent.Australia),
            new ContextualizedFirstName("Aurora", Continent.Australia),
            new ContextualizedFirstName("Charlotte", Continent.Australia),
            new ContextualizedFirstName("Daisy", Continent.Australia),
            new ContextualizedFirstName("Eleanor", Continent.Australia),
            
            new ContextualizedFirstName("Vaiana", Continent.Australia),
            new ContextualizedFirstName("Tahia", Continent.Australia),
            new ContextualizedFirstName("Manutea", Continent.Australia),
            new ContextualizedFirstName("Poe", Continent.Australia),
            new ContextualizedFirstName("Heirani", Continent.Australia),
            new ContextualizedFirstName("Vanina", Continent.Australia),
            new ContextualizedFirstName("Moeata", Continent.Australia),

        };
    }
}