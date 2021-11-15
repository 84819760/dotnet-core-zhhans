using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using DotNetCorezhHans;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    /// <summary>
    /// 节点基础信息
    /// </summary>
    internal abstract class NodeBase
    {
        private readonly IndexProvider indexProvider;
        private string qriginalValue;
        private string queryValue;

        public NodeBase(IndexProvider indexProvider
            , ITransmitData transmits, XmlNode xmlNode
            , int index, NodeBase parent)
        {
            this.indexProvider = indexProvider;
            Transmits = transmits;
            XmlNode = xmlNode;
            Parent = parent;
            Index = index;
        }

        protected IndexProvider GetIndexProvider() => indexProvider;

        public virtual int Id { get; }

        public virtual NodeBase Parent { get; private set; }

        public virtual XmlNode XmlNode { get; }

        public virtual XmlDocument XmlDoc => XmlNode.OwnerDocument;

        public virtual bool IsTranslate => true;

        public ITransmitData Transmits { get; }

        public int Index { get; }

        public virtual string Name => XmlNode.Name;

        public virtual string Key => $"{Parent.Key}-{Index}";

        public virtual int Level => Parent.Level + 1;

        /// <summary>
        /// 隶属成员节点
        /// </summary>
        public virtual string MemberPath => Parent.MemberPath;

        /// <summary>
        /// 隶属解析根节点
        /// </summary>
        public virtual RootNode Root => Parent.Root;

        public virtual void AddToMap(NodeBase node) => Parent.AddToMap(node);

        /// <summary>
        /// 译文
        /// </summary>
        public string TranslValue { get; private set; }

        /// <summary>
        /// 原文
        /// </summary>
        public virtual string OriginalValue => qriginalValue ??= GetOriginalValue();

        /// <summary>
        /// 查询文本
        /// </summary>
        public virtual string QueryValue => queryValue ??= GetQueryValue();

        public virtual void SetTranslValue(string value) => TranslValue = value;

        protected virtual string GetQueryValue() => GetNodes()
            .Select(TestQueryValue).Join();

        private string TestQueryValue(NodeBase node)
        {
            if (node is TextNode text) return text.QueryValue;
            _ = node.QueryValue;
            return Symbol.Format(node.Id);
        }

        protected string GetOriginalValue() => Root.GetSubNodes(this)
            .Select(x => x.OriginalValue)
            .Where(x => x is { Length: > 0 })
            .Join();

        public virtual IEnumerable<NodeBase> GetNodes() =>
            XmlNode.ChildNodes.Cast<XmlNode>().Select(CreateNode);

        private NodeBase CreateNode(XmlNode item, int index)
        {
            return item.Name switch
            {
                "#text" => new TextNode(indexProvider, Transmits, item, index, this),
                "c" or "code" => new CodeNode(indexProvider, Transmits, item, index, this),
                _ => new DefaultNode(indexProvider, Transmits, item, index, this),
            };
        }

        public virtual void Clear()
        {
            Parent.Clear();
            Parent = null;
        }

        public override string ToString() => $"{GetType().Name} : {Key} ,{XmlNode.OuterXml}";


        public virtual XmlNode CreateElement(string name = null) => 
            XmlDoc.CreateElement(name ?? Name);

        public virtual XmlNode GetXmlNode()
        {
            var res = CreateElement();
            foreach (var item in GetContentUnits())
            {
                XmlHelper.Supplement(res);
                res.AppendChild(item);
            }
            return res;
        }

        private IEnumerable<XmlNode> GetContentUnits() => SymbolManager
            .GetContentUnits(GetContentValue()).Select(x => x.GetXmlNode(this));

        private string GetContentValue() => TranslValue is { Length: > 0 }
            ? TranslValue
            : QueryValue;
    }
}