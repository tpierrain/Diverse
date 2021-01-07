using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// Tests related to the core features of the Fuzzer.
    /// </summary>
    [TestFixture]
    public class FuzzerShould
    {
        [Test]
        public void Indicate_what_to_do_in_the_DuplicationException_message_when_using_NoDuplication_mode()
        {
            var fuzzer = new Fuzzer(noDuplication: true);

            var set = new HashSet<int>();
            for (var i = 0; i < 5; i++)
            {
                set.Add(fuzzer.GenerateInteger(1, 5));
            }

            Check.That(set).Contains(1, 2, 3, 4, 5);

            // So far we have called 5 times fuzzer.GenerateInteger(1, 5)
            // But since the fuzzer could find 5 different values, we are OK

            Check.ThatCode(() =>
                {
                    // But this will be the call one should not make ;-)
                    return fuzzer.GenerateInteger(1, 5);
                })
                .Throws<DuplicationException>()
                .WithMessage(@$"Couldn't find a non-already provided value of System.Int32 after 100 attempts. Already provided values: 1, 2, 3, 4, 5. You can either:
- Generate a new specific fuzzer to ensure no duplication is provided for a sub-group of fuzzed values (anytime you want through the {nameof(IFuzz.GenerateFuzzerProvidingNoDuplication)}() method of your current Fuzzer instance. E.g.: var tempFuzzer = fuzzer.{nameof(IFuzz.GenerateFuzzerProvidingNoDuplication)}();)
- Increase the value of the {nameof(Fuzzer.MaxFailingAttemptsForNoDuplication)} property for your {nameof(IFuzz)} instance.");

        }

        [Test]
        public void Allow_us_to_avoid_duplication_but_only_for_various_sub_groups_of_fuzzed_elements_via_the_GenerateFuzzerProvidingNoDuplication_method()
        {
            var fuzzer = new Fuzzer();
            
            var specificFuzzerWithNoDuplication = fuzzer.GenerateFuzzerProvidingNoDuplication();
            var brandAAllKindOfStarsHotelGroup = new HotelGroupBuilder(specificFuzzerWithNoDuplication, Brand.BrandA)
                .WithHotelIn("Paris")
                .WithHotelIn("Aubervilliers")
                .WithHotelIn("Versailles")
                .WithHotelIn("Nogent sur marne")
                .WithHotelIn("Vincennes")
                .Build();  // (the Build() method will call 5 times fuzzer.GenerateInteger(1, 5))

            Check.That(brandAAllKindOfStarsHotelGroup.Hotels.Select(h => h.Stars)).Contains(1, 2, 3, 4, 5);

            // Another one.
            var anotherSpecificFuzzerWithNoDuplication = specificFuzzerWithNoDuplication.GenerateFuzzerProvidingNoDuplication();
            var brandBAllKindOfStarsHotelGroup = new HotelGroupBuilder(anotherSpecificFuzzerWithNoDuplication, Brand.BrandB)
                .WithHotelIn("Amsterdam")
                .WithHotelIn("Barcelona")
                .WithHotelIn("Los Angeles")
                .WithHotelIn("Athens")
                .WithHotelIn("Roma")
                .Build(); // (will also call 5 times fuzzer.GenerateInteger(1, 5). One should not throw DuplicationException)

            Check.That(brandBAllKindOfStarsHotelGroup.Hotels.Select(h => h.Stars)).Contains(1, 2, 3, 4, 5);
        }
    }

    /// <summary>
    /// Hotel Brand
    /// </summary>
    public enum Brand
    {
        BrandA,
        BrandB,
        BrandC
    }

    public class HotelGroupBuilder
    {
        private readonly IFuzz _fuzzer;
        private readonly Brand _brand;
        private HashSet<string> _cities = new HashSet<string>();

        public HotelGroupBuilder(IFuzz fuzzer, Brand brand)
        {
            _fuzzer = fuzzer;
            _brand = brand;
        }

        public HotelGroupBuilder WithHotelIn(string city)
        {
            _cities.Add(city);

            return this;
        }

        public HotelGroup Build()
        {
            var hotelGroup = new HotelGroup();
            foreach (var city in _cities)
            {
                var hotelName = $"Hôtel {_fuzzer.GenerateFirstName()}";
                var numberOfStars = _fuzzer.GenerateInteger(1, 5);
                var hotelDescription = new HotelDescription(_brand, hotelName, city, numberOfStars);

                hotelGroup.AddHotel(hotelDescription);
            }

            return hotelGroup;
        }
    }

    public class HotelGroup
    {
        public HashSet<HotelDescription> Hotels { get; } = new HashSet<HotelDescription>();

        public void AddHotel(HotelDescription hotel)
        {
            Hotels.Add(hotel);
        }
    }



    public class HotelDescription
    {
        public Brand Brand { get; }
        public string Name { get; }
        public string City { get; }
        public int Stars { get; }

        public HotelDescription(Brand brand, string name, string city, int stars)
        {
            Brand = brand;
            Name = name;
            City = city;
            Stars = stars;
        }
    }
}