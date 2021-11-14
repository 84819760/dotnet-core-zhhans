using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreZhHans.Service.Assistants.Placeholders
{
    /// <summary>
    /// {1} 转换 {-1-}
    /// </summary>
    internal class Placeholder2 : PlaceholderBase
    {
        /// <summary>
        /// <inheritdoc cref="Placeholder2"/>
        /// </summary>
        public Placeholder2(string original) : base(original)
        {
        }

        protected override string GetEncoded(string value) =>
            Replace(value, new[] { ("{", "{-"), ("}", "-}") });

        public override string GetDecode(string value) =>
            Replace(value, new[] { horizontalLine});
    }
}
