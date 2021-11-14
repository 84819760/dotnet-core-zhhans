using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base
{
    public class InfoData
    {
        public string Version { get; set; }

        public string UpdateUrl { get; set; }

        public string Information { get; set; }

        public string Md5 { get; set; }

        public static async Task<InfoData> GetInfoData(string url)
        {
            try
            {
                using var wc = new WebClient();
                var json = await wc.DownloadStringTaskAsync(url);
                return Extensions.Deserialize<InfoData>(json);
            }
            catch (Exception) { }
            return new() { Information = "读取更新信息错误!" };
        }
    }
}
