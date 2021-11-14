using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;
using System.Diagnostics;
using DotNetCorezhHans;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    internal class WordNode : NodeBase
    {
        private static readonly string[] symbols = new[]
        {
            ",",
            ".",
        };
        private static readonly Regex regex = new(@"[A-Z]");
        private readonly string[] ignores;
        private string endSymbol;
        private string startsSymbol;

        public WordNode(string original, IndexProvider indexProvider
            , ITransmitData transmits
            , int index
            , NodeBase parent)
            : base(indexProvider, transmits, null, index, parent)
        {
            OriginalValue = CheckEnd(original.Trim());
            ignores = transmits.Config.Ignores ?? Array.Empty<string>();
            Lower = OriginalValue.ToLower();
            IsSymbol = GetIsSymbol(OriginalValue);

            if (!IsSymbol) return;
            Id = indexProvider.GetId();
            AddToMap(this);
        }

        public override int Id { get; }

        public string Lower { get; }

        public bool IsSymbol { get; }

        public override string Name => "#word";

        public override string QueryValue =>
            GetQueryValue(IsSymbol ? Symbol.Format(Id) : OriginalValue);

        public override string OriginalValue { get; }

        public override XmlNode XmlNode => Parent.XmlNode;

        private string CheckEnd(string value)
        {
            foreach (var source in symbols)
            {
                if (value.StartsWith(source))
                {
                    value = value[1..];
                    startsSymbol = source;
                }

                if (value.EndsWith(source))
                {
                    value = value[0..^1];
                    endSymbol = source;
                }               
            }
            return value;
        }

        private string GetQueryValue(string value) => $"{startsSymbol}{value}{endSymbol}";

        private bool GetIsSymbol(string original) =>
            TestSymbol(original) || ignores.Any(TestIgnores) || TestUpper(original) || IsNum(original);

        /// <summary>
        /// 过滤测试
        /// </summary>
        private bool TestIgnores(string target) => Lower
            .Contains(target, StringComparison.Ordinal);

        /// <summary>
        /// 大写测试
        /// </summary>
        private static bool TestUpper(string original) => regex.Matches(original)
            .Cast<Match>()
            .LastOrDefault() is { Index: > 0 };

        private static bool IsNum(string original) => SymbolManager.isNumRegex.IsMatch(original);

        private static bool TestSymbol(string original) =>
            TestSymbol('\'', original) || TestSymbol('\"', original);

        private static bool TestSymbol(char value, string original) =>
            original.StartsWith(value) || original.EndsWith(value);

        public override string ToString() =>
            $"{base.ToString()}, IsSymbol = {IsSymbol} ,original = {OriginalValue}";

        public override XmlNode GetXmlNode() => XmlDoc.CreateTextNode(OriginalValue);
    }
}