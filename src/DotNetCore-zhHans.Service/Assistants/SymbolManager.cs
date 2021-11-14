using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetCoreZhHans.Service
{
    /// <summary>
    /// 占位符处理
    /// </summary>
    internal class SymbolManager
    {
        public static readonly Regex isNumRegex = new(@"(\d+)");
        public static readonly Regex numRegex = new(@"^\d*[.]?\d*$");
        public static readonly Regex English = new(@"[a-zA-Z]");
        public static readonly Regex regex = new(@"{(\d+)}");

       
      
        /// <summary>
        /// 将字符串分解为api响应行集合
        /// </summary>
        public static string[] GetApiRows(string value) => value
            .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);


        /// <summary>
        ///  将文本拆分为 内容单元
        /// </summary>
        public static ContentUnit[] GetContentUnits(string value) => regex
            .Split(Replace(value))
            .Select(x => x.Trim())
            .Where(x => x is { Length: > 0 })
            .Select(x => CreateContentUnit(x, value))
            .ToArray();

        /// <summary>
        /// 按符号拆分
        /// </summary>
        private static ContentUnit CreateContentUnit(string value, string source)
        {
            value = value.Trim();
            var isNum = numRegex.IsMatch(value) && !value.Contains(".");
            return isNum ? new ContentUnitIndex() { Id = int.Parse(value), Source = source }
                         : new ContentUnit { Value = value };
        }

        public static string Replace(string value)
        {
            while (value.Contains("  ", StringComparison.Ordinal))
                value = value.Replace("  ", " ");

            return value;
        }

        public static int[] GetIds(string value) => GetContentUnits(value)
            .OfType<ContentUnitIndex>()
            .Select(x => x.Id)
            .ToArray();
    }
}
