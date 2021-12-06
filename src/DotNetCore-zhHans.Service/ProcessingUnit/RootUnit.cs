using System.Linq;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db;
using DotNetCoreZhHans.Service.XmlNodes;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    internal class RootUnit : UnitBase<RootNode>, IAsyncDisposable
    {
        private readonly NodeCacheDataFactory nodeCacheDataFactory;
        private readonly ApiDataPackBlock apiDataPackBlock;
        private readonly CompletionBlock completionBlock;
        private readonly UpdateXmlBlock updateXmlBlock;
        private readonly ZhDbContext dbContext;

        public RootUnit(ITransmitData transmits) : base(transmits)
        {
            dbContext = new(transmits);
            updateXmlBlock = new(transmits);
            nodeCacheDataFactory = new(dbContext);
            completionBlock = new(transmits, updateXmlBlock);
            apiDataPackBlock = new(transmits, updateXmlBlock);
        }

        public override async Task Complete()
        {
            await Complete(apiDataPackBlock, completionBlock, updateXmlBlock);
            await dbContext.DisposeAsync();
        }

        protected override ITargetBlock<RootNode> TargetBlock => default;

        public override async Task SendAsync(RootNode root)
        {
            var rootData = await nodeCacheDataFactory.CreateNodeCacheData(root);
            var items = root.GetRequestNodeCacheDatas(dbContext).Union(new[] { rootData }).ToArray();

            if (rootData.CacheData.IsCompletion)
            {
                root.SetTranslValue(rootData.CacheData.Value);
            }
            foreach (var item in items) await SendAsync(item);
        }

        internal Task SendAsync(NodeCacheData data) => data switch
        {
            { IsRequest: true } => apiDataPackBlock.SendAsync(data),
            { IsCompletion: true } => updateXmlBlock.SendAsync(data),
            _ => completionBlock.SendAsync(data),
        };

        public async ValueTask DisposeAsync() => await Complete();
    }
}
