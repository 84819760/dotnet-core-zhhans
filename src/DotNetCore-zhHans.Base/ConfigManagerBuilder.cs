using System;
using System.IO;

namespace DotNetCorezhHans.Base
{
    public class ConfigManagerBuilder
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
            var path = RootConfigFilePath;
            if (!File.Exists(path)) CreateJson();
            var json = File.ReadAllText(path);
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

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string RootConfigFilePath
        {
            get
            {
                var dir = Path.GetDirectoryName(typeof(ConfigManagerBuilder).Assembly.Location);
                var pdir = Directory.GetParent(dir).FullName;
                var test = Path.Combine(pdir, "DotNetCoreZhHans.exe");
                if (!File.Exists(test))
                {
                    throw new Exception("创建配置文件失败!找不到引导程序");
                }
                return Path.Combine(pdir, target);
            }
        }

        public static void Save(object value)
        {
            var path = RootConfigFilePath;
            var json = Extensions.Serialize(value);
            File.WriteAllText(path, json);
        }

        private static ConfigManager GetInstance() => new()
        {
            IsCover = false,
            IsKeepOriginal = false,
            IsAdmin = true,
            Ignores = ignores,
            UpdateUrl = "http://www.wyj55.cn/download/DotNetCorezhHans20/Update.json",
            PackagesUrl = "http://www.wyj55.cn/download/DotNetCorezhHans20/packs/_pack.json",
            Directorys = new()
            {
                "C:\\Program Files\\dotnet\\packs\\",
                "%UserProFile%\\.nuget\\packages\\",
                "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\ReferenceAssemblies\\Microsoft\\Framework\\",
                "C:\\Program Files (x86)\\Microsoft SDKs\\",
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