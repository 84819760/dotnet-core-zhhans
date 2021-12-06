using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Db.Models;
using System.Diagnostics;

namespace DotNetCore_zhHans.Db.Import;

internal class WriteManager : TargetBlockBase<TranslData>
{
    private readonly BatchBlock<TranslData> batchBlock;
    private const int boundedCapacity = cache * 30;
    private readonly ImportHandler importHandler;
    private const int cache = 1000;
    private volatile bool isCancell;
    private readonly Task task;
    private volatile int cacheCount;

    public WriteManager(ImportHandler importHandler)
    {
        this.importHandler = importHandler;
        batchBlock = CreateBatchBlock();
        SetCancell();
        task = Task.Run(Loop);
        WriteProgress.Maximum = boundedCapacity;
        WriteProgress.ChangedHandler();
    }

    public ProgressData WriteProgress => ViewModel.WriteProgress;

    public MainWindowViewModel ViewModel => importHandler.ViewModel;

    public override Task<bool> SendAsync(TranslData value)
    {
        cacheCount++;
        return batchBlock.SendAsync(value);
    }

    private void SetCancell() => importHandler.ViewModel.CancellationTokenSource
        .Token.Register(() => isCancell = true);

    private static BatchBlock<TranslData> CreateBatchBlock() =>
        new(cache, new GroupingDataflowBlockOptions() { BoundedCapacity = boundedCapacity });

    public override ITargetBlock<TranslData> TargetBlock => batchBlock;

    public DbContext TargetDbContext => importHandler.TargetDbContext;

    public override async ValueTask DisposeAsync()
    {
        await SetComplete(batchBlock);
        await task;
        await TargetDbContext.DisposeAsync();
    }

    private async Task Write(IEnumerable<TranslData> datas)
    {
        if (isCancell) return;
        var count = datas.Count();
        cacheCount -= count;
        using var dbContext = new DbContext(TargetDbContext);
        await dbContext.AddFactory(datas);
        ViewModel.WriteCount += count;
        SetWriteProgress();
    }

    private void SetWriteProgress()
    {
        cacheCount = cacheCount < 0 ? 0 : cacheCount;
        WriteProgress.Value = cacheCount;
        WriteProgress.ChangedHandler();
    }

    private async Task Loop()
    {
        while (await batchBlock.OutputAvailableAsync())
        {
            var list = new List<TranslData>();
            while (batchBlock.TryReceive(out var items))
            {
                list.AddRange(items);
            }
            await Write(list);
        }
    }
}
