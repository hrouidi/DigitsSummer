using DigitsSummer.Benchmarks;
using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumTests
    {

        [Test]
        public static void Sum_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.Sum("000000"));
            Assert.AreEqual(45, DigitsSummer.Sum("123456789"));
            Assert.AreEqual(45, DigitsSummer.Sum("00000000000123456789"));
        }

        [Test]
        public static void SumHash_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumHash("000000"));
            Assert.AreEqual(45, DigitsSummer.SumHash("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumHash("00000000000123456789"));
        }


        [Test]
        public static void SumLinq_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumLinq("000000"));
            Assert.AreEqual(45, DigitsSummer.SumLinq("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumLinq("00000000000123456789"));
        }

        [Test]
        public static void SumPLinq_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumPLinq("000000"));
            Assert.AreEqual(45, DigitsSummer.SumPLinq("123456789"));
            Assert.AreEqual(46, DigitsSummer.SumPLinq("000000000001234567890000000000000000000000000000000000000000000000000100000000000000000000000000000"));
        }

        [Test]
        public static void SumV2_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumV2("000000"));
            Assert.AreEqual(45, DigitsSummer.SumV2("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumV2("00000000000123456789"));
        }

        [Test]
        public static void SumV3Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumV3("000000"));
            Assert.AreEqual(45, DigitsSummer.SumV3("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumV3("00000000000123456789"));
        }

        [Test]
        public static void SumV3_5_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumV3_5("000000"));
            Assert.AreEqual(45, DigitsSummer.SumV3_5("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumV3_5("00000000000123456789"));
        }

        [Test]
        public static void SumV4_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumV4("000000"));
            Assert.AreEqual(45, DigitsSummer.SumV4("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumV4("00000000000123456789"));
        }

        [Test]
        public static void SumVx_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.SumVx("000000"));
            Assert.AreEqual(45, DigitsSummer.SumVx("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumVx("00000000000123456789"));
            Assert.AreEqual(45, DigitsSummer.SumVx("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
        }

        [Test]
        public static void SumVx2_Tests()
        {
            Assert.AreEqual(1, DigitsSummer.SumVx2("0000001"));
            Assert.AreEqual(45, DigitsSummer.SumVx2("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumVx2("00000000000123456789"));
            Assert.AreEqual(45, DigitsSummer.SumVx2("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
        }

        [Test]
        public static void SumVx22_Tests()
        {
            Assert.AreEqual(1, DigitsSummer.SumVx22("0000001"));
            Assert.AreEqual(45, DigitsSummer.SumVx22("123456789"));
            Assert.AreEqual(45, DigitsSummer.SumVx22("00000000000123456789"));
            Assert.AreEqual(45, DigitsSummer.SumVx22("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
        }

        [Test]
        [TestCase("0000001",1ul)]
        [TestCase("123456789",45ul)]
        [TestCase("00000000000123456789", 45ul)]
        [TestCase("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 45ul)]
        public static void SumVx23_Tests(string data,ulong expected )
        {
            Assert.AreEqual(expected, DigitsSummer.SumVx23(data));
        }

        [Test]
        public static void Debug()
        {
            DigitsSummer.SumVxAdaptative("");
            var input = GlobalSetupHelper.GenerateDataAsString(100_000_000);

            ulong actual =DigitsSummer.SumVx22(input);
            ulong expected =DigitsSummer.SumVx2(input);
            Assert.AreEqual(actual, expected);

            //DigitsSummer.SumVx22("1234");
            //DigitsSummer.SumVx22("12345");
            //DigitsSummer.SumVx22("123456");
            //DigitsSummer.SumVx22("1234567");
            //DigitsSummer.SumVx22("12345678");
        }
    }
}