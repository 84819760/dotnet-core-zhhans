using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ApiRequests;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    /// <summary>
    /// 负责打包请求
    /// </summary>
    internal class ApiDataPackBlock : UnitBase<NodeCacheData>
    {
        private static readonly ReaderWriterLockSlim slim = new();
        private readonly ApiRequestProvider apiRequestProvider;
        private readonly ApiRequestBlock apiRequestBlock;
        private ApiDataPackBox apiPackage;

        public ApiDataPackBlock(ITransmitData transmits, UpdateXmlBlock updateXmlBlock)
            : base(transmits)
        {
            apiRequestBlock = new(transmits, updateXmlBlock);
            apiRequestProvider = new(transmits);
        }

        protected override ITargetBlock<NodeCacheData> TargetBlock => default;

        public override async Task SendAsync(NodeCacheData value)
        {
            slim.EnterWriteLock();
            try
            {
                await Send(value);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        private async Task Send(NodeCacheData value)
        {
            while (true)
            {
                var apiPack = GetApiPackage();
                if (apiPack.TryAdd(value)) break;
                await apiRequestBlock.SendAsync(apiPack);
            }
        }

        private ApiDataPackBox GetApiPackage()
        {
            if (apiPackage is null || apiPackage.IsComplete)
            {
                var apiItem = apiRequestProvider.GetApiItem().Result;
                apiPackage = new(apiItem, Transmits);
            }
            return apiPackage;
        }

        public override async Task Complete()
        {
            if (apiPackage is not null && !apiPackage.IsComplete)
            {
                await apiRequestBlock.SendAsync(apiPackage);
            }
            await apiRequestBlock.Complete();
        }
    }
}