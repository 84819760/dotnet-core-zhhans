using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DotNetCore_zhHans.Db.Import
{
    internal class ProgressManager : IAsyncDisposable
    {
        private readonly BroadcastBlock<int> broadcastBlock = new(x => x);
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly CancellationToken token;
        private readonly int count;

        public ProgressManager(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            token = mainWindowViewModel.Token;
            count = mainWindowViewModel.Count;
            TaskRun();
        }

        public bool IsCancell => token.IsCancellationRequested;

        public async ValueTask DisposeAsync()
        {
            broadcastBlock.Complete();
            await broadcastBlock.Completion;
        }

        private void TaskRun() => Task.Run(async () =>
        {
            while (!IsCancell)
            {
                await Task.Delay(1000);
                var value = await broadcastBlock.ReceiveAsync();
                SetProgress(value);
            }
        }, token);

        private void SetProgress(int value)
        {
            var progress = (int)((double)value / count * 100);
            mainWindowViewModel.Current = progress;
        }
    }
}
