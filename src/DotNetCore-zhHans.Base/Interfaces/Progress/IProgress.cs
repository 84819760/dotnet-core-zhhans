using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base.Interfaces
{
    public interface IProgress
    {
        IFilesProgress Files { get; }

        IMasterProgress Master { get; }
    }
}
