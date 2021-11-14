using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreZhHans.Service.Assistants.Placeholders
{
    internal class Placeholder3: PlaceholderBase
    {
        public Placeholder3(string original) : base(original)
        {
        }

        protected override string GetEncoded(string value) =>
            Replace(value, new[] { ("{", "("), ("}", ")") });

        public override string GetDecode(string value)
        {
            var ps = new[]
            {
                fullAngleSymbolBracketsRight,
                fullAngleSymbolBracketsLeft,
            };
            var res = Replace(value, ps);
            return res;
        }
    }
}
