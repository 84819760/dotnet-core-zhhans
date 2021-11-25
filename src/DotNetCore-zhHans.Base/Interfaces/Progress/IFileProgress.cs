using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base.Interfaces
{
    public interface IFileProgress : IFilePath
    {
        /// <summary>
        /// 进度文本
        /// </summary>
        string Progress { get; set; }

        Exception[] Exceptions { get; }

        Exception AddError(Exception exception);


        event Action<Exception> AddErrorEvent;


        /// <summary>
        /// 创建错误
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="memberPath">成员路径</param>
        /// <param name="modularName">模块名称</param>
        /// <param name="errMsg">错误消息</param>
        /// <param name="exception">附加异常</param>
        public Exception CreateAndAdd(string memberPath
          , string modularName
          , string errMsg
          , string filePath = null
          , Exception exception = null
          , bool isError = false)
        {
            var res = new ExceptionBase(errMsg, exception)
            {
                MemberPath = memberPath,
                FilePath = filePath ?? Path,
                Modular = modularName,
                IsError = isError,
            };
            return AddError(res);
        }
    }


}
