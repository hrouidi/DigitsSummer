using System;
using System.Buffers;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Perfolizer.Mathematics.OutlierDetection;

namespace DigitsSummer.Benchmarks.Micro
{

    [Outliers(OutlierMode.DontRemove)]
    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    [Config(typeof(Config))]
    public class DebugBenchmarks
    {
        private const int _length = 24_000_000;

        [Benchmark]
        public string GetItemsString()
        {
            return Random.Shared.GetItems<char>("0123456789", _length).ToString();
        }

        [Benchmark]
        public string GetItemsSpan()
        {
            char[] ret = new char[_length];
            Random.Shared.GetItems<char>("0123456789", ret);
            return ret.ToString();
        }

        [Benchmark(Baseline = true)]
        public string Baseline()
        {
            return GlobalSetupHelper.GenerateDataAsString(_length);
        }
    }
}
