using System.Collections.ObjectModel;
using System.ComponentModel;
namespace DotNetCorezhHans.Base
{
    public class ConfigManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static ConfigManager instance;

        /// <summary>
        /// 开启日志保存
        /// </summary>
        public bool IsLogging { get; set; }

        /// <summary>
        /// 是否保留原文
        /// </summary>
        public bool IsKeepOriginal { get; set; } = true;

        /// <summary>
        /// 是否用到管理员身份运行
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否覆盖DotNetCorezhHans的生成结果
        /// </summary>
        public bool IsCover { get; set; }

        public bool EnableImport { get; set; }

        /// <summary>
        /// 数据库写入缓存长度
        /// </summary>
        public int CacheCount { get; set; } = 1000;

        /// <summary>
        /// API配置
        /// </summary>
        public ObservableCollection<ApiConfig> ApiConfigs { get; set; }

        /// <summary>
        /// 要扫描的路径集合
        /// </summary>
        public ObservableCollection<string> Directorys { get; set; }

        public string[] Ignores { get; set; }

        public string UpdateUrl { get; init; }

        public string PackagesUrl { get; init; }

        public static ConfigManager Instance =>
            instance ??= ConfigManagerBuilder.CreateInstance();

        public void Save() => ConfigManagerBuilder.Save(this);

    }
}