using System;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCoreZhHans.Service.FileHandlers.FileActuators
{
    /// <summary>
    /// 负责导入文件
    /// </summary>
    internal class ImportFileActuator : FileActuatorBase
    {
        public ImportFileActuator(ITransmitData transmits) : base(transmits)
        {
        }

        public override Task Run() => throw new Exception("ImportFileActuator");
    }
}
