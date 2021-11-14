using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using DotNetCorezhHans.Base.Interfaces;


namespace DotNetCoreZhHans.Service.XmlFileProviders
{
    internal class XmlFileTest
    {
        private static readonly Regex regex = new(@"[\u4e00-\u9fa5]|[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
        private readonly ITransmitData transmits;
        protected CancellationToken token;
        private readonly string filePath;

        public XmlFileTest(ITransmitData transmits, string filePath = null)
        {
            this.transmits = transmits;
            this.filePath = filePath ?? transmits.File.Path;
            token = transmits.Token;
            ForEach();
        }

        public bool IsCancel => token.IsCancellationRequested;

        public bool IsCover => transmits.Config.IsCover;

        public string FilePath => filePath;

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool IsChinese { get; private set; }

        protected virtual IEnumerable<string> Rows => File.ReadLines(FilePath);

        private void ForEach()
        {
            foreach (var item in Rows)
            {
                if (IsCancel) break;
                Test(item);
            }
        }

        protected virtual void Test(string path) =>
            IsChinese = IsChinese || regex.IsMatch(path);

        private string GetZhHansFile()
        {
            var dir = Path.GetDirectoryName(FilePath);
            var fileName = Path.GetFileName(FilePath);
            var zhHansFile = Path.Combine(dir, "zh-hans", fileName);
            return File.Exists(zhHansFile) ? zhHansFile : null;
        }

        public XmlFileTestZhHans CreateZhHansXmlFile()
        {
            var path = GetZhHansFile();
            return path is null ? default
                : new XmlFileTestZhHans(transmits, path)
                {
                    IsChinese = true,
                    EnTest = this,
                };
        }
    }
}
