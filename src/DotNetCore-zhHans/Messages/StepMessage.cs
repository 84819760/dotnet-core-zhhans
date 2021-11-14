using NearExtend.WpfPrism.Messages;

namespace DotNetCorezhHans.Messages
{
    /// <summary>
    /// 执行步骤
    /// </summary>
    public class StepMessage : MessageBase<StepMessage, StepState> { }

    /// <summary>
    /// 执行步骤
    /// </summary>
    public enum StepState
    {
        /// <summary>
        /// 默认状态
        /// </summary>
        Default = 0,

        /// <summary>
        /// 开始执行
        /// </summary>
        Run = Default + 1,

        /// <summary>
        /// 请求停止
        /// </summary>
        StopRequest = Run + 1,

        /// <summary>
        /// 等待停止
        /// </summary>
        StopWait = StopRequest + 1,

        /// <summary>
        /// 确认停止成功
        /// </summary>
        StopConfirm = StopWait + 1,

        /// <summary>
        /// 执行完成
        /// </summary>
        Complete = StopConfirm + 1,
    }
}
