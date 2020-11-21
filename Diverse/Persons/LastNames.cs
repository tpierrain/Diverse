using System.Collections.Generic;

namespace Diverse
{
    public static class LastNames
    {
        public static IDictionary<Continent, string[]> PerContinent { get; } = new Dictionary<Continent, string[]>
        {
            {
                Continent.Africa,
                new[]
                {
                    "Soumbu", "Diak", "Keita", "Abbas", "Adomako", "Kasongo", "Abdallah", "Badu", "Chedjou",
                    "Djetou", "N'go", "Selassie", "Nwadike", "Boujettif", "Benkacem", "Zidane", "Smah", "Belkhodja",
                    "Ben Achour", "Ben Ayed", "Bouhageb"
                }
            },
            {
                Continent.Asia,
                new[]
                {
                    "NQuoc", "NGuyen", "Han", "Lǐ", "Wang", "Zhāng", "Chén", "Zhào", "Zhōu", "Wú", "Gāo", "Liáng",
                    "Féng", "Kim", "Lee", "Park", "Choi", "Lee"
                }
            },
            {
                Continent.Antartica,
                new[]
                {
                    "Karlsen", "Johnsen", "Pettersen", "Eriksen", "Johannessen", "Jørgensen", "Jacobsen", "Iversen",
                    "Solberg", "Bakken", "Rasmussen", "Sandvik", "Ruud", "Ødegård", "Strøm", "Myklebust"
                }
            },
            {
                Continent.Australia,
                new[]
                {
                    "Smith", "Wilson", "Brown", "Taylor", "Jones", "Thompson", "Campbell", "Wright", "Mills",
                    "Shepherd", "Greenwood", "Boone"
                }
            },
            {
                Continent.Europe,
                new[]
                {
                    "Di Biasio", "Di Maria", "Del Pozzo", "Zimmerman", "Pierrain", "Dupont", "Dupuy", "Marchand",
                    "García", "Fernández", "Lopes", "Gomes", "Hendricks", "Thompson", "Müller", "Schneider",
                    "Wagner", "Bauer", "Moreau", "Fournier", "Esposito", "Conti", "Rizzo", "Mancini"
                }
            },
            {
                Continent.SouthAmerica,
                new[]
                {
                    "Rodríguez", "González", "Muñoz", "Rojas", "Jimenez", "Rodríguez", "Sanchez", "Flores", "Pérez",
                    "Mora", "Quispe", "Benítez", "Hernández"
                }
            },
            {
                Continent.NorthAmerica,
                new[]
                {
                    "West", "Wayne", "Smith", "Biden", "Williams", "Johnson", "Brown", "Jones", "Miller", "Davis",
                    "Thomas", "Jackson", "Moore", "Clark", "Sanchez", "Robinson", "Young", "Perez", "Lee", "Allen",
                    "King"
                }
            }
        };
    }
}