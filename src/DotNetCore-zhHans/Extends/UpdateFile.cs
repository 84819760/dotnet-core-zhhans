using System;
using System.Diagnostics;
using System.IO;
using DotNetCorezhHans.Base;

namespace DotNetCorezhHans.Extends;

class UpdateFile
{
    public static void Run()
    {
        var rootPath = ConfigManagerBuilder.RootConfigFilePath;
        var dir = Path.GetDirectoryName(rootPath);
        var exe = Path.Combine(dir, "DotNetCoreZhHans.exe");
        Process.Start(exe, $"--update-file-check {rootPath}");
        Environment.Exit(0);
    }
}
