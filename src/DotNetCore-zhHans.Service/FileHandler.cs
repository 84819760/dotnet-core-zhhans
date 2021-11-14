using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.FileHandlers;

namespace DotNetCoreZhHans.Service
{
    /// <summary>
    /// 负责提供文件处理和对外通知
    /// </summary>
    public class FileHandler : ITransmitData, ITransmitsExtend
    {
        private readonly ITransmitDataBase transmitDataBase;
        private readonly Action<Action> updateValue;

        public FileHandler(Action<Action> updateValue, ITransmitDataBase transmitDataBase)
        {
            this.updateValue = updateValue;
            this.transmitDataBase = transmitDataBase;
        }

        public CancellationTokenSource CancellationTokenSource =>
                transmitDataBase.CancellationTokenSource;

        public CancellationToken Token => CancellationTokenSource.Token;

        public ReaderWriterLockSlim DbLock => transmitDataBase.DbLock;

        public IProgress Progress => transmitDataBase.Progress;

        public ConfigManager Config => transmitDataBase.Config;

        public GcManager GcManager => transmitDataBase.GcManager;

        public string Version => transmitDataBase.Version;

        public Exception Interrupt
        {
            get => transmitDataBase.Interrupt;
            set => transmitDataBase.Interrupt = value;
        }

        public IFileProgress File { get; init; }

        public Task Run() => FileActuatorBase.CreateFactory(this);

        public void Set(Action action) => updateValue(action);

        public string this[string key]
        {
            get => transmitDataBase[key];
            set => transmitDataBase[key] = value;
        }
    }
}