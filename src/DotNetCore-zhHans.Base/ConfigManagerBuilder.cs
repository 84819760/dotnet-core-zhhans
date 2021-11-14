using System.IO;

namespace DotNetCorezhHans.Base
{
    internal class ConfigManagerBuilder
    {
        private const string target = "DotNetCore-zhHans.Config.json";
        private static readonly string[] ignores = new[]
        {
            ".net" ,
            "framework" ,
            "lambda" ,

            "[" , "]" ,
            "{" , "}" ,
            "<" , ">" ,
            "(" , ")" ,

            "`" ,
            "~" ,

            "+" ,
            "-" ,
            "_" ,
            "." ,
            "*" ,
            "/" ,
            "%" ,
        };

        internal static ConfigManager CreateInstance()
        {
            if (!File.Exists(target)) CreateJson();
            var json = File.ReadAllText(target);
            return Extensions.Deserialize<ConfigManager>(json);
        }

        private static void CreateJson()
        {
            var instance = GetInstance();
            foreach (var item in instance.ApiConfigs)
            {
                item.SecretId = "";
                item.SecretKey = "";
            }
            Save(instance);
        }

        public static void Save(object value)
        {
            var json = Extensions.Serialize(value);
            File.WriteAllText(target, json);
        }

        private static ConfigManager GetInstance() => new()
        {
            IsCover = false,
            IsKeepOriginal = true,
            Ignores = ignores,
            UpdateUrl = "http://www.wyj55.cn/download/DotNetCorezhHans/Update.json",
            Directorys = new()
            {
                "C:\\Program Files\\dotnet\\packs\\",
                "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\ReferenceAssemblies\\Microsoft\\Framework",
                "%UserProFile%\\.nuget\\packages\\",
            },
            ApiConfigs = new()
            {
                new ApiConfig()
                {
                    Name = "百度",
                    IntervalTime = 1000,
                    MaxChar = 6000,
                    ThreadCount = 1,
                },
                new ApiConfig()
                {
                    Name = "腾讯",
                    IntervalTime = 1000,
                    MaxChar = 2000,
                    ThreadCount = 1,
                    Region = "ap-beijing",
                    CancelPlaceholder = true,
                },
                new ApiConfig()
                {
                    Name = "阿里",
                    IntervalTime = 1000,
                    MaxChar = 5000,
                    ThreadCount = 1,
                    Region = "cn-beijing",
                    CancelPlaceholder = true,
                },
            }
        };
    }
}