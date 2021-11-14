using DotNetCorezhHans;
using System.Text.Json.Serialization;

namespace TranslateApi.Baidu
{
    public class ResponseData : IErrorValue, ITranslateValue
    {
        [JsonPropertyName("trans_result")]
        public TransResult[] TransResult { get; set; }

        [JsonPropertyName("error_code")]
        public string ErrorCode { get; init; }

        [JsonPropertyName("error_msg")]
        public string ErrorMsg { get; init; }

        [JsonIgnore]
        public string Original => TransResult?.GetValue(x => x.Original);

        [JsonIgnore]
        public string Translation => TransResult?.GetValue(x => x.Translation);
    }

    public class TransResult : ITranslateValue
    {
        [JsonPropertyName("src")]
        public string Original { get; set; }

        [JsonPropertyName("dst")]
        public string Translation { get; set; }
    }
}
