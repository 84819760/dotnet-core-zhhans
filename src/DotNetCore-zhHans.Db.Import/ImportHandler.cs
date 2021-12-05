using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Db.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace DotNetCore_zhHans.Db.Import;

internal class ImportHandler : IAsyncDisposable
{
    private readonly ProgressManager progressManager;
    private readonly WriteManager writeManager;
    private volatile int count;

    public ImportHandler(MainWindowViewModel mainWindowViewModel
        , string sourceFile
        , string targetFile)
    {
        ViewModel = mainWindowViewModel;
        SourceDbContext = new(sourceFile);
        TargetDbContext = new(targetFile);
        progressManager = new(this);
        writeManager = new(this);
        Init();
    }

    private void Init()
    {
        ViewModel.WriteTitle = "写入 : 0 行";
        ViewModel.Current = 0;
        ViewModel.Count = SourceDbContext.Count;
        ViewModel.UpdateCurrent(0);
    }

    internal DbContext SourceDbContext { get; }

    internal DbContext TargetDbContext { get; }

    internal MainWindowViewModel ViewModel { get; }

    public bool IsCancell => ViewModel.IsCancell;

    public async ValueTask DisposeAsync()
    {
        await progressManager.DisposeAsync();
        await writeManager.DisposeAsync();
        await SourceDbContext.DisposeAsync();
        await TargetDbContext.DisposeAsync();
        SqliteConnection.ClearAllPools();
    }

    internal void UpdateCurrent(int value) => ViewModel.UpdateCurrent(value);
    internal void UpdateWriteTitle(int value) => ViewModel.UpdateWriteTitle(value);
    internal void UpdateWriteTitle(string value) => ViewModel.UpdateWriteTitle(value);


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
        var isExists = await TargetDbContext.IsExists(item.Original);
        if (!isExists) await writeManager.SendAsync(item);
        SetRead();
    }

    private async void SetRead()
    {
        count++;
        await progressManager.SendAsync(count);
    }
}