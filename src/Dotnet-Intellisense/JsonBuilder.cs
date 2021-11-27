using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Dotnet_Intellisense
{
    internal class JsonBuilder
    {
        private readonly static JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };


        public static string GetJson(object obj) => JsonSerializer.Serialize(obj, jsonOptions);

        public static async Task<DataModel[]> GetDataModels()
        {         
            using var hc = new HttpClient();
            var url = "http://www.wyj55.cn/download/DotNetCorezhHans/Dotnet-Intellisense.json";
            var json = await hc.GetStringAsync(url);          
            return JsonSerializer.Deserialize<DataModel[]>(json) ?? Array.Empty<DataModel>();
        }
    }
}
