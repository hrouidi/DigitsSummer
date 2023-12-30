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

        private readonly Action[] _workers;

        public SumProducerConsumers(string fileName, int maxDegreeOfParallelism, int bufferSize = 1024 * 16)
        {
            _fileName = fileName;
            _bufferSize = bufferSize;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _blocks = new BlockingCollection<IMemoryOwner<char>>();
            //_producer = new Task(Produce);
            _workers = Enumerable.Range(1, _maxDegreeOfParallelism - 1)
                                 .Select(i => (Action)ConsumeTask)
                                 .Append(ProduceTask)
                                 .Reverse()
                                 .ToArray();
        }

        private void ProduceTask()
        {
            using var reader = new StreamReader(_fileName);
            while (true)
            {
                IMemoryOwner<char> lease = MemoryPool<char>.Shared.Rent(_bufferSize);
                int ret = reader.Read(lease.Memory.Span);
                if (ret >= _bufferSize)
                    _blocks.Add(lease);
                else// last chunk => compute sum directly here
                {
                    _blocks.CompleteAdding();
                    var sum = DigitsSummer.SumVx2A(lease.Memory.Span.Slice(0, ret));
                    Interlocked.Add(ref _result, sum);
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
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, _workers);
            return Interlocked.Read(ref _result);
        }

        public static ulong Run(string fileName, int bufferSize = 1024 * 16, int maxDegreeOfParallelism = -1)
        {
            if (maxDegreeOfParallelism == -1)
                maxDegreeOfParallelism = Environment.ProcessorCount;

            var pc = new SumProducerConsumers(fileName, maxDegreeOfParallelism, bufferSize);
            return (ulong)pc.Run();
        }
    }

    public class SumInMemoryFakeProducerConsumers
    {
        private long _result;
        private readonly BlockingCollection<ReadOnlyMemory<char>> _blocks;
        private readonly int _bufferSize;
        private readonly int _maxDegreeOfParallelism;
        private readonly string _fileName;

        private readonly Action[] _workers;

        public SumInMemoryFakeProducerConsumers(string fileName, int maxDegreeOfParallelism, int bufferSize = 1024 * 16)
        {
            _fileName = fileName;
            _bufferSize = bufferSize;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _blocks = new BlockingCollection<ReadOnlyMemory<char>>();
            _workers = Enumerable.Range(1, _maxDegreeOfParallelism - 1)
                                 .Select(i => (Action)ConsumeTask)
                                 .Append(ProduceTask)
                                 .Reverse()
                                 .ToArray();
        }

        private void ProduceTask()
        {
            Memory<char> tmp = new Memory<char>(new char[_bufferSize]);
            for (int i = 1; i < 2_000_000_000 / _bufferSize; ++i)
            {
                _blocks.Add(tmp);
            }
            _blocks.CompleteAdding();
        }

        private void ConsumeTask()
        {
            Vector256<long> localAccVx = Vector256<long>.Zero;
            foreach (var block in _blocks.GetConsumingEnumerable())
            {
                localAccVx = Avx2.Add(localAccVx, DigitsSummer.SumVx3(block.Span));
            }
            long localRet = 0;
            for (int i = 0; i < Vector256<long>.Count; ++i)
                localRet += localAccVx.GetElement(i);
            Interlocked.Add(ref _result, localRet);
        }

        private long Run()
        {
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, _workers);
            return Interlocked.Read(ref _result);
        }

        public static ulong Run(string fileName, int bufferSize = 1024 * 16, int maxDegreeOfParallelism = -1)
        {
            if (maxDegreeOfParallelism == -1)
                maxDegreeOfParallelism = Environment.ProcessorCount;

            var pc = new SumInMemoryFakeProducerConsumers(fileName, maxDegreeOfParallelism, bufferSize);
            return (ulong)pc.Run();
        }
    }
}