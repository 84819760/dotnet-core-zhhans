using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DotNetCorezhHans.Base;
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
            .Version.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
            var args = e.Args ?? Array.Empty<string>();
            Share.Show("DotNetCorezhHansMain", e.Args);
            ShowUpdate(args);
            if (IsAdmin) UacHelper.RunAdmin();
            InfoDataTask = Task.Run(() => InfoData.GetInfoData(Config.UpdateUrl));
            base.OnStartup(e);
        }

        private static void ShowUpdate(string[] args)
        {
            if (args.Any(x => x == "--update-ok"))
            {
                MessageBoxShow("更新完成!");
                return;
            }
            else if (!args.Any(x => x == "--run"))
            {
                MessageBoxShow("启动参数异常,无法启动。");
                Environment.Exit(0);
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
