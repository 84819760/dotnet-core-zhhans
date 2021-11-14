using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NearExtend.WpfPrism;
using Prism.Services.Dialogs;

namespace DotNetCorezhHans.ViewModels
{
    internal class ErrorListViewModel : ViewModelBase<ErrorListViewModel>, IDialogAware
    {
        public string[] Items { get; private set; }

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Items = parameters
                .GetValue<IEnumerable<Exception>>("")
                .Select(x => x.ToString())
                .ToArray();
        }

        public string Title { get; } = "";

        public event Action<IDialogResult> RequestClose;

        public void CallMethod()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
