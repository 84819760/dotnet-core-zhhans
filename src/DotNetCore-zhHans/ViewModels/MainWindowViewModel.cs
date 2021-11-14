using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    public class MainWindowViewModel : ViewModelBase<MainWindowViewModel>
    {
        private static readonly TaskbarItemInfoProgress progress = TaskbarItemInfoProgress.Instance;

        public MainWindowViewModel() => progress.Subscribe(x => WindowsProgress = x);

        public string Title { get; set; } = "Nuget汉化工具";

        public double WindowsProgress { get; set; }
    }
}
