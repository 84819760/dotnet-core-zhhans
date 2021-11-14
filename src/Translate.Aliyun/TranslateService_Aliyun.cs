using Aliyun.Acs.alimt.Model.V20181012;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using DotNetCorezhHans;
using System.Threading.Tasks;
using System;
using DotNetCorezhHans.Base;
using System.Collections.Generic;

namespace TranslateApi
{
    [ApiConfigSign("阿里")]
    public class TranslateService_Aliyun : TranslateServiceBase
    {
        private readonly Lazy<DefaultAcsClient> client;
        private static readonly Dictionary<string, string> codeMap = new()
        {
            { "10001", "请求超时" },
            { "10002", "系统错误" },
            { "10003", "原文解码失败，请检查原文是否UrlEncode" },
            { "10004", "参数缺失" },
            { "10005", "语项不支持" },
            { "10006", "语种识别失败" },
            { "10007", "翻译失败" },
            { "10008", "字符长度过长" },
            { "19999", "未知异常" },
            { "10009", "子账号没有权限" },
            { "10010", "账号没有开通服务" },
            { "10011", "子账号服务失败" },
            { "10012", "翻译服务调用失败" },
            { "10013", "账号服务没有开通或者欠费" },
        };

        public TranslateService_Aliyun(ApiConfig apiConfig) : base(apiConfig)
        {
            client = new(() => new DefaultAcsClient(CreateClientProfile()));
        }

        private IClientProfile CreateClientProfile() => DefaultProfile
            .GetProfile(ApiConfig.Region, ApiConfig.SecretId, ApiConfig.SecretKey);

        protected override Task<string> PostAsync(string request) =>
            Task.Run(() => GetData(request));

        private string GetData(string request)
        {
            try
            {
                var req = CreateTranslateRequest(request);
                var res = client.Value.GetAcsResponse(req);
                return CherckAndResult(request, res.Data.Translated);
            }
            catch (Exception)
            {
                //内容和文档不一致，无法提取。
                throw;
            }
        }

        private static TranslateRequest CreateTranslateRequest(string request) => new()
        {
            TargetLanguage = "zh",
            SourceLanguage = "en",
            SourceText = request,
            FormatType = "text",
            Scene = "title",
        };

        public override string GetErrorCode(string code) => codeMap[code];
    }
}
