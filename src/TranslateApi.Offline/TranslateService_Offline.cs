using System.Diagnostics;
using DotNetCorezhHans;

namespace TranslateApi;

public class TranslateService_Offline : TranslateServiceBase
{
    public const string OfflineKey = "!本地数据库中无翻译数据，需要开启API！";

    public TranslateService_Offline() : base(new()
    {
        Name = "离线翻译",
        Enable = true,
        MaxChar = 6000,
        ThreadCount = 1,
        IntervalTime = 0,
    })
    { }

    protected override Task<string> PostAsync(string request)
    {
        //Debug.Print(request);
        IEnumerable<string> rows = request.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        rows = rows.Select(x => $"{x} {OfflineKey}");
        var res = string.Join("\r\n", rows);
        //return Task.FromResult("");用于测试行丢失！
        return Task.FromResult(res);
    }

    public override string GetErrorCode(string code) => string.Empty;
}
