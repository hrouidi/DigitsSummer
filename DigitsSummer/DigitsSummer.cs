using System;
using System.Buffers;
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
            ulong ret = accVector.SumVx();
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

            Vector256<uint> extra = Vector256.Create(allZero);


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
            ulong ret = accVector.Sum();
            if (rem > 0)
                ret += data[^rem..].Sum();
            return ret;
        }

        public static ulong SumVx24(in ReadOnlySpan<char> data)
        {
            Vector256<uint> accVector = Vector256<uint>.Zero;
            int size = Vector256<uint>.Count;

            ReadOnlySpan<Vector128<ushort>> vectors = MemoryMarshal.Cast<char, Vector128<ushort>>(data);

            int rem = data.Length % size;
            int iterationCount = vectors.Length;
            uint zero = 48 * (uint)vectors.Length;

            Vector256<uint> extra = Vector256.Create(zero);

            for (int i = 0; i < vectors.Length; i++)
            {
                //Vector.
                var tmp1 = Avx2.ConvertToVector256Int32(vectors[i]).AsUInt32();
                accVector = Avx2.Add(accVector, tmp1);
            }

            accVector = Avx2.Subtract(accVector, extra);
            ulong ret = accVector.Sum();

            if (rem > 0)
                ret += data[^rem..].Sum();
            return ret;
        }

        public static ulong SumVx241(in ReadOnlySpan<char> data)
        {
            Vector256<uint> accVector = Vector256<uint>.Zero;
            ReadOnlySpan<Vector128<ushort>> vectors = MemoryMarshal.Cast<char, Vector128<ushort>>(data);

            foreach (ref readonly var vector in vectors)
            {
                var tmp1 = Avx2.ConvertToVector256Int32(vector).AsUInt32();
                accVector = Avx2.Add(accVector, tmp1);
            }

            ulong ret = accVector.Sum();
            // incomplete vector
            int rem = data.Length % Vector256<uint>.Count;
            if (rem > 0)
                for (int i = rem; i < data.Length; ++i)
                    ret += data[^i];
            //remove unicode extra byte
            ulong extra = 48 * (ulong)data.Length;
            ret -= extra;
            return ret;
        }

        public static ulong SumVx2411(in ReadOnlySpan<char> data)
        {
            Vector256<uint> accVector = Vector256<uint>.Zero;

            ReadOnlySpan<Vector128<ushort>> vectors = MemoryMarshal.Cast<char, Vector128<ushort>>(data);

            foreach (var vector in vectors)
            {
                var tmp1 = Avx2.ConvertToVector256Int32(vector).AsUInt32();
                accVector = Avx2.Add(accVector, tmp1);
            }

            ulong ret = accVector.Sum();

            //remaining elements
            int rem = data.Length % Vector<ushort>.Count;
            if (rem > 0)
                ret += data[rem..].SumAsChar();
            // Remove Extra Unicode value
            ulong extra = 48 * (ulong)data.Length;
            ret -= extra;
            return ret;
        }

        public static ulong SumVx242(in ReadOnlySpan<char> data)
        {
            Vector<uint> accVector = Vector<uint>.Zero;

            ReadOnlySpan<Vector<ushort>> vectors = MemoryMarshal.Cast<char, Vector<ushort>>(data);

            foreach (var vector in vectors)
            {
                Vector.Widen(vector, out Vector<uint> left, out Vector<uint> right);
                accVector += left + right;
            }

            ulong ret = accVector.Sum();

            int rem = data.Length % Vector<uint>.Count;
            if (rem > 0)
                for (int i = rem; i < data.Length; ++i)
                    ret += data[^i];
            ulong extra = 48 * (ulong)data.Length;
            ret -= extra;
            return ret;
        }

        public static ulong SumVx25(in ReadOnlySpan<char> data)
        {
            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);
            ushort maxSumUInt16 = ushort.MaxValue / (48 + 9);//1149
            int blocksCount = vectors.Length / maxSumUInt16; //54
            Span<Vector256<ushort>> shortAccumulators = new Vector256<ushort>[blocksCount + 1];

            for (int blockIndex = 0; blockIndex < blocksCount; blockIndex++)
            {
                int startIndex = blockIndex * maxSumUInt16  ;
                int endIndex = maxSumUInt16 * (blockIndex + 1);
                Vector256<ushort> tmp = Vector256<ushort>.Zero;
                for (int i = startIndex; i < endIndex; i++)
                {
                    tmp = Avx2.Add(tmp, vectors[i]);
                }
                shortAccumulators[blockIndex] = tmp;
            }
            //Last incomplete block
            Vector256<ushort> last = Vector256<ushort>.Zero;
            for (int i = blocksCount * maxSumUInt16 ; i < vectors.Length; i++)
            {
                last = Avx2.Add(last, vectors[i]);
            }
            shortAccumulators[^1] = last;

            // sum accumulators
            Vector256<uint> accVector = Vector256<uint>.Zero;
            foreach (ref readonly Vector256<ushort> acc in shortAccumulators)
            {
                Vector.Widen(acc.AsVector(), out Vector<uint> left, out Vector<uint> right);
                accVector = Avx2.Add(accVector, (left + right).AsVector256());
            }

            ulong ret = accVector.Sum();
            //remaining elements
            int rem = data.Length % Vector<ushort>.Count;
            if (rem > 0)
                for (int i = rem; i < data.Length; ++i)
                    ret += data[^i];
            // Remove Extra Unicode value
            ulong extra = 48 * (ulong)data.Length;
            ret -= extra;
            return ret;
        }

        public static ulong SumVx25_memoryPool(in ReadOnlySpan<char> data)
        {
            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);
            ushort maxSumUInt16 = ushort.MaxValue / (48 + 9);//1149
            int blocksCount = vectors.Length / maxSumUInt16; //54

            IMemoryOwner<Vector256<ushort>> lease = MemoryPool<Vector256<ushort>>.Shared.Rent(blocksCount + 1);
            Span<Vector256<ushort>> shortAccumulators = lease.Memory.Span[..(blocksCount + 1)];
            for (int blockIndex = 0; blockIndex < blocksCount; blockIndex++)
            {
                int startIndex = blockIndex * maxSumUInt16;
                int endIndex = maxSumUInt16 * (blockIndex + 1);
                Vector256<ushort> tmp = Vector256<ushort>.Zero;
                for (int i = startIndex; i < endIndex; i++)
                {
                    tmp = Avx2.Add(tmp, vectors[i]);
                }
                shortAccumulators[blockIndex] = tmp;
            }
            //Last incomplete block
            Vector256<ushort> last = Vector256<ushort>.Zero;
            for (int i = blocksCount * maxSumUInt16; i < vectors.Length; i++)
            {
                last = Avx2.Add(last, vectors[i]);
            }
            shortAccumulators[^1] = last;

            // sum accumulators
            Vector256<uint> accVector = Vector256<uint>.Zero;
            foreach (ref readonly Vector256<ushort> acc in shortAccumulators)
            {
                Vector.Widen(acc.AsVector(), out Vector<uint> left, out Vector<uint> right);
                accVector = Avx2.Add(accVector, (left + right).AsVector256());
            }

            lease.Dispose();
            ulong ret = accVector.Sum();
            //remaining elements
            int rem = data.Length % Vector<ushort>.Count;
            if (rem > 0)
                ret += data[rem..].SumAsChar();

            // Remove Extra Unicode value
            ulong extra = 48 * (ulong)data.Length;
            ret -= extra;
            return ret;
        }

        public static ulong SumVx25_memoryPool_unrolled(in ReadOnlySpan<char> data)
        {
            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);
            ushort maxSumUInt16 = ushort.MaxValue / (48 + 9);//1149
            int blocksCount = vectors.Length / maxSumUInt16; //54

            IMemoryOwner<Vector256<ushort>> lease = MemoryPool<Vector256<ushort>>.Shared.Rent(blocksCount + 1);
            Span<Vector256<ushort>> shortAccumulators = lease.Memory.Span[..(blocksCount + 1)];
            for (int blockIndex = 0; blockIndex < blocksCount; blockIndex++)
            {
                int startIndex = blockIndex * maxSumUInt16;
                int endIndex = maxSumUInt16 * (blockIndex + 1) ;
                Vector256<ushort> tmp = Vector256<ushort>.Zero;
                for (int i = startIndex; i < endIndex-4; i+=4)
                {
                    tmp = Avx2.Add(tmp, vectors[i]);
                    tmp = Avx2.Add(tmp, vectors[i + 1]);
                    tmp = Avx2.Add(tmp, vectors[i + 2]);
                    tmp = Avx2.Add(tmp, vectors[i + 3]);
                }
                tmp = Avx2.Add(tmp, vectors[endIndex-1]);
                shortAccumulators[blockIndex] = tmp;
            }
            //Last incomplete block
            Vector256<ushort> last = Vector256<ushort>.Zero;
            for (int i = blocksCount * maxSumUInt16; i < vectors.Length; i++)
            {
                last = Avx2.Add(last, vectors[i]);
            }
            shortAccumulators[^1] = last;

            // sum accumulators
            Vector256<uint> accVector = Vector256<uint>.Zero;
            foreach (ref readonly Vector256<ushort> acc in shortAccumulators)
            {
                Vector.Widen(acc.AsVector(), out Vector<uint> left, out Vector<uint> right);
                accVector = Avx2.Add(accVector, (left + right).AsVector256());
            }

            lease.Dispose();
            ulong ret = accVector.Sum();
            //remaining elements
            int rem = data.Length % Vector<ushort>.Count;
            if (rem > 0)
                ret += data[rem..].SumAsChar();

            // Remove Extra Unicode value
            ulong extra = 48 * (ulong)data.Length;
            ret -= extra;
            return ret;
        }


        public static ulong SumVx26(in ReadOnlySpan<char> data)
        {
            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);
            ushort maxSumUInt16 = ushort.MaxValue / 48;//1365
            int blocksCount = vectors.Length / maxSumUInt16; //45
            Vector256<ushort> extra = Vector256.Create((ushort)('0' * maxSumUInt16));
            IMemoryOwner<Vector256<ushort>> lease = MemoryPool<Vector256<ushort>>.Shared.Rent(blocksCount + 1);
            Span<Vector256<ushort>> shortAccumulators = lease.Memory.Span[..(blocksCount + 1)];
            for (int blockIndex = 0; blockIndex < blocksCount; blockIndex++)
            {
                int startIndex = blockIndex * maxSumUInt16;
                int endIndex = maxSumUInt16 * (blockIndex + 1);
                Vector256<ushort> tmp = Vector256<ushort>.Zero;
                
                for (int i = startIndex; i < endIndex; i++)
                {
                    tmp = Avx2.Add(tmp, vectors[i]);
                }

                shortAccumulators[blockIndex] = Avx2.Subtract(tmp , extra);
            }
            //Last incomplete block
            Vector256<ushort> last = Vector256<ushort>.Zero;
            var lastBlock = vectors[(blocksCount * maxSumUInt16)..];
            foreach (ref readonly var current in lastBlock)
            {
                last = Avx2.Add(last, current);
            }
            shortAccumulators[^1] = Avx2.Subtract(last, Vector256.Create((ushort)('0' * lastBlock.Length)));

            // sum accumulators
            Vector256<uint> accVector = Vector256<uint>.Zero;
            foreach (ref readonly Vector256<ushort> acc in shortAccumulators)
            {
                Vector.Widen(acc.AsVector(), out Vector<uint> left, out Vector<uint> right);
                accVector = Avx2.Add(accVector, (left + right).AsVector256());
            }

            lease.Dispose();
            ulong ret = accVector.Sum();
            //remaining elements
            int rem = data.Length % Vector<ushort>.Count;
            if (rem > 0)
                ret += data[rem..].SumAsChar();

            // Remove Extra Unicode value
            //ulong extra = 48 * (ulong)data.Length;
            //ret -= extra;
            return ret;
        }

        //public static ulong SumVx27(in ReadOnlySpan<char> data)
        //{
        //    ReadOnlySpan<Vector256<byte>> vectors = MemoryMarshal.Cast<char, Vector256<byte>>(data);
        //    byte maxSumUsingByteVector = byte.MaxValue / (48 + 9);//1149
        //    int byteVectorsCount = vectors.Length / maxSumUsingByteVector/2; //54

        //    IMemoryOwner<Vector256<byte>> lease = MemoryPool<Vector256<byte>>.Shared.Rent(byteVectorsCount + 1);
        //    Span<Vector256<byte>> byteAccumulators = lease.Memory.Span[..(byteVectorsCount + 1)];
        //    for (int blockIndex = 0; blockIndex < byteVectorsCount; blockIndex++)
        //    {
        //        int startIndex = blockIndex * maxSumUsingByteVector;
        //        int endIndex = maxSumUsingByteVector * (blockIndex + 1);
        //        Vector256<byte> tmp = Vector256<byte>.Zero;
        //        for (int i = startIndex; i < endIndex; i=+2)
        //        {
        //            var adj = Avx2.HorizontalAdd(vectors[i], vectors[i + 1]);
        //            tmp = Avx2.Add(tmp, vectors[i]);
        //        }
        //        byteAccumulators[blockIndex] = tmp;
        //    }
        //    //Last incomplete block
        //    Vector256<byte> last = Vector256<byte>.Zero;
        //    for (int i = byteVectorsCount * maxSumUsingByteVector; i < vectors.Length; i++)
        //    {
        //        last = Avx2.Add(last, vectors[i]);
        //    }
        //    byteAccumulators[^1] = last;

        //    // sum accumulators
        //    Vector256<ushort> accVector = Vector256<ushort>.Zero;
        //    foreach (ref readonly Vector256<byte> acc in byteAccumulators)
        //    {
        //        Vector.Widen(acc.AsVector(), out Vector<ushort> left, out Vector<ushort> right);
        //        accVector = Avx2.Add(accVector, (left + right).AsVector256());
        //    }

        //    lease.Dispose();
        //    ulong ret = 0;// accVector.Sum();
        //    //remaining elements
        //    int rem = data.Length % Vector<ushort>.Count;
        //    if (rem > 0)
        //        ret += data[rem..].SumAsChar();

        //    // Remove Extra Unicode value
        //    ulong extra = 48 * (ulong)data.Length;
        //    ret -= extra;
        //    return ret;
        //}

    }

    public unsafe struct ShortBlock
    {
        private fixed ushort _vectors[1149 * 16];

        private ReadOnlySpan<ushort> _span
        {
            get
            {
                fixed (ushort* ptr = _vectors)
                    return new ReadOnlySpan<ushort>(ptr, 1149 * 16);
                //return new ReadOnlySpan<ushort>(MemoryMarshal.GetReference(_vectors);
            }
        }

        public Vector<ushort> this[int index] => new(_span.Slice(index * 1149, 16));
    }
}