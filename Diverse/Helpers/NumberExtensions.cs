using System;

namespace Diverse
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random long from min (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="min">The inclusive minimum bound</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
        public static long NextLong(this Random random, long min, long max)
        {
            if (max <= min)
            {
                throw new ArgumentOutOfRangeException("max", "max must be > min!");
            }

            //Working with ulong so that modulo works correctly with values > long.MaxValue
            var inclusiveMaxBound = max + 1;
            if (max == long.MaxValue)
            {
                inclusiveMaxBound = long.MaxValue;
            }
            var uRange = (ulong)(inclusiveMaxBound - min);

            //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
            //for more information.
            //In the worst case, the expected number of calls is 2 (though usually it's
            //much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            var maxValue = ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange;
            do
            {
                var buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > maxValue);

            var modulo = (long)(ulongRand % uRange);
            var result = modulo + min;

            return result;
        }

    }

    /// <summary>
    /// Extension methods for numbers.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Extract the scale of a <see cref="decimal"/>.
        /// </summary>
        /// <param name="number">The <see cref="decimal"/> number.</param>
        /// <returns>The scale of the <see cref="decimal"/> number.</returns>
        public static byte ExtractScale(this decimal number)
        {
            var bits = decimal.GetBits(number);
            var scale = (byte)((bits[3] >> 16) & 0x7F);

            return scale;
        }

        /// <summary>
        /// Fuzz a scale for a <see cref="decimal"/> number that is included between a min scale and a max scale.
        /// </summary>
        /// <param name="minValue">The lower bound for this scale to be fuzzed.</param>
        /// <param name="maxValue">The upper bound for this scale to be fuzzed.</param>
        /// <param name="random">The <see cref="Random"/> instance to be used to fuzz.</param>
        /// <returns>A scale for a <see cref="decimal"/> number that is included between the min and max provided (inclusive).</returns>
        public static byte FuzzDecimalScaleBetween(byte minValue, byte maxValue, Random random)
        {
            const byte minScaleForADecimal = 0;
            const byte maxScaleForADecimal = 28;

            ThrowIfMinIsGreaterThanMax(minValue, maxValue);
            ThrowIfNotInAcceptableRange(minValue, maxValue, minScaleForADecimal, maxScaleForADecimal);

            // Adjust the inclusiveness of the Fuzzer API to the exclusiveness of the Random API.
            var inclusiveMaxScale = (maxValue == maxScaleForADecimal) ? maxScaleForADecimal : maxValue + 1;
            var scale = (byte)random.Next(minValue, inclusiveMaxScale);

            return scale;
        }

        private static void ThrowIfMinIsGreaterThanMax(byte minValue, byte maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue", $"minValue ({minValue}) must be lower than maxValue ({maxValue}).");
            }
        }

        private static void ThrowIfNotInAcceptableRange(byte minValueScale, byte maxValueScale, int minScaleForADecimal,
            int maxScaleForADecimal)
        {
            if (minValueScale < minScaleForADecimal)
            {
                throw new ArgumentOutOfRangeException("minValueScale", $"minValueScale must be >= {minScaleForADecimal}");
            }

            if (maxValueScale > maxScaleForADecimal)
            {
                throw new ArgumentOutOfRangeException("maxValueScale", $"maxValueScale must be <= {maxScaleForADecimal}");
            }
        }
    }
}