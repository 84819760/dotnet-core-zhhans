using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCoreZhHans.Service
{
    internal interface ITransmitsExtend : ITransmitData
    {
        public void Set(Action action);

        //internal IDbSingleWrite DbSingleWrite { get; }

    }

    internal static class TransmitsExtend
    {
        public static void Set(this ITransmitData transmits, Action action) =>
            (transmits as ITransmitsExtend).Set(action);

        //public static IDbSingleWrite GetDbSingle(this ITransmitData transmits) =>
        //    (transmits as ITransmitsExtend).DbSingleWrite;
    }
}
