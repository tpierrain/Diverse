using System;

namespace Diverse
{
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

        public static ulong GetRange(long? minValue, long? maxValue)
        {
            minValue = minValue ?? long.MinValue;
            maxValue = maxValue ?? long.MaxValue;

            var inclusiveMaxBound = maxValue.Value + 1;
            if (maxValue.Value == long.MaxValue)
            {
                inclusiveMaxBound = long.MaxValue;
            }

            var uRange = (ulong)(inclusiveMaxBound - minValue.Value);
            return uRange;
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