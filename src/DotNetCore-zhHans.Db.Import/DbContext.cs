using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCorezhHans.Db.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore_zhHans.Db.Import
{
    internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly string dbFilePath;

        static DbContext() => AppDomain.CurrentDomain
            .ProcessExit += (_, _) => SqliteConnection.ClearAllPools();

        public DbContext(string dbFilePath) => this.dbFilePath = dbFilePath;

        public DbSet<TranslData> TranslDatas { get; set; } = null!;

        public DbSet<TranslSource> TranslSources { get; set; } = null!;

        public int Count => TranslDatas.Count();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conn = new SqliteConnectionStringBuilder() { DataSource = dbFilePath };
            optionsBuilder.UseSqlite(conn.ConnectionString);
        }

        public async Task<TranslSource> FindTranslSource(string name) => TranslSources
            .Local.FirstOrDefault(x => x.Name == name) ?? await AddTranslSource(name);

        private async Task<TranslSource> AddTranslSource(string name)
        {
            var res = new TranslSource() { Name = name };
            TranslSources.Add(res);
            await SaveChangesAsync();
            return res;
        }

        public Task<bool> IsExists(string original) => TranslDatas
            .AsNoTracking()
            .AnyAsync(x => x.Original == original);

        public async Task Write(TranslData[] datas)
        {
            using var bt = await Database.BeginTransactionAsync();
            //try
            //{
            //    var rows = datas.Select(Create).ToArray();
            //    await targetDbContext.TranslDatas.AddRangeAsync(rows!);
            //    await bt.CommitAsync();
            //    await targetDbContext.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    await bt.RollbackAsync();
            //    MessageBox.Show(ex.Message);
            //}
        }

        private TranslData Create(TranslData data)
        {
            //data.TranslSource = targetDbContext.FindTranslSource(data.TranslSource.Name);
            //return data;
        }
    }
}
