namespace Diverse.Address
{
    /// <summary>
    /// Provides samples of what could be find in <see cref="Country.France"/>.
    /// </summary>
    public static class France
    {
        /// <summary>
        /// Gets the list of <see cref="CityWithRelatedInformations"/> one can find in <see cref="Country.France"/>.
        /// </summary>
        public static CityWithRelatedInformations[] Cities = new []
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
        };

        /// <summary>
        /// Gets samples of street names one could find in <see cref="Country.France"/>.
        /// </summary>
        public static string[] StreetNames = new[]
        {
            "rue Anatole France", "rue des Martyrs", "bd Saint-Germain", "rue du Commandant Cartouche",
            "rue de la palissade", "rue de la Gare", "rue de la Poste", "fronton des épivents",
            "rue des Archives", "rue Tristan Tzara", "rue de l'évangile", "bd de la Somme",
            "rue des flots bleus", "rue de la résistance", "bd de la Mer", "rue Paul Doumer",
            "cours Saint-Louis", "rue Albert", "rue Condorcet", "cour du Médoc", "rue Camille Godard",
            "rue Frère", "promenade du Peyrou", "Cours Gambetta", "rue Oberkampf", "avenue de la liberté",
            "rue de Toiras", "avenue de la Croix du Capitaine", "avenue de Lodève", "bd des Arceaux",
            "place Bellecour", "place des fêtes", "rue Pablo Picasso", "rue Garibaldi", "quai Claude-Bernard",
            "quai des Orfèvres", "avenue Jean Jaurès", "rue Pasteur", "rue des Calanques", "rue de la Loge",
            "rue des Caisseries", "rue de la République", "avenue de la République", "rue Paradis",
            "rue du Parlement", "rue Jean Guéhenno", "bd Maréchal de Lattre de Tassigny", "rue Albert Camus",
        };
    }
}