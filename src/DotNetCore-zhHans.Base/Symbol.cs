using System.Text.RegularExpressions;
using System.Text;

namespace DotNetCorezhHans
{
    /// <summary>
    /// 符号处理
    /// </summary>
    public class Symbol
    {
        private static readonly Regex isNumRegex = new(@"(\d+)");
        private static readonly Regex isRangeRegex = new(@"[0-9]");
        private static readonly (string l, string r)[] replaces = new[]
        {
             ("{ ","{"),
             (" }","}"),
             ("{{","{"),
             ("}}","}"),
        };

        public static string AfterStaticHandling(string value)
        {
            foreach (var (l, r) in replaces)
            {
                value = value.Replace(l, r);
            }
            return value;
        }

        public static string Format(object value) => $"{{{value}}}";

        /// <summary>
        /// 分组标志
        /// </summary>
        public const string Group = "\r\n";

        /// <summary>
        /// 翻译前字符串处理。
        /// </summary>
        public virtual string BeforeHandler(string value) => value;

        /// <summary>
        /// 翻译后字符串处理。
        /// </summary>
        public virtual string AfterHandler(string value)
        {
            var sb = new StringBuilder();
            var tmp = isNumRegex.Split(value);
            foreach (var item in tmp)
            {
                var isTarget = isRangeRegex.IsMatch(item);
                if (isTarget) sb.Append('{');
                sb.Append(item);
                if (isTarget) sb.Append('}');
            }
            return AfterStaticHandling(sb.ToString());
        }
    }
}
