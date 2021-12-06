using System.Windows;

namespace DotNetCore_zhHans.Db.Import
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 1)
            {
                Source = e.Args[0];
                Target = e.Args[1];
            }
            base.OnStartup(e);
        }

        public static string Source { get; private set; } = @"D:\tmp\TranslData.db";
        public static string Target { get; private set; } = @"D:\tmp\TranslData2.db";
    }
}
