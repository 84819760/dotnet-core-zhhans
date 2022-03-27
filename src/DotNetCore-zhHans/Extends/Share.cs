using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

static class Share
{
    public static string GetMd5(string filePath)
    {
        try
        {
            using var file = File.OpenRead(filePath);
            var md5 = MD5.Create();
            var hashValues = md5.ComputeHash(file);
            var hashStr = hashValues.Select(x => x.ToString("X2"));
            return string.Join("", hashStr);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static JsonSerializerOptions JsonOptions => new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString,
    };

    public static void Show(string title, string[] args)
    {
        var ps = string.Join("\r\n", args);
        MessageBox.Show($"路径 : {Directory.GetCurrentDirectory()} \r\n命令:\r\n{ps}", title);

    }
}
