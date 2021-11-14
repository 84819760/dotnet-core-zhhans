using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base.Interfaces
{
    public interface ITranslaResults
    {
        string Original { get; }

        string Transl { get; }

        string Source { get; }
    }
}
