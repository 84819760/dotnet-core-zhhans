using System;
using System.ComponentModel;

namespace DotNetCorezhHans
{
    public class ApiConfig : INotifyPropertyChanged
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 百度为appId, 阿里为AccessKey Id
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// 阿里为AccessKey Secret
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 间隔时间ms(例如百度免费版每秒只能请求一次)
        /// </summary>
        public int IntervalTime { get; set; }

        /// <summary>
        /// 线程数量(百度免费版只能为1)
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// 是否启用API
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 最大字符数
        /// </summary>
        public int MaxChar { get; set; } 


        /// <summary>
        /// 取消处理包含占位符的内容(已知某些情况下会丢失内容)
        /// </summary>
        public virtual bool CancelPlaceholder { get; set; }

        /// <summary>
        /// 大区设定
        /// </summary>
        public virtual string Region { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;       
    }
}
