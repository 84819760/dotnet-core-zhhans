using System.Collections.Generic;
using System.Diagnostics;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ApiRequests;
using System.Linq;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    internal class ApiDataPackBox
    {
        private readonly List<NodeCacheData> list = new();
        private readonly static object lockObj = new();
        private readonly ITransmitData transmits;
        private readonly int maxCount;

        public ApiDataPackBox(ApiRequestItem apiRequestItem, ITransmitData transmits)
        {
            maxCount = apiRequestItem.ApiConfig.MaxChar;
            Api = apiRequestItem;
            this.transmits = transmits;
        }

        public bool IsComplete { get; private set; }

        public bool TryAdd(NodeCacheData data)
        {
            lock (lockObj)
            {
                if (list.Count > 0)
                {
                    var count = GetLength() + data.Length;
                    IsComplete = count >= maxCount;
                    if (IsComplete) return false;
                }
                list.Add(data);
            }
            return true;
        }

        private int GetLength() => list.Sum(x => x.Length);

        public ApiRequestItem Api { get; }

        public NodeCacheDataGroup NodeCacheDataGroup => new(list.ToArray(), transmits);

    }
}
