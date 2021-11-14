using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DotNetCorezhHans;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    internal class TextNode : NodeBase
    {
        private static readonly StringSplitOptions removeEmpty = StringSplitOptions.RemoveEmptyEntries;
        private readonly IndexProvider indexProvider;
        private string originalValue;
        private static readonly (string source, string target)[] symbols = new[]
        {
            ("(","( "),
            (")"," )"),
        };

        public TextNode(IndexProvider indexProvider
            , ITransmitData transmits
            , XmlNode xmlNode
            , int index
            , NodeBase parent)
            : base(indexProvider, transmits, xmlNode, index, parent)
        {
            Id = indexProvider.GetId();
            AddToMap(this);
            this.indexProvider = indexProvider;
        }

        public override int Id { get; }

        public override string OriginalValue
        {
            get
            {
                _ = QueryValue;
                return originalValue;
            }
        }

        protected override string GetQueryValue()
        {
            var items = CreateWordNodes().ToArray();
            originalValue = items.Select(x => x.OriginalValue).Join();
            return items.Select(x => x.QueryValue).Join();
        }

        protected IEnumerable<WordNode> CreateWordNodes()
        {
            var str = XmlNode.Value.Split(new[] { '\r', '\n' }, removeEmpty).Join();
            return Replace(str).Split(new[] { ' ' }, removeEmpty).Select(CreateWordNode);
        }

        private string Replace(string value) => symbols.Replace(value);

        private WordNode CreateWordNode(string value, int index) =>
            new(value, indexProvider, Transmits, index, this);

        public override XmlNode CreateElement(string name = null) => XmlDoc.CreateElement("Text");
    }
}
