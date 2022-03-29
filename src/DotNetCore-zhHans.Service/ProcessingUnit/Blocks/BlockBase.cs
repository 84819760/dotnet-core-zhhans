using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Base.Interfaces;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    internal abstract class UnitBase<T> : UnitBase, ITargetBlock<T>
    {
        private int parallelCount;

        protected UnitBase(ITransmitData transmits) : base(transmits) { }

        public virtual Task SendAsync(T value) => TargetBlock.SendAsync(value, Token);

        protected abstract ITargetBlock<T> TargetBlock { get; }

        protected int ParallelCount => parallelCount;

        protected void Increment() => Interlocked.Increment(ref parallelCount);
        protected void Decrement() => Interlocked.Decrement(ref parallelCount);
        protected void ClearCount() => Interlocked.Exchange(ref parallelCount, 0);

        protected ExecutionDataflowBlockOptions CreateExecutionDataflowBlockOption(int count)
        {
            return CreateExecutionDataflowBlockOption(count, count);
        }

        protected ExecutionDataflowBlockOptions CreateExecutionDataflowBlockOption
            (int boundedCapacity, int maxDegreeOfParallelism)
        {
            boundedCapacity = boundedCapacity > 0 ? boundedCapacity : 1;
            maxDegreeOfParallelism = maxDegreeOfParallelism > 0 ? maxDegreeOfParallelism : 1;
            return new()
            {
                BoundedCapacity = boundedCapacity,
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = Token
            };
        }

        protected static async Task SetComplete(IDataflowBlock block)
        {
            try
            {
                block.Complete();
                await block.Completion;
            }
            catch (Exception ex)
            {
                if (!ex.IsCanceled()) throw;
            }
        }

        #region ITargetBlock<T>

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader
            , T messageValue
            , ISourceBlock<T> source
            , bool consumeToAccept)
            => TargetBlock.OfferMessage(messageHeader, messageValue, source, consumeToAccept);

        void IDataflowBlock.Complete() => TargetBlock.Complete();

        public void Fault(Exception exception) => TargetBlock.Fault(exception);

        public Task Completion => TargetBlock.Completion;
        #endregion
    }
}
