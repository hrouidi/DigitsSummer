using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;

namespace DigitsSummer.Benchmarks
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic)]
    [MemoryDiagnoser]
    //[SimpleJob(RunStrategy.Monitoring)]
    public class SumFromFileDataFlowBenchmarks
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
        public ulong SumV4FromFile() => DigitsSummer.SumV4FromFile(_fileMap[Data]);


        [Benchmark]
        public ulong SumV5FromFile() => SumFromFileDataFlow.SumV5FromFile(_fileMap[Data]);
    }
}
