using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base
{
    /// <summary>
    /// 中断执行
    /// </summary>
    public class InterruptException : Exception
    {
        public InterruptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
