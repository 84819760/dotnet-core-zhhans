using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base.Interfaces
{
    /// <summary>
    /// 主进度
    /// </summary>
    public interface IMasterProgress
    {

        /// <summary>
        /// 进度条
        /// </summary>
        int ProgressValue { get; set; }

        /// <summary>
        /// 文件进度
        /// </summary>
        int FileProgressValue { get; set; }

        /// <summary>
        /// 进度文本
        /// </summary>
        string ProgressText { get; set; }

        /// <summary>
        /// 进度描述
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 缓存描述
        /// </summary>
        string Title2 { get; set; }

        /// <summary>
        /// 更新XML描述
        /// </summary>
        string Title3 { get; set; }

        /// <summary>
        /// 设置元素显示
        /// </summary>
        void SetDisplayState(bool value);

        /// <summary>
        /// 请求
        /// </summary>
        string RequestStatus { get; set; }

        /// <summary>
        /// 和响
        /// </summary>
        string ResponseStatus { get; set; }

    }
}
