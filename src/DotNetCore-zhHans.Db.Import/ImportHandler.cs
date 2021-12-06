using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Db.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore_zhHans.Db.Import;

internal class ImportHandler : IAsyncDisposable
{
    private readonly WriteManager writeManager;

    public ImportHandler(MainWindowViewModel mainWindowViewModel
        , string sourceFile
        , string targetFile)
    {
        ViewModel = mainWindowViewModel;
        SourceDbContext = new(sourceFile);
        TargetDbContext = new(targetFile);
        writeManager = new(this);
        ViewModel.ReadProgress.AddToMaximum(SourceDbContext.Count);
        ViewModel.WriteProgress.AddToMaximum(1);
    }

    internal DbContext SourceDbContext { get; }

    internal DbContext TargetDbContext { get; }

    internal MainWindowViewModel ViewModel { get; }

    public bool IsCancell => ViewModel.IsCancell;

    public async ValueTask DisposeAsync()
    {
        await writeManager.DisposeAsync();
        await SourceDbContext.DisposeAsync();
        await TargetDbContext.DisposeAsync();
        SqliteConnection.ClearAllPools();
    }

    private IAsyncEnumerable<TranslData> GetTranslDatas() => SourceDbContext
        .TranslDatas
        .Include(x => x.TranslSource)
        .AsNoTracking()
        .AsAsyncEnumerable();

    public async Task Run()
    {
        var items = GetTranslDatas();
        await foreach (var item in items)
        {
            if (IsCancell) break;
            await Run(item);
        }
        await DisposeAsync();
    }

    private async Task Run(TranslData item)
    {
        ViewModel.ReadProgress.AddToValue();
        var isExists = await TargetDbContext.IsExists(item.Original);
        if (!isExists) await writeManager.SendAsync(item);
        await Delay();
    }

    private Task Delay() => ViewModel.WriteProgress.Progress > 0.99 
        ? Task.Delay(1) 
        : Task.CompletedTask;
}