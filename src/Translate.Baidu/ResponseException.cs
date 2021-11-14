using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;

namespace TranslateApi.Baidu
{
    internal class ResponseException : ResponseExceptionBase
    {
        private static readonly string[] splitdatas = new[] { "code:", "msg:" };

        public ResponseException(string value, TranslateServiceBase translateService) 
            : base(value, splitdatas, translateService)
        {
        }
    }
}
