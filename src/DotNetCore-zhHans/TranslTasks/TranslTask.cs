using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Messages;
using DotNetCoreZhHans.Service;
using NearCoreExtensions;

namespace DotNetCorezhHans.TranslTasks
{
    internal class TranslTask : TaskBase, ITransmitDataBase
    {
        private readonly Dictionary<string, string> map = new();
        private readonly IndexProvider indexProvider = new();
        private readonly IFileProgress[] fileProgresses;
        private readonly IMasterProgress master;
        private readonly LogHandler logHandler;
        private readonly int length;

        public TranslTask(TranslManager translManager) : base(translManager)
        {
            fileProgresses = GetFiles();
            length = fileProgresses.Length;
            master = Progress.Master;
            logHandler = InitLogHandler();
        }

        public CancellationTokenSource CancellationTokenSource =>
            translManager.CancellationTokenSource;

        public Exception Interrupt { get; set; }

        public ConfigManager Config => translManager.Config;

        public GcManager GcManager => translManager.GcManager;

        public ReaderWriterLockSlim DbLock { get; } = new();

        public string Version => App.Version;

        private IFileProgress[] GetFiles() => Progress.Files.FileItems.ToArray();

        private LogHandler InitLogHandler() => translManager.Config.IsLogging ? new() : default;

        public string this[string key]
        {
            get
            {
                map.TryGetValue(key, out var res);
                return res;
            }
            set => map[key] = value;
        }

        public override async Task<PageControlType> Run()
        {
            SetMaster();
            Exception exception = null;
            for (var i = 0; i < length; i++)
            {
                var item = fileProgresses[i];
                item.AddErrorEvent += e => ExceptionHandler(e, item);
                if (IsCancel || (exception ??= await TryRun(item, i)) != null) break;
                ItemHandler(item);
                SetProgressValue(i + 1);
            }
            await SetLogHandlerComplete();
            return GetResult();
        }

        private void ExceptionHandler(Exception exception, IFileProgress file)
        {
            var index = indexProvider.GetId();
            if (exception is ExceptionBase eb) eb.Index = index;
            logHandler?.AddError(exception, file, index);
        }

        private PageControlType GetResult()
        {
            var files = Progress.Files;
            var count = files.AlertCount + files.ErrorCount;
            return count > 0
                ? PageControlType.AbnormalList
                : PageControlType.EndTask;
        }

        private void ItemHandler(IFileProgress item) =>
            Progress.Files.ItemHandler(item);

        private async Task<Exception> TryRun(IFileProgress item, int index)
        {
            var fileHandler = CreateFileHandler(item);
            var title = "翻译任务";
            try
            {
                await fileHandler.Run();
            }
            catch (UnauthorizedAccessException)
            {
                var error = $"没有文件读写权限，请开启\"管理员身份运行\"";
                item.CreateAndAdd(null, title, error, item.Path, null, true);
            }
            catch (Exception ex)
            {
                ex = ex.UnAggregateException();
                if (!ex.IsCanceled())
                {
                    item.CreateAndAdd(null, title, ex.Message, item.Path, ex, true);
                    ShowError(item, index);
                }
            }
            return default;
        }

        private void ShowError(IFileProgress file, int index)
        {
            if (Interrupt is null) return;
            Progress.Files.Clear();
            logHandler?.AddError(Interrupt.InnerException ?? Interrupt, file, index);
            Task.Run(() => App.MessageBoxShow(Interrupt.Message));
        }

        private Task SetLogHandlerComplete() =>
          logHandler?.SetComplete() ?? Task.CompletedTask;

        private void SetMaster() => App.Invoke(() =>
        {
            master.Title = "执行翻译";
            master.ProgressValue = 0;
            master.ProgressText = "0%";
            master.SetDisplayState(true);
            Progress.Files.ShowScan(false);
        });

        private void SetProgressValue(int i)
        {
            var res = (decimal)i / length * 100;
            var value = (int)res;
            master.ProgressValue = value;
            master.ProgressText = $"{value}%";
        }

        private FileHandler CreateFileHandler(IFileProgress file) => new(UpdateValue, this)
        {
            File = file,
            ErrorAction = AddError,
        };

        private void AddError(Exception exception, IFilePath file, int index) => logHandler?.AddError(exception, file, index);

        private void UpdateValue(Action action) => App.Invoke(action);


    }
}