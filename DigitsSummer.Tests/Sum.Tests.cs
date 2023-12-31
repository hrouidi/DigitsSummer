using System;
using System.Buffers;
using System.Buffers.Text;
using System.Linq;
using System.Runtime;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using DigitsSummer.Benchmarks;
using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumTests
    {
        private static readonly string _input;//= GlobalSetupHelper.GenerateDataAsString(1_000_000);

        [Test]
        [TestCase("0000001", 1ul)]
        [TestCase("123456789", 45ul)]
        [TestCase("00000000000123456789", 45ul)]
        [TestCase("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 45ul)]
        [TestCase("0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789", 450ul)]
        public static void SumVx_Tests(string data, ulong expected)
        {
            Assert.AreEqual(expected, DigitsSummer.Sum(data));
            Assert.AreEqual(expected, DigitsSummer.SumChar(data));
            Assert.AreEqual(expected, DigitsSummer.SumParallel(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx24Parallel(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx251Parallel(data));

            Assert.AreEqual(expected, DigitsSummer.SumV3(data));
            Assert.AreEqual(expected, DigitsSummer.SumV3_5(data));
            Assert.AreEqual(expected, DigitsSummer.SumV4(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx2(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx22(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx23(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx24(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx240(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx250(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx251(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx252(data));

        }

        [Test, Explicit]
        public static void Debug()
        {
            const int length = 10_000;
            string? input = GlobalSetupHelper.GenerateOnesAsString(length);
            //string? tmp = GlobalSetupHelper.GenerateDataAsString(length);
            Assert.AreEqual(length,input.Length);
            ulong expected = DigitsSummer.SumChar(input);
            ulong actual = DigitsSummer.SumVx252(input);
            Assert.AreEqual(expected, actual);
        }

        [Test, Explicit]
        public static void SumOverFlow()
        {
            uint acc = 0;
            foreach (int i in Enumerable.Range(0, 75_350_303))
                acc += '9';

            Assert.Throws<OverflowException>(() =>
            {
                checked
                {
                    acc += '9';
                }
            });


            ushort smallAcc = 0;
            foreach (int i in Enumerable.Range(0, 1149))
                acc += '9';

            Assert.Throws<OverflowException>(() =>
            {
                checked
                {
                    acc += '9';
                }
            });

        }


        [Test, Explicit]
        public static void Profile()
        {
            var tmp = GC.AllocateUninitializedArray<byte>(102);

            var config = GC.GetConfigurationVariables();

            if (GC.TryStartNoGCRegion(1))
            {
                GCSettings.LatencyMode = GCLatencyMode.NoGCRegion;


                GC.RegisterNoGCRegionCallback(1, () =>
                {

                });
                if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
                    GC.EndNoGCRegion();
            }



        }
    }

}