using DotNetCorezhHans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranslateApi.Baidu
{
    class RequestData
    {
        private readonly string salt = GetTimeStamp();
        private readonly string encryptString;
        private readonly ApiConfig config;
        private readonly string value;

        public RequestData(ApiConfig apiConfig, string value)
        {
            config = apiConfig;
            this.value = value;
            encryptString = GetEncrypt();
        }

        private string GetEncrypt() => EncryptString($"{config.SecretId}{value}{salt}{config.SecretKey}");

        public IEnumerable<KeyValuePair<string, string>> GetKeyValues()
        {
            return new KeyValuePair<string, string>[]
            {
                new ("sign", encryptString),
                new ("q", value),
                new ("appid", config.SecretId),
                new ("salt", salt),
                new ("from", "en"),
                new ("to", "zh"),
            };
        }

        public static string GetTimeStamp()
        {
            var ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        private static string EncryptString(string str) => Encoding.UTF8.GetBytes(str)
            .Md5ComputeHash()
            .Select(x => x.ToString("x2"))
            .JoinBuilder();
    }
}
