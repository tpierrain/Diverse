using System;

namespace Diverse
{
    /// <summary>
    /// Represent a <see cref="Person"/> with some common characteristics.
    /// Very useful to test most of the application requesting Accounts.
    /// </summary>
    public class Person
    {
        public string FirstName { get; }
        public string LastName { get; }
        public Gender Gender { get; }
        public GenderTitle Title { get; }
        public string EMail { get; }
        public bool IsMarried { get;  }
        public int Age { get; }

        internal Person(string firstName, string lastName, Gender gender, string eMail, bool isMarried, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            EMail = eMail;
            IsMarried = isMarried;
            Age = age;
            switch (gender)
            {
                case Gender.Male:
                    Title = GenderTitle.Mr;
                    break;
                case Gender.Female:
                    Title = isMarried ? GenderTitle.Mrs : GenderTitle.Ms;
                    break;
                case Gender.NonBinary:
                    Title = GenderTitle.Mx;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender), gender, "not handled properly yet.");
            }
        }

        public override string ToString()
        {
            return $"{Title}. {FirstName} {LastName.ToUpper()} ({Gender}) {EMail} (is married: {IsMarried} - age: {Age} yrs)";
        }
    }
}