namespace Diverse.Address.Geography.Countries
{
    /// <summary>
    /// Provides samples of what could be find in <see cref="Country.France"/>.
    /// </summary>
    public static class France
    {
        /// <summary>
        /// Gets the list of <see cref="CityWithRelatedInformation"/> one can find in <see cref="Country.France"/>.
        /// </summary>
        public static CityWithRelatedInformation[] Cities = new []
        {
            new CityWithRelatedInformation("Paris", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat: "7501#"),
            new CityWithRelatedInformation("Saint-Ouen", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe,zipCodeFormat: "93400"),
            new CityWithRelatedInformation("Saint-Denis", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat: "93200"),
            new CityWithRelatedInformation("Versailles", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat:"78000"),
            new CityWithRelatedInformation("La Courneuve", StateProvinceArea.IleDeFrance, Country.France, Continent.Europe, zipCodeFormat:"93120"),
            new CityWithRelatedInformation("Quiberon", StateProvinceArea.Bretagne, Country.France, Continent.Europe, zipCodeFormat:"56170"),
            new CityWithRelatedInformation("Rennes", StateProvinceArea.Bretagne, Country.France, Continent.Europe, zipCodeFormat:"35#00"),
            new CityWithRelatedInformation("Brest", StateProvinceArea.Bretagne, Country.France, Continent.Europe, zipCodeFormat:"29200"),
            new CityWithRelatedInformation("Nantes", StateProvinceArea.PaysDeLaLoire, Country.France, Continent.Europe, zipCodeFormat:"44###"),
            new CityWithRelatedInformation("Bordeaux", StateProvinceArea.NouvelleAquitaine, Country.France, Continent.Europe, zipCodeFormat:"33#00"),
            new CityWithRelatedInformation("Marseille", StateProvinceArea.ProvenceAlpesCoteDAzur, Country.France, Continent.Europe, zipCodeFormat:"1301#"),
            new CityWithRelatedInformation("Nice", StateProvinceArea.ProvenceAlpesCoteDAzur, Country.France, Continent.Europe, zipCodeFormat:"06#00"),
            new CityWithRelatedInformation("Lyon", StateProvinceArea.AuvergneRhoneAlpes, Country.France, Continent.Europe, zipCodeFormat:"6900#"),
            new CityWithRelatedInformation("Grenoble", StateProvinceArea.AuvergneRhoneAlpes, Country.France, Continent.Europe, zipCodeFormat:"38###"),
            new CityWithRelatedInformation("Montpellier", StateProvinceArea.Occitanie, Country.France, Continent.Europe, zipCodeFormat:"340##"),
            new CityWithRelatedInformation("Toulouse", StateProvinceArea.Occitanie, Country.France, Continent.Europe, zipCodeFormat:"31###"),
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