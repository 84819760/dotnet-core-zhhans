using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetCore_zhHans.Boot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {                      
            Args = e.Args ?? Array.Empty<string>();
            Share.Show("Boot", Args);
        }
    }
}
