using System;
using DotNetCorezhHans.Base;

namespace DotNetCorezhHans
{
    /// <summary>
    /// 翻译结果
    /// </summary>
    public interface ITranslateValue
    {
        /// <summary>
        /// 原文
        /// </summary>
        string Original { get; }

        /// <summary>
        /// 译文
        /// </summary>
        string Translation { get; }
    }

    public class TranslateValue : ITranslateValue
    {
        public string Original { get; init; }
        public string Translation { get; init; }
    }
}
