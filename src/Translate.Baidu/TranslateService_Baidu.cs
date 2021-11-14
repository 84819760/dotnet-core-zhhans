using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TranslateApi.Baidu;

namespace TranslateApi
{
    [ApiConfigSign("百度")]
    public class TranslateService_Baidu : TranslateServiceBase
    {
        private const string url = "https://fanyi-api.baidu.com/api/trans/vip/translate";
        private static readonly Dictionary<string, string> codeMap = new()
        {
            { "52000", "成功。" },
            { "52001", "请求超时，请重试。" },
            { "52002", "系统错误，请重试。" },
            { "52003", "未授权用户，请检查appid是否正确或者服务是否开通。" },
            { "54000", "必填参数为空，请检查是否少传参数。" },
            { "54001", "签名错误，请检查您的签名生成方法。" },
            { "54003", "访问频率受限，请降低您的调用频率，或进行身份认证后切换为高级版/尊享版。" },
            { "54004", "账户余额不足，请前往管理控制台为账户充值。" },
            { "54005", "长query请求频繁，请降低长query的发送频率，3s后再试。" },
            { "58000", "客户端IP非法，检查个人资料里填写的IP地址是否正确，可前往开发者信息-基本信息修改。" },
            { "58001", "译文语言方向不支持，检查译文语言是否在语言列表里。" },
            { "58002", "服务当前已关闭，请前往管理控制台开启服务。" },
            { "90107", "认证未通过或未生效，请前往我的认证查看认证进度。" },
        };

        public TranslateService_Baidu(ApiConfig apiConfig) : base(apiConfig) { }

        protected override async Task<string> PostAsync(string request)
        {
            try
            {
                var json = TestJson(await GetJson(request));
                var res = Deserialize<ResponseData>(json);
                return CheckAndResult(res);
            }
            catch (DecompositionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ResponseException(ex.Message, this);
            }
        }

        private static string TestJson(string json)
        {
            if (json is { Length: > 0 }) return json;
            throw new DecompositionException();
        }

        private Task<string> GetJson(string request)
        {          
            var httpContent = GetHttpContent(request);
            return new PostManager(httpContent, url).SendAsync();
        }

        private HttpContent GetHttpContent(string request) =>
            new FormUrlEncodedContent(GetPostData(request));

        private IEnumerable<KeyValuePair<string, string>> GetPostData(string request) =>
           new RequestData(ApiConfig, request).GetKeyValues();

        public override string GetErrorCode(string code) => codeMap[code];
    }
}