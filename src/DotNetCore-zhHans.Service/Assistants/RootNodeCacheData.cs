using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Db;
using DotNetCoreZhHans.Service.XmlNodes;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service
{
    internal class RootNodeCacheData
    {
        private readonly List<NodeCacheData> nodeCacheDatas = new();

        public NodeCacheDataFactory NodeCache { get; private set; }

        public ReaderWriterLockSlim LockData { get; } = new();

        public bool IsCompletion => nodeCacheDatas.All(x => x.IsCompletion);

        public IEnumerable<NodeCacheData> GetRequestNodeCacheDatas(ZhDbContext zhDbContext
            , IEnumerable<NodeBase> items)
        {
            NodeCache ??= new NodeCacheDataFactory(zhDbContext) { Name = nameof(RootNodeCacheData) };
            return items
                .Select(CreateNodeCacheData)
                .Select(AddToList)
                .Where(x => x.IsRequest);
        }


        private NodeCacheData CreateNodeCacheData(NodeBase node) => NodeCache.CreateNodeCacheData(node).Result;

        private NodeCacheData AddToList(NodeCacheData data)
        {
            LockData.LockAndWrite(() => nodeCacheDatas.Add(data));
            return data;
        }

        public void Supplement() => nodeCacheDatas.ForEach(x => x.Supplement());
    }
}
