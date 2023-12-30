using ILGPU.Runtime;
using ILGPU;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using ILGPU.Runtime.CPU;
using TInput = ushort;

namespace DigitsSummer
{
    public class GpuDigitsSummer : IDisposable
    {
        const int cores = 768;

        private readonly Context _context;
        private readonly Accelerator _accelerator;
        private readonly Action<Index1D, ArrayView<TInput>, ArrayView<ulong>> _kernel;

        public GpuDigitsSummer()
        {
            _context = Context.Create(x => x.CPU());
            _accelerator = _context.Devices[0].CreateAccelerator(_context);
            _kernel = _accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<TInput>, ArrayView<ulong>>(MyKernel);

            static void MyKernel(Index1D index, ArrayView<TInput> dataView, ArrayView<ulong> output)
            {
                
                int chunkSize = dataView.IntLength / cores;
                chunkSize+= dataView.IntLength % cores != 0 ? 1 : 0;
                int startIndex = index.X * chunkSize;
                int max = startIndex + chunkSize;
                ulong acc = 0;
                int i;
                for ( i = startIndex; i < max && i < dataView.IntLength; i++)
                    acc += dataView[i];

                output[index] = acc;
            
                Interop.WriteLine("{0} = [{1}-{2}]  ={3}", index, startIndex,i-1, i-startIndex);
            }
        }

        public ulong Sum(in ReadOnlySpan<char> data)
        {
            using var buffer = _accelerator.Allocate1D<TInput>(data.Length);
            using var output = _accelerator.Allocate1D<ulong>(cores);
            buffer.CopyFromCPU(MemoryMarshal.Cast<char, ushort>(data).ToArray());

            _kernel((int)output.Length, buffer.View, output.View);

            ulong[]? outputArray = output.GetAsArray1D();

            return outputArray.Aggregate((x, y) => x + y);
        }

        public void Dispose()
        {
            _context.Dispose();
            _accelerator.Dispose();
        }
    }
}