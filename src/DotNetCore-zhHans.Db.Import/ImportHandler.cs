using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly DbContext sourceDbContext;
        private readonly DbContext targetDbContext;

        public ImportHandler(MainWindowViewModel mainWindowViewModel
            , string sourceFile
            , string targetFile)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            sourceDbContext = new(sourceFile);
            targetDbContext = new(targetFile);
            progressManager = new(mainWindowViewModel);
            writeManager = new(targetDbContext);
            Run();
        }

        public async ValueTask DisposeAsync()
        {
            await progressManager.DisposeAsync();
            await writeManager.DisposeAsync();
            await sourceDbContext.DisposeAsync();
            await targetDbContext.DisposeAsync();
        }

        private void SetCount() => mainWindowViewModel.Count = sourceDbContext.Count;

        private IAsyncEnumerable<TranslData> GetTranslDatas() => sourceDbContext.TranslDatas.AsNoTracking().AsAsyncEnumerable();

        private async void Run()
        {
            SetCount();
            await foreach (var item in GetTranslDatas()) await Run(item);
        }

        private async Task Run(TranslData item)
        {
            if (await targetDbContext.IsExists(item.Original)) return;
            await writeManager.SendAsync(item);
        }
    }
}
