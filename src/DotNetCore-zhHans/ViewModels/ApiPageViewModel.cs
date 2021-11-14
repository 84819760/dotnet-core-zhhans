using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;
using Prism.Services.Dialogs;
using NearCoreExtensions;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetCorezhHans.ViewModels
{
    public class ApiPageViewModel : ViewModelBase<ApiPageViewModel>
    {
        /// <summary>
        /// target ,source
        /// </summary>
        private static Action<ApiConfig, ApiConfig> apiConfigUpdate;
        private readonly IDialogService dialog;

        public ApiPageViewModel(IDialogService dialog) => this.dialog = dialog;

        public ObservableCollection<ApiConfig> Datas { get; set; }

        public void ToggleButtonClick()
        {

        }

        public void Edit(object item)
        {
            var param = new DialogParameters() { { "", item } };
            dialog.ShowDialog("ApiDetails", param, CallBack);
        }

        private void CallBack(IDialogResult dr)
        {
            if (dr.Result != ButtonResult.OK) return;
            var data = dr.Parameters.GetValue<ApiConfig>("");
            CallBack(data);
        }

        private void CallBack(ApiConfig data)
        {
            var item = Datas.First(x => x.Id == data.Id);
            GetAction()(item, data);
        }

        private static Action<ApiConfig, ApiConfig> GetAction() => 
            apiConfigUpdate ??= CreateAction();

        private static Action<ApiConfig, ApiConfig> CreateAction() => new PropertyAssignBuilder<ApiConfig>()
            .Ignore(x => new { x.Name, x.Id }).CreateAction();
    }
}