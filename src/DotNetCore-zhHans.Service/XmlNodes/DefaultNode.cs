using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    internal class DefaultNode : NodeBase
    {
        public DefaultNode(IndexProvider indexProvider
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

        public override XmlNode GetXmlNode() => QueryValue is { Length: 0 } 
            ? XmlNode.CloneNode(true) 
            : base.GetXmlNode();
    }
}
