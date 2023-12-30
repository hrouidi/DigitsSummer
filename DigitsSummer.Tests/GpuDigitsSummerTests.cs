using DigitsSummer.Benchmarks;
using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class GpuDigitsSummerTests
    {
        [Test]
        public void SumAsChar_Test()
        {
            const int length= 100_000;
            string? input = GlobalSetupHelper.GenerateOnesAsString(length);
            using GpuDigitsSummer gpuDigitsSummer = new();
            ulong actual = gpuDigitsSummer.Sum(input);

            Assert.AreEqual(length, actual);
        }
    }
}