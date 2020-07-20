using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;

namespace DigitsSummer
{
    public class SumProducerConsumers
    {
        private long _result;
        private readonly BlockingCollection<IMemoryOwner<char>> _blocks;
        private readonly int _bufferSize;
        private readonly int _maxDegreeOfParallelism;
        private readonly string _fileName;

        private readonly Action[] _consumers;

        public SumProducerConsumers(string fileName, int maxDegreeOfParallelism, int bufferSize = 1024 * 16)
        {
            _fileName = fileName;
            _bufferSize = bufferSize;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _blocks = new BlockingCollection<IMemoryOwner<char>>();
            _consumers = Enumerable.Range(1, _maxDegreeOfParallelism)
                .Select(i => (Action)ConsumeTask)
                .ToArray();
        }

        private async Task Produce()
        {
            using var reader = new StreamReader(_fileName);
            while (true)
            {
                IMemoryOwner<char> lease = MemoryPool<char>.Shared.Rent(_bufferSize);
                int ret = await reader.ReadAsync(lease.Memory);
                if (ret >= _bufferSize)
                    _blocks.Add(lease);
                else// last chunk => compute sum directly here
                {
                    _blocks.CompleteAdding();
                    var sum = DigitsSummer.SumVx2(lease.Memory.Span.Slice(0, ret));
                    Interlocked.Add(ref _result, (long)sum);
                    lease.Dispose();
                    break;
                }
            }
        }

        private void ConsumeTask()
        {
            Vector256<long> localAccVx = Vector256<long>.Zero;
            foreach (var block in _blocks.GetConsumingEnumerable())
            {
                localAccVx = Avx2.Add(localAccVx, DigitsSummer.SumVx3(block.Memory.Span));
                block.Dispose();
            }
            long localRet = 0;
            for (int i = 0; i < Vector256<long>.Count; ++i)
                localRet += localAccVx.GetElement(i);
            Interlocked.Add(ref _result, localRet);
        }

        private long Run()
        {
            var producer = Produce();
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, _consumers);
            producer.Wait();
            return Interlocked.Read(ref _result);
        }

        public static ulong Run(string fileName,  int bufferSize = 1024 * 16, int maxDegreeOfParallelism = -1)
        {
            if (maxDegreeOfParallelism == -1)
                maxDegreeOfParallelism = Environment.ProcessorCount - 1;

            var pc = new SumProducerConsumers(fileName, maxDegreeOfParallelism, bufferSize);
            return (ulong)pc.Run(); 
        }
    }
}