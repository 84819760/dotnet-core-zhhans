using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCorezhHans.Messages;

namespace DotNetCorezhHans.ViewModels
{
    public class NavigationItamDonationViewModel : PrimaryAndSecondaryViewModel
    {
        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public NavigationItamDonationViewModel() => pageState.Subscribe(PageStateHandler);

        protected void PageStateHandler(PageControlType obj)
        {
            switch (obj)
            {
                case PageControlType.Default:
                    ShowPrimary();
                    break;
                case PageControlType.Surprised:
                    ShowSecondary();
                    break;
            }
        }


    }
}
