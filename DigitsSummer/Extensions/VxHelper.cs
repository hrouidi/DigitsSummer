using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace DigitsSummer.Extensions;

public static class VxHelper
{
    const int _maxSumUInt16 = 1149;//ushort.MaxValue / (48 + 9);
    const int _maxSumUInt32 = 65_579;// uint.MaxValue / (48 + 9) /maxSumUInt16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<Vector256<ushort>> Sum(in ReadOnlySpan<Vector256<ushort>> vectors)
    {
        int shortChunkSize = vectors.Length / _maxSumUInt16;

        List<Vector256<ushort>> shortAccumulatorsList = new(shortChunkSize + 1);

        foreach (ReadOnlySpan<Vector256<ushort>> chunk in vectors.Chunk(shortChunkSize))
        {
            Vector256<ushort> shortAcc = Vector256<ushort>.Zero;
            foreach (ref readonly Vector256<ushort> vector in chunk)
                shortAcc += vector;

            shortAccumulatorsList.Add(shortAcc);
        }

        return CollectionsMarshal.AsSpan(shortAccumulatorsList);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<Vector256<uint>> SumOverflow(in ReadOnlySpan<Vector256<ushort>> vectors)
    {
        int chunkSize = vectors.Length / _maxSumUInt32;
        List<Vector256<uint>> accumulatorsList = new(chunkSize + 1);
        foreach (ReadOnlySpan<Vector256<ushort>> chunk in vectors.Chunk(chunkSize))
        {
            Vector256<uint> acc = Vector256<uint>.Zero;
            foreach (ref readonly Vector256<ushort> vector in chunk)
            {
                (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(vector);
                acc += low + high;
            }
            accumulatorsList.Add(acc);
        }

        return CollectionsMarshal.AsSpan(accumulatorsList);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<ulong> Sum(in ReadOnlySpan<Vector256<uint>> vectors)
    {
        Vector256<ulong> finalAcc = Vector256<ulong>.Zero;
        foreach (ref readonly Vector256<uint> vector in vectors)
        {
            (Vector256<ulong> low, Vector256<ulong> high) = Vector256.Widen(vector);
            finalAcc += low + high;
        }

        return finalAcc;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SumRemainder(in ReadOnlySpan<char> data)
    {
        int rem = data.Length % Vector256<ushort>.Count;
        if (rem > 0)
            return data[^rem..].SumAsChar();

        return 0;
    }
}