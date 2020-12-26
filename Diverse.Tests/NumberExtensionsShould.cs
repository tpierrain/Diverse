using System;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class NumberExtensionsShould
    {
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(1, 28)]
        [TestCase(27, 28)]
        [TestCase(0, 28)]
        [TestCase(28, 28)]
        public void FuzzDecimalScaleBetween_a_specified_range(byte min, byte max)
        {
            var random = new Random();
            var scale = NumberExtensions.FuzzDecimalScaleBetween(min, max, random);
            TestContext.WriteLine(scale);

            Check.WithCustomMessage($"scale: {scale} should be lower or equal to maxValue: {max}")
                .That(scale <= max).IsTrue();

            Check.WithCustomMessage($"number: {scale} should be greater or equal to minValue: {min}")
                .That(scale >= min).IsTrue();
        }        
    }
}