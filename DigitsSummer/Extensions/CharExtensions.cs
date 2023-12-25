using System.Collections.Frozen;
using System.Collections.Generic;

namespace DigitsSummer.Extensions;

public static class CharExtensions
{
    private static readonly FrozenDictionary<char, ulong> _hashUInt64 = new Dictionary<char, ulong>
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
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<char, long> _hashInt64 = new Dictionary<char, long>
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
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<char, byte> _hash8 = new Dictionary<char, byte>
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
    }.ToFrozenDictionary();

    public static byte ToByte(this char @char) => _hash8[@char];
    public static long ToLong(this char @char) => _hashInt64[@char];
    public static ulong ToULong(this char @char) => _hashUInt64[@char];
}