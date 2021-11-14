using DotNetCorezhHans;
using System.Text.Json.Serialization;

namespace TranslateApi.Kate
{
    public class ResponseData : ITranslateValue, IErrorValue
    {
        [JsonPropertyName("info")]
        public string Original { get; init; }

        [JsonPropertyName("fanyi")]
        public string Translation { get; init; }

        [JsonPropertyName("code")]
        public int Code { get; init; }

        public string ErrorCode => Code.ToString();

        [JsonPropertyName("msg")]
        public string ErrorMsg { get; init; }     
    }
}