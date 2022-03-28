using System.Windows;

namespace DotNetCore_zhHans.Boot.Execs
{
    partial class ExecUpdateFile : ExecBase
    {
        private const string exe = "DotNetCoreZhHans.exe";
        public ExecUpdateFile(ViewModel viewModel) : base(viewModel)
        {
        }

        public override async void Run()
        {
            vm.Title = "移动文件";
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
                      .ToList().ForEach(Move);
                Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Move((string file, int index, int count) v)
        {
            var (file, index, count) = v;
            vm.Progress = (double)index / count;
            vm.Details = Path.GetFileName(file);
            var pdir = Directory.GetParent(CurrentDirectory)!.FullName;

            if (Path.GetFileName(file) == exe)
            {
                pdir = Directory.GetParent(pdir)!.FullName;
                var target = Path.Combine(pdir, exe);
                File.Copy(file, target, true);
            }
            else
            {
                var target = Path.Combine(pdir, exe);
                File.Move(file, target, true);
            }
        }

        private void Start()
        {
            var dir = CurrentDirectory;
            if (CurrentDirectory.Contains("_download"))
            {
                dir = Directory.GetParent(dir)!.FullName;
            }

            if (CurrentDirectory.Contains("lib"))
            {
                dir = Directory.GetParent(dir)!.FullName;
            }
            Environment.CurrentDirectory = dir;
            Process.Start(Path.Combine(dir, exe),"--updateOk");
            Environment.Exit(0);
        }
    }
}
