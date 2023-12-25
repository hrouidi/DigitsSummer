using System;
using BenchmarkDotNet.Attributes;

namespace DigitsSummer.Benchmarks.Micro
{

    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    public class VectorSumBenchmarks
    {
        private const string _data = "012345678912345";

        //[Benchmark]
        //public ulong SumRemainder_Extension() => _data.AsSpan().SumRemainder();

        //[Benchmark(Baseline = true)]
        //public ulong SumRemainder() => VxExtensions.SumRemainder2(_data.AsSpan());

    }
}
