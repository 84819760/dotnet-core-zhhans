using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Db.Models;

namespace DotNetCore_zhHans.Db.Import;

internal class WriteManager : TargetBlockBase<TranslData>
{
    private readonly ActionBlock<TranslData[]> actionBlock;
    private readonly BatchBlock<TranslData> batchBlock;
    private readonly ImportHandler importHandler;
    private const int cache = 10000;

    public WriteManager(ImportHandler importHandler)
    {
        this.importHandler = importHandler;
        batchBlock = CreateBatchBlock();
        actionBlock = CreateActionBlock();
        batchBlock.LinkTo(actionBlock);
    }
    private static BatchBlock<TranslData> CreateBatchBlock() =>
        new(cache, new GroupingDataflowBlockOptions() { BoundedCapacity = cache * 2 });

    private ActionBlock<TranslData[]> CreateActionBlock() =>
        new(Write, new ExecutionDataflowBlockOptions()
        {
            BoundedCapacity = 2,
            MaxDegreeOfParallelism = 1
        });

    public override ITargetBlock<TranslData> TargetBlock => batchBlock;

    public DbContext TargetDbContext => importHandler.TargetDbContext;

    internal ReaderWriterLockSlim LockSlim => importHandler.LockSlim;

    public override async ValueTask DisposeAsync()
    {
        await SetComplete(batchBlock);
        await SetComplete(actionBlock);
        await TargetDbContext.DisposeAsync();
    }

    private async Task Write(TranslData[] datas)
    {
        if (importHandler.IsCancell) return;
        LockSlim.EnterWriteLock();
        try
        {
            using var dbContext = new DbContext(TargetDbContext);
            await dbContext.AddFactory(datas);
            importHandler.UpdateWriteTitle(datas.Length);
        }
        finally
        {
            LockSlim.ExitWriteLock();
        }
    }
}
