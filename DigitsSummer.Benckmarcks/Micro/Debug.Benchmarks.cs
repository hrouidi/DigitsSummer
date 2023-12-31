using System;
using System.Buffers;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using Perfolizer.Mathematics.OutlierDetection;

namespace DigitsSummer.Benchmarks.Micro
{

    //[Outliers(OutlierMode.DontRemove)]
    [RankColumn]
    [MemoryDiagnoser]
    [MedianColumn]
    [Config(typeof(Config))]
    public class DebugBenchmarks
    {
        private Vector256<ushort> _a;
        private Vector<ushort> _aa;
        private const int max = ushort.MaxValue;

        [GlobalSetup]
        public void GlobalSetup()
        {
            //_a = Vector256.Create(Random.Shared.GetItems<ushort>([48, 49, 50, 51, 52, 53, 54, 56, 57, 58], Vector256<ushort>.Count));
            _a = Vector256<ushort>.One;
            _aa = Vector<ushort>.One;
        }

        [Benchmark]
        public Vector<ushort> VectorAdd()
        {
            Vector<ushort> ret = Vector<ushort>.Zero;
            for (int i = 0; i < max; i++)
                ret += _aa;
            return ret;
        }

        [Benchmark]
        public Vector256<ushort> Avx2Add()
        {
            Vector256<ushort> ret = Vector256<ushort>.Zero;
            for (int i = 0; i < max; i++)
                ret = Avx2.Add(ret, _a);
            return ret;
        }


        [Benchmark(Baseline = true)]
        public Vector256<ushort> Baseline()
        {
            Vector256<ushort> ret = Vector256<ushort>.Zero;
            for (int i = 0; i < max; i++)
                ret += _a;
            return ret;
        }
    }
}
