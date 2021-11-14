using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TranslateApi.Kate;

namespace TranslateApi
{
    [ApiConfigSign("kate")]
    public class TranslateService_Kate : TranslateServiceBase
    {
        private const string url = "https://api.66mz8.com/api/translation.php?info=";

        public TranslateService_Kate(ApiConfig apiConfig) : base(apiConfig)
        {
        }
        
        protected override Task<string> PostAsync(string request)
        {
            //var u = $"{url}{request}";
            //var json = await new HttpClient().GetStringAsync(u);
            //var data = Deserialize<ResponseData>(json);
            //return CheckAndResult(data);
            throw new Exception("翻译不准确，待定");
        }

        public override string GetErrorCode(string value) => throw new NotImplementedException();
    }
}
