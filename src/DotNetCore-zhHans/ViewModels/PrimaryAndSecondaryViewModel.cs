using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    /// <summary>
    /// 负责导航和内容之间的切换
    /// </summary>
    public class PrimaryAndSecondaryViewModel : ViewModelBase<PrimaryAndSecondaryViewModel>
    {
        public PrimaryAndSecondaryViewModel() { }

        public Visibility PrimaryVisibility { get; set; } = Visibility.Visible;

        public Visibility SecondaryVisibility { get; set; } = Visibility.Collapsed;


        /// <summary>
        /// 显示主要内容
        /// </summary>
        public void ShowPrimary()
        {
            PrimaryVisibility = Visibility.Visible;
            SecondaryVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示次要内容
        /// </summary>
        public void ShowSecondary()
        {
            PrimaryVisibility = Visibility.Collapsed;
            SecondaryVisibility = Visibility.Visible;
        }      
    }
}
