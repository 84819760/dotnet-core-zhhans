using System;
using System.Windows;

namespace DotNetCore_zhHans.Boot.Execs
{
    partial class ExecUpdateFile : ExecBase
    {
        public ExecUpdateFile(ViewModel viewModel) : base(viewModel) { }

        public override async void Run()
        {
            vm.Title = $"移动文件 {App.Version}";
            vm.Details = "获取更新配置";
            vm.Context = "移动文件";
            vm.IsIndeterminate = true;
            try
            {
                var items = await GetJsonFileInfos();
                items.Select((x, i) => (fileInfo: x, index: i)).ToList()
                    .ForEach(async x => await Move(x.fileInfo, x.index, items.Length));
                Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "更新失败");
                DeleteMain();
            }
        }

        private async Task Move(FileInfo fileInfo, int index, int length)
        {
            vm.Progress = (double)index / length;
            vm.Details = fileInfo.SourceName;
            Exception? exception = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                     Move(fileInfo);
                    exception = null;
                }
                catch (Exception ex)
                {
                    await Task.Delay(3000);
                    exception = ex;
                }
            }
            if (exception is null) return;
            throw exception;
        }

        private void Move(FileInfo fileInfo)
        {
            var file = fileInfo.SourceName;
            var sourceFile = Path.Combine(CurrentDirectory, file);
            var targetFile = GetTargetDirectory(file);
            if (fileInfo.Index == 0) File.Move(sourceFile, targetFile, true);
            else File.Copy(sourceFile, targetFile, true);
        }

        private void DeleteMain()
        {
            try
            {
                var pDir = Directory.GetParent(CurrentDirectory)!.FullName;
                var target = Path.Combine(pDir, "DotNetCorezhHansMain.exe");
                if (File.Exists(target)) File.Delete(target);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        private string GetTargetDirectory(string fileName)
        {
            var pDir = Directory.GetParent(CurrentDirectory)!.FullName;
            if (fileName == Share.RootExe) pDir = Directory.GetParent(pDir)!.FullName;
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
