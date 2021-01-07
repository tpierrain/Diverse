using System;
using System.Collections.Generic;
using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Contains all the last names used by the library.
    /// </summary>
    public static class LastNames
    {
        /// <summary>
        /// Gets a dictionary of all the last name per <see cref="Continent"/>.
        /// </summary>
        public static IDictionary<Continent, string[]> PerContinent => RemoveDuplicates(_perContinent);

        private static IDictionary<Continent, string[]> RemoveDuplicates(Dictionary<Continent, string[]> perContinent)
        {
            var result = new Dictionary<Continent, string[]>();

            foreach (var continent in perContinent.Keys)
            {
                result[continent] = perContinent[continent].Distinct().ToArray();
            }

            return result;
        }

        private static readonly Dictionary<Continent, string[]> _perContinent = new Dictionary<Continent, string[]>
        {
            {
                Continent.Africa,
                new[]
                {
                    "Soumbu", "Diak", "Keita", "Abbas", "Adomako", "Kasongo", "Abdallah", "Badu", "Chedjou",
                    "Djetou", "N'go", "Selassie", "Nwadike", "Boujettif", "Benkacem", "Zidane", "Smah", "Belkhodja",
                    "Ben Achour", "Ben Ayed", "Bouhageb", "Soua", "Smahi", "Soumbou", "Ewé", "Traore", "Ali",
                    "Kamara", "Tesfaye", "Banda", "NGuema", "NGoma", "Phiri", "Mensah", "Drogba", "Ibrahim",
                    "Sissoko", "Diabaté", "Samparé", "Diop", "Ba", "Mwangi", "Salem", "NDiaye", "Lulunga", "Diouf"
                }
            },
            {
                Continent.Asia,
                new[]
                {
                    "Bhundoo", "Rughoobur", "Sauba", "Khatri", "Laghari", "Patel", "Acharya", "Balakrishnan",
                    "Jain", "Dhar", "Kulkarni", "Madan", "Chandra", "Thakur", "Singh", "Dalal", "Chakrabarti",
                    "Kohli", "Jha", "NQuoc", "NGuyen", "Han", "Lǐ", "Wang", "Zhāng", "Chén", "Zhào", "Zhōu", "Wú",
                    "Gāo", "Liáng", "Féng", "Kim", "Lee", "Park", "Choi", "Lee", "Truong", "Jeong", "Cho", "Yoon",
                    "Bak", "Suzuki", "Takahashi", "Tanaka", "Nakamura", "Watanabe", "Ito"
                }
            },
            {
                Continent.Antarctica,
                new[]
                {
                    "Karlsen", "Johnsen", "Pettersen", "Eriksen", "Johannessen", "Jørgensen", "Jacobsen", "Iversen",
                    "Solberg", "Bakken", "Rasmussen", "Sandvik", "Ruud", "Ødegård", "Strøm", "Myklebust", "Nygård",
                    "Berntsen", "Thomassen", "Haugland", "Gulbrandsen", "Tveit", "Ødegård", "Madsen", "Abrahamsen",
                    "Brekke", "Ruud", "Myhre", "Aas"
                }
            },
            {
                Continent.Australia,
                new[]
                {
                    "Smith", "Wilson", "Brown", "Taylor", "Jones", "Thompson", "Campbell", "Wright", "Mills",
                    "Shepherd", "Greenwood", "Boone", "Lee", "Kelly", "Robinson", "Walker", "Chen", "Harris",
                    "Patel", "Reddy", "Prakash", "Khan", "Tudu", "Turner", "Martin", "Ryan", "Williams", "Wilson",
                    "White", "Thomas", "King", "Falconer", "Forsythe", "Grath", "Hodge", "Ivey", "Kingston",
                    "Rayner"
                }
            },
            {
                Continent.Europe,
                new[]
                {
                    "Di Biasio", "Di Maria", "Del Pozzo", "Zimmerman", "Pierrain", "Dupont", "Dupuy", "Marchand",
                    "García", "Fernández", "Lopes", "Gomes", "Hendricks", "Thompson", "Müller", "Schneider",
                    "Wagner", "Bauer", "Moreau", "Fournier", "Esposito", "Conti", "Rizzo", "Mancini", "Boucard",
                    "Brandolini", "Tune", "Lemaire", "Lorphelin", "Berdot", "Bellec", "Kaiser", "Mateudi",
                    "Roccaserra", "Mattei", "Dodelier", "Muraz", "Truchot", "De Funes", "Bernard", "Johnson",
                    "Dubois", "Petit", "Kovacs", "Dupuydauby", "Mercier", "Girard", "Moreau", "Gonzales", "Lefevre"
                }
            },
            {
                Continent.SouthAmerica,
                new[]
                {
                    "Rodríguez", "González", "Muñoz", "Rojas", "Jimenez", "Rodríguez", "Sanchez", "Flores", "Pérez",
                    "Mora", "Quispe", "Benítez", "Hernández", "Aburto", "Acuna", "Aimar", "Alcantara", "Allende",
                    "Ambríz", "Araquistain", "Arellano", "Arrisola", "Borges", "Botelho", "Burruchaga",
                    "Bustamante", "Caldera", "Canizales", "Clores", "Coutinho", "Costa", "De Jesus", "De la Hoya",
                    "Denilson", "De Souza"
                }
            },
            {
                Continent.NorthAmerica,
                new[]
                {
                    "West", "Wayne", "Smith", "Biden", "Williams", "Johnson", "Brown", "Jones", "Miller", "Davis",
                    "Thomas", "Jackson", "Moore", "Clark", "Sanchez", "Robinson", "Young", "Perez", "Lee", "Allen",
                    "King", "Beck", "Lewis", "Hill", "Baker", "Rivera", "Mitchell", "Carter", "Carhart", "Hughes",
                    "Burgis", "Hoadley", "Moore", "Brand", "Acker", "Saint-John", "Talbert", "Wynn", "Swetel",
                    "Postma", "Woskowski", "Panamera", "Winter", "Fantasia", "Barimore", "Jordan", "Rosenberg",
                    "Gertz", "Nash"
                }
            }
        };


        /// <summary>
        /// Find which <see cref="Continent"/> we relate any given last name.
        /// </summary>
        /// <param name="lastName">The last name we want to find an associated <see cref="Continent"/> for.</param>
        /// <returns>The <see cref="Continent"/> we have associated with this last name.</returns>
        public static Continent FindAssociatedContinent(string lastName)
        {
            foreach (var keyValuePair in PerContinent)
            {
                if (keyValuePair.Value.Contains(lastName))
                {
                    return keyValuePair.Key;
                }
            }

            throw new NotSupportedException($"The lastname ({lastName}) is not associated to any Continent in our lib.");
        }
    }
}