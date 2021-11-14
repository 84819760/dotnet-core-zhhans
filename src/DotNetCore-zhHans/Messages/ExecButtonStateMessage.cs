using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NearExtend.WpfPrism.Messages;
using NearCoreExtensions;
using MaterialDesignThemes.Wpf;
using System.Threading;

namespace DotNetCorezhHans.Messages
{
    /// <summary>
    /// 执行按钮状态图标
    /// </summary>
    public class ExecButtonStateMessage : MessageBase<ExecButtonStateMessage, ExecButtonStateData>
    {
        public const string ResponseInstance = "ResponseInstance";

        public Task PublishAsync(ExecButtonState exec)
        {
            var res = new ExecButtonStateData(exec);
            App.Invoke(() => Publish(res));
            return res.Complete;
        }

    }

    public class ExecButtonStateData : SemaphoreMessageBase<ExecButtonState>
    {
        public ExecButtonStateData(ExecButtonState value) : base(value)
        {
        }
    }

    public enum ExecButtonState
    {
        /// <summary>
        /// 默认状态
        /// </summary>
        [EnumMapping(PackIconKind.Play)]
        Default,

        /// <summary>
        /// 开始执行
        /// </summary>
        [EnumMapping(PackIconKind.Stop)]
        Run,

        /// <summary>
        /// 锁定等待
        /// </summary>
        [EnumMapping(PackIconKind.LockReset)]
        Lock,
    }
}
