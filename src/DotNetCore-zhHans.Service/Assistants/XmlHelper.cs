using System.Linq;
using System.Net.Http;
using System.Xml;
using DotNetCoreZhHans.Service.XmlNodes;

namespace DotNetCoreZhHans.Service
{
    internal class XmlHelper
    {
        internal static void Supplement(XmlNode xmlNode)
        {
            var last = xmlNode.LastChild;
            if (last is null) return;
            xmlNode.AppendChild(xmlNode.OwnerDocument.CreateTextNode(" "));
        }

        internal static void ReplaceXml(RootNode root)
        {
            root.IsUpdateXml = true;
            var para = root.CreateElement("para");
            MoveXml(root.XmlNode, para);
            var newXml = root.GetXmlNode();
            MoveXml(newXml, root.XmlNode);
            if (!root.Transmits.Config.IsKeepOriginal) return;
            root.XmlNode.AppendChild(GetOriginal(root));
            root.XmlNode.AppendChild(para);
        }

        private static XmlNode GetOriginal(RootNode root)
        {
            var doc = root.XmlDoc;
            var text = doc.CreateTextNode("原文:");
            var para= root.CreateElement("para");
            para.AppendChild(text);
            return para;
        }

        private static void MoveXml(XmlNode source, XmlNode target)
        {
            var itmes = source.ChildNodes.Cast<XmlNode>().ToArray();
            foreach (var item in itmes)
            {
                target.AppendChild(item.CloneNode(true));
                source.RemoveChild(item);
            }
        }
    }
}
