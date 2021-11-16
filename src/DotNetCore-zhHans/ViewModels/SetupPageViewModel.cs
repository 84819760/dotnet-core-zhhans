using System.ComponentModel;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    public class SetupPageViewModel : ViewModelBase<SetupPageViewModel>
    {
        private readonly PageStateMessage pageState = PageStateMessage.Instance;
        private readonly ConfigManager config;

        public SetupPageViewModel()
        {
            if (IsDesignMode) return;
            config = App.GetConfigManager();
            SetEvent();
        }

        private void SetEvent()
        {
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
    }
}
