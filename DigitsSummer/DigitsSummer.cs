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

        private static readonly Dictionary<char, long> _hashInt64 = new()
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

        private static readonly Dictionary<char, byte> _hash8 = new()
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

        public static ulong ByHash(in ReadOnlySpan<char> cha) => _hash[cha[0]];

        public static ulong Sum(string data)
        {
            ulong ret = 0;
            foreach (char current in data)
            {
                uint tmp = uint.Parse(current.ToString());
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
            int size = uint.MaxValue.ToString().Length - 1;
            foreach (string? current in data.Partition(size))
            {
                uint tmp = uint.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV3(string data)
        {
            ulong ret = 0;
            int size = ulong.MaxValue.ToString().Length - 1;
            foreach (string? current in data.Partition(size))
            {
                ulong tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV3_5(string data)
        {
            ulong ret = 0;
            int size = ulong.MaxValue.ToString().Length - 1;
            for (int index = 0; index < data.Length; index += size)
            {
                string? current = data.Substring(index, Math.Min(size, data.Length - index));
                ulong tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV4(in ReadOnlySpan<char> data)
        {
            ulong ret = 0;
            int size = ulong.MaxValue.ToString().Length - 1;
            for (int index = 0; index < data.Length; index += size)
            {
                ReadOnlySpan<char> current = data.Slice(index, Math.Min(size, data.Length - index));
                ulong tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }
            return ret;
        }

        public static ulong SumVx(in ReadOnlySpan<char> data)
        {
            ulong ret = 0;
            Vector<byte> tmp = Vector<byte>.Zero;
            int size = Vector<byte>.Count;
            Span<byte> values = stackalloc byte[size];
            for (int index = 0; index < data.Length; index += size)
            {
                ReadOnlySpan<char> current = data.Slice(index, Math.Min(size, data.Length - index));
                values.Fill(0);
                for (int i = 0; i < current.Length; ++i)
                    values[i] = _hash8[current[i]];
                Vector<byte> vector = new Vector<byte>(values);
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
            Vector256<long> accVector = Vector256<long>.Zero;
            int size = Vector256<long>.Count;
            long zero = 48L;
            Vector256<long> _48 = Avx2.BroadcastScalarToVector256(&zero);
            int index = 0;
            for (; index + size < data.Length; index += size)
            {
                ReadOnlySpan<char> current = data.Slice(index, Math.Min(size, data.Length - index));
                ReadOnlySpan<ushort> bytes = MemoryMarshal.Cast<char, ushort>(current);
                fixed (ushort* pStart = bytes)
                {
                    Vector256<long> bytesVx = Avx2.ConvertToVector256Int64(pStart);
                    Vector256<long> ori = Avx2.Subtract(bytesVx, _48);
                    accVector = Avx2.Add(accVector, ori);
                }
            }

            for (int i = 0; i < size; ++i)
                ret += accVector.GetElement(i);
            // remaining digits: Max 3 digits
            for (; index < data.Length; ++index)
                ret += _hashInt64[data[index]];
            return ret;
        }

        //Assume that data.Length % Vector256<long>.Count == 0
        public static unsafe Vector256<long> SumVx3(in ReadOnlySpan<char> data)
        {
            long ret = 0;
            Vector256<long> accVector = Vector256<long>.Zero;
            int size = Vector256<long>.Count;
            long zero = 48L;
            Vector256<long> _48 = Avx2.BroadcastScalarToVector256(&zero);
            int index = 0;
            for (; index + size < data.Length; index += size)
            {
                ReadOnlySpan<char> current = data.Slice(index, Math.Min(size, data.Length - index));
                ReadOnlySpan<ushort> bytes = MemoryMarshal.Cast<char, ushort>(current);
                fixed (ushort* pStart = bytes)
                {
                    Vector256<long> bytesVx = Avx2.ConvertToVector256Int64(pStart);
                    Vector256<long> ori = Avx2.Subtract(bytesVx, _48);
                    accVector = Avx2.Add(accVector, ori);
                }
            }
            return accVector;
        }

        public static unsafe ulong SumVx22(in ReadOnlySpan<char> data)
        {
            Vector256<ulong> accVector = Vector256<ulong>.Zero;
            int size = Vector256<ulong>.Count;


            ReadOnlySpan<ushort> dataBytes = MemoryMarshal.Cast<char, ushort>(data);

            int lastIndex = dataBytes.Length - size;
            int rem = dataBytes.Length % size;
            int iterationCount = dataBytes.Length / size;
            ulong allZero = 48 * (ulong)iterationCount;
            Vector256<ulong> extra = Avx2.BroadcastScalarToVector256(&allZero);

            for (int index = 0; index <= lastIndex; index += size)
            {
                ReadOnlySpan<ushort> current = dataBytes.Slice(index, size);
                fixed (ushort* pStart = current)
                {
                    Vector256<long> bytesVx = Avx2.ConvertToVector256Int64(pStart);
                    accVector = Avx2.Add(accVector, bytesVx.AsUInt64());
                }
            }

            accVector = Avx2.Subtract(accVector, extra);
            ulong ret = accVector.Sum(size);
            if (rem > 0)
                ret += data[^rem..].Sum();
            return ret;
        }

        public static unsafe ulong SumVx23(in ReadOnlySpan<char> data)
        {
            Vector256<uint> accVector = Vector256<uint>.Zero;
            int size = Vector256<uint>.Count;


            ReadOnlySpan<ushort> dataBytes = MemoryMarshal.Cast<char, ushort>(data);

            int lastIndex = dataBytes.Length - size;
            int rem = dataBytes.Length % size;
            int iterationCount = dataBytes.Length / size;
            uint allZero = 48 * (uint)iterationCount;
            Vector256<uint> extra = Avx2.BroadcastScalarToVector256(&allZero);

            for (int index = 0; index <= lastIndex; index += size)
            {
                ReadOnlySpan<ushort> current = dataBytes.Slice(index, size);
                fixed (ushort* pStart = current)
                {
                    Vector256<int> bytesVx = Avx2.ConvertToVector256Int32(pStart);
                    accVector = Avx2.Add(accVector, bytesVx.AsUInt32());
                }
            }

            accVector = Avx2.Subtract(accVector, extra);
            ulong ret = accVector.Sum(size);
            if (rem > 0)
                ret += data[^rem..].Sum();
            return ret;
        }

        public static unsafe ulong SumVxAdaptative(in ReadOnlySpan<char> data)
        {
            byte m = byte.MaxValue ;
            m = (byte) (m + 255);
            Vector256<byte> max = Vector256<byte>.AllBitsSet;
            Vector256<byte> max2= Vector256<byte>.AllBitsSet;
            var tmp = Avx2.AddSaturate(max, max2);
            Vector256<uint> accVector = Vector256<uint>.Zero;
            //int size8 = Vector256<byte>.Count;
            int size16 = Vector256<ushort>.Count;
            int size32 = Vector256<uint>.Count;
            int size64 = Vector256<ulong>.Count;


            ReadOnlySpan<ushort> dataBytes = MemoryMarshal.Cast<char, ushort>(data);

            //int lastIndex = dataBytes.Length - size;
            //int rem = dataBytes.Length % size;
            //int iterationCount = dataBytes.Length / size;
            //uint allZero = 48 * (uint)iterationCount;
            //Vector256<uint> extra = Avx2.BroadcastScalarToVector256(&allZero);

            //for (int index = 0; index <= lastIndex; index += size)
            //{
            //    ReadOnlySpan<ushort> current = dataBytes.Slice(index, size);
            //    fixed (ushort* pStart = current)
            //    {
            //        Vector256<int> bytesVx = Avx2.ConvertToVector256Int32(pStart);
            //        accVector = Avx2.Add(accVector, bytesVx.AsUInt32());
            //    }
            //}

            //accVector = Avx2.Subtract(accVector, extra);
            //ulong ret = accVector.Sum(size);
            //if (rem > 0)
            //    ret += data[^rem..].Sum();
            //return ret;
            return 0;
        }
    }
}