using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ProcessingUnit;
using DotNetCoreZhHans.Service.XmlNodes;
using TencentCloud.Mrs.V20200910.Models;

namespace DotNetCoreZhHans.Service.FileHandlers.FileActuators
{
    /// <summary>
    /// 负责翻译文件
    /// </summary>
    internal class TranslateFileActuator : FileActuatorBase
    {
        private int memberNodeCount;
        private int memberProgress;

        public TranslateFileActuator(ITransmitData transmits) : base(transmits) { }

        public override async Task Run()
        {
            await using var unit = new RootUnit(Transmits);
            var xmlDoc = CreateXmlDocument();

            foreach (var item in GetXmlNodes(xmlDoc))
            {
                if (Transmits.Token.IsCancellationRequested) break;
                if (item.QueryValue is { Length: > 0 })
                    await unit.SendAsync(item);
            }

            await unit.Complete();
            Save(xmlDoc);
            SetDefault();
        }

        private void SetDefault()
        {
            var master = Transmits.Progress.Master;
            master.Title2 = "缓存 : 0";
            master.Title3 = "等待 : 0";
        }

        protected XmlDocument CreateXmlDocument()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Transmits.File.Path);
            memberNodeCount = GetMemberNode(xmlDoc).Count();
            return xmlDoc;
        }

        private IEnumerable<RootNode> GetXmlNodes(XmlDocument doc) => GetMemberNode(doc)?
            .Select(CreateMemberNode)
            .SelectMany(CreateRootNodes);

        private static IEnumerable<XmlNode> GetMemberNode(XmlDocument doc) => new[]
        {
            "doc/members",
            // 待定，这个格式很奇怪,为毛有这种情况？
            //"span/doc/members"
        }.Select(doc.SelectSingleNode).OfType<XmlNode>()
            .SelectMany(x => x.ChildNodes.OfType<XmlNode>());

        private MemberNode CreateMemberNode(XmlNode xmlNode)
        {
            SendProgress();
            return new MemberNode(Transmits, xmlNode);
        }

        private IEnumerable<RootNode> CreateRootNodes(MemberNode node) => node.XmlNode
            .ChildNodes.Cast<XmlNode>()
            .Select((x, i) => CreateRootNode(x, i, node));

        private RootNode CreateRootNode(XmlNode xmlNode, int index, MemberNode parent) =>
            new(Transmits, xmlNode, index, parent);

        public void SendProgress()
        {
            memberProgress++;
            var value = (int)((decimal)memberProgress / memberNodeCount * 100);
            Transmits.Set(() =>
            {
                Transmits.File.Progress = $"{value}%";
                Transmits.Progress.Master.FileProgressValue = value;
            });

        }

        private void Save(XmlDocument xmlDoc)
        {
            if (Token.IsCancellationRequested) return;
            AddDotNetCoreZhHans(xmlDoc);
            var path = Path.GetDirectoryName(Transmits.File.Path);
            var file = Path.GetFileName(Transmits.File.Path);
            var zhPath = Path.Combine(path, "zh-hans");
            Directory.CreateDirectory(zhPath);
            zhPath = Path.Combine(zhPath, file);
            xmlDoc.Save(zhPath);
        }

        private void AddDotNetCoreZhHans(XmlDocument doc)
        {
            var newNode = doc.CreateElement("dotNetCoreZhHans");
            newNode.Attributes.Append(CreateXmlAttribute(doc));
            newNode.AppendChild(doc.CreateTextNode(DateTime.Now.ToString()));
            (doc.SelectSingleNode("doc") ?? doc.SelectSingleNode("span/doc")).AppendChild(newNode);
        }

        private XmlAttribute CreateXmlAttribute(XmlDocument doc)
        {
            var attr = doc.CreateAttribute("version");
            attr.Value = Transmits.Version;
            return attr;
        }
    }
}