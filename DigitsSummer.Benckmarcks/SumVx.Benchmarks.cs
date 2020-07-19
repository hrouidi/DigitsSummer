using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Perfolizer.Mathematics.OutlierDetection;

namespace DigitsSummer.Benchmarks
{
    [Outliers(OutlierMode.DontRemove)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic)]
    [MemoryDiagnoser]
    //[SimpleJob(RunStrategy.Monitoring)]
    public class SumVxBenchmarks
    {
        private string _data1M;
        private string _data10M;
        private string _data100M;
        //private readonly string _data1G;

        //[Params("1M", "10M", "100M")]

        [Params("100M")]
        public string Data { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data1M = GlobalSetupHelper.GenerateDataAsString(1_000_000);
            _data10M = GlobalSetupHelper.GenerateDataAsString(10_000_000);
            _data100M = GlobalSetupHelper.GenerateDataAsString(100_000_000);
            //_data1G = DigitsSummer.GenerateData(1_000_000_000);
        }


        [Benchmark(Baseline = true)]
        public ulong SumV4() => Data switch
        {
            "1M" => DigitsSummer.SumV4(_data1M),
            "10M" => DigitsSummer.SumV4(_data10M),
            "100M" => DigitsSummer.SumV4(_data100M),
            _ => throw new Exception(),
        };


        [Benchmark]
        public ulong SumVx() => Data switch
        {
            "1M" => DigitsSummer.SumVx(_data1M),
            "10M" => DigitsSummer.SumVx(_data10M),
            "100M" => DigitsSummer.SumVx(_data100M),
            _ => throw new Exception(),
        };

        [Benchmark]
        public ulong SumVx2() => Data switch
        {
            "1M" => DigitsSummer.SumVx2(_data1M),
            "10M" => DigitsSummer.SumVx2(_data10M),
            "100M" => DigitsSummer.SumVx2(_data100M),
            _ => throw new Exception(),
        };

    }
}
