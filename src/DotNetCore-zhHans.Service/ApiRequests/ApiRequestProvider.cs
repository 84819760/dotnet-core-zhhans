using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using TranslateApi;
using DotNetCoreZhHans.Service.ProcessingUnit;
using System.Diagnostics;
using System.Threading;

namespace DotNetCoreZhHans.Service.ApiRequests
{
    internal class ApiRequestProvider
    {
        private readonly Channel<ApiRequestItem> channel = Channel.CreateUnbounded<ApiRequestItem>();
        private readonly ITransmitData transmits;

        public ApiRequestProvider(ITransmitData transmits)
        {
            this.transmits = transmits;
            InitChannel();
        }

        private async void InitChannel()
        {
            var maps = GetMap();
            var items = transmits.Config.ApiConfigs.Where(x => x.Enable).ToArray();
            foreach (var item in items)
            {
                var targetType = maps[item.Name];
                Add(targetType, item).Wait();
            }
            await AddOffline(items);
        }

        private async Task Add(Type type, ApiConfig apiConfig)
        {
            for (var i = 0; i < apiConfig.ThreadCount; i++)
            {
                var instance = CreateInstance(type, apiConfig);
                var apiItem = new ApiRequestItem(this, instance, transmits);
                await channel.Writer.WriteAsync(apiItem, transmits.Token);
            }
        }

        private async Task AddOffline(ApiConfig[] items)
        {
            if (items.Length > 0) return;
            var apiItem = new ApiRequestItem(this, new TranslateService_Offline(), transmits);
            await channel.Writer.WriteAsync(apiItem, transmits.Token);
        }

        public async Task<ApiRequestItem> GetApiItem() =>
            await channel.Reader.ReadAsync(transmits.Token);

        private static TranslateServiceBase CreateInstance(Type type, ApiConfig apiConfig) =>
            Activator.CreateInstance(type, apiConfig) as TranslateServiceBase;

        private static IDictionary<string, Type> GetMap() => new Type[]
        {
            typeof(TranslateService_Tencent),
            typeof(TranslateService_Aliyun),
            typeof(TranslateService_Baidu),
        }
            .Select(GetpiConfigSign)
            .ToDictionary(key => key.apiName, value => value.Type);

        private static (string apiName, Type Type) GetpiConfigSign(Type type)
        {
            var res = ApiConfigSignAttribute.GetApiConfigSign(type);
            return (res.ApiName, type);
        }

        public async Task Add(ApiRequestItem item) => await channel.Writer.WriteAsync(item);
    }
}
