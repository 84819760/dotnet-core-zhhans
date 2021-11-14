using System;

namespace DotNetCorezhHans
{
    public interface IErrorValue
    {
        public string ErrorCode { get; }

        public string ErrorMsg { get; }
    }
}
