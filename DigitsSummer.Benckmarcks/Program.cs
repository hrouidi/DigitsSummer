using System;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace DigitsSummer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //Summary summary = BenchmarkRunner.Run<SumBenchmarks>();
            Summary summary = BenchmarkRunner.Run<SumFromFileBenchmarks>();
        }
    }
}
