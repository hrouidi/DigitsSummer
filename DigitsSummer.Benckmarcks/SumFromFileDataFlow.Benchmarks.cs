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
        private const string _data = "2G";

        [Params(1024* 16/*,1024*64,1024*256,1024*1024,1024*1024*32*/)]
        public int BufferSize{ get; set; }


        [GlobalSetup]
        public void GlobalSetup()
        {
            //_fileMap = GlobalSetupHelper.GenerateDataFilesIfDoesNotExist("1M", "10M", "100M");
            Stopwatch sw = Stopwatch.StartNew();
            _fileMap = GlobalSetupHelper.GenerateDataFilesIfDoesNotExist(_data);
            sw.Stop();
            Console.WriteLine($"Data file generated on {sw.Elapsed}");
        }

        //[Benchmark(Baseline = true)]
        //public ulong SumV4FromFile() => DigitsSummer.SumV4FromFile(_fileMap["1G"]);


        //[Benchmark]
        [Benchmark(Baseline = true)]
        //public ulong SumFromFileDataFlowV1() => SumFromFileDataFlow.SumV5FromFile(_fileMap[Data]);

        //[Benchmark]
        //public ulong SumFromFileDataFlowV2() => SumFromFileDataFlow.SumV6FromFile(_fileMap[Data]);

        //[Benchmark]
        public ulong SumFromFileDataFlowVx2() => SumFromFileDataFlow.SumVx2FromFile(_fileMap[_data], BufferSize);

        [Benchmark]
        public ulong SumFromFileDataFlowVx3() => SumFromFileDataFlow.SumVx3FromFile(_fileMap[_data], BufferSize);

        [Benchmark]
        public ulong SumFromFileDataFlowVx4() => SumFromFileDataFlow.SumVx4FromFile(_fileMap[_data], BufferSize);

    }
}
