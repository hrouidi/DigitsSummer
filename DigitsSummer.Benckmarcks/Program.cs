﻿using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace DigitsSummer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //Summary summary = BenchmarkRunner.Run<SumBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumFromFileBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumLinqBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumHashBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<HashVsParseBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumFromFileDataFlowBenchmarks>();
            Summary summary = BenchmarkRunner.Run<SumVxBenchmarks>();

            //Summary summary = BenchmarkRunner.Run<WinnerBenchmarks>();

        }
    }
}
