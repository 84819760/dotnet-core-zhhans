//using System.Threading;
//using System.Threading.Tasks;
//using System.Threading.Tasks.Dataflow;

//namespace DotNetCore_zhHans.Db.Import;

//internal class ProgressManager : TargetBlockBase<int>
//{
//    private readonly BroadcastBlock<int> broadcastBlock = new(x => x);
//    private readonly ImportHandler importHandler;

//    public ProgressManager(ImportHandler importHandler)
//    {
//        this.importHandler = importHandler;
//        TaskRun();
//    }

//    public override ITargetBlock<int> TargetBlock => broadcastBlock;

//    public override ValueTask DisposeAsync() => SetComplete(broadcastBlock);

//    private void TaskRun() => Task.Run(async () =>
//    {
//        while (await IsContinue())
//        {
//            await Task.Delay(100);
//            var value = await broadcastBlock.ReceiveAsync();
//            importHandler.ViewModel.ProgressValue = value;
//        }
//    });

//    private async ValueTask<bool> IsContinue() =>
//        !importHandler.IsCancell
//        && await broadcastBlock.OutputAvailableAsync();
//}
