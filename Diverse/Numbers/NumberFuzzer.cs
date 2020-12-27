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
            
            if (maxValue.Value <= minValue.Value)
            {
                throw new ArgumentOutOfRangeException("maxValue", "maxValue must be > minValue!");
            }

            //Working with ulong so that modulo works correctly with values > long.MaxValue
            var inclusiveMaxBound = maxValue.Value + 1;
            if (maxValue.Value == long.MaxValue)
            {
                inclusiveMaxBound = long.MaxValue;
            }
            var uRange = (ulong)(inclusiveMaxBound - minValue.Value);

            //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
            //for more information.
            //In the worst case, the expected number of calls is 2 (though usually it's
            //much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            var maxValue1 = ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange;
            do
            {
                var buf = new byte[8];
                _fuzzer.Random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > maxValue1);

            var modulo = (long)(ulongRand % uRange);
            var result = modulo + minValue.Value;

            return result;
        }
    }
}