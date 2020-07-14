using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumFromFileTests
    {

        private const string SmallFile = "Small.txt";
        private const ulong SmallFileSum = 45;
        private const string File1K = "1K.txt";
        private static ulong _file1KSum;

        [SetUp]
        public static void Setup()
        {
            Helper.GenerateDataFile(SmallFile, "00000000001234567890000000000");
            _file1KSum = Helper.GenerateDataFile(File1K, 1_0000);
        }


        [Test]
        public static void SumFromFile_Tests()
        {
            var ret = DigitsSummer.SumFromFile(SmallFile);
            Assert.AreEqual(SmallFileSum, ret);
            ret = DigitsSummer.SumFromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV2_Tests()
        {
            var ret = DigitsSummer.SumV2FromFile(SmallFile);
            Assert.AreEqual(SmallFileSum, ret);
            ret = DigitsSummer.SumV2FromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV3_Tests()
        {
            var ret = DigitsSummer.SumV3FromFile(SmallFile);
            Assert.AreEqual(SmallFileSum, ret);
            ret = DigitsSummer.SumV3FromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV3_5Tests()
        {
            var ret = DigitsSummer.SumV3_5FromFile(SmallFile);
            Assert.AreEqual(SmallFileSum, ret);
            ret = DigitsSummer.SumV3_5FromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV4_Tests()
        {
            var ret = DigitsSummer.SumV4FromFile(SmallFile);
            Assert.AreEqual(SmallFileSum, ret);
            ret = DigitsSummer.SumV4FromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }


    }
}