using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NearExtend.WpfPrism.Messages;

namespace DotNetCorezhHans.Messages
{
    /// <summary>
    /// 页面状态控制
    /// </summary>
    public class PageStateMessage : MessageBase<PageStateMessage, PageControlType>
    {
    }

    /// <summary>
    /// 显示状态
    /// </summary>
    public enum PageControlType
    {
        /// <summary>
        /// 默认状态
        /// </summary>
        Default,

        /// <summary>
        /// 设置状态
        /// </summary>
        Setup,

        /// <summary>
        /// 惊喜状态
        /// </summary>
        Surprised,

        /// <summary>
        /// 翻译状态
        /// </summary>
        Translate,

        /// <summary>
        /// 终止任务
        /// </summary>
        TerminationTask,

        /// <summary>
        /// 翻译异常结果列表状态
        /// </summary>
        AbnormalList,

        /// <summary>
        /// 结束任务
        /// </summary>
        EndTask,
    }
}
