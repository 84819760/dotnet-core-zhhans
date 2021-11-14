using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base
{
    public class ExceptionBase : Exception, IErrorValue
    {
        public ExceptionBase() { }

        public ExceptionBase(string message) : base(message) { }

        public ExceptionBase(string message, Exception exception) : base(message, exception) { }

        public Guid Id { get; } = Guid.NewGuid();

        public int Index { get; set; }

        public bool IsError { get; set; }

        public DateTime DateTime { get; } = DateTime.Now;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 成员路径
        /// </summary>
        public string MemberPath { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Modular { get; set; }

        public override string ToString() => @$"
编号 : {Index}
文件 : {FilePath}
日期 : {DateTime}
成员 : {MemberPath}
模块 : {Modular}
错误 : {Message}
";

        public string ErrorCode { get; }
        public string ErrorMsg => ToString();
    }
}
