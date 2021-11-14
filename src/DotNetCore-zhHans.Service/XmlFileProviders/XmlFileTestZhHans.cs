using System;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCoreZhHans.Service.XmlFileProviders
{
    internal class XmlFileTestZhHans : XmlFileTest
    {
        public XmlFileTestZhHans(ITransmitData transmits, string filePath = null)
            : base(transmits, filePath) { }

        public bool IsDotNetCoreZhHans { get; private set; }

        public XmlFileTest EnTest { get; init; }

        protected override void Test(string path)
        {
            IsDotNetCoreZhHans = IsDotNetCoreZhHans
                || path.Contains("<dotNetCoreZhHans version=", StringComparison.Ordinal);
        }
    }
}
