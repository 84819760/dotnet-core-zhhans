using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base.Interfaces
{
    public interface IFilePath
    {
        public string Path { get; }
    }

    public class FilePath : IFilePath
    {
        public string Path { get; init; }
    }
}
