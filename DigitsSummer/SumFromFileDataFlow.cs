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
    public static class SumFromFileDataFlow
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

                    var buffer = MemoryPool<char>.Shared.Rent(bufferSize).Memory; ;
                    var ret = reader.ReadBlockAsync(buffer).AsTask().Result;
                    if (ret < bufferSize)
                        yield return buffer.Slice(0, ret);
                    else
                        yield return buffer;
                }
            }
        }

        private static TransformBlock<ReadOnlyMemory<char>, ulong> BuildSumBlock()
        {
            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            return new TransformBlock<ReadOnlyMemory<char>, ulong>(data => DigitsSummer.SumV4(data.Span), options);
        }

        private static ActionBlock<ulong> BuildAccumulator()
        {
            return new ActionBlock<ulong>(ret => Interlocked.Add(ref _result, (long)ret));
        }

        private static (ITargetBlock<string>, IDataflowBlock) CreatePipeline(int bufferSize)
        {
            var options = new DataflowLinkOptions { PropagateCompletion = true };
            var head = BuildReader(bufferSize);
            var sumBlock = BuildSumBlock();
            var acc = BuildAccumulator();
            head.LinkTo(sumBlock, options);
            sumBlock.LinkTo(acc, options);
            return (head, acc);
        }

        public static ulong SumV5FromFile(string fileName, int bufferSize = 1024 * 16)
        {
            Interlocked.Exchange(ref _result, 0);
            var (head, acc) = CreatePipeline(bufferSize);
            head.Post(fileName);
            head.Complete();
            acc.Completion.Wait();
            return (ulong)Interlocked.Read(ref _result);
        }

    }
}