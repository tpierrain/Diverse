using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class CollectionFuzzerShould
    {
        [Test]
        public void Be_able_to_PickOneFrom()
        {
            var fuzzer = new Fuzzer();
            var candidates = new[] {"one", "two", "three", "four", "five"};

            for (var i = 0; i < 10; i++)
            {
                var chosenOne = fuzzer.PickOneFrom(candidates);
                Check.That(chosenOne).IsOneOf(candidates);
            }
        }

        [Test]
        public void Throw_an_Exception_when_list_of_candidates_is_null()
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() =>
            {
                List<string> candidates = null;
                fuzzer.PickOneFrom(candidates);
            }).Throws<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'candidates')");
        }

        [Test]
        public void Throw_an_Exception_when_list_of_candidates_is_empty()
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() =>
            {
                fuzzer.PickOneFrom(new List<string>());
            }).Throws<ArgumentException>().WithMessage("candidates list must not be empty.");
        }
    }
}