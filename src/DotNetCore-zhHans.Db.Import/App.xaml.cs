using System.IO;
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
            if (e.Args.Length > 0)
            {
                Source = e.Args[0];
            }
            base.OnStartup(e);
        }

        public static string Source { get; private set; } 
            = @"D:\tmp\TranslData2.db";

        public static string Target { get; private set; } 
            = Path.Combine(GetDirectory(), "TranslData.db");

        private static string GetDirectory() => 
            Path.GetDirectoryName(typeof(App).Assembly.Location)!;
    }
}
