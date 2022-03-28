using System;
using System.Windows;

namespace DotNetCore_zhHans.Boot.Execs
{
    partial class ExecUpdateFile : ExecBase
    {

        public ExecUpdateFile(ViewModel viewModel) : base(viewModel)
        {
        }

        public override async void Run()
        {
            vm.Title = $"移动文件 {App.Version}";
            vm.Details = "获取更新配置";
            vm.Context = "移动文件";
            vm.IsIndeterminate = true;
            try
            {
                var hashs = (await GetJsonFileInfos())
                    .Select(x => x.SourceName)
                    .ToHashSet();

                var files = Directory.GetFiles(CurrentDirectory)
                        .Where(x => hashs.Contains(Path.GetFileName(x)))
                        .ToList();

                files.Select((x, i) => (file: x, index: i, count: files.Count))
                          .ToList().ForEach(async x => await Move(x));

                Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "更新失败");
                var dir = GetTargetDirectory("c:\\DotNetCorezhHansMain.exe", false);
                var target = Path.Combine(dir, "DotNetCorezhHansMain.exe");
                File.Delete(target);
            }
        }

        private async Task Move((string file, int index, int count) v)
        {
            var (file, index, count) = v;
            Exception? exception = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Move(file, index, count);
                    exception = null;
                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(3000);
                    exception = ex;
                }
            }
            if (exception is not null)
            {
                throw exception;
            }
        }

        private void Move(string file, int index, int count)
        {
            vm.Progress = (double)index / count;
            vm.Details = Path.GetFileName(file);
            var isRootExe = IsRootExe(file);
            var target = Path.Combine(GetTargetDirectory(file, isRootExe), vm.Details);

            if (isRootExe) File.Copy(file, target, true);
            else File.Move(file, target, true);
        }

        private static bool IsRootExe(string file) => Path.GetFileName(file) == Share.RootExe;

        private string GetTargetDirectory(string file, bool isRootExe)
        {
            var pDir = Directory.GetParent(CurrentDirectory)!.FullName;
            if (isRootExe) pDir = Directory.GetParent(pDir)!.FullName;
            return pDir;
        }

        public static void Start()
        {
            var dir = Share.GetRootDirectory();
            Process.Start(Path.Combine(dir, Share.RootExe), "--updateOk");
            Environment.Exit(0);
        }
    }
}
