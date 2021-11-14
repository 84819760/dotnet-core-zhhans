using System;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using System.Windows;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.ViewModels;
using DotNetCorezhHans.Views;
using Prism.Ioc;
using System.IO;
using System.Threading.Tasks;

namespace DotNetCorezhHans
{
    public partial class App
    {
        public readonly static ConfigManager Config = ConfigManager.Instance;
        public static bool IsAdmin { get; set; } = Config.IsAdmin;

        public static string Version => Assembly
            .GetExecutingAssembly().GetName()
            .Version.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
            ShowUpdate(e.Args?.FirstOrDefault());
            if (IsAdmin) UacHelper.RunAdmin();
            base.OnStartup(e);
        }

        private static void ShowUpdate(string v)
        {
            if (v is "update") MessageBoxShow("更新完成!");
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
