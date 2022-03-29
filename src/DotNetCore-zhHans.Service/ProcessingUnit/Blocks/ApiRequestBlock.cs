using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ApiRequests;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    /// <summary>
    /// 负责执行请求
    /// </summary>
    internal class ApiRequestBlock : UnitBase<ApiDataPackBox>
    {
        private readonly ActionBlock<ApiDataPackBox> block;
        private readonly UpdateXmlBlock updateXmlBlock;
        private readonly DbContextBlock dbContextUnit;

        public ApiRequestBlock(ITransmitData transmits, UpdateXmlBlock updateXmlBlock) : base(transmits)
        {
            this.updateXmlBlock = updateXmlBlock;
            dbContextUnit = new(transmits);
            var opt = CreateExecutionDataflowBlockOption(GetConcurrentCount());
            block = new(Handler, opt);
        }

        private IEnumerable<ApiConfig> GetApiConfigs() => Transmits.Config.ApiConfigs;

        private int GetConcurrentCount() => GetApiConfigs()
            .Where(x => x.Enable)
            .Sum(x => x.ThreadCount);

        protected override ITargetBlock<ApiDataPackBox> TargetBlock => block;

        private async Task Handler(ApiDataPackBox apiDataPackBox)
        {
            try
            {
                if (Token.IsCancellationRequested) return;
                var group = apiDataPackBox.NodeCacheDataGroup;
                var api = apiDataPackBox.Api;
                api.Master = Transmits.Progress.Master;
                var resValue = await api.SendRequest(group.QueryValue, Token);
                var datas = await group.SetResponse(resValue, api, Token);
                var isOffline = api.ApiConfig.Name == "离线翻译";
                await SendNext(datas, resValue, group, isOffline);
            }
            catch (Exception)
            {
                Transmits.CancellationTokenSource.Cancel();
                throw;
            }
         
        }

        private async Task SendNext(NodeCacheData[] datas, string response, NodeCacheDataGroup group
            , bool isOffline)
        {
            foreach (var item in datas)
            {
                if (Token.IsCancellationRequested) return;
                if (item.IsResponseValue && item.IsCheckPassed && !isOffline)
                    await dbContextUnit.SendAsync(item);
                await updateXmlBlock.SendAsync(item);
            }
        }

        public override async Task Complete()
        {
            await SetComplete(block);
            await dbContextUnit.Complete();
        }
    }
}
