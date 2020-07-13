using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;

namespace DigitsSummer.Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic)]
    [MemoryDiagnoser]
    public class SumFromFileBenchmarks
    {
        private Dictionary<string, string> _fileMap;

        [Params("1M", "10M", "100M")]
        public string Data { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _fileMap = GlobalSetupHelper.GenerateDataFilesIfDoesNotExist("1M", "10M", "100M");
        }

        [Benchmark(Baseline = true)]
        public ulong Sum() => DigitsSummer.SumFromFile(_fileMap[Data]);

        [Benchmark]
        public ulong SumV2() => DigitsSummer.SumV2FromFile(_fileMap[Data]);

        [Benchmark]
        public ulong SumV3() => DigitsSummer.SumV3FromFile(_fileMap[Data]);

        [Benchmark]
        public ulong SumV3_5() => DigitsSummer.SumV3_5FromFile(_fileMap[Data]);

        [Benchmark]
        public ulong SumV4() => DigitsSummer.SumV4FromFile(_fileMap[Data]);

    }
}
