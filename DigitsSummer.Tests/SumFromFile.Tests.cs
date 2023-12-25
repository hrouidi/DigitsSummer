using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumFromFileTests
    {

        private const string _smallFile = "Small.txt";
        private const ulong _smallFileSum = 45;
        private const string _file1K = "1K.txt";
        private static ulong _file1KSum;

        [SetUp]
        public static void Setup()
        {
            Helper.GenerateDataFile(_smallFile, "00000000001234567890000000000");
            _file1KSum = Helper.GenerateDataFile(_file1K, 1_0000);
        }


        [Test]
        public static void SumFromFile_Tests()
        {
            var ret = DigitsSummer.SumFromFile(_smallFile);
            Assert.AreEqual(_smallFileSum, ret);
            ret = DigitsSummer.SumFromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }


        [Test]
        public static void SumFromFileV3_Tests()
        {
            var ret = DigitsSummer.SumV3FromFile(_smallFile);
            Assert.AreEqual(_smallFileSum, ret);
            ret = DigitsSummer.SumV3FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV3_5Tests()
        {
            var ret = DigitsSummer.SumV3_5FromFile(_smallFile);
            Assert.AreEqual(_smallFileSum, ret);
            ret = DigitsSummer.SumV3_5FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV4_Tests()
        {
            var ret = DigitsSummer.SumV4FromFile(_smallFile);
            Assert.AreEqual(_smallFileSum, ret);
            ret = DigitsSummer.SumV4FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }
        [Test]
        public static void SumFromFileVx2_Tests()
        {
            var ret = DigitsSummer.SumVx2FromFile(_smallFile);
            Assert.AreEqual(_smallFileSum, ret);
            ret = DigitsSummer.SumVx2FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

    }
}