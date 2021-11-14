using System;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;
using Prism.Regions;

namespace DotNetCorezhHans.ViewModels
{
    public class UserFunctionListViewModel : PrimaryAndSecondaryViewModel
    {
        private readonly  PageStateMessage pageState = PageStateMessage.Instance;

        public UserFunctionListViewModel()
        {
            pageState.Subscribe(PageStateHandler);
        }

        private void PageStateHandler(PageControlType obj)
        {
            switch (obj)
            {
                case PageControlType.EndTask:
                case PageControlType.Default:
                    ShowPrimary();
                    break;
                case PageControlType.Translate:                  
                case PageControlType.AbnormalList:
                    ShowSecondary();
                    break;
                case PageControlType.TerminationTask:
                    ShowPrimary();
                    break;
            }
        }
    }
}