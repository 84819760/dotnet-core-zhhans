using System;
using System.Linq;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;

namespace TranslateApi.Tencent
{
    internal class ResponseException : ResponseExceptionBase
    {
        private static readonly string[] splitdatas = new[] { "code:", "message:" };

        public ResponseException(string value, TranslateServiceBase translateService)
            : base(value, splitdatas, translateService)
        {
        }
    }
}
