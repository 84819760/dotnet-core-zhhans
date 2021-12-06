using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.Sqlite;

namespace DotNetCorezhHans.Db
{
    public class ZhDbContext : DbContext
    {
        private readonly ReaderWriterLockSlim dbLock;
        private readonly ITransmitData transmitData;

        static ZhDbContext() => AppDomain.CurrentDomain
            .ProcessExit += (_, _) => SqliteConnection.ClearAllPools();

        public ZhDbContext(ITransmitData transmitData)
        {
            this.transmitData = transmitData;
            dbLock = transmitData.DbLock;
        }

        public DbSet<TranslData> TranslDatas { get; set; }

        public DbSet<TranslSource> TranslSources { get; set; }

        public DbSet<FileMd5> FileMd5s { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = @$"Data Source={GetDbPath()}";
            optionsBuilder.UseSqlite(path);
        }

        private string GetDbPath()
        {
            var dir = Path.GetDirectoryName(GetType().Assembly.Location);
            return Path.Combine(dir, "TranslData.db");
        }

        internal Task<TranslSource> FindSource(string name) => TranslSources
            .FirstOrDefaultAsync(x => x.Name == name) ??
            Task.FromResult(FindLocalSource(name));

        private TranslSource FindLocalSource(string name) => TranslSources.Local
          .FirstOrDefault(x => x.Name == name);

        public Task<TranslData> FindDataLock(string original)
        {
            dbLock.EnterReadLock();
            try
            {
                return FindData(original);
            }
            finally
            {
                dbLock.ExitReadLock();
            }
        }

        internal Task<TranslData> FindData(string original) => TranslDatas
            .AsNoTracking()
            .Include(x => x.TranslSource)
            .FirstOrDefaultAsync(x => x.Original == original);


        public Task WriteRowsLock(IEnumerable<ITranslaResults> rows)
        {
            dbLock.EnterWriteLock();
            try
            {
                return new WriteHelper(this, transmitData, rows).WriteRows();
            }
            finally
            {
                dbLock.ExitWriteLock();
            }
        }
    }
}