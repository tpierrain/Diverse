namespace Diverse.Address.Geography.Countries
{
    /// <summary>
    /// Provides samples of what could be find in <see cref="Country.Usa"/>.
    /// </summary>
    public static class UnitedStatesOfAmerica
    {
        /// <summary>
        /// Gets the list of <see cref="CityWithRelatedInformation"/> one can find in <see cref="Country.Usa"/>.
        /// </summary>
        public static CityWithRelatedInformation[] Cities = new[]
        {
            new CityWithRelatedInformation("New York", StateProvinceArea.NewYork, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"1001#"),
            new CityWithRelatedInformation("Los Angeles", StateProvinceArea.California, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"900##"),
            new CityWithRelatedInformation("Denver", StateProvinceArea.Colorado, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"8020#"),
            new CityWithRelatedInformation("Chicago", StateProvinceArea.Illinois, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"606##"),
            new CityWithRelatedInformation("Seattle", StateProvinceArea.Washington, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"981##"),
            new CityWithRelatedInformation("Washington", StateProvinceArea.DC, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"2000#"),
            new CityWithRelatedInformation("Miami", StateProvinceArea.Florida, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"3314#"),
            new CityWithRelatedInformation("Las Vegas", StateProvinceArea.Nevada, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"891##"),
            new CityWithRelatedInformation("New Orleans", StateProvinceArea.Louisiana, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"701##"),
            new CityWithRelatedInformation("Houston", StateProvinceArea.Texas, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"770##"),
            new CityWithRelatedInformation("Milwaukee", StateProvinceArea.Wisconsin, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"5320#"),
            new CityWithRelatedInformation("Philadelphia", StateProvinceArea.Pennsylvania, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"191##"),
            new CityWithRelatedInformation("Phoenix", StateProvinceArea.Arizona, Country.Usa, Continent.NorthAmerica, zipCodeFormat:"850##"),
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
           "Boren Avenue",
           "South King Street",
           "K Street NW",
           "15th Street North West",
           "17th Street North West",
           "Collins Avenue",
           "Harding Avenue",
           "Giles Street",
           "Saint Rose Parkway",
           "South Boulevard",
           "Poydras St",
           "Magnolia St",
           "Philip St",
           "Martin Luther King Jr Blvd",
           "Loyola Avenue",
           "Bourbon Street",
           "West Loop South",
           "Fannin Street",
           "Main Street",
           "West Kilbourn Avenue",
           "East Wisconsin Avenue",
           "South Howell Avenue",
           "Market Street",
           "Tinicum Boulevard",
           "E Van Buren St",
           "North 44th Street",
           "West Willetta Street"
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

        981##
        401 Lenora Street, Belltown, Seattle, WA 98121, États-Unis
        1821 Boren Avenue, Belltown, Seattle, WA 98101, États-Unis
        455 South King Street, Pioneer Square, Seattle, WA 98104, États-Unis 

        2000#
        1522 K Street NW, Northwest, Washington, DC 20005, États-Unis 
         839 17th Street, NW, Northwest, Washington, DC 20006, États-Unis
        806 15th Street North West, Northwest, Washington, DC 20005, États-Unis
        
        3314#
        1545 Collins Avenue, Miami Beach, FL 33139, États-Unis
        2901 Collins Avenue, Miami Beach, FL 33140, États-Unis 
        8601 Harding Avenue, Miami Beach, FL 33141, États-Unis

        891##
        3570 Las Vegas Boulevard South, Strip, Las Vegas, NV 89109, États-Uni
        3600 Las Vegas Boulevard South, Strip, Las Vegas, NV 89109, États-Unis
        7850 Giles Street, Las Vegas, NV 89123, États-Unis
        3245 Saint Rose Parkway, Henderson, Las Vegas, NV 89052, États-Unis

         
        701##
        300 Bourbon Street, Quartier français (Vieux Carré), La Nouvelle-Orléans, LA 70130, États-Unis 
        330 Loyola Avenue, Quartier central des affaires de La Nouvelle-Orléans, La Nouvelle-Orléans, LA 70112,

        770##
        2222 West Loop South, Houston, TX 77027, États-Unis
         820 Fannin Street, Downtown Houston, Houston, TX 77002, États-Unis
        6800 Main Street, Houston, TX 77030, États-Unis

        5320#
        333 West Kilbourn Avenue, Milwaukee, WI 53203, États-Unis 
        424 East Wisconsin Avenue, Milwaukee, 53202, États-Unis
        5105 South Howell Avenue, Milwaukee, WI 53207, États-Unis

        191##
        1800 Market Street, Philadelphie, PA 19103, États-Unis
        117 South 17th Street, Philadelphie, PA 19103, États-Unis 
        9000 Tinicum Boulevard, Philadelphie, PA 19153, États-Unis

        850##
        3037 E Van Buren St , Centre de Phoenix, Phoenix, 85008-6806, États-Unis
        320 North 44th Street, Camelback East, Phoenix, AZ 85008, États-Unis
         5215 West Willetta Street, Phoenix, AZ 85043, États-Unis

         */
    }
}