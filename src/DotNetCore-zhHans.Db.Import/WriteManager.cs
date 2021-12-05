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
    private const int boundedCapacity = cache * 10;
    private readonly ImportHandler importHandler;
    private const int cache = 1000;
    private volatile bool isCancell;
    private volatile int cacheCount;
    private readonly Task task;

    public WriteManager(ImportHandler importHandler)
    {
        this.importHandler = importHandler;
        batchBlock = CreateBatchBlock();
        SetCancell();
        task = Task.Run(Loop);
    }

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
        var len = datas.Count();
        using var dbContext = new DbContext(TargetDbContext);
        await dbContext.AddFactory(datas);
        //importHandler.UpdateWriteTitle(len);
        //Debug.Print($"写入:{len}, 缓存:{GetCacheCount(len)}");
    }

    private int GetCacheCount(int count)
    {
        cacheCount -= count;
        return cacheCount > boundedCapacity ? boundedCapacity : cacheCount;
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
