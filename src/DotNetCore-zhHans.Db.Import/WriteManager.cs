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
    private readonly ImportHandler importHandler;
    private const int cache = 1000;
    private volatile bool isCancell;
    private readonly Task task;

    public WriteManager(ImportHandler importHandler)
    {
        this.importHandler = importHandler;
        batchBlock = CreateBatchBlock();
        SetCancell();
        task = Task.Run(Loop);
    }

    private void SetCancell() => importHandler.ViewModel.CancellationTokenSource
        .Token.Register(() => isCancell = true);

    private static BatchBlock<TranslData> CreateBatchBlock() =>
        new(cache, new GroupingDataflowBlockOptions() { BoundedCapacity = cache * 10 });

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
        using var dbContext = new DbContext(TargetDbContext);
        await dbContext.AddFactory(datas);
        importHandler.UpdateWriteTitle(datas.Count());
        Debug.Print($"写入:{datas.Count()}, 缓存:{batchBlock.OutputCount * cache}");
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
