using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumFromFileDataFlowTests
    {

        private const string _smallFile = "Small.txt";
        private const ulong _smallFileSum = 45;
        private const string _file1K = "1K.txt";
        private static ulong _file1KSum;

        [SetUp]
        public static void Setup()
        {
            Helper.GenerateDataFile(_smallFile, "000000000012345678900000000000");
            _file1KSum = Helper.GenerateDataFile(_file1K, 1_0000);
        }


        [Test]
        public static void SumV5FromFile_Test()
        {
            var ret = SumFromFileDataFlow.SumV5FromFile(_smallFile,15);
            Assert.AreEqual(_smallFileSum, ret);
            ret = SumFromFileDataFlow.SumV5FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV6_Tests()
        {
            var ret = SumFromFileDataFlow.SumV6FromFile(_smallFile, 15);
            Assert.AreEqual(_smallFileSum, ret);
            ret = SumFromFileDataFlow.SumV5FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileVx2_Tests()
        {
            var ret = SumFromFileDataFlow.SumVx2FromFile(_smallFile, 15);
            Assert.AreEqual(_smallFileSum, ret);
            ret = SumFromFileDataFlow.SumVx2FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileVx3_Tests()
        {
            var ret = SumFromFileDataFlow.SumVx3FromFile(_smallFile, 15);
            Assert.AreEqual(_smallFileSum, ret);
            ret = SumFromFileDataFlow.SumVx3FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileVx4_Tests()
        {
            var ret = SumFromFileDataFlow.SumVx4FromFile(_smallFile, 15);
            Assert.AreEqual(_smallFileSum, ret);
            ret = SumFromFileDataFlow.SumVx4FromFile(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }

    }
}