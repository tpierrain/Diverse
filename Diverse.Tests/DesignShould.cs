using System.Collections.Generic;
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
        [Ignore("This test made for documentation is failing on purpose. Ignored to avoid CI breaking.")]
        public void Allow_us_To_avoid_duplication_for_a_group_of_fuzzed_elements_only_otherwise_it_will_throw_for_nothing()
        {
            var fuzzer = new Fuzzer(avoidDuplication: true);

            var brandAAllKindOfStarsHotelGroup = new HotelGroupBuilder(fuzzer, Brand.BrandA)
                .WithHotelIn("Paris")
                .WithHotelIn("Aubervilliers")
                .WithHotelIn("Versailles")
                .WithHotelIn("Nogent sur marne")
                .WithHotelIn("Vincennes")
                .Build();  // (the Build() method will call 5 times fuzzer.GenerateInteger(1, 5))

            // So far we have called 5 times fuzzer.GenerateInteger(1, 5)
            // But since the fuzzer could find 5 different values, we are OK

            // But any other call to fuzzer.GenerateInteger(1, 5) will throw a duplication exception (can't find any non already provided value)
            
            // This is what will happen in the builder calls below.
            
            // THE NEXT LINE WILL THROW ;-(
            var brandBAllKindOfStarsHotelGroup = new HotelGroupBuilder(fuzzer, Brand.BrandB)
                .WithHotelIn("Amsterdam")
                .WithHotelIn("Barcelona")
                .WithHotelIn("Los Angeles")
                .WithHotelIn("Athens")
                .WithHotelIn("Roma")
                .Build(); // (will also call 5 times fuzzer.GenerateInteger(1, 5))

            

            // This is a problem. We need to find another way to express how we want to avoid duplication.
            // So far, I have these options in mind:
            //
            // 1. Remove the "avoid duplication" from the fuzzer level and allow to avoid duplication for some calls only,
            //    through a token/scope object or identifier that could be provided to the fuzzer calls that we want
            //    to associate and avoid duplication among them
            //
            // 2. Keep the "avoid duplication" option in general at the fuzzer level (like today), but with an
            //    optional boolean added for every fuzzer method, that will allow us to escape/avoid the
            //    no-duplication checks for these very specific method calls
            //
            //  What do you think?

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
        private readonly Fuzzer _fuzzer;
        private readonly Brand _brand;
        private HashSet<string> _cities = new HashSet<string>();

        public HotelGroupBuilder(Fuzzer fuzzer, Brand brand)
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