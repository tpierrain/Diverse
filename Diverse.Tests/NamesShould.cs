using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class NamesShould
    {
        [Test]
        public void Have_no_duplication_within_female_name()
        {
            var femaleNames = new SortedSet<string>();

            foreach (var firstName in Female.FirstNames)
            {
                femaleNames.Add(firstName);
            }

            Check.That(femaleNames.Count).IsEqualTo(Female.FirstNames.Length);
        }

        [Test]
        public void Have_346_female_names()
        {
            Check.That(Female.FirstNames).HasSize(354);
        }

        [Test]
        public void Have_330_male_names()
        {
            Check.That(Male.FirstNames).HasSize(336);
        }

        [Test]
        public void Have_no_duplication_within_male_name()
        {
            var maleNames = new SortedSet<string>();
            foreach (var firstName in Male.FirstNames)
            {
                maleNames.Add(firstName);
            }

            Check.That(maleNames.Count).IsEqualTo(Male.FirstNames.Length);
        }
    }
}