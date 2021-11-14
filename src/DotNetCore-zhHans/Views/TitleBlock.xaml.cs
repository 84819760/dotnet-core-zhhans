using System.Windows.Controls;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    /// <summary>
    /// TitleBlock.xaml 的交互逻辑
    /// </summary>
    public partial class TitleBlock : UserControl
    {

        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public TitleBlock()
        {
            InitializeComponent();
            pageState.Subscribe(PageStateHandler);
        }

        private void PageStateHandler(PageControlType pageControlType)
        {
            var call = pageControlType switch
            {
                PageControlType.Translate or PageControlType.TerminationTask => default,
                PageControlType.Default or PageControlType.EndTask => "Default",
                _ => "SubPage",
            };
            this.CallStory(call);
        }
    }
}
