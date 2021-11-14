using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db;
using DotNetCoreZhHans.Service.XmlNodes;

namespace DotNetCoreZhHans.Service
{
    internal class NodeCacheData : ITranslaResults
    {
        public NodeCacheData() { }

        string ITranslaResults.Original => QueryValue;

        string ITranslaResults.Transl => Node.TranslValue ?? Node.QueryValue;

        string ITranslaResults.Source => CacheData.ResponseSource;

        public ZhDbContext DbContext { get; init; }

        public CacheData CacheData { get; init; }

        public NodeBase Node { get; init; }

        public bool IsRequest { get; init; }

        public bool IsRoot => Node is RootNode;

        public string QueryValue => Node.QueryValue;

        public RootNode RootNode => Node.Root;

        public bool IsResponseValue => CacheData.IsResponseValue;

        public bool IsCompletion => CacheData.IsCompletion;

        public bool IsCheckPassed => CacheData.IsCheckPassed;

        public int Length => QueryValue.Length + 2;

        public void SetValue(string value, string apiName, bool isResponseValue = false)
        {
            Node.SetTranslValue(value);
            CacheData.SetValue(value, apiName, isResponseValue);
            SetError(apiName, value);
        }

        private void SetError(string apiName, string value)
        {
            if (CacheData.IsCheckPassed) return;
            var missingContent = string.Join(", ", CacheData.MissingContent);
            var errorMsg = $@"{apiName}占位符丢失:{missingContent}
原文:  {QueryValue}
译文:  {value}";
            var file = RootNode.Transmits.File;
            file.CreateAndAdd(Node.MemberPath, "占位符检查", errorMsg);
        }

        public Task Wait(CancellationToken token) => CacheData.Wait(token);

        public override string ToString()
        {
            try
            {
                return $"IsCompletion = {CacheData.IsCompletion}, IsRequest = {IsRequest}, {Node.QueryValue}";
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void Supplement() => Node.SetTranslValue(CacheData.Value);

    }
}
