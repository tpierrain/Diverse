using System.Collections.Generic;
using System.Linq;

// ReSharper disable StringLiteralTypo

namespace Diverse.Address
{
    /// <summary>
    /// Gathers all infos related to Geography (continent, countries, cities).
    /// </summary>
    /// <remarks>Note: All associations between cities, state/province/area and Countries are made here.</remarks>
    public static class Geography
    {
        private static string[] _cityNames;
        public static string[] CityNames
        {
            get
            {
                if (_cityNames == null)
                {
                    _cityNames = _citiesOfTheWorld.Select(cc => cc.CityName).Distinct().ToArray();
                }

                return _cityNames;
            }

            set => _cityNames = value;
        }

        public static string[] GiveMeCitiesOf(Country country)
        {
            return _citiesOfTheWorld
                .Where(cw => cw.Country == country)
                .Select(x => x.CityName)
                .Distinct()
                .ToArray();
        }

        public static StateProvinceArea[] GiveMeStateProvinceAreaOf(Country country)
        {
            return _citiesOfTheWorld
                .Where(cw => cw.Country == country)
                .Select(x => x.StateProvinceArea)
                .Distinct()
                .ToArray();
        }

        private static readonly ContextualizedCity[] _citiesOfTheWorld = new ContextualizedCity[]
        {
            new ContextualizedCity("Paris", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe),
            new ContextualizedCity("Saint-Ouen", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe),
            new ContextualizedCity("Saint-Denis", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe),
            new ContextualizedCity("Versailles", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe),
            new ContextualizedCity("La Courneuve", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe),
            new ContextualizedCity("Quiberon", StateProvinceArea.Bretagne, Country.France, Continent.Europe),
            new ContextualizedCity("Rennes", StateProvinceArea.Bretagne, Country.France, Continent.Europe),
            new ContextualizedCity("Brest", StateProvinceArea.Bretagne, Country.France, Continent.Europe),
            new ContextualizedCity("Nantes", StateProvinceArea.PaysDeLaLoire, Country.France, Continent.Europe),
            new ContextualizedCity("Bordeaux", StateProvinceArea.NouvelleAquitaine, Country.France, Continent.Europe),
            new ContextualizedCity("Marseille", StateProvinceArea.ProvenceAlpesCoteDAzur, Country.France, Continent.Europe),
            new ContextualizedCity("Nice", StateProvinceArea.ProvenceAlpesCoteDAzur, Country.France, Continent.Europe),
            new ContextualizedCity("Lyon", StateProvinceArea.AuvergneRhoneAlpes, Country.France, Continent.Europe),
            new ContextualizedCity("Grenoble", StateProvinceArea.AuvergneRhoneAlpes, Country.France, Continent.Europe),
            new ContextualizedCity("Montpellier", StateProvinceArea.Occitanie, Country.France, Continent.Europe),
            new ContextualizedCity("Toulouse", StateProvinceArea.Occitanie, Country.France, Continent.Europe),
           
            new ContextualizedCity("London", StateProvinceArea.London, Country.England, Continent.Europe),
            new ContextualizedCity("New York", StateProvinceArea.NewYork, Country.US, Continent.NorthAmerica),

            new ContextualizedCity("Honk-Kong", StateProvinceArea.ChineseAutonomous, Country.China, Continent.Asia),
            new ContextualizedCity("Macau", StateProvinceArea.ChineseAutonomous,Country.China, Continent.Asia),
            new ContextualizedCity("Wuhan", StateProvinceArea.Hubei, Country.China, Continent.Asia),
            new ContextualizedCity("Beijing", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new ContextualizedCity("Shanghai", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new ContextualizedCity("Tianjin", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new ContextualizedCity("Chongqing",StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new ContextualizedCity("Danzhou", StateProvinceArea.Hainan, Country.China, Continent.Asia),
            new ContextualizedCity("Kunming", StateProvinceArea.Yunnan, Country.China, Continent.Asia),
            new ContextualizedCity("Lijiang", StateProvinceArea.Yunnan, Country.China, Continent.Asia),
            new ContextualizedCity("Shangri-La", StateProvinceArea.Yunnan, Country.China, Continent.Asia),

            new ContextualizedCity("Mumbai", StateProvinceArea.Maharashtra, Country.India, Continent.Asia),
            new ContextualizedCity("New Delhi", StateProvinceArea.Delhi ,Country.India, Continent.Asia),
            new ContextualizedCity("Bangalore", StateProvinceArea.Karnataka, Country.India, Continent.Asia),
            
            new ContextualizedCity("Bangui", StateProvinceArea.Bangui, Country.CentralAfricanRepublic, Continent.Africa),
            new ContextualizedCity("Dakar", StateProvinceArea.Dakar,Country.Senegal, Continent.Africa),
        };

        private static readonly Dictionary<Country, string[]> _streetsOfCountries =
            new Dictionary<Country, string[]>
            {
                { Country.France, new string[]
                            {
                                        "rue Anatole France",
                                        "rue des Martyrs",
                                        "bd Saint-Germain",
                                        "rue du Commandant Cartouche",
                                        "rue de la palissade",
                                        "rue de la Gare",
                                        "rue de la Poste",
                                        "fronton des épivents",
                                        "rue des Archives",
                                        "rue Tristan Tzara",
                                        "rue de l'évangile",
                                        "bd de la Somme",
                                        "rue des flots bleus",
                                        "rue de la résistance",
                                        "bd de la Mer",
                                        "rue Paul Doumer",
                                        "cours Saint-Louis",
                                        "rue Albert",
                                        "rue Condorcet",
                                        "cour du Médoc",
                                        "rue Camille Godard",
                                        "rue Frère",
                                        "promenade du Peyrou",
                                        "Cours Gambetta",
                                        "rue Oberkampf",
                                        "avenue de la liberté",
                                        "rue de Toiras",
                                        "avenue de la Croix du Capitaine",
                                        "avenue de Lodève",
                                        "bd des Arceaux",
                                        "place Bellecour",
                                        "place des fêtes",
                                        "rue Pablo Picasso",
                                        "rue Garibaldi",
                                        "quai Claude-Bernard",
                                        "quai des Orfèvres",
                                        "avenue Jean Jaurès",
                                        "rue Pasteur",
                                        "rue des Calanques",
                                        "rue de la Loge",
                                        "rue des Caisseries",
                                        "rue de la République",
                                        "avenue de la République",
                                        "rue Paradis",
                                        "rue du Parlement",
                                        "rue Jean Guéhenno",
                                        "bd Maréchal de Lattre de Tassigny",
                                        "rue Albert Camus",
                            }}
            };

        public static StateProvinceArea GiveMeStateProvinceAreaOf(string cityName)
        {
            return _citiesOfTheWorld.Single(c => c.CityName == cityName).StateProvinceArea;
        }

        public static Country GiveMeCountryOf(string cityName)
        {
            return _citiesOfTheWorld.Single(c => c.CityName == cityName).Country;
        }

        public static string[] GiveMeStreetsOf(Country country)
        {
            return _streetsOfCountries[country];
        }
    }
}