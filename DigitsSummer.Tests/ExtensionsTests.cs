using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DigitsSummer.Extensions;

namespace DigitsSummer.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void SumAsChar_Test()
        {
            ReadOnlySpan<char> digits = "0123456789".AsSpan();
            ulong actual = digits.SumAsChar();
            ulong expected = digits.SumChar() + (ulong)digits.Length * '0';
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SubStringsTests()
        {
            string str = "0123456789";
            var subStings = str.SubStrings(3).ToArray();
            Assert.AreEqual(4, subStings.Length);
        }


        [Test]
        public void PartitionsTests()
        {
            int[] data = Enumerable.Range(1, 114).ToArray();


            ReadOnlySpan<int> tmp = data.AsSpan();
            (int index, int size)[] partitions = tmp.Partitions(12).ToArray();

            var seq = partitions.Select(i => data.AsSpan().Slice(i.index, i.size).ToArray())
                                .ToArray();

            Assert.AreEqual(12, partitions.Length);
        }

        [Test]
        public void ChuckTests()
        {
            int[] data = Enumerable.Range(1, 114).ToArray();

            ReadOnlySpan<int> tmp = data.AsSpan();
            List<int[]> actual = new ();
            foreach (ReadOnlySpan<int> chunk in tmp.Chunk(10))
            {
                actual.Add(chunk.ToArray());
            }
            Assert.AreEqual(12, actual.Count);

            actual.Clear();
            foreach (ReadOnlySpan<int> chunk in tmp.Chunk(0)) 
                actual.Add(chunk.ToArray());

            Assert.AreEqual(1, actual.Count);
        }
    }
}
