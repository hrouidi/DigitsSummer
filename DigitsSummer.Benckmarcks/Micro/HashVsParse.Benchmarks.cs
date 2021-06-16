using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;

namespace DigitsSummer.Benchmarks.Micro
{

    [RankColumn()]
    [MemoryDiagnoser]
    [MedianColumn]
    //[SimpleJob(RunStrategy.Monitoring)]
    public class HashVsParseBenchmarks
    {

        [Benchmark(Baseline = true)]
        public ulong Parse() => ulong.Parse("9");

        [Benchmark]
        public ulong Hash() => DigitsSummer.ByHash("9");

        [Benchmark]
        public ulong Sum() => 1234567890ul.Sum();

    }
}
