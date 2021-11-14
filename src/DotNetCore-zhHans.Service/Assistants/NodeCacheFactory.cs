using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db;
using DotNetCorezhHans.Db.Models;
using DotNetCoreZhHans.Service.XmlNodes;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreZhHans.Service
{
    /// <summary>
    /// 根据缓存状态创建NodeCacheData
    /// </summary>
    internal class NodeCacheDataFactory
    {
        private readonly CacheDictionary<CacheData> cache = new();
        private readonly ZhDbContext dbContext;

        public NodeCacheDataFactory(ZhDbContext dbContext) => this.dbContext = dbContext;

        public ReaderWriterLockSlim CacheLockData => cache.LockData;

        public CacheData[] Values => cache.Values;

        public string Name { get; set; }

        public async Task<NodeCacheData> CreateNodeCacheData(NodeBase node)
        {
            var key = node.QueryValue;
            var isRequest = false;
            if (!cache.TryGetValue(key, out var data))
            {
                cache[key] = data = await GetCacheData(key, node);
                isRequest = data.Value is null;
            }
            return new()
            {
                DbContext = dbContext,
                CacheData = data,
                Node = node,
                IsRequest = isRequest,
            };
        }

        private static CacheData FindSuperior(string key, ITransmitData transmitData)
        {
            var data = transmitData[key];
            return data is null ? default : new CacheData("缓存", data);
        }

        private async Task<CacheData> FindDb(string key)
        {
            var data = await dbContext.FindDataLock(key);
            return data is null ? default : new CacheData(data.TranslSource.Name, data.Translation);
        }


        private async Task<CacheData> GetCacheData(string key, NodeBase node)
        {
            var res = FindSuperior(key, node.Transmits) ?? await FindDb(key);
            if (res is null) return new CacheData(Name);
            node.SetTranslValue(res.Value);
            return res;
        }

        public void Clear() => cache.Clear();
    }
}
