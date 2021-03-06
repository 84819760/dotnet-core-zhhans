using DotNetCorezhHans;

namespace TranslateApi;

public class TranslateService_Offline : TranslateServiceBase
{
    public const string OfflineKey = ",   未配置翻译API，默认使用原文替代。";

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
        var res = $"{request}{OfflineKey}";
        return Task.FromResult(res);
    }

    public override string GetErrorCode(string code) => string.Empty;
}
