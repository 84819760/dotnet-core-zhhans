using System;
using System.Threading;

namespace DotNetCorezhHans.Base.Interfaces
{
    public interface ITransmitDataBase
    {
        bool IsCancel => CancellationTokenSource.IsCancellationRequested;

        public CancellationTokenSource CancellationTokenSource { get; }

        CancellationToken Token => CancellationTokenSource.Token;

        ConfigManager Config { get; }

        IProgress Progress { get; }

        GcManager GcManager { get; }

        string Version { get; }

        string this[string key] { get; set; }

        ReaderWriterLockSlim DbLock { get; }

        Exception Interrupt { get; set; }
    }


    /// <summary>
    /// 负责发送错误和消息
    /// </summary>
    public interface ITransmitData : ITransmitDataBase
    {
        IFileProgress File { get; }     
    }
}
