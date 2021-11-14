using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreZhHans.Service.Assistants.Placeholders
{
    /// <summary>
    /// {1} 转换 (-1-)
    /// </summary>
    internal class Placeholder1 : PlaceholderBase
    {
        /// <summary>
        /// <inheritdoc cref="Placeholder1"/>
        /// </summary>
        public Placeholder1(string original) : base(original)
        {
        }

        protected override string GetEncoded(string value) =>
            Replace(value, new[] { ("{", "(-"), ("}", "-)") });

        public override string GetDecode(string value)
        {
            var ps = new[]
            {
                fullAngleSymbolBracketsRight,
                fullAngleSymbolBracketsLeft,
                horizontalLine,
            };
            var res = Replace(value, ps);
            return res;
        }
    }
}
