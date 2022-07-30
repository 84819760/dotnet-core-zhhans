using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Extends;
using DotNetCorezhHans.ViewModels;
using DotNetCorezhHans.Views;
using Prism.Ioc;

namespace DotNetCorezhHans
{
    public partial class App
    {
        private static string version;

        public static ConfigManager Config { get; private set; }

        public static bool IsAdmin { get; set; }

        public static Task<InfoData> InfoDataTask { get; private set; }

        public static string Version => version ??= Assembly
            .GetExecutingAssembly().GetName()
            .Version.ToString(3);

        protected override void OnStartup(StartupEventArgs e)
        {
            DotNetCoreZhHansFileMove();
            DbTest();
            var args = e.Args ?? Array.Empty<string>();
            Share.Show("DotNetCorezhHansMain", e.Args);
            ShowUpdate(args);
            if (IsAdmin) UacHelper.RunAdmin();
            InfoDataTask = Task.Run(() => InfoData.GetInfoData(Config.UpdateUrl));
            base.OnStartup(e);
        }

        private static void DotNetCoreZhHansFileMove()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            new[]
            {
                Path.Combine(currentDirectory, "lib"),
                Path.Combine(currentDirectory, "lib", "_download"),
            }.ToList()
            .ForEach(async f => await Move(Path.Combine(f, "DotNetCoreZhHans.exe"), currentDirectory));

        }

        private static async Task Move(string source, string dir)
        {
            if (!File.Exists(source)) return;
            var target = Path.Combine(dir, "DotNetCoreZhHans.exe");
            Exception error = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {

                    File.Move(source, target, true);
                    return;
                }
                catch (Exception ex)
                {
                    await Task.Delay(1000);
                    error = ex;
                }
            }
            if (error is null) return;
            throw error;
        }

        private static void DbTest()
        {
            var target = Path.Combine(Directory.GetCurrentDirectory(), "lib", "TranslData.db");
            if (File.Exists(target)) return;
            //UpdateFile.Run();
            MessageBoxShow($"找不到数据库文件:{target}");
        }

        private static void ShowUpdate(string[] args)
        {
            if (args.Any(x => x == "--update-ok"))
            {
                MessageBoxShow("更新完成!");
                return;
            }
            try
            {
                Config = ConfigManager.Instance;
                IsAdmin = Config.IsAdmin;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ConfigManager>(x => ConfigManager.Instance);
            containerRegistry.RegisterDialog<ApiDetails, ApiDetailsViewModel>();
            containerRegistry.RegisterDialog<ErrorList, ErrorListViewModel>();
        }

        internal static ConfigManager GetConfigManager() => ContainerLocator
         .Container?.Resolve<ConfigManager>();

        internal static void Invoke(Action action) =>
            Current?.Dispatcher.Invoke(action);

        internal static void BeginInvoke(Action action) =>
            Current?.Dispatcher.BeginInvoke(action);

        public static void MessageBoxShow(string value) =>
            Invoke(() => MessageBox.Show(value));
    }
}
