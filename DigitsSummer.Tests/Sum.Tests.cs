using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DigitsSummer.Benchmarks;
using NUnit.Framework;

namespace DigitsSummer.Tests
{
    public class SumTests
    {

        [Test]
        [TestCase("0000001", 1ul)]
        [TestCase("123456789", 45ul)]
        [TestCase("00000000000123456789", 45ul)]
        [TestCase("0000000000012345678900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 45ul)]
        public static void SumVx_Tests(string data, ulong expected)
        {
            Assert.AreEqual(expected, DigitsSummer.Sum(data));
            Assert.AreEqual(expected, DigitsSummer.SumHash(data));
            Assert.AreEqual(expected, DigitsSummer.SumLinq(data));
            Assert.AreEqual(expected, DigitsSummer.SumPLinq(data));

            Assert.AreEqual(expected, DigitsSummer.SumV2(data));
            Assert.AreEqual(expected, DigitsSummer.SumV3(data));
            Assert.AreEqual(expected, DigitsSummer.SumV3_5(data));
            Assert.AreEqual(expected, DigitsSummer.SumV4(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx2(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx22(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx23(data));
            Assert.AreEqual(expected, DigitsSummer.SumVx24(data));

        }

        [Test, Explicit]
        public static void Debug()
        {
            var input = GlobalSetupHelper.GenerateDataAsString(1_000_000);
            ulong actual = DigitsSummer.SumVx25_memoryPool_unrolled(input);
            ulong expected = DigitsSummer.SumVx24(input);
            Assert.AreEqual(actual, expected);
        }

        [Test, Explicit]
        public static void Profile()
        {
            var input = GlobalSetupHelper.GenerateDataAsString(1_000_000);
            ulong actual = DigitsSummer.SumVx25_memoryPool(input);
        }

        [Test]
        public void MutatingStringTest()
        {
            const string source = "0123456789";
            ref var tmp = ref MemoryMarshal.GetReference(source.AsSpan());
            Span<char> mutated = MemoryMarshal.CreateSpan(ref tmp, source.Length);
            mutated.Fill('m');

            Assert.AreEqual("mmmmmmmmmm", mutated.ToString());
        }
    }
}