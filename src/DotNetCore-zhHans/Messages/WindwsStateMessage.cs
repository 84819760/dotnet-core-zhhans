using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NearExtend.WpfPrism.Messages;
using Prism.Events;

namespace DotNetCorezhHans.Messages
{
    public class WindwsStateMessage : MessageBase<WindwsStateMessage, WindwsState> { }

    public enum WindwsState
    {
        /// <summary>
        /// 鼠标移动移动窗体
        /// </summary>
        WindowDragMove,

        /// <summary>
        /// 最小化窗体
        /// </summary>
        WindowMinimize,

        /// <summary>
        /// 关闭窗体
        /// </summary>
        WindowClose,
    }   
}
