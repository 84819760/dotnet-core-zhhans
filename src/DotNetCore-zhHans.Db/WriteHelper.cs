using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db.Models;

namespace DotNetCorezhHans.Db
{
    internal class WriteHelper
    {
        private readonly IEnumerable<ITranslaResults> translas;
        private readonly ITransmitData transmitData;
        private readonly ZhDbContext dbContext;

        public WriteHelper(ZhDbContext dbContext
            , ITransmitData transmitData
            , IEnumerable<ITranslaResults> translas)
        {
            this.translas = translas;
            this.dbContext = dbContext;
            this.transmitData = transmitData;
        }

        internal async Task WriteRows()
        {
            await using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                foreach (var item in translas)
                {
                    await WriteRows(item);
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transmitData.File.AddError(ex);
                await transaction.RollbackAsync();
            }
        }

        private async Task WriteRows(ITranslaResults item)
        {
            if (await IsExists(item.Original)) return;
            var row = await CreateTranslData(item);
            dbContext.TranslDatas.Add(row);
            await dbContext.SaveChangesAsync();
        }

        private async Task<TranslData> CreateTranslData(ITranslaResults item) => new()
        {
            Original = item.Original,
            Translation = item.Transl,
            TranslSource = await GetSource(item.Source),
            UpdateDate = DateTime.Now
        };

        private async Task<bool> IsExists(string original)
        {
            var res = await dbContext.FindData(original);
            return res != null;
        }

        private async Task<TranslSource> GetSource(string name) =>
            await dbContext.FindSource(name) ?? await CreateSource(name);

        private async Task<TranslSource> CreateSource(string name)
        {
            var data = new TranslSource() { Name = name };
            dbContext.TranslSources.Add(data);
            await dbContext.SaveChangesAsync();
            return data;
        }
    }
}
