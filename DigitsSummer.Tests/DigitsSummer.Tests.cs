using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class DigitsSummerTests
    {
        
        public const string DataFile1M = @"../../../../Data/data1M.txt";
        public const string DataFile10M = "../../../../Data/data10M.txt";
        public const string DataFile100M = "../../../../Data/data100M.txt";
        public const string DataFile1G = "../../../../Data/data1G.txt";


        [Test]
        public static async Task Sum1M_Test()
        {
            string data = await File.ReadAllTextAsync(DataFile1M);
            ulong ret = DigitsSummer.Sum(data);
        }
        [Test]
        public static async Task Sum10M_Test()
        {
            string data = await File.ReadAllTextAsync(DataFile10M);
            ulong ret = DigitsSummer.Sum(data);
        }
        [Test]
        public static async Task Sum100M_Test()
        {
            string data = await File.ReadAllTextAsync(DataFile100M);
            ulong ret = DigitsSummer.Sum(data);
        }
        [Test]
        public static async Task Sum1G_Test()
        {
            string data = await File.ReadAllTextAsync(DataFile1G);
            ulong ret = DigitsSummer.Sum(data);
        }

        [Test]
        public static async Task Sum2_10M_Test()
        {
            string data = await File.ReadAllTextAsync(DataFile10M);
            ulong ret2 = DigitsSummer.SumV2(data);
            ulong ret = DigitsSummer.Sum(data);
            Assert.AreEqual(ret, ret2);
            ulong ret3 = DigitsSummer.SumV3(data);
            Assert.AreEqual(ret, ret3);
        }

        [Test]
        public static void SumFromFileV4_Tests()
        {
            var ret = DigitsSummer.SumV3FromFile(DataFile10M);
            var ret4 = DigitsSummer.SumV4FromFile(DataFile10M);
            Assert.AreEqual(ret, ret4);
        }

        [Test]
        public static void Sum_Tests()
        {
            Assert.AreEqual(0, DigitsSummer.Sum("000000"));
            Assert.AreEqual(45, DigitsSummer.Sum("123456789"));
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