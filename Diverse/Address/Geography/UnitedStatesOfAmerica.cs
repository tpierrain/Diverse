namespace Diverse.Address.Geography
{
    /// <summary>
    /// Provides samples of what could be find in <see cref="Country.Usa"/>.
    /// </summary>
    public static class UnitedStatesOfAmerica
    {
        /// <summary>
        /// Gets the list of <see cref="CityWithRelatedInformations"/> one can find in <see cref="Country.Usa"/>.
        /// </summary>
        public static CityWithRelatedInformations[] Cities = new[]
        {
            new CityWithRelatedInformations("New York", StateProvinceArea.NewYork, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"1001#"),
            new CityWithRelatedInformations("Los Angeles", StateProvinceArea.California, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"900##"),
            new CityWithRelatedInformations("Denver", StateProvinceArea.Colorado, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"8020#"),
            new CityWithRelatedInformations("Chicago", StateProvinceArea.Illinois, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"606##"),
            new CityWithRelatedInformations("Seattle", StateProvinceArea.Washington, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Washington", StateProvinceArea.DC, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Miami", StateProvinceArea.Florida, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Las Vegas", StateProvinceArea.Nevada, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("New Orleans", StateProvinceArea.Louisiana, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Houston", StateProvinceArea.Texas, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Milwaukee", StateProvinceArea.Wisconsin, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Philadelphia", StateProvinceArea.Pennsylvania, Country.Usa, Continent.NorthAmerica),
            new CityWithRelatedInformations("Phoenix", StateProvinceArea.Arizona, Country.Usa, Continent.NorthAmerica),
        };

        /// <summary>
        /// Gets samples of street names one can find in <see cref="Country.Usa"/>.
        /// </summary>
        public static string[] StreetNames = new[]
        {
           "1st Avenue",
           "2nd Avenue",
           "3th Avenue",
           "4th Avenue",
           "5th Avenue",
           "6th Avenue",
           "7th Avenue",
           "8th Avenue",
           "14th Street",
           "West 28th Street",
           "West 36th Street",
           "Canal Street",
           "Lafayette Avenue",
           "Nevins Street",
           "Francisco Street",
           "Wilshire Boulevard",
           "West Century Boulevard",
           "Ocean Avenue",
           "Stout Street",
           "Grant Street",
           "Amphitheatre Parkway Mountain View",
           "North State Street",
           "North Wabash Avenue",
        };

        /*

        100##
        485 7th Avenue, New York, NY 10018, États-Unis 
        105 West 28th Street, Chelsea, New York, NY 10001, États-Unis
        338 West 36th Street, Hell's Kitchen, New York, NY 10018, États-Unis
        370 Canal Street, TriBeCa, New York, NY 10013, États-Unis
        378 Lafayette Avenue, Brooklyn, NY
        46 Nevins Street, Downtown Brooklyn, Brooklyn, NY 11217, États-Unis

        900##
        899 Francisco Street, Centre de Los Angeles, Los Angeles, CA 90017, États-Unis
        900 Wilshire Boulevard, Centre de Los Angeles, Los Angeles, 90017, États-Unis
        101 West Century Boulevard, Los Angeles, CA 90045, États-Unis 
        1515 Ocean Avenue, Santa Monica, Los Angeles, CA 90401, États-Unis 

        1600 Amphitheatre Parkway Mountain View, CA 94043, USA

        Denver
        8020#
        440 14Th Street, Denver, CO 80202 , États-Unis 
        1420 Stout Street, Denver, CO 80202, États-Unis
        1776 Grant Street, Capitol Hill, Denver, CO 80203, États-Unis
        
        606##
        660 North State Street, River North, Chicago, IL 60654, États-Unis
        203 N Wabash Ave, Le Loop (Chicago), Chicago, 60601, États-Unis 
        221 North Columbus Drive, Le Loop (Chicago), Chicago, IL 60601, États-Unis
         330 North Wabash Avenue, River North, Chicago, IL 60611, États-Unis 
         */
    }
}