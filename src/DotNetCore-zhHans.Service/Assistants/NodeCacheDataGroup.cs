using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ApiRequests;
using DotNetCoreZhHans.Service.Assistants.Placeholders;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service
{
    internal class NodeCacheDataGroup
    {
        private readonly ITransmitData transmits;

        public NodeCacheDataGroup(NodeCacheData[] datas, ITransmitData transmits)
        {
            Datas = datas;
            this.transmits = transmits;
            QueryValue = GetQueryValue(datas);
        }

        public NodeCacheData[] Datas { get; }

        private static string GetQueryValue(NodeCacheData[] datas) => datas
            .Select(x => x.Node.QueryValue)
            .Join("\r\n");

        public string QueryValue { get; }

        private NodeCacheData FirstData => Datas.FirstOrDefault();

        public string MemberPath => FirstData?.Node?.MemberPath;

        public string FilePath => FirstData?.Node?.XmlNode.BaseURI;

        internal async Task<NodeCacheData[]> SetResponse(string value
            , ApiRequestItem api
            , CancellationToken token)
        {
            var rows = CheckLength(value, api.Name);
            foreach (var (resValue, cacheData) in rows.Zip(Datas))
            {
                var qv = cacheData.QueryValue;
                var res = await PlaceholderCheck
                    .GetCorrect(resValue, qv, transmits, api, cacheData, token);
                cacheData.SetValue(res, api.Name, true);
            }
            await api.Reduction();
            return Datas;
        }

        private string[] CheckLength(string value, string apiName)
        {
            var res = GetRows(value);
            if (res.Length == Datas.Length) return res;
            var error = @$"翻译返回值出现行丢失！
原文:{QueryValue}
译文:{value}";
            var errorData = transmits.File
                .CreateAndAdd(FilePath, MemberPath, nameof(NodeCacheDataGroup), error);
            return GetRows(QueryValue);
        }

        private static string[] GetRows(string value) => value?
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    }
}
