using System;

namespace Diverse.Numbers
{
    /// <summary>
    /// Fuzz numbers.
    /// </summary>
    internal class NumberFuzzer : IFuzzNumbers
    {
        private readonly IFuzz _fuzzer;

        /// <summary>
        /// Instantiates a <see cref="NumberFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public NumberFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Generates a random integer value between a min (inclusive) and a max (exclusive) value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>An integer value generated randomly.</returns>
        public int GenerateInteger(int? minValue = null, int? maxValue = null)
        {
            minValue = minValue ?? int.MinValue;
            maxValue = maxValue ?? int.MaxValue;

            // Adjust the inclusiveness of the Fuzzer API to the exclusiveness of the Random API.
            maxValue = (maxValue == int.MaxValue) ? maxValue : maxValue + 1;

            return _fuzzer.Random.Next(minValue.Value, maxValue.Value);
        }

        /// <summary>
        /// Generates a random positive integer value.
        /// </summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A positive integer value generated randomly.</returns>
        public int GeneratePositiveInteger(int? maxValue = null)
        {
            maxValue = maxValue ?? int.MaxValue;

            return GenerateInteger(0, maxValue.Value);
        }

        /// <summary>
        /// Generates a random decimal value.
        /// </summary>
        /// <param name="minValue">(optional) The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">(optional) The inclusive upper bound of the random number returned.</param>
        /// <returns>A decimal value generated randomly.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref>.</exception>
        public decimal GenerateDecimal(decimal? minValue = null, decimal? maxValue = null)
        {
            // cross guard here
            minValue = minValue ?? decimal.MinValue;
            maxValue = maxValue ?? decimal.MaxValue;

            ThrowIfMinGreaterThanMax(minValue, maxValue);

            var minValueScale = minValue.Value.ExtractScale();
            var maxValueScale = maxValue.Value.ExtractScale();

            var isNegative = (minValue < 0) ? _fuzzer.HeadsOrTails() : false;

            byte scale;
            if (isNegative)
            {
                scale = FuzzDecimalScaleGreaterOrEqualToMinValueScale(minValueScale);
            }
            else
            {
                scale = FuzzDecimalScaleLowerOrEqualToMaxValueScale(maxValueScale);
            }

            var low = _fuzzer.GenerateInteger();
            var mid = _fuzzer.GenerateInteger();
            var hi = _fuzzer.GenerateInteger();

            var result = new decimal(low, mid, hi, isNegative, scale);

            if (result < minValue.Value || result > maxValue.Value)
            {
                // fix
                result = TakeAValueInBetweenOrARandomBound(minValue.Value, maxValue.Value);
            }

            return result;
        }

        private decimal TakeAValueInBetweenOrARandomBound(decimal minValue, decimal maxValue)
        {
            decimal result;
            var maxIncrement = Math.Abs(maxValue - minValue);
            var increment = (maxIncrement / _fuzzer.GenerateInteger(2, 15));

            if (increment != 0)
            {
                if (_fuzzer.HeadsOrTails())
                {
                    result = minValue + increment;
                }
                else
                {
                    increment = (increment / _fuzzer.GenerateInteger(2, 15));

                    if (increment != 0)
                    {
                        result = minValue + increment;
                    }
                    else
                    {
                        result = _fuzzer.HeadsOrTails() ? minValue : maxValue;
                    }
                }
            }
            else
            {
                result = _fuzzer.HeadsOrTails() ? minValue : maxValue;
            }

            return result;
        }

        private byte FuzzDecimalScaleLowerOrEqualToMaxValueScale(byte maxValueScale)
        {
            return NumberExtensions.FuzzDecimalScaleBetween(0, maxValueScale, _fuzzer.Random);
        }

        private byte FuzzDecimalScaleGreaterOrEqualToMinValueScale(byte minValueScale)
        {
            return NumberExtensions.FuzzDecimalScaleBetween(minValueScale, 28, _fuzzer.Random);
        }

        /// <summary>
        /// Generates a random positive decimal value.
        /// </summary>
        /// <param name="minValue">(optional) The inclusive positive lower bound of the random number returned.</param>
        /// <param name="maxValue">(optional) The inclusive positive upper bound of the random number returned.</param>
        /// <returns>A positive decimal value generated randomly.</returns>
        public decimal GeneratePositiveDecimal(decimal? minValue = null, decimal? maxValue = null)
        {
            // cross guard here
            minValue = minValue ?? 0.0m;
            maxValue = maxValue ?? decimal.MaxValue;

            ThrowIfMinGreaterThanMax(minValue, maxValue);
            ThrowIfMinIsNegative(minValue);

            return GenerateDecimal(minValue, maxValue);
        }

        private static void ThrowIfMinIsNegative(decimal? minValue)
        {
            if (minValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue),
                    $"minValue should be positive. minValue: {minValue}");
            }
        }

        private static void ThrowIfMinGreaterThanMax(decimal? minValue, decimal? maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    $"maxValue should be greater than minValue. minValue: {minValue} - maxValue: {maxValue}");
            }
        }

        /// <summary>
        /// Generates a random long value.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A long value generated randomly.</returns>
        public long GenerateLong(long? minValue = null, long? maxValue = null)
        {
            minValue = minValue ?? long.MinValue;
            maxValue = maxValue ?? long.MaxValue;
            return _fuzzer.Random.NextLong(minValue.Value, maxValue.Value);

            // found here: https://stackoverflow.com/questions/6651554/random-number-in-long-range-is-this-the-way
            var buf = new byte[8];
            _fuzzer.Random.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);

            var nbOfElements = maxValue.Value - minValue.Value + 1;
            nbOfElements = (nbOfElements == 0) ? 1 : nbOfElements; // avoid division by zero

            var modulo = longRand % nbOfElements;
            var abs = Math.Abs(modulo);
            var result = (abs + minValue.Value);

            return result;
        }
    }
}