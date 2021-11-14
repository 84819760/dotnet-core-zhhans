using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetCorezhHans.Base
{
    internal class CheckContent
    {
        private const string regex = "[a-zA-Z]";

        protected static string GetLetter(string value) => Regex.Matches(value, regex)
                 .OfType<Match>()
                 .Select(x => x.Value)
                 .JoinBuilder();

        private static string CherckLength(ITranslateValue value) => value.Translation;

        //{
        //    if (CherckLength(value.Original, value.Translation)) return value.Translation;
        //    //return CherckLetter(value);
        //    return value.Translation;
        //}

        //private static bool CherckLength(string l, string r) => l.Length == r.Length && r.Length < 2;

        //private static string CherckLetter(ITranslateValue value)
        //{
        //    return CherckLetter(value.Original, value.Translation)
        //            ? throw new Exception($"翻译失败,原文和译文相同:{value.Original}")
        //            : value.Translation;
        //}

        //private static bool CherckLetter(string l, string r)
        //{
        //    var ld = GetLetter(l.ToLower());
        //    var rd = GetLetter(r.ToLower());
        //    return ld == rd;
        //}

        public static string CherckAndResult(ITranslateValue value)
        {
            if (value is IErrorValue error && error.ErrorMsg is { Length: > 0 })
                throw error.GetException();
            return CherckLength(value);
        }
    }
}
