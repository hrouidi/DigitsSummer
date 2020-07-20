using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace DigitsSummer
{
    public static partial class DigitsSummer
    {
        private static readonly Dictionary<char, ulong> Hash = new Dictionary<char, ulong>
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

        private static readonly Dictionary<char, long> HashInt64 = new Dictionary<char, long>
        {
            ['0'] = 0L,
            ['1'] = 1L,
            ['2'] = 2L,
            ['3'] = 3L,
            ['4'] = 4L,
            ['5'] = 5L,
            ['6'] = 6L,
            ['7'] = 7L,
            ['8'] = 8L,
            ['9'] = 9L,
        };

        private static readonly Dictionary<char, byte> Hash8 = new Dictionary<char, byte>
        {
            ['0'] = 0,
            ['1'] = 1,
            ['2'] = 2,
            ['3'] = 3,
            ['4'] = 4,
            ['5'] = 5,
            ['6'] = 6,
            ['7'] = 7,
            ['8'] = 8,
            ['9'] = 9,
        };

        public static ulong ByHash(in ReadOnlySpan<char> cha) => Hash[cha[0]];

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
                ret += Hash[data[i]];
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
                .Select(current => Hash[current])
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

        public static ulong SumVx(in ReadOnlySpan<char> data)
        {
            ulong ret = 0;
            var tmp = Vector<byte>.Zero;
            var size = Vector<byte>.Count;
            Span<byte> values = stackalloc byte[size];
            for (int index = 0; index < data.Length; index += size)
            {
                var current = data.Slice(index, Math.Min(size, data.Length - index));
                values.Fill(0);
                for (int i = 0; i < current.Length; ++i)
                    values[i] = Hash8[current[i]];
                var vector = new Vector<byte>(values);
                tmp += vector;
            }
            for (int i = 0; i < size; ++i)
                ret += tmp[i];
            return ret;
        }

        public static ulong SumVx2(in ReadOnlySpan<char> data)
        {
            return (ulong)SumVx2A(data);
        }

        public static unsafe long SumVx2A(in ReadOnlySpan<char> data)
        {
            long ret = 0;
            var accVector = Vector256<long>.Zero;
            int size = Vector256<long>.Count;
            long zero = 48L;
            var _48 = Avx2.BroadcastScalarToVector256(&zero);
            int index = 0;
            for (; index + size < data.Length; index += size)
            {
                var current = data.Slice(index, Math.Min(size, data.Length - index));
                var bytes = MemoryMarshal.Cast<char, ushort>(current);
                fixed (ushort* pStart = bytes)
                {
                    var bytesVx = Avx2.ConvertToVector256Int64(pStart);
                    var ori = Avx2.Subtract(bytesVx, _48);
                    accVector = Avx2.Add(accVector, ori);
                }
            }

            for (int i = 0; i < size; ++i)
                ret += accVector.GetElement(i);
            // remaining digits: Max 3 digits
            for (; index < data.Length; ++index)
                ret += HashInt64[data[index]];
            return ret;
        }

        //Assume that data.Length % Vector256<long>.Count == 0
        public static unsafe Vector256<long> SumVx3(in ReadOnlySpan<char> data)
        {
            long ret = 0;
            var accVector = Vector256<long>.Zero;
            int size = Vector256<long>.Count;
            long zero = 48L;
            var _48 = Avx2.BroadcastScalarToVector256(&zero);
            int index = 0;
            for (; index + size < data.Length; index += size)
            {
                var current = data.Slice(index, Math.Min(size, data.Length - index));
                var bytes = MemoryMarshal.Cast<char, ushort>(current);
                fixed (ushort* pStart = bytes)
                {
                    var bytesVx = Avx2.ConvertToVector256Int64(pStart);
                    var ori = Avx2.Subtract(bytesVx, _48);
                    accVector = Avx2.Add(accVector, ori);
                }
            }
            return accVector;
        }

    }
}