using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class SumFromFileDataFlowBenchmarks
    {
        private Dictionary<string, string> _fileMap;

        //[Params("1M", "10M", "100M")]
        //[Params("1G")]
        //public string Data { get; set; }

        //[Params(0,1,2,4,8,11,12,16)]
        //public int MaxDegreeOfParallelism { get; set; }
        private const string Data = "2G";
        [GlobalSetup]
        public void GlobalSetup()
        {
            //_fileMap = GlobalSetupHelper.GenerateDataFilesIfDoesNotExist("1M", "10M", "100M");
            Stopwatch sw = Stopwatch.StartNew();
            _fileMap = GlobalSetupHelper.GenerateDataFilesIfDoesNotExist(Data);
            sw.Stop();
            Console.WriteLine($"Data file generated on {sw.Elapsed}");
        }

        //[Benchmark(Baseline = true)]
        //public ulong SumV4FromFile() => DigitsSummer.SumV4FromFile(_fileMap["1G"]);


        //[Benchmark]
        [Benchmark(Baseline = true)]
        public ulong SumFromFileDataFlowV1() => SumFromFileDataFlow.SumV5FromFile(_fileMap[Data]);

        [Benchmark]
        public ulong SumFromFileDataFlowV2() => SumFromFileDataFlow.SumV6FromFile(_fileMap[Data]);

        [Benchmark]
        public ulong SumFromFileDataFlowVx2() => SumFromFileDataFlow.SumVx2FromFile(_fileMap[Data]);

        //[Benchmark]
        //public ulong SumV5FromFile1() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 1);

        //[Benchmark]
        //public ulong SumV5FromFile2() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 2);

        //[Benchmark]
        //public ulong SumV5FromFile4() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 4);
        //[Benchmark]
        //public ulong SumV5FromFile8() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 8);
        //[Benchmark]
        //public ulong SumV5FromFile11() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 11);

        //[Benchmark]
        //public ulong SumV5FromFile12() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 12);

        //[Benchmark]
        //public ulong SumV5FromFile16() => SumFromFileDataFlow.SumV5FromFile(_fileMap["1G"], 16);
    }
}
