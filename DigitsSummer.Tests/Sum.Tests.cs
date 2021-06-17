using DigitsSummer.Benchmarks;
using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumTests
    {

        [Test]
        [TestCase("0000001",1ul)]
        [TestCase("123456789", 45ul)]
        [TestCase("00000000000123456789", 45ul)]
        [TestCase("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 45ul)]
        public static void SumVx23_Tests(string data,ulong expected )
        {
            Assert.AreEqual(expected, DigitsSummer.Sum(data));
            Assert.AreEqual(expected, DigitsSummer.SumHash(data));
            Assert.AreEqual(expected, DigitsSummer.SumLinq(data));
            Assert.AreEqual(expected, DigitsSummer.SumPLinq(data));

            Assert.AreEqual(expected, DigitsSummer.SumV2(data));
            Assert.AreEqual(expected, DigitsSummer.SumV3(data));
            Assert.AreEqual(expected, DigitsSummer.SumV3_5(data));
            Assert.AreEqual(expected, DigitsSummer.SumV4(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx2(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx22(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx23(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx24(data));
        }

        [Test,Explicit]
        public static void Debug()
        {
            DigitsSummer.SumVx23("123456789012345");
            DigitsSummer.SumVxAdaptative("");
            var input = GlobalSetupHelper.GenerateDataAsString(100_000_000);

            ulong actual =DigitsSummer.SumVx22(input);
            ulong expected =DigitsSummer.SumVx2(input);
            Assert.AreEqual(actual, expected);
        }
    }
}