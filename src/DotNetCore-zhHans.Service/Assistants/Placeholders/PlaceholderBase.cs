using System.Linq;
namespace DotNetCoreZhHans.Service.Assistants.Placeholders
{
    internal abstract class PlaceholderBase
    {
        protected static readonly (string source, string target) fullAngleSymbolBracketsRight = ("）", "");
        protected static readonly (string source, string target) fullAngleSymbolBracketsLeft = ("（", "");
        protected static readonly (string source, string target) horizontalLine = ("-", "");
        protected readonly string original;
        private string encoded;

        public PlaceholderBase(string original) => this.original = original;


        /// <summary>
        /// 编码
        /// </summary>
        public string Encoded => encoded ??= GetEncoded(original);

        protected abstract string GetEncoded(string value);

        /// <summary>
        /// 解码
        /// </summary>
        public abstract string GetDecode(string value);

        protected virtual string Replace(string value, (string source, string target)[] ps)
        {
            var items = ps.Concat(new[] { ("{{", "{"), ("}}", "}"), ("({", "{"), ("})", "}"), ("{}", "") });
            foreach (var (source, target) in items)
            {
                value = value.Replace(source, target);
            }
            return value;
        }

        protected static string Replace(string value, string source, string target) =>
            value.Replace(source, target);
    }
}
