using System;
using System.IO;
using System.Threading.Tasks;

namespace DigitsSummer
{
    public static partial class DigitsSummer
    {
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