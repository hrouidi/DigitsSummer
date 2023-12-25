using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DigitsSummer.Extensions
{
    public readonly ref struct ChunkSpanGenerator<TItem>(ReadOnlySpan<TItem> current, int chuckSize)
    {
        private readonly ReadOnlySpan<TItem> _current = current;

        public Enumerator GetEnumerator() => new(_current, chuckSize);

        public ref struct Enumerator(ReadOnlySpan<TItem> items, int chuckSize)
        {
            private readonly ReadOnlySpan<TItem> _items = items;
            private readonly int _chuckSize = chuckSize <= 0 ? items.Length : chuckSize;
            private int _startIndex = 0;

            public ReadOnlySpan<TItem> Current { get; private set; }

            public bool MoveNext()
            {
                if (_startIndex + _chuckSize < _items.Length)
                {
                    Current = _items.Slice(_startIndex, _chuckSize);
                    _startIndex += _chuckSize;
                    return true;
                }

                if (_startIndex < _items.Length)
                {
                    Current = _items[_startIndex..];
                    _startIndex += _chuckSize;
                    return true;
                }

                return false;
            }
        }
    }

    public static class Extensions
    {
        public static IEnumerable<(int index, int size)> Partitions<TItem>(this ReadOnlySpan<TItem> span, int count = 0)
        {
            return EnumeratesPartitions(span.Length, count == 0 ? Environment.ProcessorCount : count);

            static IEnumerable<(int index, int size)> EnumeratesPartitions(int length, int count)
            {
                (int chunkSize, int remainder) = int.DivRem(length, count);
                if (chunkSize == 0)
                {
                    yield return (0, length);
                    yield break;
                }

                int startIndex = 0;
                for (; startIndex + chunkSize <= length;)
                {
                    int fixedSize = chunkSize + (remainder-- > 0 ? 1 : 0);
                    yield return (startIndex, fixedSize);
                    startIndex += fixedSize;
                }
            }
        }

        public static ChunkSpanGenerator<TItem> Chunk<TItem>(this ReadOnlySpan<TItem> span, int chuckSize) => new(span, chuckSize);

        public static IEnumerable<string> SubStrings(this string source, int size)
        {
            int startIndex = 0;
            for (; startIndex + size < source.Length; startIndex += size)
                yield return source.Substring(startIndex, size);

            if (startIndex < source.Length)
                yield return source[startIndex..];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SumULong(this ulong digits)
        {
            ulong ret = 0;
            while (digits != 0)
            {
                ret += digits % 10;
                digits /= 10;
            }
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SumChar(this in ReadOnlySpan<char> smallString)
        {
            ulong ret = 0;
            foreach (char unicode16Char in smallString)
                ret += unicode16Char;

            return ret - '0' * (ulong)smallString.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SumAsChar(this in ReadOnlySpan<char> smallString)
        {
            ulong ret = 0;
            foreach (char unicode16Char in smallString)
                ret += unicode16Char;
            return ret;
        }
    }
}
