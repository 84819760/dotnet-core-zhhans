using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    /// <summary>
    /// 负责数据库写入
    /// </summary>
    internal class DbContextBlock : UnitBase<NodeCacheData>
    {
        private readonly TransformBlock<NodeCacheData, NodeCacheData> inBlock;
        private readonly ActionBlock<NodeCacheData[]> outBlock;
        private readonly BatchBlock<NodeCacheData> batchBlock;

        public DbContextBlock(ITransmitData transmits) : base(transmits)
        {
            inBlock = CerateInBlock();
            batchBlock = CreateBatchBlock();
            outBlock = CreateOutBlock();
            inBlock.LinkTo(batchBlock);
            batchBlock.LinkTo(outBlock);
        }

        private TransformBlock<NodeCacheData, NodeCacheData> CerateInBlock()
        {
            var inBlockOption = CreateExecutionDataflowBlockOption(1000);
            return new(InBlockHandler, inBlockOption);
        }

        private BatchBlock<NodeCacheData> CreateBatchBlock()
        {
            var groupOption = new GroupingDataflowBlockOptions()
            {
                BoundedCapacity = 1000,
                MaxMessagesPerTask = 1,
                CancellationToken = Token,
            };
            return new(1000, groupOption);
        }

        private ActionBlock<NodeCacheData[]> CreateOutBlock()
        {
            var actionOption = CreateExecutionDataflowBlockOption(1);
            return new(Write, actionOption);
        }

        protected override ITargetBlock<NodeCacheData> TargetBlock => inBlock;

        private NodeCacheData InBlockHandler(NodeCacheData data)
        {
            Increment();
            Show($"缓存 : {ParallelCount}");
            return data;
        }

        private async Task Write(NodeCacheData[] data)
        {
            if (Token.IsCancellationRequested || data is null) return;
            ClearCount();
            Show($"写入 : {data.Length}");
            using var context = new ZhDbContext(Transmits);
            await context.WriteRowsLock(data);
        }

        private void Show(string value) => Transmits
            .Set(() => Transmits.Progress.Master.Title2 = value);

        public override async Task Complete()
        {
            await SetComplete(inBlock);
            await SetComplete(batchBlock);
            await SetComplete(outBlock);
        }
    }
}
