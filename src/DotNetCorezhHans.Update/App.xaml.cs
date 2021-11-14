using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetCorezhHans.Update
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args;

        protected override void OnStartup(StartupEventArgs e)
        {
            Args = e.Args;
            if (Args.Length > 1)
            {               
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("参数不正确");
                Environment.Exit(0);
            }
        }
    }
}
