using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitsSummer
{
    public static partial class DigitsSummer
    {
        private static readonly Dictionary<char, ulong> _hash = new Dictionary<char, ulong>
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

        public static ulong Sum(string data)
        {
            ulong ret = 0;
            foreach (char current in data)
            {
                var tmp = uint.Parse(current.ToString());
                ret += tmp;
            }

            return ret;
        }

        public static ulong SumHash(in ReadOnlySpan<char> data)
        {
            ulong ret = 0;
            for (int i = 0; i < data.Length; ++i)
                ret += _hash[data[i]];
            return ret;
        }

        public static ulong SumLinq(string data)
        {
            return data.Select(current => uint.Parse(current.ToString()))
                .Aggregate((current1, tmp) => current1 + tmp);
        }

        public static ulong SumPLinq(string data)
        {
            return data.AsParallel()
                .Select(current => _hash[current])
                .Aggregate((current1, tmp) => current1 + tmp);
        }

        public static ulong SumV2(string data)
        {
            ulong ret = 0;
            var size = uint.MaxValue.ToString().Length - 1;
            foreach (var current in data.Partition(size))
            {
                var tmp = uint.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV3(string data)
        {
            ulong ret = 0;
            var size = ulong.MaxValue.ToString().Length - 1;
            foreach (var current in data.Partition(size))
            {
                var tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV3_5(string data)
        {
            ulong ret = 0;
            var size = ulong.MaxValue.ToString().Length - 1;
            for (int index = 0; index < data.Length; index += size)
            {
                var current = data.Substring(index, Math.Min(size, data.Length - index));
                var tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV4(in ReadOnlySpan<char> data)
        {
            ulong ret = 0;
            var size = ulong.MaxValue.ToString().Length - 1;
            for (int index = 0; index < data.Length; index += size)
            {
                var current = data.Slice(index, Math.Min(size, data.Length - index));
                var tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }
            return ret;
        }

    }
}