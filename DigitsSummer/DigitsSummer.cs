using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using DigitsSummer.Extensions;

namespace DigitsSummer
{
    public static partial class DigitsSummer
    {
        public static ulong Sum(string data)
        {
            ulong ret = 0;
            foreach (char current in data)
            {
                byte tmp = byte.Parse(current.ToString());
                ret += tmp;
            }

            return ret;
        }

        public static ulong SumChar(in ReadOnlySpan<char> data) => data.SumChar();

        public static ulong SumParallel(string data)
        {
            return data.AsSpan()
                       .Partitions()
                       .AsParallel()
                       .Select(x => data.AsSpan().Slice(x.index, x.size).SumChar())
                       .Aggregate((x, y) => x + y);
        }

        public static ulong SumVx24Parallel(string data)
        {
            return data.AsSpan()
                       .Partitions()
                       .AsParallel()
                       .Select(x => data.AsSpan().Slice(x.index, x.size).SumVx240())
                       .Aggregate((x, y) => x + y);
        }

        public static ulong SumVx251Parallel(string data)
        {
            return data.AsSpan()
                       .Partitions()
                       .AsParallel()
                       .Select(x => data.AsSpan().Slice(x.index, x.size).SumVx251())
                       .Aggregate((x, y) => x + y);
        }

        public static ulong SumV3(string data)
        {
            ulong ret = 0;
            int size = ulong.MaxValue.ToString().Length - 1;
            foreach (string? current in data.SubStrings(size))
            {
                ulong tmp = ulong.Parse(current);
                ret += tmp.SumULong();
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
                ret += tmp.SumULong();
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
                ret += tmp.SumULong();
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
                    values[i] = current[i].ToByte();
                Vector<byte> vector = new Vector<byte>(values);
                tmp += vector;
            }
            for (int i = 0; i < size; ++i)
                ret += tmp[i];
            return ret;
        }

        public static ulong SumVx2(in ReadOnlySpan<char> data) => (ulong)SumVx2A(data);

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
                ret += data[index].ToLong();
            return ret;
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
            ulong ret = Vector256.Sum(accVector);
            if (rem > 0)
                ret += data[^rem..].SumChar();
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
            ulong ret = Vector256.Sum(accVector);
            if (rem > 0)
                ret += data[^rem..].SumChar();
            return ret;
        }

        public static ulong SumVx24(this in ReadOnlySpan<char> data)
        {
            //
            Vector256<uint> accVector = Vector256<uint>.Zero;
            int size = Vector256<uint>.Count;

            ReadOnlySpan<Vector128<ushort>> vectors = MemoryMarshal.Cast<char, Vector128<ushort>>(data);

            int rem = data.Length % size;
            uint zero = 48 * (uint)vectors.Length;

            Vector256<uint> extra = Vector256.Create(zero);

            for (int i = 0; i < vectors.Length; i++)
            {
                //Vector.
                Vector256<uint> tmp1 = Avx2.ConvertToVector256Int32(vectors[i]).AsUInt32();
                accVector = Avx2.Add(accVector, tmp1);
            }

            accVector = Avx2.Subtract(accVector, extra);
            ulong ret = Vector256.Sum(accVector);

            if (rem > 0)
                ret += data[^rem..].SumChar();
            return ret;
        }

        public static ulong SumVx240(this in ReadOnlySpan<char> data)
        {
            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);

            Vector256<uint> accVector1 = Vector256<uint>.Zero;


            foreach (ref readonly Vector256<ushort> vector in vectors)
            {
                (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(vector);
                accVector1 += low + high;
            }

            ulong ret = Vector256.Sum(accVector1);

            ret += VxHelper.SumRemainder(data);

            return ret - '0' * (ulong)data.Length;
        }

        public static ulong SumVx250(this in ReadOnlySpan<char> data)
        {
            const int MaxSumUInt16 = 1149;//ushort.MaxValue / (48 + 9);
            const int MaxSumUInt32 = 65_579;// uint.MaxValue / (48 + 9) /maxSumUInt16;

            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);

            Vector256<ulong> finalAcc = Vector256<ulong>.Zero;

            //////////////////////////////////////////////////

            int shortChunkSize = vectors.Length / MaxSumUInt16;
            List<Vector256<ushort>> shortAccumulatorsList = new(shortChunkSize + 1);

            foreach (ReadOnlySpan<Vector256<ushort>> chunk in vectors.Chunk(MaxSumUInt16))
            {
                Vector256<ushort> saturatedShortAcc = Vector256<ushort>.Zero;
                foreach (ref readonly Vector256<ushort> vector in chunk)
                    saturatedShortAcc += vector;

                shortAccumulatorsList.Add(saturatedShortAcc);
            }
            //////////////////////////////////////////////////
            ReadOnlySpan<Vector256<ushort>> shortAccs = CollectionsMarshal.AsSpan(shortAccumulatorsList);

