using System.Threading.Tasks;
using DotNetCorezhHans.Base;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    internal class VersionUpdateDataProvider
    {
        private readonly string url;
        private InfoData data;

        public VersionUpdateDataProvider()
        {
            if (Extends.IsDesignMode) return;
            url = App.Config.UpdateUrl;
        }

        public bool NeedUpdate { get;private set; }

        public async Task<InfoData> GetInfoData()
        {
            var res = data ??= await InfoData.GetInfoData(url);
            NeedUpdate = Version != data.Version;
            return res;
        }

        private string Version => GetType().Assembly.GetName().Version.ToString();
    }
}
