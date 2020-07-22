using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class WinnerBenchmarks
    {
        private Dictionary<string, string> _fileMap;

        private Dictionary<string, int> _sizes = new Dictionary<string, int>
        {
            ["32K"] = 1024 * 16,
            ["64K"] = 1024 * 32,
            ["128K"] = 1024 * 64,
            ["256K"] = 1024 * 128,
            ["512K"] = 1024 * 256,
            ["1M"] = 1024 * 512,
            ["2M"] = 1024 * 1024,
            ["4M"] = 1024 * 1024 * 2,
            ["32M"] = 1024 * 1024 * 16,
            ["64M"] = 1024 * 1024 * 32,
            ["128M"] = 1024 * 1024 * 64,
        };

        public IEnumerable<string> BufferSizeParamsSource => _sizes.Keys.Take(3);

        //[Params(0,1,2,4,8,11,12,16)]
        //public int MaxDegreeOfParallelism { get; set; }

        //private const string Data = "2G";
        [Params("2G")]
        public string Data{ get; set; }

        [ParamsSource(nameof(BufferSizeParamsSource))]
        public string BufferSize { get; set; }


        [GlobalSetup]
        public void GlobalSetup()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _fileMap = GlobalSetupHelper.GenerateDataFilesIfDoesNotExist(Data);
            sw.Stop();
            Console.WriteLine($"Data file generated on {sw.Elapsed}");
        }

        [Benchmark(Baseline = true)]
        public ulong Sum_FS_DataFlow_Vx2() => SumFromFileDataFlow.SumVx2FromFile(_fileMap[Data], _sizes[BufferSize]);

        [Benchmark]
        public ulong Sum_FS_ProducerConsumers_VX3() => SumProducerConsumers.Run(_fileMap[Data], _sizes[BufferSize]);

    }
}
