using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace DotNetCorezhHans
{
    internal class UacHelper
    {
        private static readonly string target = GetExePath();
        public static void RunAdmin()
        {
            if (IsAdmin) return;
            var startInfo = new ProcessStartInfo()
            {
                FileName = GetRunAdmin(),
                Arguments = Application.ExecutablePath,
                UseShellExecute = true,
                Verb = "runas",
            };
            Process.Start(startInfo);
            Environment.Exit(0);
        }

        public static bool IsAdmin
        {
            get
            {
                var currentId = WindowsIdentity.GetCurrent();
                var winPrincipal = new WindowsPrincipal(currentId);
                return winPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static string GetRunAdmin() => new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(),"lib",target),
            Path.Combine(Directory.GetCurrentDirectory(),target),
         }.First(Exists);

        private static bool Exists(string filePath) => File.Exists(filePath);

        private static string GetExePath()
        {
            var target = typeof(UacHelper).Assembly.Location;
            var dir = Path.GetDirectoryName(target);
            var fileName = Path.GetFileNameWithoutExtension(target);
            return Path.Combine(dir, $"{fileName}.exe");
        }
    }
}
