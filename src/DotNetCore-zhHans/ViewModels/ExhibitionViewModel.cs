using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Messages;
using DotNetCorezhHans.TranslTasks;
using MaterialDesignThemes.Wpf;
using NearCoreExtensions;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    using ProcessMessage = Message<ExhibitionProcessViewModel>;
    using FileListPageMsg = Message<FileListPageViewModel>;

    public class ExhibitionViewModel : ViewModelBase<ExhibitionViewModel>, IProgress
    {
        private readonly ExecButtonStateMessage execButtonState = ExecButtonStateMessage.Instance;
        private readonly FileListPageMsg fileListMsg = FileListPageMsg.Instance;
        private readonly ProcessMessage processMsg = ProcessMessage.Instance;
        private ExhibitionProcessViewModel exhibitionProcess;
        private FileListPageViewModel fileListPageViewModel;
        private const string ri = "ResponseInstance";
        private TranslManager translManager;

        public ExhibitionViewModel()
        {
            execButtonState.Subscribe(SetButtonState);
            fileListMsg.Subscribe(x => fileListPageViewModel = x.Data, x => x.Target is ri);
            processMsg.Subscribe(x => exhibitionProcess = x.Data, x => x.Target is ri);
        }

        public string Title { get; set; } = "DotNet Core Nuget";

        public string Subtitle { get; set; } = "Xml Document 汉化工具\r\n重庆 New +___+";

        public string Version { get; set; } = App.Version;

        public PackIconKind PackIconValue { get; set; } = PackIconKind.Play;

        public bool IsIndicator { get; set; }

        public int ProgressValue { get; set; }

        public bool IsEnabled { get; set; } = true;

        public IFilesProgress Files => fileListPageViewModel;

        public IMasterProgress Master => exhibitionProcess;

        public void ClickHandler()
        {
            if (TestApis()) return;
            GetTranslManager().Run?.Invoke();
        }

        private TranslManager GetTranslManager()
        {
            if (translManager is null || translManager.IsEnd)
            {
                translManager = new(this);
            }
            return translManager;
        }

        private void SetButtonState(ExecButtonStateData data) => App.Invoke(() =>
        {
            switch (data.Value)
            {
                case ExecButtonState.Default:
                    SetValue(PackIconKind.Play, false, 0);
                    break;

                case ExecButtonState.Run:
                    SetValue(PackIconKind.Stop, true, 30);
                    break;

                case ExecButtonState.Lock:
                    SetValue(PackIconKind.LockReset, true, 30);
                    break;
            }
            data.SetComplete();
        });

        private static bool TestApis()
        {
            var res = App.GetConfigManager().ApiConfigs.Count(x => x.Enable) is 0;
            if (res) MessageBox.Show("至少配置并且开启一个翻译API");
            return res;
        }

        private void SetValue(PackIconKind iconKind, bool isIndicator, int progressValue)
        {
            PackIconValue = iconKind;
            IsIndicator = isIndicator;
            ProgressValue = progressValue;
        }
    }
}