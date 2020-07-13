using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using DigitsSummer.Tests;

namespace DigitsSummer.Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic)]
    [MemoryDiagnoser]
    //[SimpleJob(RunStrategy.Monitoring)]
    public class SumBenchmarks
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
            _data1M = Helper.GenerateData(1_000_000);
            _data10M = Helper.GenerateData(10_000_000);
            _data100M = Helper.GenerateData(100_000_000);
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
        public ulong SumV2() => Data switch
        {
            "1M" => DigitsSummer.SumV2(_data1M),
            "10M" => DigitsSummer.SumV2(_data10M),
            "100M" => DigitsSummer.SumV2(_data100M),
            _ => throw new Exception(),
        };

        [Benchmark()]
        public ulong SumV3() => Data switch
        {
            "1M" => DigitsSummer.SumV3(_data1M),
            "10M" => DigitsSummer.SumV3(_data10M),
            "100M" => DigitsSummer.SumV3(_data100M),
            _ => throw new Exception(),
        };

        [Benchmark]
        public ulong SumV3_5() => Data switch
        {
            "1M" => DigitsSummer.SumV3_5(_data1M),
            "10M" => DigitsSummer.SumV3_5(_data10M),
            "100M" => DigitsSummer.SumV3_5(_data100M),
            _ => throw new Exception(),
        };

        [Benchmark]
        public ulong SumV4() => Data switch
        {
            "1M" => DigitsSummer.SumV4(_data1M),
            "10M" => DigitsSummer.SumV4(_data10M),
            "100M" => DigitsSummer.SumV4(_data100M),
            _ => throw new Exception(),
        };

    }
}
