using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using DotNetCorezhHans.Base.Interfaces;


namespace DotNetCoreZhHans.Service.XmlFileProviders
{
    internal class XmlFileTest
    {
        //|[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]
        private static readonly Regex regex = new(@"[\u4e00-\u9fa5]");
        private readonly ITransmitData transmits;
        protected CancellationToken token;
        private readonly string filePath;

        public XmlFileTest(ITransmitData transmits, string filePath = null)
        {
            this.transmits = transmits;
            this.filePath = filePath ?? transmits.File.Path;
            token = transmits.Token;
            ForEachTestChinese();
        }

        public bool IsCancel => token.IsCancellationRequested;

        public bool IsCover => transmits.Config.IsCover;

        public string FilePath => filePath;

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool IsChinese { get; private set; }

        /// <summary>
        /// 跳过文件原因
        /// </summary>
        public string SkipReason { get; set; }

        protected virtual IEnumerable<string> Rows => File.ReadLines(FilePath);

        private void ForEachTestChinese()
        {
            foreach (var item in Rows)
            {
                if (IsCancel || IsChinese) break;
                TestChinese(item);
            }
        }

        protected virtual void TestChinese(string context)
        {
            IsChinese = IsChinese || regex.IsMatch(context);
            if (IsChinese) SkipReason = context;
        }

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
            return path is null
                ? default
                : new XmlFileTestZhHans(transmits, path)
                {
                    IsChinese = true,
                    EnTest = this,
                };
        }
    }
}
