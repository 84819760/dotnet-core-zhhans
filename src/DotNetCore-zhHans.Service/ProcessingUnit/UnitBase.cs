using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;
using NearCoreExtensions;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{

    /// <summary>
    /// 处理单元
    /// </summary>
    internal abstract class UnitBase
    {
        public UnitBase(ITransmitData transmits)
        {
            Transmits = transmits;
            Token = transmits.Token;
            transmits.GcManager.Add(this);
        }

        ~UnitBase()
        {
            Transmits.GcManager.Remove(this);
        }

        public ITransmitData Transmits { get; }

        public CancellationToken Token { get; }

        public abstract Task Complete();

        public static async Task Complete(params UnitBase[] items)
        {
            foreach (var item in items)
            {
                try
                {
                    await item.Complete();
                }
                catch (Exception ex)
                {
                    if (!ex.IsCanceled())
                        throw;
                }
            }
        }
    }
}
