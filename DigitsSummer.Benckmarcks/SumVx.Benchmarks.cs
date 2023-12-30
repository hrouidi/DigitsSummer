using BenchmarkDotNet.Attributes;
using Perfolizer.Mathematics.OutlierDetection;

namespace DigitsSummer.Benchmarks
{
    [Outliers(OutlierMode.DontRemove)]
    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    [Config(typeof(Config))]
    public class SumVxBenchmarks
    {

        private string _data;


        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = GlobalSetupHelper.GenerateDataAsString(1_000_000_000);
        }

        [Benchmark(Baseline = true)]
        public ulong Sum() => DigitsSummer.SumChar(_data);

        [Benchmark]
        public ulong SumVx251() => DigitsSummer.SumVx251(_data);


        [Benchmark]
        public ulong SumParallel() => DigitsSummer.SumParallel(_data);

        [Benchmark]
        public ulong SumVx24Parallel() => DigitsSummer.SumVx24Parallel(_data);

        [Benchmark]
        public ulong SumVx251Parallel() => DigitsSummer.SumVx251Parallel(_data);


        //[Benchmark]
        //public ulong SumVx240() => DigitsSummer.SumVx240(_data);
        

        //[Benchmark]
        //public long SumSseInner() => DigitsSummer.SumSseInner(_data);


        ////[Benchmark]
        //public ulong SumVx23() => DigitsSummer.SumVx23(_data);

        ////[Benchmark]
        //public ulong SumVx241() => DigitsSummer.SumVx241(_data);

    }
}
