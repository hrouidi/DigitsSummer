using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace DigitsSummer
{
    public static class Extensions
    {
        public static IEnumerable<string> Partition(this string source, int size)
        {
            var partition = new StringBuilder(size);
            var counter = 0;

            using var enumerator = source.GetEnumerator();
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
    }
}
