using System;

namespace DotNetCorezhHans.Base
{
    public abstract class ResponseExceptionBase : Exception, IErrorValue
    {
        public ResponseExceptionBase(string value, string[] splitdatas
            , TranslateServiceBase translateService)
        {
            var (code, msg) = TranslateServiceBase.GetErrorCode(value, splitdatas);
            ErrorCode = code;
            ErrorMsg = $"{translateService.GetErrorCode(ErrorCode)} >>> {msg}";
            Message = $"code : {code}, msg : {ErrorMsg}";
        }

        public string ErrorCode { get; protected set; }

        public string ErrorMsg { get; protected set; }

        public override string Message { get; }
    }
}
