using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;

namespace DigitsSummer.Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic)]
    [MemoryDiagnoser]
    //[SimpleJob(RunStrategy.Monitoring)]
    public class SumLinqBenchmarks
    {
        private string _data1M;
        private string _data10M;
        private string _data100M;
        //private readonly string _data1G;

        [Params("1M", "10M", "100M")]
        public string Data { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data1M = GlobalSetupHelper.GenerateData(1_000_000);
            _data10M = GlobalSetupHelper.GenerateData(10_000_000);
            _data100M = GlobalSetupHelper.GenerateData(100_000_000);
            //_data1G = DigitsSummer.GenerateData(1_000_000_000);
        }

        [Benchmark(Baseline = true)]
        public ulong Sum() => Data switch
        {
            "1M" => DigitsSummer.Sum(_data1M),
            "10M" => DigitsSummer.Sum(_data10M),
            "100M" => DigitsSummer.Sum(_data100M),
            _ => throw new Exception(),
        };

        

        [Benchmark]
        public ulong SumPLinq() => Data switch
        {
            "1M" => DigitsSummer.SumPLinq(_data1M),
            "10M" => DigitsSummer.SumPLinq(_data10M),
            "100M" => DigitsSummer.SumPLinq(_data100M),
            _ => throw new Exception(),
        };
    }
}
