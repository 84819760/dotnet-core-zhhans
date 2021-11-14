using System;
using System.Windows.Controls;
using DotNetCorezhHans.Messages;
using DotNetCorezhHans.ViewModels;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    public partial class Exhibition : UserControl
    {
        private readonly ExecButtonStateMessage execButtonState = ExecButtonStateMessage.Instance;
        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public Exhibition()
        {
            InitializeComponent();
            this.SetDesignData<ExhibitionViewModel>();
            execButtonState.Subscribe(SetStory);
            pageState.Subscribe(PageStateHandler);
        }

        private void SetStory(ExecButtonStateData data)
        {
            switch (data.Value)
            {
                case ExecButtonState.Default:
                    this.CallStory("TranslateEnd");
                    break;
                case ExecButtonState.Run:
                    this.CallStory("TranslateStart");
                    break;
                case ExecButtonState.Lock:
                    break;
            }
            TextBox1.Focus();
        }

        private void PageStateHandler(PageControlType obj)
        {
            switch (obj)
            {
                case PageControlType.Default:
                    this.CallStory("Default");
                    break;
                case PageControlType.Setup:
                case PageControlType.Surprised:
                    this.CallStory("SubPage");
                    break;
            }
        }
    }
}