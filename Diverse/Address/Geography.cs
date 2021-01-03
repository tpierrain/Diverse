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

        private static readonly CityWithRelatedInformations[] _citiesOfTheWorld = new CityWithRelatedInformations[]
        {
            new CityWithRelatedInformations("Paris", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat: "750##"),
            new CityWithRelatedInformations("Saint-Ouen", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe,zipCodeFormat: "93400"),
            new CityWithRelatedInformations("Saint-Denis", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat: "93200"),
            new CityWithRelatedInformations("Versailles", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat:"78000"),
            new CityWithRelatedInformations("La Courneuve", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat:"93120"),
            new CityWithRelatedInformations("Quiberon", StateProvinceArea.Bretagne, Country.France, Continent.Europe, zipCodeFormat:"56170"),
            new CityWithRelatedInformations("Rennes", StateProvinceArea.Bretagne, Country.France, Continent.Europe, zipCodeFormat:"35#00"),
            new CityWithRelatedInformations("Brest", StateProvinceArea.Bretagne, Country.France, Continent.Europe, zipCodeFormat:"29200"),
            new CityWithRelatedInformations("Nantes", StateProvinceArea.PaysDeLaLoire, Country.France, Continent.Europe, zipCodeFormat:"44###"),
            new CityWithRelatedInformations("Bordeaux", StateProvinceArea.NouvelleAquitaine, Country.France, Continent.Europe, zipCodeFormat:"33#00"),
            new CityWithRelatedInformations("Marseille", StateProvinceArea.ProvenceAlpesCoteDAzur, Country.France, Continent.Europe, zipCodeFormat:"130##"),
            new CityWithRelatedInformations("Nice", StateProvinceArea.ProvenceAlpesCoteDAzur, Country.France, Continent.Europe, zipCodeFormat:"06#00"),
            new CityWithRelatedInformations("Lyon", StateProvinceArea.AuvergneRhoneAlpes, Country.France, Continent.Europe, zipCodeFormat:"6900#"),
            new CityWithRelatedInformations("Grenoble", StateProvinceArea.AuvergneRhoneAlpes, Country.France, Continent.Europe, zipCodeFormat:"38###"),
            new CityWithRelatedInformations("Montpellier", StateProvinceArea.Occitanie, Country.France, Continent.Europe, zipCodeFormat:"340##"),
            new CityWithRelatedInformations("Toulouse", StateProvinceArea.Occitanie, Country.France, Continent.Europe, zipCodeFormat:"31###"),
           
            new CityWithRelatedInformations("London", StateProvinceArea.London, Country.England, Continent.Europe),
            new CityWithRelatedInformations("New York", StateProvinceArea.NewYork, Country.US, Continent.NorthAmerica),

            new CityWithRelatedInformations("Honk-Kong", StateProvinceArea.ChineseAutonomous, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Macau", StateProvinceArea.ChineseAutonomous,Country.China, Continent.Asia),
            new CityWithRelatedInformations("Wuhan", StateProvinceArea.Hubei, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Beijing", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Shanghai", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Tianjin", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Chongqing",StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Danzhou", StateProvinceArea.Hainan, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Kunming", StateProvinceArea.Yunnan, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Lijiang", StateProvinceArea.Yunnan, Country.China, Continent.Asia),
            new CityWithRelatedInformations("Shangri-La", StateProvinceArea.Yunnan, Country.China, Continent.Asia),

            new CityWithRelatedInformations("Mumbai", StateProvinceArea.Maharashtra, Country.India, Continent.Asia),
            new CityWithRelatedInformations("New Delhi", StateProvinceArea.Delhi ,Country.India, Continent.Asia),
            new CityWithRelatedInformations("Bangalore", StateProvinceArea.Karnataka, Country.India, Continent.Asia),
            
            new CityWithRelatedInformations("Bangui", StateProvinceArea.Bangui, Country.CentralAfricanRepublic, Continent.Africa),
            new CityWithRelatedInformations("Dakar", StateProvinceArea.Dakar,Country.Senegal, Continent.Africa),
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

        public static string GiveMeZipCodeFormatOf(string cityName)
        {
            return _citiesOfTheWorld.Single(c => c.CityName == cityName).ZipCodeFormat;
        }
    }
}