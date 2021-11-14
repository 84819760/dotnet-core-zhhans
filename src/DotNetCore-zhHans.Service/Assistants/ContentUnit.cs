using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DotNetCoreZhHans.Service.XmlNodes;

namespace DotNetCoreZhHans.Service
{
    internal class ContentUnit
    {
        public string Value { get; init; }

        public virtual XmlNode GetXmlNode(NodeBase node) => node.XmlDoc.CreateTextNode(Value);

        public override string ToString() => $"Text = \"{Value}\"";
    }

    internal class ContentUnitIndex : ContentUnit
    {
        public string Source { get; set; }

        public int Id { get; init; }

        public override XmlNode GetXmlNode(NodeBase node) => node.Root.FindMap(Id).GetXmlNode();

        public override string ToString() => $"Id = \"{Id}\"";
    }
}
