using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db;
using DotNetCorezhHans.Base;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    internal class RootNode : NodeBase
    {
        private readonly Dictionary<int, NodeBase> map = new();
        private readonly HashSet<int> firstHash = new();

        public RootNode(ITransmitData transmits
            , XmlNode xmlNode
            , int index
            , NodeBase parent)
            : base(new IndexProvider(), transmits, xmlNode, index, parent)
        {
        }

        public override RootNode Root => this;

        public override int Id => Index * -1;

        public override int Level => 0;

        public override string Key => $"{Id}";

        public NodeBase FindMap(int id) => IsFirst(id) ? default : FindId(id);

        private bool IsFirst(int id)
        {
            var res = firstHash.Contains(id);
            if (!res) firstHash.Add(id);
            return res;
        }

        private NodeBase FindId(int id)
        {
            if (!map.TryGetValue(id, out var res))
                res = CreateErrorNode(id);
            return res;
        }

        private ErrorNode CreateErrorNode(int id)
        {
            var errorNode = XmlDoc.CreateElement("code");
            var error = XmlDoc.CreateTextNode($"(错误:[找不到ID{id}])");
            errorNode.AppendChild(error);
            return new ErrorNode(GetIndexProvider(), Transmits, errorNode, 99999, this);
        }


        public bool IsUpdateXml { get; set; }

        public IEnumerable<NodeBase> GetSubNodes(NodeBase node) => map.Values
            .Where(x => x.Parent == node).OrderBy(x => x.Index);

        public override void AddToMap(NodeBase node) => map.Add(node.Id, node);

        public override void Clear()
        {
            map.Clear();
            RootNodeCacheData = null;
        }

        public bool IsCompletion => TranslValue is { Length: > 0 } && TestCompletion();

        private bool TestCompletion()
        {
            var isContainNum = SymbolManager.regex.IsMatch(QueryValue);
            if (!isContainNum) return true;
            return RootNodeCacheData.IsCompletion;
        }

        public RootNodeCacheData RootNodeCacheData { get; private set; }

        public IEnumerable<NodeCacheData> GetRequestNodeCacheDatas(ZhDbContext zhDbContext)
        {
            RootNodeCacheData ??= new();
            return RootNodeCacheData.GetRequestNodeCacheDatas(zhDbContext, GetRequestNodes());
        }

        public void Supplement() => RootNodeCacheData?.Supplement();

        private IEnumerable<NodeBase> GetRequestNodes() => map.Values
            .OfType<TextNode>()
            .Select(x => x.Parent)
            .Where(x => x.IsTranslate)
            .Distinct()
            .Where(x => x is not RootNode)
            .Where(IsEnglish)
            .OrderByDescending(x => x.Level);

        private bool IsEnglish(NodeBase node) => SymbolManager.English.IsMatch(node.QueryValue);
    }
}
