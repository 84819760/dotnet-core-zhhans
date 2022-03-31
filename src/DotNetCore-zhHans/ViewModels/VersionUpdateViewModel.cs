using System;
using System.IO;
using System.Threading;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Extends;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    internal class VersionUpdateViewModel : ViewModelBase<VersionUpdateViewModel>
    {
        //private VersionUpdateProvider updateProvider;
        //private Action action = null;

        public VersionUpdateViewModel()
        {
            Data = new() { Information = "加载中...请稍后！" };
            if (IsDesignMode) return; 
            SetData();
        }

        private async void SetData()
        {
            Data = await App.InfoDataTask;
            ButtonContent = $"无更新！";
            if (Data.TestVersion(App.Version)) return;
            ButtonContent = $"检测到更新:{ Data.Version}";          
        }

        public InfoData Data { get; set; }

        public string ButtonContent { get; set; } = "加载中";

        public int Progress { get; set; }

        //private VersionUpdateProvider GetVersionUpdateProvider() => updateProvider ??= new()
        //{
        //    FirstRun = () => Progress = 0,
        //    SetProgress = SetProgress,
        //    SetTitle = SetTitle,
        //};

        public void CallMethod() => UpdateFile.Run();


        //private void SetProgress(int value)
        //{
        //    Progress = value;
        //    ButtonContent = $"下载中({value}%)";
        //}

        //private void SetTitle(string value) => ButtonContent = value;
    }
}
