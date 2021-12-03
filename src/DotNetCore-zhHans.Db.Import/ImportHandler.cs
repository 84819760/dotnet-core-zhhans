using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore_zhHans.Db.Import
{
    internal class ImportHandler : IAsyncDisposable
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ProgressManager progressManager;
        private readonly WriteManager writeManager;

        public ImportHandler(MainWindowViewModel mainWindowViewModel
            , string sourceFile
            , string targetFile)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            SourceDbContext = new(sourceFile);
            TargetDbContext = new(targetFile);
            progressManager = new(mainWindowViewModel);
            writeManager = new(this);
        }

        internal DbContext SourceDbContext { get; }

        internal DbContext TargetDbContext { get; }

        internal ReaderWriterLockSlim LockSlim { get; } = new();

        public bool IsCancell => mainWindowViewModel.IsCancell;

        public CancellationToken Token => mainWindowViewModel.Token;

        public async ValueTask DisposeAsync()
        {
            await progressManager.DisposeAsync();
            await writeManager.DisposeAsync();
            await SourceDbContext.DisposeAsync();
            await TargetDbContext.DisposeAsync();
        }

        internal void Show(string value) => mainWindowViewModel.Title = value;

        private void SetCount() => mainWindowViewModel.Count = SourceDbContext.Count;

        private IAsyncEnumerable<TranslData> GetTranslDatas() => SourceDbContext.TranslDatas
            .Include(x => x.TranslSource)
            .AsNoTracking()
            .AsAsyncEnumerable();

        public async Task Run()
        {
            SetCount();
            await foreach (var item in GetTranslDatas())
            {
                await Run(item);
                Debug.Print(item.Id.ToString());
            }
        }

        private async Task Run(TranslData item)
        {
            if (await TargetDbContext.IsExists(item.Original)) return;
            await writeManager.SendAsync(item);
        }
    }
}