            foreach (ReadOnlySpan<Vector256<ushort>> chunk1 in shortAccs.Chunk(MaxSumUInt32))
            {
                Vector256<uint> saturatedAcc = Vector256<uint>.Zero;
                foreach (ref readonly Vector256<ushort> vector1 in chunk1)
                {
                    (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(vector1);
                    saturatedAcc += low + high;
                }
                (Vector256<ulong> low1, Vector256<ulong> high1) = Vector256.Widen(saturatedAcc);
                finalAcc += low1 + high1;
            }

            //////////////////////////////////////////////////

            ulong ret = Vector256.Sum(finalAcc);

            ret += VxHelper.SumRemainder(data);// Remaining

            return ret - '0' * (uint)data.Length;
        }

        public static ulong SumVx251(this in ReadOnlySpan<char> data)
        {
            const int MaxSumUInt16 = 1149;//ushort.MaxValue / (48 + 9);
            const int MaxSumUInt32 = 65_579; //uint.MaxValue / (48 + 9) /maxSumUInt16;
            const int OuterPage = MaxSumUInt16 * MaxSumUInt32; // 75 350 271

            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);
            Vector256<ulong> finalAcc = Vector256<ulong>.Zero;
            for (int longIndex = 0; longIndex < vectors.Length; longIndex = +OuterPage)
            {
                Vector256<uint> saturatedLowAcc = Vector256<uint>.Zero;
                Vector256<uint> saturatedHighAcc = Vector256<uint>.Zero;
                for (int index = 0; index < OuterPage && index + longIndex < vectors.Length; index += MaxSumUInt16)
                {
                    Vector256<ushort> saturatedShortAcc = Vector256<ushort>.Zero;
                    for (int shortIndex = 0; shortIndex < MaxSumUInt16 && shortIndex + index + longIndex < vectors.Length; shortIndex++)
                        saturatedShortAcc += vectors[shortIndex + index + longIndex];

                    (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(saturatedShortAcc);
                    saturatedLowAcc += low;
                    saturatedHighAcc += high;
                }

                (Vector256<ulong> low11, Vector256<ulong> high11) = Vector256.Widen(saturatedLowAcc);
                finalAcc += low11 + high11;
                (Vector256<ulong> low12, Vector256<ulong> high12) = Vector256.Widen(saturatedHighAcc);
                finalAcc += low12 + high12;
            }
            ulong ret = Vector256.Sum(finalAcc);
            ret += VxHelper.SumRemainder(data);// Remaining
            return ret - '0' * (ulong)data.Length;
        }

        public static ulong SumVx252(this in ReadOnlySpan<char> data)
        {
            const int MaxSumUInt16 = 1149;//1149;//ushort.MaxValue / (48 + 9);
            const int MaxSumUInt32 = 65_579; //uint.MaxValue / (48 + 9) /maxSumUInt16;
            const int OuterStep = MaxSumUInt16 * MaxSumUInt32; // 75 350 271

            ReadOnlySpan<Vector256<ushort>> vectors = MemoryMarshal.Cast<char, Vector256<ushort>>(data);
            ref Vector256<ushort> startRef = ref MemoryMarshal.GetReference(vectors);
            Vector256<ulong> finalAcc = Vector256<ulong>.Zero;
            for (int longIndex = 0; longIndex < vectors.Length; longIndex += OuterStep)
            {
                Vector256<uint> saturatedLowAcc = Vector256<uint>.Zero;
                Vector256<uint> saturatedHighAcc = Vector256<uint>.Zero;
                int maxMidIteration = Math.Min(OuterStep, vectors.Length - longIndex) + longIndex;
                for (int middleIndex = longIndex; middleIndex < maxMidIteration; middleIndex += MaxSumUInt16)
                {
                    Vector256<ushort> saturatedShortAcc = Vector256<ushort>.Zero;
                    ref Vector256<ushort> innerRef = ref Unsafe.Add(ref startRef, middleIndex);

                    int maxInnerIteration = Math.Min(MaxSumUInt16, vectors.Length - middleIndex);
                    for (int innerIndex = 0; innerIndex < maxInnerIteration; innerIndex++)
                        saturatedShortAcc += Unsafe.Add(ref innerRef, innerIndex);

                    (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(saturatedShortAcc);
                    saturatedLowAcc += low;
                    saturatedHighAcc += high;
                }

                (Vector256<ulong> low11, Vector256<ulong> high11) = Vector256.Widen(saturatedLowAcc);
                finalAcc += low11 + high11;
                (Vector256<ulong> low12, Vector256<ulong> high12) = Vector256.Widen(saturatedHighAcc);
                finalAcc += low12 + high12;

            }
            ulong ret = Vector256.Sum(finalAcc);
            ret += VxHelper.SumRemainder(data);// Remaining
            return ret - '0' * (ulong)data.Length;
        }
    }
}