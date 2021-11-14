using System;
using System.Windows.Controls;
using DotNetCorezhHans.Messages;
using DotNetCorezhHans.ViewModels;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    /// <summary>
    /// UserFunction.xaml 的交互逻辑
    /// </summary>
    public partial class UserFunctionList : UserControl
    {
        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public UserFunctionList()
        {
            InitializeComponent();
            this.SetDesignData<UserFunctionListViewModel>();
            pageState.Subscribe(PageStateHandler);
        }

        private void PageStateHandler(PageControlType pageControlType)
        {
            switch (pageControlType)
            {
                case PageControlType.Default:
                case PageControlType.Setup:
                case PageControlType.Surprised:
                case PageControlType.Translate:        
                    this.CallStory(pageControlType.ToString());
                    break;
            }
        }
    }
}