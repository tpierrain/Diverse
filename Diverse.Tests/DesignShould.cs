using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// A naive test to illustrate a design issue I just found yesterday
    /// </summary>
    [TestFixture]
    public class DesignShould
    {
        [Test]
        public void Allow_us_To_avoid_duplication_for_a_group_of_fuzzed_elements_only_otherwise_it_will_throw_for_nothing()
        {
            var fuzzer = new Fuzzer(avoidDuplication:true);

            var set = new HashSet<int>();
            for (var i = 0; i < 5; i++)
            {
                set.Add(fuzzer.GenerateInteger(1, 5));
            }

            Check.That(set).Contains(1, 2, 3, 4, 5);

            Check.ThatCode(() => fuzzer.GenerateInteger(1, 5)).Throws<DuplicationException>();

            var brandAAllKindOfStarsHotelGroup = new HotelGroupBuilder(fuzzer.GenerateFuzzerProvidingNoDuplication(), Brand.BrandA)
                .WithHotelIn("Paris")
                .WithHotelIn("Aubervilliers")
                .WithHotelIn("Versailles")
                .WithHotelIn("Nogent sur marne")
                .WithHotelIn("Vincennes")
                .Build();  // (the Build() method will call 5 times fuzzer.GenerateInteger(1, 5))

            Check.That(brandAAllKindOfStarsHotelGroup.Hotels.Select(h => h.Stars)).Contains(1, 2, 3, 4, 5);

            // So far we have called 5 times fuzzer.GenerateInteger(1, 5)
            // But since the fuzzer could find 5 different values, we are OK

            // But any other call to fuzzer.GenerateInteger(1, 5) will throw a duplication exception (can't find any non already provided value)

            // This is what will happen in the builder calls below.

            // THE NEXT LINE WILL THROW ;-(

            var brandBAllKindOfStarsHotelGroup = new HotelGroupBuilder(fuzzer.GenerateFuzzerProvidingNoDuplication(), Brand.BrandB)
                .WithHotelIn("Amsterdam")
                .WithHotelIn("Barcelona")
                .WithHotelIn("Los Angeles")
                .WithHotelIn("Athens")
                .WithHotelIn("Roma")
                .Build(); // (will also call 5 times fuzzer.GenerateInteger(1, 5))

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