using DotNetCorezhHans.Db.Models;
using Microsoft.Data.Sqlite;

namespace DotNetCore_zhHans.Db.Import;

internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly Dictionary<string, TranslSource> map = new();

    static DbContext() => AppDomain.CurrentDomain
        .ProcessExit += (_, _) => SqliteConnection.ClearAllPools();

    public DbContext() => FilePath = "默认路径";

    public DbContext(string dbFilePath) => FilePath = dbFilePath;

    public DbContext(DbContext dbContext) : this(dbContext.FilePath) { }

    public string FilePath { get; }

    public DbSet<TranslData> TranslDatas { get; set; } = null!;

    public DbSet<TranslSource> TranslSources { get; set; } = null!;

    public int Count => TranslDatas.Count();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = new SqliteConnectionStringBuilder() { DataSource = FilePath };
        optionsBuilder.UseSqlite(conn.ConnectionString);
    }

    public Task<bool> IsExists(string original) => TranslDatas
        .AsNoTracking()
        .AnyAsync(x => x.Original == original);

    internal async Task AddFactory(IEnumerable<TranslData> datas)
    {
        var items = datas.Select(CreateTranslData);
        await TranslDatas.AddRangeAsync(items);
        await SaveChangesAsync();
    }

    private TranslData CreateTranslData(TranslData data)
    {
        var source = data.TranslSource.Name;
        data.Id = 0;
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
