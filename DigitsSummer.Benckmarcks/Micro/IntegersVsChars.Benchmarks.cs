using System;
using BenchmarkDotNet.Attributes;
using DigitsSummer.Extensions;

namespace DigitsSummer.Benchmarks.Micro
{

    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    public class IntegersVsCharsBenchmarks
    {
        private const string _data = "18446744073709551615";

        [Benchmark]
        public ulong SumULong() => ulong.Parse(_data).SumULong();

        [Benchmark(Baseline = true)]
        public ulong SumChar() => _data.AsSpan().SumChar();

    }
}
