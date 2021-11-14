using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace DotNetCore_zhHans.Boot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //InitializeComponent();
            var target = Path.Combine(Directory.GetCurrentDirectory(),"lib", "DotNetCorezhHans.exe");
            Process.Start(target);
            Environment.Exit(0);
        }
    }
}
