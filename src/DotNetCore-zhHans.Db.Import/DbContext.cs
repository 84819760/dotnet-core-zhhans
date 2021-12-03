using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DotNetCorezhHans.Db.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore_zhHans.Db.Import
{
    internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly Dictionary<string, TranslSource> map = new();
        private readonly SemaphoreSlim _mutex = new(1, 1);
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

        private Task<bool> AnyAsync(string original) => TranslDatas
            .AsNoTracking()
            .AnyAsync(x => x.Original == original);

        public async Task<bool> IsExists(string original)
        {
            await _mutex.WaitAsync();
            try
            {
                return await AnyAsync(original);
            }
            finally
            {
                _mutex.Release();
            }
        }

        internal async Task Write(TranslData[] datas)
        {
            await _mutex.WaitAsync();
            try
            {
                await RunWrite(datas);
            }
            finally
            {
                _mutex.Release();
            }
        }

        private async Task RunWrite(TranslData[] datas)
        {
            using var bt = await Database.BeginTransactionAsync();
            try
            {
                var items = datas.Select(CreateTranslData);
                await TranslDatas.AddRangeAsync(items);
                await bt.CommitAsync();
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await bt.RollbackAsync();
                MessageBox.Show(ex.Message);
            }
            TranslDatas.Local.Clear();
            Debug.Print($"写入{datas.Length} {DateTime.Now}");
        }

        private TranslData CreateTranslData(TranslData data)
        {
            var source = data.TranslSource.Name;
            data.TranslSource = FindTranslSource(source);
            return data;
        }

        private TranslSource FindTranslSource(string name)
        {
            if (!map.TryGetValue(name, out var res))
            {
                res = map[name] = GetFirstSource(name) ?? AddTranslSource(name);
            }
            return res;

        }

        private TranslSource? GetFirstSource(string name) => TranslSources
            .FirstOrDefault(x => x.Name == name);

        private TranslSource AddTranslSource(string name)
        {
            var res = new TranslSource() { Name = name };
            TranslSources.Add(res);
            return res;
        }
    }
}