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

        /// <summary>
        /// Gets the names of all the cities registered in the Diverse lib.
        /// </summary>
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

        /// <summary>
        /// Gives an array with all the cities related to a <see cref="Country"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/>.</param>
        /// <returns>An array with all the cities related to a <see cref="Country"/>.</returns>
        public static string[] GiveMeCitiesOf(Country country)
        {
            return _citiesOfTheWorld
                .Where(cw => cw.Country == country)
                .Select(x => x.CityName)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Gives an array with all the <see cref="StateProvinceArea"/> related to a <see cref="Country"/>.
        /// /// </summary>
        /// <param name="country">The <see cref="Country"/>.</param>
        /// <returns>An array of all the <see cref="StateProvinceArea"/> registered for this <see cref="Country"/> in the Diverse lib.</returns>
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
            new CityWithRelatedInformations("New York", StateProvinceArea.NewYork, Country.Usa, Continent.NorthAmerica),

            new CityWithRelatedInformations("Wuhan", StateProvinceArea.Hubei, Country.China, Continent.Asia, zipCodeFormat:"4300##"),
            new CityWithRelatedInformations("Beijing", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia, zipCodeFormat:"1000##"),
            new CityWithRelatedInformations("Shanghai", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia, zipCodeFormat:"2000##"),
            new CityWithRelatedInformations("Tianjin", StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia, zipCodeFormat:"300###"),
            new CityWithRelatedInformations("Chongqing",StateProvinceArea.ChineseMunicipal, Country.China, Continent.Asia, zipCodeFormat:"4000##"),
            new CityWithRelatedInformations("Danzhou", StateProvinceArea.Hainan, Country.China, Continent.Asia, zipCodeFormat:"5717##"),
            new CityWithRelatedInformations("Kunming", StateProvinceArea.Yunnan, Country.China, Continent.Asia, zipCodeFormat: "650###"),
            new CityWithRelatedInformations("Lijiang", StateProvinceArea.Yunnan, Country.China, Continent.Asia, zipCodeFormat: "6741##"),
            new CityWithRelatedInformations("Shangri-La", StateProvinceArea.Yunnan, Country.China, Continent.Asia, zipCodeFormat:"674400"),

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
                            }},

             { Country.China, new string[]
                            {
                                        "Shuangshiduan, Xinhua Street, Dayan Street",
                                        "Zhonghe Rd,Shuhe Ancient Town",
                                        "Xiangjiang Road, Gucheng District",
                                        "Guangbi Alley Guangyi Street",
                                        "DongSanHuan Middle Road, Chao-Yang",
                                        "Jinyu Hutong Wangfujing, Dongcheng",
                                        "North Dongsanhua Road, Chao-Yang",
                                        "Yong An Dong Li, Jian Guo Men Wai Avenue, Chao-Yang",
                                        "Hong Hua Qiao, Wuhua District",
                                        "Qianxing Road, Xishan District, Xishan District",
                                        "Zhong Shan San Lu, Yu Zhong",
                                        "Changjiang Binjiang Road，Yuzhong District, Yu Zhong",
                                        "Chicika Street, Jiantang Town",
                                        "Biesailang, the ancient city of dukezong",
                                        "Binjiang Ave, Hanyang District",
                                        "Zhongshan Avenue, Qiaokou District, Hankou (CapitaMall Wusheng Road), Qiaokou District",
                                        "Guosheng Road",
                                        "E Man Town",
                                        "Chifeng Road, Intersection of Nanjing Road and Harbin Road, Heping District",
                                        "Phoenix Shopping Mall, East Haihe Road, Hebei",
                                        "Zhong Shan Dong Yi Road, Huangpu",
                                        "Middle Yan'an Road, Jing'an",
                            }}

            };

        /*
         
        6741##
         No.38, Shuangshiduan, Xinhua Street, Dayan Street, 674100 Lijiang, Chine 
        No. 33 Zhonghe Rd,Shuhe Ancient Town, 674100 Lijiang, Chine
         8 Xiangjiang Road, Gucheng District, 674199 Lijiang, Chine
        57 Guangbi Alley Guangyi Street, 674100 Lijiang, Chine 
         
        
        1000##
        Fortune Plaza No. 7 DongSanHuan Middle Road, Chao-Yang, 100020 Pékin, Chine 
         5-15 Jinyu Hutong Wangfujing, Dongcheng, 100006 Pékin, Chine
        29 North Dongsanhua Road, Chao-Yang, 100027 Pékin, Chine
         8 Yong An Dong Li, Jian Guo Men Wai Avenue, Chao-Yang, 100022 Pékin, Chine

        650###
         20 Hong Hua Qiao, Wuhua District, 650031 Kunming, Chine
         No.888 Qianxing Road, Xishan District, Xishan District, 650228 Kunming, Chine 

        4000##
        No 139 Zhong Shan San Lu, Yu Zhong, 400015 Chongqing, Chine
        No.151 Changjiang Binjiang Road，Yuzhong District, Yu Zhong, 400011 


         No.1, Chicika Street, Jiantang Town, Shangri-La City, Diqing Tibetan Autonomous Prefecture, 674400 
        No. 17 Biesailang, the ancient city of dukezong, 674400 Shangri-La, Chine

        4300##
        No.190 Binjiang Ave, Hanyang District , Hanyang District, 430050 Wuhan, Chine – 
        No.238 Zhongshan Avenue, Qiaokou District, Hankou (CapitaMall Wusheng Road), Qiaokou District, 430032 Wuhan, Chine

        5717##
        No.1 Guosheng Road, 571700 Danzhou, Chine 
        No.19 E Man Town, 571745 Danzhou, Chine

        300###
         138 Chifeng Road, Intersection of Nanjing Road and Harbin Road, Heping District, Heping, 300041 Tianjin,
         Phoenix Shopping Mall, East Haihe Road, Hebei, 300141 Tianjin, Chine

        2000##
        No. 2 Zhong Shan Dong Yi Road, Huangpu, 200002 Shanghai, Chine
        No.1218 Middle Yan'an Road, Jing'an, 200040 Shanghai, Chine

        The Londoner Macao, Estrada do Istmo, s/n, Cotai, Macau SAR, P.R. China, Cotai, Macao, Macao
        956-1110 Avenida Da Amizade, Macao, Macao
         Avenida Marginal Flor de Lotus, Cotai, Macao, Macao

        72 Gloucester Road, Wan Chai, Hong Kong, Hong Kong
         1 Harbour Road, Hong Kong, Hong Kong


         */

        /// <summary>
        /// Gives the <see cref="StateProvinceArea"/> of a given city name.
        /// </summary>
        /// <param name="cityName">The name of the city.</param>
        /// <returns>The <see cref="StateProvinceArea"/> where this city belongs.</returns>
        public static StateProvinceArea GiveMeStateProvinceAreaOf(string cityName)
        {
            return _citiesOfTheWorld.Single(c => c.CityName == cityName).StateProvinceArea;
        }

        /// <summary>
        /// Gives the <see cref="Country"/> of a given city name.
        /// </summary>
        /// <param name="cityName">The name of the city.</param>
        /// <returns>The <see cref="Country"/> where this city belongs.</returns>
        public static Country GiveMeCountryOf(string cityName)
        {
            return _citiesOfTheWorld.Single(c => c.CityName == cityName).Country;
        }

        /// <summary>
        /// Gives all the registered streets of a <see cref="Country"/>.
        /// </summary>
        /// <param name="country">The <see cref="Country"/></param>
        /// <returns>All the streets of the provided <see cref="Country"/> that are registered in Diverse lib.</returns>
        public static string[] GiveMeStreetsOf(Country country)
        {
            return _streetsOfCountries[country];
        }

        /// <summary>
        /// Gives the ZipCode format registered for this city in Diverse.
        /// </summary>
        /// <param name="cityName">The name of the city you want to get the ZipCode format.</param>
        /// <returns>The ZipCode format registered for this city in Diverse.</returns>
        public static string GiveMeZipCodeFormatOf(string cityName)
        {
            return _citiesOfTheWorld.Single(c => c.CityName == cityName).ZipCodeFormat;
        }
    }
}