using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCoreZhHans.Service.ApiRequests;
using TencentCloud.Cfw.V20190904.Models;

namespace DotNetCoreZhHans.Service.Assistants.Placeholders
{
    /// <summary>
    /// 负责占位符检查
    /// </summary>
    internal class PlaceholderCheck
    {
        public static IEnumerable<PlaceholderBase> CreatePlaceholders(string value) => new PlaceholderBase[]
        {
            new Placeholder1(value),
            new Placeholder2(value),
            new Placeholder3(value),
            //new Placeholder4(value),
        };

        internal static async Task<string> GetCorrect(string transl
            , string original
            , ITransmitData transmitData
            , ApiRequestItem api
            , NodeCacheData data
            , CancellationToken token)
        {
            if (CheckRow(original, transl, out var datas))
            {
                data.CacheData.MissingContent = datas;
                return transl;
            }
            api.Master.Title = "重试!";
            return await Retry(original, api, data, transmitData, token);
        }

        private static bool CheckRow(string original, string transl, out int[] datas)
        {
            datas = CheckRow(original, transl);
            return datas.Length is 0;
        }

        private static int[] CheckRow(string original, string transl) =>
            GetIndexs(original).Except(GetIndexs(transl)).ToArray();

        private static IEnumerable<int> GetIndexs(string value) => SymbolManager
            .GetContentUnits(value)
            .OfType<ContentUnitIndex>()
            .Select(x => x.Id);


        private static async Task<string> Retry(string original
            , ApiRequestItem api
            , NodeCacheData data
            , ITransmitData transmitData
            , CancellationToken token)
        {
            var items = CreatePlaceholders(original).ToArray();
            var req = GetRequest(items);
            var res = await api.SendRequest(req, token);
            var arrray = GetResponse(res, items);
            var resValue = Retry(arrray, data, original);
            transmitData[original] = resValue;
            return resValue;
        }

        private static string Retry(string[] arrray, NodeCacheData data, string original)
        {
            string res = null;
            foreach (var transl in arrray)
            {
                res = transl;
                data.CacheData.MissingContent = CheckRow(original, transl);
                if (data.CacheData.MissingContent.Length is 0) return res;
            }
            return res;
        }

        private static string GetRequest(IEnumerable<PlaceholderBase> placeholders) =>
            string.Join("\r\n", placeholders.Select(x => x.Encoded));

        private static string[] GetResponse(string value, PlaceholderBase[] placeholders) => SymbolManager
            .GetApiRows(value)
            .Zip(placeholders)
            .Select(x => x.Second.GetDecode(x.First))
            .ToArray();
    }
}
