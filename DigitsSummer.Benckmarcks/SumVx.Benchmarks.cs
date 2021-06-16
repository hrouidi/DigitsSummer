using System;
using BenchmarkDotNet.Attributes;
using Perfolizer.Mathematics.OutlierDetection;

namespace DigitsSummer.Benchmarks
{
    [Outliers(OutlierMode.DontRemove)]
    [MemoryDiagnoser]
    //[SimpleJob(RunStrategy.Monitoring)]
    public class SumVxBenchmarks
    {
        private string _data;

        //[Params("1M", "10M", "100M")]

        [Params(300_000_000)]
        public int DataSize { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = GlobalSetupHelper.GenerateDataAsString(DataSize);
        }


        //[Benchmark(Baseline = true)]
        //public ulong SumV4() => DigitsSummer.SumV4(Input);


        //[Benchmark]
        //public ulong SumVx() => DigitsSummer.SumVx(Input);

        //[Benchmark(Baseline = true)]
        //public ulong SumVx2() => DigitsSummer.SumVx2(_data);
        
        [Benchmark(Baseline = true)]
        public ulong SumVx22() => DigitsSummer.SumVx22(_data);

        [Benchmark]
        public ulong SumVx23() => DigitsSummer.SumVx23(_data);

    }
}
