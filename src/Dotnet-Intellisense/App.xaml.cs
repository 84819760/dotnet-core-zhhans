using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dotnet_Intellisense
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string json = "Dotnet-Intellisense.json";
        protected override void OnStartup(StartupEventArgs e)
        {
            DataBuilder.InitJson(json);           
            base.OnStartup(e);
        }

    }
}
