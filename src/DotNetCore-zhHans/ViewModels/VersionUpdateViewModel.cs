using DotNetCorezhHans.Base;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    internal class VersionUpdateViewModel : ViewModelBase<VersionUpdateViewModel>
    {
        private readonly PageStateMessage pageState = PageStateMessage.Instance;
        private readonly VersionUpdateDataProvider dataProvider = new();
        private VersionUpdateProvider updateProvider;


        public VersionUpdateViewModel()
        {
            pageState.Subscribe(VersionUpdateHandler, x => x is PageControlType.Surprised);
            Data = new() { Information = "加载中...请稍后！" };
        }

        public InfoData Data { get; set; }

        public string ButtonContent { get; set; } = "加载中";

        public int Progress { get; set; }

        private async void VersionUpdateHandler(PageControlType _)
        {
            Data = await dataProvider.GetInfoData();
            if (dataProvider.NeedUpdate)
            {
                ButtonContent = $"检测到更新:{ Data.Version}";
                GetVersionUpdateProvider();
            }
            else
            {
                ButtonContent = $"无更新！";
            }
        }

        private VersionUpdateProvider GetVersionUpdateProvider()
        {
            return updateProvider ??= new()
            {
                FirstRun = FirstRun,
                SetProgress = SetProgress,
                SetTitle = SetTitle,
            };
        }


        public async void CallMethod()
        {
            await updateProvider?.DownloadFile(Data.UpdateUrl);
        }

        private void FirstRun() => Progress = 0;


        private void SetProgress(int value)
        {
            Progress = value;
            ButtonContent = $"下载中({value}%)";
        }

        private void SetTitle(string value) => ButtonContent = value;
    }
}
