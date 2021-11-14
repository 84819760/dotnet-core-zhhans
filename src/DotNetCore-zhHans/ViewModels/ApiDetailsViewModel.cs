using System;
using System.ComponentModel;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;
using Prism.Events;
using Prism.Services.Dialogs;


namespace DotNetCorezhHans.ViewModels
{
    public class ApiDetailsViewModel : ViewModelBase<ApiDetailsViewModel>, IDialogAware
    {
        public ApiConfig ApiConfig { get; set; }

        public string Title => "API参数";

        public bool ButtonState { get; set; }

        private void SetChangedHandler(INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (s, e) => ButtonState = true;
        }

        public void Save()
        {
            ButtonState = false;
            var param = new DialogParameters() { { "", ApiConfig } };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, param));
        }

        public void Exit() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var p = parameters.GetValue<ApiConfig>("");
            ApiConfig = p.Clone();
            SetChangedHandler(ApiConfig);
        }

        public event Action<IDialogResult> RequestClose;
    }
}