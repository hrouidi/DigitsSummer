using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace DigitsSummer.Tests
{
    public class SumFromFileDataFlowTests
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
        public static void SumFromFileV5_Tests()
        {
            var ret = SumFromFileDataFlow.SumV5FromFile(SmallFile,15);
            Assert.AreEqual(SmallFileSum, ret);
            ret = SumFromFileDataFlow.SumV5FromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }

        [Test]
        public static void SumFromFileV6_Tests()
        {
            //var rettt = Marshal.SizeOf(typeof(ReadOnlyMemory<char>));
            var ret = SumFromFileDataFlow.SumV6FromFile(SmallFile, 15);
            Assert.AreEqual(SmallFileSum, ret);
            ret = SumFromFileDataFlow.SumV5FromFile(File1K);
            Assert.AreEqual(_file1KSum, ret);
        }

    }
}