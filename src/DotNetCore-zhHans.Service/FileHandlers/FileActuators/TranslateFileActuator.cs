using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ProcessingUnit;
using DotNetCoreZhHans.Service.XmlNodes;

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
            var xmlDoc = await CreateXmlDocument();

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

        protected async Task<XmlDocument> CreateXmlDocument()
        {
            var xmlDoc = new XmlDocument();
            await NetstandardTest(xmlDoc);
            memberNodeCount = GetMemberNode(xmlDoc).Count();
            return await Task.FromResult(xmlDoc);
        }

        private async Task NetstandardTest(XmlDocument doc)
        {
            string path;
            try
            {
                path = await GetXmlFilePath();
                doc.Load(path);
            }
            catch (Exception)
            {
                //if (IsNetstandard()) ReplaceFile(ex);
                throw;
            }
            DeleteFile(path);
        }

        private static void DeleteFile(string path)
        {
            if (!System.IO.File.Exists(path)) return;
            System.IO.File.Delete(path);
        }

        private async Task<string> GetXmlFilePath()
        {
            var resPath = Environment.GetEnvironmentVariable("TEMP");
            resPath = Path.Combine(resPath, "DotNetCore-zhHans");
            Directory.CreateDirectory(resPath);
            resPath = Path.Combine(resPath, $"{Guid.NewGuid()}.xml");
            await Task.Run(() => new XmlCollate(Transmits.File.Path, resPath));
            return resPath;
        }

        //private bool IsNetstandard()
        //{
        //    var path = Transmits.File.Path.ToLower();
        //    var target = Path.GetFileName(Transmits.File.Path);
        //    return target == "netstandard.xml" 
        //        && new[] { "netstandard.library", @"\ref\" }.All(x => path.Contains(x));
        //}

        //private void ReplaceFile(Exception ex)
        //{
        //    var dir = Environment.CurrentDirectory;
        //    var path = GetNetstandardXmlPath(dir) ?? GetNetstandardXmlPath(Path.Combine(dir, "lib"));
        //    ReplaceFile(path);
        //    throw new Exception($"读取文件失败:{ex.Message}\r\n使用官方文件netstandard2.1替换");
        //}

        //private void ReplaceFile(string netstandardXmlPath)
        //{
        //    var dir = Path.GetDirectoryName(Transmits.File.Path);
        //    dir = Path.Combine(dir, "zh-hans");
        //    Directory.CreateDirectory(dir);

        //    var fileName = Path.GetFileName(Transmits.File.Path);
        //    var targetFile = Path.Combine(dir, fileName);

        //    System.IO.File.Copy(netstandardXmlPath, targetFile);
        //}

        //private static string GetNetstandardXmlPath(string dir)
        //{
        //    var path = Path.Combine(dir, "netstandard.xml");
        //    if (System.IO.File.Exists(path)) return path;
        //    return default;
        //}

        private IEnumerable<RootNode> GetXmlNodes(XmlDocument doc) => GetMemberNode(doc)?
            .Select(CreateMemberNode)
            .SelectMany(CreateRootNodes);

        private static IEnumerable<XmlNode> GetMemberNode(XmlDocument doc) => new[]
        {
            "doc/members",
            "span/doc/members", // 这个格式很奇怪,为毛有这种情况？
        }
        .Select(doc.SelectSingleNode).OfType<XmlNode>()
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