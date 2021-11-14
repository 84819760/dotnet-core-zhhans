using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetCorezhHans.Base;

namespace DotNetCorezhHans
{
    public abstract class TranslateServiceBase
    {
        public TranslateServiceBase(ApiConfig apiConfig) => ApiConfig = apiConfig;

        public ApiConfig ApiConfig { get; }

        public string Name => ApiConfig.Name;

        /// <summary>
        /// 通知
        /// </summary>
        public Action<int?, int?> Notice { get; }

        protected virtual Symbol Symbol { get; } = new();

        public string GetTranslate(string request) => GetTranslateAsync(request).Result;

        public async Task<string> GetTranslateAsync(string request)
        {
            Notice?.Invoke(request?.Length, null);
            request = Symbol.BeforeHandler(request);
            return Symbol.AfterHandler(await PostAsync(request));
        }

        protected abstract Task<string> PostAsync(string request);

        protected string CherckAndResult(string original, string translated)
        {
            Notice?.Invoke(null, translated?.Length);
            return CheckAndResult(GetTranslateValue(original, translated));
        }

        protected static string CheckAndResult(ITranslateValue value) =>
            CheckContent.CherckAndResult(value);

        private static ITranslateValue GetTranslateValue(string original, string translated) =>
            new TranslateValue() { Original = original, Translation = translated };

        protected static T Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json);


        public abstract string GetErrorCode(string code);

        public static (string code, string msg) GetErrorCode(string value
            , string[] splitdatas)
        {
            var data = value.Split(splitdatas, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
            return (data.First().Replace(",", ""), data.Last());
        }
    }
}