using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

static class Share
{
    public const string RootExe = "DotNetCoreZhHans.exe";

    public static string? GetMd5(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return default;
            using var file = File.OpenRead(filePath);
            var md5 = MD5.Create();
            var hashValues = md5.ComputeHash(file);
            var hashStr = hashValues.Select(x => x.ToString("X2"));
            return string.Join("", hashStr);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static JsonSerializerOptions JsonOptions => new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        IgnoreReadOnlyProperties = true,
        IgnoreReadOnlyFields = true,
    };



    public static string GetRootDirectory(bool isSetCurrentDirectory = false)
    {
        var dir = Directory.GetCurrentDirectory();
        if (dir.Contains(@"\lib\_download"))
            dir = Directory.GetParent(dir)!.Parent!.FullName;
        if (isSetCurrentDirectory) Environment.CurrentDirectory = dir;
        return dir;
    }

    public static void Show(string title, string[] args)
    {
#if ShowPath
        var ps = string.Join("\r\n", args);
        MessageBox.Show($"路径 : {Directory.GetCurrentDirectory()} \r\n命令:\r\n{ps}", title);
#endif 
    }
}
