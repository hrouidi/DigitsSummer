using BenchmarkDotNet.Attributes;
using Perfolizer.Mathematics.OutlierDetection;

namespace DigitsSummer.Benchmarks
{
    [Outliers(OutlierMode.DontRemove)]
    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    public class SumParallelBenchmarks
    {

        private string _data10M;
  

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data10M = GlobalSetupHelper.GenerateDataAsString(10_000_000);
        }

        [Benchmark(Baseline = true)]
        public ulong SumParallel() => DigitsSummer.SumParallel(_data10M);

        [Benchmark]
        public ulong SumChar() => DigitsSummer.SumChar(_data10M);

    }
}
