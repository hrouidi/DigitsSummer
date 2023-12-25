using BenchmarkDotNet.Attributes;
using DigitsSummer.Extensions;

namespace DigitsSummer.Benchmarks.Micro
{

    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    public class HashVsParseBenchmarks
    {

        [Benchmark(Baseline = true)]
        public ulong Parse() => ulong.Parse("9");

        [Benchmark]
        public ulong Hash() => '9'.ToULong();

    }
}
