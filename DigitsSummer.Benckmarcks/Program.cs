using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using DigitsSummer.Benchmarks.Micro;

namespace DigitsSummer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //Summary summary = BenchmarkRunner.Run<IntegersVsCharsBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumParallelBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<VectorSumBenchmarks>();
            Summary summary = BenchmarkRunner.Run<SumVxBenchmarks>();

            //Summary summary = BenchmarkRunner.Run<SumFromFileBenchmarks>();

            //Summary summary = BenchmarkRunner.Run<SumHashBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<HashVsParseBenchmarks>();
            //Summary summary = BenchmarkRunner.Run<SumFromFileDataFlowBenchmarks>();




            //Summary summary = BenchmarkRunner.Run<DebugBenchmarks>();

        }
    }
    public class Config : ManualConfig
    {
        public Config() { SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend); }
    }
}
