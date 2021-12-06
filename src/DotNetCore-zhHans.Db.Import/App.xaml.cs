using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            if (e.Args.Length > 2)
            {
                Source = e.Args[0];
                Target = e.Args[1];
                Caller = e.Args[2];
            }
            base.OnStartup(e);
        }

        public static string Source { get; private set; } = @"E:\tmp\TranslData.db";
        public static string Target { get; private set; } = @"E:\tmp\TranslData2.db";
        public static string Caller { get; private set; } = "cmd";
    }
}
