using System;
using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    internal class CodeNode : NodeBase
    {
        public CodeNode(IndexProvider indexProvider
            , ITransmitData transmits
            , XmlNode xmlNode
            , int index
            , NodeBase parent)
            : base(indexProvider, transmits, xmlNode, index, parent)
        {
            Id = indexProvider.GetId();
            AddToMap(this);
        }

        public override int Id { get; }

        public override bool IsTranslate => false;

        public override XmlNode GetXmlNode() => XmlNode.CloneNode(true);
               
    }
}
