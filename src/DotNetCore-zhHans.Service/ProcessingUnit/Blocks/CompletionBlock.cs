using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Base.Interfaces;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    /// <summary>
    /// 负责等待翻译
    /// </summary>
    internal class CompletionBlock : UnitBase<NodeCacheData>
    {
        private readonly TransformBlock<NodeCacheData, NodeCacheData> block;

        public CompletionBlock(ITransmitData transmits, UpdateXmlBlock updateXmlUnit) : base(transmits)
        {
            var execOption = CreateExecutionDataflowBlockOption(10000, 100);
            block = new(Handler, execOption);
            block.LinkTo(updateXmlUnit);
        }

        protected override ITargetBlock<NodeCacheData> TargetBlock => block;

        private async Task<NodeCacheData> Handler(NodeCacheData obj)
        {
            if (Token.IsCancellationRequested) return default;
            Token.ThrowIfCancellationRequested();
            Increment();
            ShowInformation();
            await obj.Wait(Token);
            obj.Node.SetTranslValue(obj.CacheData.Value);
            Decrement();
            ShowInformation();
            return obj;
        }

        private void ShowInformation() => Transmits
            .Set(() => Transmits.Progress.Master.Title3 = $"等待 : {ParallelCount}");

        public override Task Complete() => SetComplete(block);
    }
}
