using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumProducerConsumerTests
    {

        private const string SmallFile = "Small.txt";
        private const ulong SmallFileSum = 45;
        private const string File1K = "1K.txt";
        private static ulong _file1KSum;

        [SetUp]
        public static void Setup()
        {
            Helper.GenerateDataFile(SmallFile, "000000000012345678900000000000");
            _file1KSum = Helper.GenerateDataFile(File1K, 1_0000);
        }


        [Test]
        public static void SumProducerConsumer_Test()
        {
            ulong ret = SumProducerConsumers.Run(SmallFile);
            Assert.AreEqual(SmallFileSum, ret);
            ret = SumProducerConsumers.Run(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }
    }
}