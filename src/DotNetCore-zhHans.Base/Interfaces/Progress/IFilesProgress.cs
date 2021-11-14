using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base.Interfaces
{
    /// <summary>
    /// 文件扫描列表
    /// </summary>
    public interface IFilesProgress
    {
        /// <summary>
        /// 总数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 完成数量
        /// </summary>
        int CheckCount { get; }

        /// <summary>
        /// 异常数量
        /// </summary>
        int AlertCount { get; }

        /// <summary>
        /// 错误数量
        /// </summary>
        int ErrorCount { get; }

        /// <summary>
        /// 扫描内容
        /// </summary>
        string ScanContent { get; set; }

        IEnumerable<IFileProgress> FileItems { get; }


        /// <summary>
        /// 是否显示扫描，否则显式文件列表
        /// </summary>
        void ShowScan(bool value);

        void AddFile(string path);

        /// <summary>
        /// 结束时处理列表
        /// </summary>
        void ItemHandler(IFileProgress fileProgress);

        /// <summary>
        /// 初始化文件列表
        /// </summary>
        void InitFileItems();

        void Clear();
    }
}
