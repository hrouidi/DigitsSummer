using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace DigitsSummer
{
    public static class Extensions
    {
        private static readonly Dictionary<char, ulong> _hash = new()
        {
            ['0'] = 0ul,
            ['1'] = 1ul,
            ['2'] = 2ul,
            ['3'] = 3ul,
            ['4'] = 4ul,
            ['5'] = 5ul,
            ['6'] = 6ul,
            ['7'] = 7ul,
            ['8'] = 8ul,
            ['9'] = 9ul,
        };
        public static IEnumerable<string> Partition(this string source, int size)
        {
            var partition = new StringBuilder(size);
            var counter = 0;

            using CharEnumerator? enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                partition.Append(enumerator.Current);
                counter++;
                if (counter % size == 0)
                {
                    yield return partition.ToString();
                    partition.Clear();
                    counter = 0;
                }
            }

            if (counter != 0)
                yield return partition.ToString();
        }

        public static ulong Sum(this ulong digits)
        {
            ulong ret = 0;
            while (digits != 0)
            {
                ret += digits % 10;
                digits /= 10;
            }
            return ret;
        }

        public static ulong Sum(this uint digits)
        {
            ulong ret = 0;
            while (digits != 0)
            {
                ret += digits % 10;
                digits /= 10;
            }
            return ret;
        }

        public static ulong Sum(this Vector256<ulong> vector,int elementCount)
        {
            ulong ret = 0;
            for (int i = 0; i < elementCount; ++i)
                ret += vector.GetElement(i);
            return ret;
        }

        public static ulong SumVx(this in Vector256<ulong> vector)
        {
            return Vector.Dot(vector.AsVector(), Vector<ulong>.One);
        }
        
        public static ulong Sum(this in Vector256<uint> vector)
        {
            return Vector.Dot(vector.AsVector(), Vector<uint>.One);
        }
        public static ulong Sum(this in Vector<uint> vector)
        {
            return Vector.Dot(vector, Vector<uint>.One);
        }

        public static ulong Sum(this in  ReadOnlySpan<char> smallString)
        {
            // remaining digits: Max 3 digits
            ulong ret = 0;
            foreach (var ch in smallString)
                ret += _hash[ch];
            return ret;

        }

        public static ulong SumAsChar(this in ReadOnlySpan<char> smallString)
        {
            ulong ret = 0;
            foreach (var unicode16Char in smallString)
                ret += unicode16Char;
            return ret;
        }
    }
}
