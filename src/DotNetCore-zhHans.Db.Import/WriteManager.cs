﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using DotNetCorezhHans.Db.Models;

namespace DotNetCore_zhHans.Db.Import
{
    internal class WriteManager : IAsyncDisposable, ITargetBlock<TranslData>
    {
        private readonly ActionBlock<TranslData[]> actionBlock;
        private readonly BatchBlock<TranslData> batchBlock;
        private readonly ImportHandler importHandler;
        private readonly DbContext targetDbContext;

        public WriteManager(ImportHandler importHandler)
        {
            targetDbContext = importHandler.TargetDbContext;
            batchBlock = new(10240);
            actionBlock = new(Write);
            batchBlock.LinkTo(actionBlock);
            this.importHandler = importHandler;
        }

        public async ValueTask DisposeAsync()
        {
            batchBlock.Complete();
            await batchBlock.Completion;

            actionBlock.Complete();
            await actionBlock.Completion;

            await targetDbContext.DisposeAsync();
        }

        private async Task Write(TranslData[] datas)
        {
            importHandler.Show($"执行写入:{datas.Length}");
            await targetDbContext.Write(datas);
            importHandler.Show("");
        }    

        #region ITargetBlock<TranslData>   

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, TranslData messageValue, ISourceBlock<TranslData>? source, bool consumeToAccept) => ((ITargetBlock<TranslData>)batchBlock).OfferMessage(messageHeader, messageValue, source, consumeToAccept);

        public void Complete() => ((IDataflowBlock)batchBlock).Complete();

        public void Fault(Exception exception) => ((IDataflowBlock)batchBlock).Fault(exception);

        public Task Completion => ((IDataflowBlock)batchBlock).Completion;

        #endregion
    }
}
