using DigitsSummer;
using NUnit.Framework;
using System;

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
            ulong expected = digits.Sum() + (ulong)digits.Length * '0';
            Assert.AreEqual(expected, actual);
        }
    }
}
