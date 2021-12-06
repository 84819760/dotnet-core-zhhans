using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    public class SetupPageViewModel : ViewModelBase<SetupPageViewModel>
    {
        private readonly PageStateMessage pageState = PageStateMessage.Instance;
        private readonly WindwsStateMessage windwsState = WindwsStateMessage.Instance;
        private readonly Microsoft.Win32.OpenFileDialog openFileDialog = new()
        {
            Filter = "Sqlite文件(*.db)|*.db",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        };
        private readonly ConfigManager config;

        public SetupPageViewModel()
        {
            if (IsDesignMode) return;
            config = App.GetConfigManager();
            SetEvent();
        }

        private void SetEvent()
        {
            openFileDialog.FileOk += OpenFileDialog_FileOk;
            config.PropertyChanged += SetButtonState;
            config.ApiConfigs.CollectionChanged += SetButtonState;
            config.Directorys.CollectionChanged += SetButtonState;
            foreach (var item in config.ApiConfigs)
            {
                item.PropertyChanged += SetButtonState;
            }
        }

        public bool ButtonState { get; set; }

        private void SetButtonState(object sender, object e) => ButtonState = true;

        public void Save()
        {
            config.Save();
            ButtonState = false;
            pageState.Publish(PageControlType.Default);
            if (!App.IsAdmin && config.IsAdmin)
            {
                App.IsAdmin = true;
                UacHelper.RunAdmin();
            }
        }

        public void ImportData() => openFileDialog.ShowDialog();

        private string GetDirectory() => Path.GetDirectoryName(GetType().Assembly.Location);

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            windwsState.Publish(WindwsState.WindowMinimize);
            pageState.Publish(PageControlType.Default);
            var source = openFileDialog.FileName;
            Process.Start(GetImportExe(), source);
        }

        private string GetImportExe() => Path.Combine(GetDirectory(), "DotNetCore-zhHans.Db.Import.exe");
       
    }
}
