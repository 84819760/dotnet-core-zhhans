using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Messages;


namespace DotNetCorezhHans.TranslTasks
{
    internal class TranslManager
    {
        protected readonly ExecButtonStateMessage buttonState = ExecButtonStateMessage.Instance;

        public TranslManager(IProgress progress)
        {
            Run = FirstRun;
            Progress = progress;
        }

        ~TranslManager() => CancellationTokenSource.Dispose();

        public CancellationTokenSource CancellationTokenSource { get; } = new();

        public CancellationToken Token => CancellationTokenSource.Token;

        public virtual ConfigManager Config { get; } = ConfigManager.Instance;

        public PageStateMessage PageState { get; } = PageStateMessage.Instance;

        public virtual GcManager GcManager { get; } = new();

        public bool IsEnd { get; private set; }

        public bool IsCancel => CancellationTokenSource.IsCancellationRequested;

        public Action Run { get; private set; }

        public IProgress Progress { get; }

        private Task SetButtonState(ExecButtonState execButton) =>
            buttonState.PublishAsync(execButton);

        public void SetShowScan(bool value) => App.Invoke(() => Progress.Files.ShowScan(value));

        public void SetMasterTitlet(string value) => App.Invoke(() =>
        {
            Progress.Master.Title =
            Progress.Files.ScanContent = value;
        });

        private async void FirstRun()
        {
            Run = SetCancel;
            await SetButtonState(ExecButtonState.Run);
            PageState.Publish(PageControlType.Translate);
            SetShow();
            await TryRun();
        }

        private void SetShow()
        {
            SetMasterTitlet("开始扫描");
            SetShowScan(true);
        }

        private async Task TryRun()
        {
            GcManager.SetGc(int.MaxValue);
            try
            {
                await CallRun(Token);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
            finally
            {
                GcManager.SetGc();
            }
        }

        private async Task CallRun(CancellationToken token)
        {
            var res = await Task.Run(RunTask);
            SetMasterTitlet("结束任务");          
            token.ThrowIfCancellationRequested();
            PageStatePublish(res, PageControlType.AbnormalList);
            await SetButtonState(ExecButtonState.Lock);
            await GcManager.Task;
            PageStatePublish(res, PageControlType.EndTask);
            await SetEnd();
        }

        private void PageStatePublish(PageControlType data, PageControlType target)
        {
            if (data != target) return;
            PageState.Publish(data);
        }

        private void ExceptionHandler(Exception ex)
        {
            if (ex is TaskCanceledException or OperationCanceledException)
            {
                Run?.Invoke();
            }
            else
            {
                Debug.Print($"非预期异常:{ex.GetType()}");
                App.MessageBoxShow(ex.ToString());
            }
        }

        public async void SetCancel()
        {
            Run = null;
            await SetButtonState(ExecButtonState.Lock);
            CancellationTokenSource.Cancel();
            await GcManager.Task;
            await Task.Delay(500);
            PageState.Publish(PageControlType.TerminationTask);
            await SetEnd();
        }

        private async Task<PageControlType> RunTask()
        {
            await Task.Delay(500);
            Progress.Master.SetDisplayState(false);
            return await new SecnFileTask(this).Run();
        }

        private async Task SetEnd()
        {
            await SetButtonState(ExecButtonState.Default);
            IsEnd = true;
        }
    }
}
