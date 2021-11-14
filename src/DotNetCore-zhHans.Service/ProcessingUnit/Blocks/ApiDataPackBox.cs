using System.Collections.Generic;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ApiRequests;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    internal class ApiDataPackBox
    {
        private readonly List<NodeCacheData> list = new();
        private readonly ITransmitData transmits;
        private readonly int maxCount;
        private int count;

        public ApiDataPackBox(ApiRequestItem apiRequestItem, ITransmitData transmits)
        {          
            maxCount = apiRequestItem.ApiConfig.MaxChar;
            Api = apiRequestItem;
            this.transmits = transmits;
        }

        public bool IsComplete { get; private set; }

        public bool TryAdd(NodeCacheData data)
        {
            count += data.Length;
            IsComplete = count > maxCount;
            if (IsComplete) return false;
            list.Add(data);
            return true;
        }

        public ApiRequestItem Api{ get; }

        public NodeCacheDataGroup NodeCacheDataGroup => new(list.ToArray(), transmits);

    }
}
