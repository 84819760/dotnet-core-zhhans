using DotNetCorezhHans;
using System.Threading.Tasks;
using System;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tmt.V20180321;
using TencentCloud.Tmt.V20180321.Models;
using DotNetCorezhHans.Base;
using System.Collections.Generic;
using System.Linq;
using TranslateApi.Tencent;
using NearCoreExtensions;

namespace TranslateApi
{
    [ApiConfigSign("腾讯")]
    public class TranslateService_Tencent : TranslateServiceBase
    {
        private readonly static ClientProfile clientProfile = CreateClientProfile();
        private const string url = "tmt.tencentcloudapi.com";
        private readonly Lazy<TmtClient> tmtClient;
        private readonly Lazy<Credential> cred;

        private static readonly Dictionary<string, string> codeMap = new()
        {
            { "ActionOffline", "接口已下线。" },
            { "AuthFailure.InvalidAuthorization", "请求头部的 Authorization 不符合腾讯云标准。" },
            { "AuthFailure.InvalidSecretId", "密钥非法（不是云 API 密钥类型）。" },
            { "AuthFailure.MFAFailure", "MFA 错误。" },
            { "AuthFailure.SecretIdNotFound", "密钥不存在。请在控制台检查密钥是否已被删除或者禁用，如状态正常，请检查密钥是否填写正确，注意前后不得有空格。" },
            { "AuthFailure.SignatureExpire", "签名过期。Timestamp 和服务器时间相差不得超过五分钟，请检查本地时间是否和标准时间同步。" },
            { "AuthFailure.SignatureFailure", "签名错误。签名计算错误，请对照调用方式中的签名方法文档检查签名计算过程。" },
            { "AuthFailure.TokenFailure", "token 错误。" },
            { "AuthFailure.UnauthorizedOperation", "请求未授权。请参考 CAM 文档对鉴权的说明。" },
            { "DryRunOperation", "DryRun 操作，代表请求将会是成功的，只是多传了 DryRun 参数。" },
            { "FailedOperation", "操作失败。" },
            { "InternalError", "内部错误。" },
            { "InvalidAction", "接口不存在。" },
            { "InvalidParameter", "参数错误（包括参数格式、类型等错误）。" },
            { "InvalidParameterValue", "参数取值错误。" },
            { "InvalidRequest", "请求 body 的 multipart 格式错误。" },
            { "IpInBlacklist", "IP地址在黑名单中。" },
            { "IpNotInWhitelist", "IP地址不在白名单中。" },
            { "LimitExceeded", "超过配额限制。" },
            { "MissingParameter", "缺少参数。" },
            { "NoSuchProduct", "产品不存在" },
            { "NoSuchVersion", "接口版本不存在。" },
            { "RequestLimitExceeded", "请求的次数超过了频率限制。" },
            { "RequestLimitExceeded.GlobalRegionUinLimitExceeded", "主账号超过频率限制。" },
            { "RequestLimitExceeded.IPLimitExceeded", "IP限频。" },
            { "RequestLimitExceeded.UinLimitExceeded", "主账号限频。" },
            { "RequestSizeLimitExceeded", "请求包超过限制大小。" },
            { "ResourceInUse", "资源被占用。" },
            { "ResourceInsufficient", "资源不足。" },
            { "ResourceNotFound", "资源不存在。" },
            { "ResourceUnavailable", "资源不可用。" },
            { "ResponseSizeLimitExceeded", "返回包超过限制大小。" },
            { "ServiceUnavailable", "当前服务暂时不可用。" },
            { "UnauthorizedOperation", "未授权操作。" },
            { "UnknownParameter", "未知参数错误，用户多传未定义的参数会导致错误。" },
            { "UnsupportedOperation", "操作不支持。" },
            { "UnsupportedProtocol", "http(s) 请求协议错误，只支持 GET 和 POST 请求。" },
            { "UnsupportedRegion", "接口不支持所传地域。" },
            { "FailedOperation.NoFreeAmount", "本月免费额度已用完，如需继续使用您可以在机器翻译控制台升级为付费使用。" },
            { "FailedOperation.ServiceIsolate", "账号因为欠费停止服务，请在腾讯云账户充值。" },
            { "FailedOperation.UserNotRegistered", "服务未开通，请在腾讯云官网机器翻译控制台开通服务。" },
            { "InternalError.BackendTimeout", "后台服务超时，请稍后重试。" },
            { "InternalError.ErrorUnknown", "未知错误。" },
            { "InternalError.RequestFailed", "请求失败。" },
            { "InvalidParameter.DuplicatedSessionIdAndSeq", "重复的SessionUuid和Seq组合。" },
            { "InvalidParameter.MissingParameter", "参数错误。" },
            { "InvalidParameter.SeqIntervalTooLarge", "Seq之间的间隙请不要大于2000。" },
            { "LimitExceeded.LimitedAccessFrequency", "超出请求频率。" },
            { "UnauthorizedOperation.ActionNotFound", "请填写正确的Action字段名称。" },
            { "UnsupportedOperation.AudioDurationExceed", "音频分片长度超过限制，请保证分片长度小于8s。" },
            { "UnsupportedOperation.TextTooLong", "单次请求text超过长度限制，请保证单次请求⻓度低于2000。" },
            { "UnsupportedOperation.UnSupportedTargetLanguage", "不支持的目标语言，请参照语言列表。" },
            { "UnsupportedOperation.UnsupportedLanguage", "不支持的语言，请参照语言列表。" },
            { "UnsupportedOperation.UnsupportedSourceLanguage", "不支持的源语言，请参照语言列表。" },

        };

        public TranslateService_Tencent(ApiConfig apiConfig) : base(apiConfig)
        {
            cred = new(CreateCredential);
            tmtClient = new(CreateTmtClient);
        }

        private TmtClient CreateTmtClient() => new(cred.Value, ApiConfig.Region, clientProfile);

        private Credential CreateCredential() => new()
        {
            SecretId = ApiConfig.SecretId,
            SecretKey = ApiConfig.SecretKey,
        };

        private static ClientProfile CreateClientProfile() =>
            new() { HttpProfile = new HttpProfile { Endpoint = url } };

        private static TextTranslateRequest CreateTextTranslateRequest(string source) => new()
        {
            SourceText = source,
            Source = "en",
            Target = "zh",
            ProjectId = 0
        };

        protected override Task<string> PostAsync(string request) =>
            Task.Run(() => GetData(request));

        private string GetData(string request)
        {
            try
            {
                var req = CreateTextTranslateRequest(request);
                var res = tmtClient.Value.TextTranslateSync(req);
                return CherckAndResult(request, res.TargetText);
            }
            catch (AggregateException aex)
            {
                throw new ResponseException(aex.UnAggregateException().Message, this);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override string GetErrorCode(string code) => codeMap[code];
    }
}
