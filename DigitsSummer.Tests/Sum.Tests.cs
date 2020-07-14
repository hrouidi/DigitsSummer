using System.IO;
using System.Threading.Tasks;
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
    }
}