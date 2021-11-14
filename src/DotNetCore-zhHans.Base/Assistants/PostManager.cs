using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base
{
    public class PostManager : IDisposable
    {
        private readonly HttpClient httpClient = new();
        private readonly HttpContent httpContent;
        private readonly string url;

        public PostManager(HttpContent httpContent, string url)
        {
            this.httpContent = httpContent;
            this.url = url;
        }

        [ModuleInitializer]
        internal static void Init() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        public async Task<string> SendAsync() => await Post();

        async private Task<string> Post()
        {
            var resMsg = await httpClient.PostAsync(url, httpContent);
            return await resMsg.Content.ReadAsStringAsync();
        }

        public void Dispose() => httpClient.Dispose();
    }
}
