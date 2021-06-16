using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumProducerConsumerTests
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
        public static void SumProducerConsumer_Test()
        {
            ulong ret = SumProducerConsumers.Run(_smallFile);
            Assert.AreEqual(_smallFileSum, ret);
            ret = SumProducerConsumers.Run(_file1K);
            Assert.AreEqual(_file1KSum, ret);
        }
    }
}