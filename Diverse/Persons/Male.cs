using System.Linq;

namespace Diverse
{
    /// <summary>
    /// Information related to male persons.
    /// </summary>
    public static class Male
    {
        public static ContextualizedFirstName[] ContextualizedFirstNames => _contextualizedFirstNames;

        private static string[] _firstNames = null;

        public static string[] FirstNames
        {
            get
            {
                if (_firstNames == null)
                {
                    _firstNames =  Male.ContextualizedFirstNames.Select(m => m.FirstName).ToArray();
                }

                return _firstNames;
            }
        }

        private static readonly ContextualizedFirstName[] _contextualizedFirstNames = new ContextualizedFirstName[]
        {
            new ContextualizedFirstName("Moussa", Continent.Africa),
            new ContextualizedFirstName("Djibril", Continent.Africa),
            new ContextualizedFirstName("Issa", Continent.Africa),
            new ContextualizedFirstName("Sékou", Continent.Africa),
            new ContextualizedFirstName("Djiby", Continent.Africa),
            new ContextualizedFirstName("Koffi", Continent.Africa),
            new ContextualizedFirstName("Mahdi", Continent.Africa),
            new ContextualizedFirstName("Demba", Continent.Africa),
            new ContextualizedFirstName("Mamadou", Continent.Africa),
            new ContextualizedFirstName("Atef", Continent.Africa),
            new ContextualizedFirstName("Ousmane", Continent.Africa),
            new ContextualizedFirstName("Ladji", Continent.Africa),
            new ContextualizedFirstName("Aly-Bocar", Continent.Africa),
            new ContextualizedFirstName("Uélé", Continent.Africa),

            new ContextualizedFirstName("Ali", Continent.Africa),
            new ContextualizedFirstName("Karim", Continent.Africa),
            new ContextualizedFirstName("Mohammed", Continent.Africa),
            new ContextualizedFirstName("Mehdi", Continent.Africa),
            new ContextualizedFirstName("Noam", Continent.Africa),
            new ContextualizedFirstName("Jalil", Continent.Africa),
            new ContextualizedFirstName("Malik", Continent.Africa),
            new ContextualizedFirstName("Hakim", Continent.Africa),
            new ContextualizedFirstName("Hisham", Continent.Africa),
            new ContextualizedFirstName("Abdourahmane", Continent.Africa),
            new ContextualizedFirstName("Youssef", Continent.Africa),
            new ContextualizedFirstName("Lounes", Continent.Africa),
            new ContextualizedFirstName("Khaled", Continent.Africa),

            new ContextualizedFirstName("Aleksander", Continent.Antartica),
            new ContextualizedFirstName("Ansgar", Continent.Antartica),
            new ContextualizedFirstName("Einar", Continent.Antartica),
            new ContextualizedFirstName("Casper", Continent.Antartica),
            new ContextualizedFirstName("Eirik", Continent.Antartica),
            new ContextualizedFirstName("Enok", Continent.Antartica),
            new ContextualizedFirstName("Espen", Continent.Antartica),
            new ContextualizedFirstName("Frans", Continent.Antartica),
            new ContextualizedFirstName("Gjurd", Continent.Antartica),
            new ContextualizedFirstName("Gunne", Continent.Antartica),
            new ContextualizedFirstName("Olle", Continent.Antartica),
            new ContextualizedFirstName("Oluf", Continent.Antartica),
            new ContextualizedFirstName("Bjorn", Continent.Antartica),
            
            
            new ContextualizedFirstName("Yi", Continent.Asia),
            new ContextualizedFirstName("Hoàng", Continent.Asia),
            new ContextualizedFirstName("Bao", Continent.Asia),
            new ContextualizedFirstName("Hoà", Continent.Asia),
            new ContextualizedFirstName("Quang", Continent.Asia),
            new ContextualizedFirstName("Toàn", Continent.Asia),
            new ContextualizedFirstName("Vinh", Continent.Asia),
            new ContextualizedFirstName("Jian", Continent.Asia),
            new ContextualizedFirstName("Jin", Continent.Asia),
            new ContextualizedFirstName("Cheng", Continent.Asia),
            new ContextualizedFirstName("Tian", Continent.Asia),
            new ContextualizedFirstName("Zhen", Continent.Asia),
            new ContextualizedFirstName("Ba-Thong", Continent.Asia),

            new ContextualizedFirstName("Aslan", Continent.Asia),
            new ContextualizedFirstName("Eren", Continent.Asia),
            new ContextualizedFirstName("Enis", Continent.Asia),
            new ContextualizedFirstName("Kaan", Continent.Asia),
            new ContextualizedFirstName("Aylan", Continent.Asia),
            new ContextualizedFirstName("Mehmet", Continent.Asia),
            new ContextualizedFirstName("Ömer", Continent.Asia),
            new ContextualizedFirstName("Kim", Continent.Asia),
            new ContextualizedFirstName("Kwon", Continent.Asia),
            new ContextualizedFirstName("Hwang", Continent.Asia),
            new ContextualizedFirstName("Abraham", Continent.Asia),
            new ContextualizedFirstName("Elias", Continent.Asia),

            new ContextualizedFirstName("Kumar", Continent.Asia),
            new ContextualizedFirstName("Arjun", Continent.Asia),
            new ContextualizedFirstName("Aadesh", Continent.Asia),
            new ContextualizedFirstName("Abhimanyu", Continent.Asia),
            new ContextualizedFirstName("Adrith", Continent.Asia),
            new ContextualizedFirstName("Akshay", Continent.Asia),
            new ContextualizedFirstName("Kamal", Continent.Asia),
            new ContextualizedFirstName("Lavish", Continent.Asia),
            new ContextualizedFirstName("Krishna", Continent.Asia),
            new ContextualizedFirstName("Rajiv", Continent.Asia),
            new ContextualizedFirstName("Sumedh", Continent.Asia),
            new ContextualizedFirstName("Jaycarran", Continent.Asia),
            new ContextualizedFirstName("Ashok", Continent.Asia),




            new ContextualizedFirstName("Achille", Continent.Europe),
            new ContextualizedFirstName("Markus", Continent.Europe),
            new ContextualizedFirstName("Wolfgang", Continent.Europe),
            new ContextualizedFirstName("Lukas", Continent.Europe),
            new ContextualizedFirstName("Karl", Continent.Europe),
            new ContextualizedFirstName("Edouard", Continent.Europe),
            new ContextualizedFirstName("Jean-Pierre", Continent.Europe),
            new ContextualizedFirstName("Dimitri", Continent.Europe),
            new ContextualizedFirstName("Jeremie", Continent.Europe),
            new ContextualizedFirstName("Louis", Continent.Europe),
            new ContextualizedFirstName("Thomas", Continent.Europe),
            new ContextualizedFirstName("Silvin", Continent.Europe),
            new ContextualizedFirstName("Jérôme", Continent.Europe),
            
            new ContextualizedFirstName("Julien", Continent.Europe),
            new ContextualizedFirstName("Bruno", Continent.Europe),
            new ContextualizedFirstName("Sacha", Continent.Europe),
            new ContextualizedFirstName("Anton", Continent.Europe),
            new ContextualizedFirstName("Vadim", Continent.Europe),
            new ContextualizedFirstName("Maxime", Continent.Europe),
            new ContextualizedFirstName("Mikhail", Continent.Europe),
            new ContextualizedFirstName("Alexei", Continent.Europe),
            
            new ContextualizedFirstName("Sonny", Continent.NorthAmerica),
            new ContextualizedFirstName("Barack", Continent.NorthAmerica),
            new ContextualizedFirstName("David", Continent.NorthAmerica),
            new ContextualizedFirstName("Kevin", Continent.NorthAmerica),
            new ContextualizedFirstName("John", Continent.NorthAmerica),
            new ContextualizedFirstName("Jacob", Continent.NorthAmerica),
            new ContextualizedFirstName("Liam", Continent.NorthAmerica),
            new ContextualizedFirstName("Sean", Continent.NorthAmerica),
            new ContextualizedFirstName("Connor", Continent.NorthAmerica),
            new ContextualizedFirstName("James", Continent.NorthAmerica),
            new ContextualizedFirstName("George", Continent.NorthAmerica),
            new ContextualizedFirstName("Glenn", Continent.NorthAmerica),
            new ContextualizedFirstName("Matthew", Continent.NorthAmerica),
            new ContextualizedFirstName("Jerry", Continent.NorthAmerica),
            
            new ContextualizedFirstName("Adriano", Continent.SouthAmerica),
            new ContextualizedFirstName("Alejandro", Continent.SouthAmerica),
            new ContextualizedFirstName("Alberto", Continent.SouthAmerica),
            new ContextualizedFirstName("Alberto", Continent.SouthAmerica),
            new ContextualizedFirstName("Esteban", Continent.SouthAmerica),
            new ContextualizedFirstName("Abraão", Continent.SouthAmerica),
            new ContextualizedFirstName("Alfonso", Continent.SouthAmerica),
            new ContextualizedFirstName("Javier", Continent.SouthAmerica),
            new ContextualizedFirstName("Angel", Continent.SouthAmerica),
            new ContextualizedFirstName("Sergio", Continent.SouthAmerica),
            new ContextualizedFirstName("Diego", Continent.SouthAmerica),
            new ContextualizedFirstName("Hernán", Continent.SouthAmerica),
            new ContextualizedFirstName("Héctor", Continent.SouthAmerica),
            new ContextualizedFirstName("Jesús", Continent.SouthAmerica),
            new ContextualizedFirstName("Carlos", Continent.SouthAmerica),
            
            
            new ContextualizedFirstName("Jomdah", Continent.Australia),
            new ContextualizedFirstName("Mallinbilly", Continent.Australia),
            new ContextualizedFirstName("Bilaranora", Continent.Australia),
            new ContextualizedFirstName("Naibar", Continent.Australia),
            new ContextualizedFirstName("Guemajarry", Continent.Australia),
            new ContextualizedFirstName("Linjah", Continent.Australia),
            new ContextualizedFirstName("Drell", Continent.Australia),
            
            new ContextualizedFirstName("Vaea", Continent.Australia),
            new ContextualizedFirstName("Temoe", Continent.Australia),
            new ContextualizedFirstName("Manua", Continent.Australia),
            new ContextualizedFirstName("Teiki", Continent.Australia),
            new ContextualizedFirstName("Rahiti", Continent.Australia),
            new ContextualizedFirstName("Aata", Continent.Australia),
            new ContextualizedFirstName("Heimanu", Continent.Australia),

        };
    }
}