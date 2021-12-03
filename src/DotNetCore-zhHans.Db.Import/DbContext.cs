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

        public TranslSource FindTranslSource(string name) => TranslSources
            .Local.FirstOrDefault(x => x.Name == name) ?? AddTranslSource(name);

        private TranslSource AddTranslSource(string name)
        {
            var res = new TranslSource() { Name = name };
            TranslSources.Add(res);
            SaveChanges();
            return res;
        }

        public Task<bool> IsExists(string original) => TranslDatas
            .AsNoTracking()
            .AnyAsync(x => x.Original == original);
    }
}
