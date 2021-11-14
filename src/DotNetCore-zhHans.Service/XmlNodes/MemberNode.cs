using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    internal class MemberNode : NodeBase
    {
        private string memberPath;
        public MemberNode(ITransmitData transmits
            , XmlNode xmlNode)
            : base(null, transmits, xmlNode, -99999, null)
        {
        }

        public override string MemberPath => memberPath ??= GetMemberPath();

        private string GetMemberPath() => XmlNode
            .Attributes["name"]?.Value ?? $"读取异常: {XmlNode?.OuterXml}";
    }
}
