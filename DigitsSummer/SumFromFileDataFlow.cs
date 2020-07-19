using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DigitsSummer
{
    public static class PipelineV1
    {
        private static long _result;

        private static TransformManyBlock<string, ReadOnlyMemory<char>> BuildReader(int bufferSize)
        {
            return new TransformManyBlock<string, ReadOnlyMemory<char>>(ReadLines);

            IEnumerable<ReadOnlyMemory<char>> ReadLines(string fileName)
            {
                using var reader = new StreamReader(fileName);
                while (!reader.EndOfStream)
                {
                    var lease = MemoryPool<char>.Shared.Rent(bufferSize);
                    var ret = reader.Read(lease.Memory.Span);
                    if (ret < bufferSize)
                        yield return lease.Memory.Slice(0, ret);
                    else
                        yield return lease.Memory;
                }
            }
        }

        private static ActionBlock<ReadOnlyMemory<char>> BuildSumBlock(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism == 0)
            {
                return new ActionBlock<ReadOnlyMemory<char>>(data =>
                {
                    var ret = DigitsSummer.SumV4(data.Span);
                    Interlocked.Add(ref _result, (long)ret);
                });
            }
            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism };
            return new ActionBlock<ReadOnlyMemory<char>>(data =>
            {
                var ret = DigitsSummer.SumV4(data.Span);
                Interlocked.Add(ref _result, (long)ret);
            }, options);
        }

        private static (ITargetBlock<string>, IDataflowBlock) CreatePipeline(int bufferSize, int maxDegreeOfParallelism)
        {
            var options = new DataflowLinkOptions { PropagateCompletion = true };
            var head = BuildReader(bufferSize);
            var sumBlock = BuildSumBlock(maxDegreeOfParallelism);
            head.LinkTo(sumBlock, options);
            return (head, sumBlock);
        }

        public static ulong Run(string fileName, int maxDegreeOfParallelism, int bufferSize = 1024 * 16)
        {
            if (maxDegreeOfParallelism == -1)
                maxDegreeOfParallelism = Environment.ProcessorCount - 1;
            Interlocked.Exchange(ref _result, 0);
            var (head, acc) = CreatePipeline(bufferSize, maxDegreeOfParallelism);
            head.Post(fileName);
            head.Complete();
            acc.Completion.Wait();
            return (ulong)Interlocked.Read(ref _result);
        }
    }

    public static class PipelineV2
    {
        private static long _result;

        private static TransformManyBlock<string, (IMemoryOwner<char>, int?)> BuildReader(int bufferSize)
        {
            return new TransformManyBlock<string, (IMemoryOwner<char>, int?)>(ReadLines);

            IEnumerable<(IMemoryOwner<char>, int?)> ReadLines(string fileName)
            {
                using var reader = new StreamReader(fileName);
                while (!reader.EndOfStream)
                {
                    IMemoryOwner<char> lease = MemoryPool<char>.Shared.Rent(bufferSize);
                    int ret = reader.Read(lease.Memory.Span);
                    if (ret >= bufferSize)
                        yield return (lease, null);
                    else
                        yield return (lease, ret);
                }
            }
        }

        private static ActionBlock<(IMemoryOwner<char>, int?)> BuildSumBlock(int maxDegreeOfParallelism)
        {
            static void Sum((IMemoryOwner<char>, int?) data)
            {
                var (owner, size) = data;
                Span<char> current = size.HasValue ? owner.Memory.Span.Slice(0, size.Value) : owner.Memory.Span;
                var ret = DigitsSummer.SumV4(current);
                Interlocked.Add(ref _result, (long)ret);
                owner.Dispose();
            }

            if (maxDegreeOfParallelism == 0)
                return new ActionBlock<(IMemoryOwner<char>, int?)>(Sum);
            return new ActionBlock<(IMemoryOwner<char>, int?)>(Sum, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });
        }

        private static (ITargetBlock<string>, IDataflowBlock) CreatePipeline(int bufferSize, int maxDegreeOfParallelism)
        {
            var options = new DataflowLinkOptions { PropagateCompletion = true };
            var head = BuildReader(bufferSize);
            var sumBlock = BuildSumBlock(maxDegreeOfParallelism);
            head.LinkTo(sumBlock, options);
            return (head, sumBlock);
        }

        public static ulong Run(string fileName, int maxDegreeOfParallelism, int bufferSize = 1024 * 16)
        {
            if (maxDegreeOfParallelism == -1)
                maxDegreeOfParallelism = Environment.ProcessorCount - 1;
            Interlocked.Exchange(ref _result, 0);
            var (head, acc) = CreatePipeline(bufferSize, maxDegreeOfParallelism);
            head.Post(fileName);
            head.Complete();
            acc.Completion.Wait();
            return (ulong)Interlocked.Read(ref _result);
        }
    }

    public static class PipelineVx2 
    {
        private static long _result;

        private static TransformManyBlock<string, (IMemoryOwner<char>, int?)> BuildReader(int bufferSize)
        {
            return new TransformManyBlock<string, (IMemoryOwner<char>, int?)>(ReadLines);

            IEnumerable<(IMemoryOwner<char>, int?)> ReadLines(string fileName)
            {
                using var reader = new StreamReader(fileName);
                while (!reader.EndOfStream)
                {
                    IMemoryOwner<char> lease = MemoryPool<char>.Shared.Rent(bufferSize);
                    int ret = reader.Read(lease.Memory.Span);
                    if (ret >= bufferSize)
                        yield return (lease, null);
                    else
                        yield return (lease, ret);
                }
            }
        }

        private static ActionBlock<(IMemoryOwner<char>, int?)> BuildSumBlock(int maxDegreeOfParallelism)
        {
            static void Sum((IMemoryOwner<char>, int?) data)
            {
                var (owner, size) = data;
                Span<char> current = size.HasValue ? owner.Memory.Span.Slice(0, size.Value) : owner.Memory.Span;
                var ret = DigitsSummer.SumVx2(current);
                Interlocked.Add(ref _result, (long)ret);
                owner.Dispose();
            }

            if (maxDegreeOfParallelism == 0)
                return new ActionBlock<(IMemoryOwner<char>, int?)>(Sum);
            return new ActionBlock<(IMemoryOwner<char>, int?)>(Sum, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });
        }

        private static (ITargetBlock<string>, IDataflowBlock) CreatePipeline(int bufferSize, int maxDegreeOfParallelism)
        {
            var options = new DataflowLinkOptions { PropagateCompletion = true };
            var head = BuildReader(bufferSize);
            var sumBlock = BuildSumBlock(maxDegreeOfParallelism);
            head.LinkTo(sumBlock, options);
            return (head, sumBlock);
        }

        public static ulong Run(string fileName, int maxDegreeOfParallelism, int bufferSize = 1024 * 16)
        {
            if (maxDegreeOfParallelism == -1)
                maxDegreeOfParallelism = Environment.ProcessorCount - 1;
            Interlocked.Exchange(ref _result, 0);
            var (head, acc) = CreatePipeline(bufferSize, maxDegreeOfParallelism);
            head.Post(fileName);
            head.Complete();
            acc.Completion.Wait();
            return (ulong)Interlocked.Read(ref _result);
        }
    }

    public static class SumFromFileDataFlow
    {
        public static ulong SumV5FromFile(string fileName, int maxDegreeOfParallelism = -1, int bufferSize = 1024 * 16)
        {
            return PipelineV1.Run(fileName, maxDegreeOfParallelism, bufferSize);
        }

        public static ulong SumV6FromFile(string fileName, int maxDegreeOfParallelism = -1, int bufferSize = 1024 * 16)
        {
            return PipelineV2.Run(fileName, maxDegreeOfParallelism, bufferSize);
        }

        public static ulong SumVx2FromFile(string fileName, int maxDegreeOfParallelism = -1, int bufferSize = 1024 * 16)
        {
            return PipelineVx2.Run(fileName, maxDegreeOfParallelism, bufferSize);
        }

    }
}