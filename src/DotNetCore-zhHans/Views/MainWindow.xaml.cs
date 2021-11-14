using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    public partial class MainWindow : Window
    {
        private readonly WindwsStateMessage windwsState = WindwsStateMessage.Instance;
        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            windwsState.Subscribe(SetWindwsState);
            pageState.Subscribe(PageStateHandler);
            InitConfig(serviceProvider);
        }


        private static void InitConfig(IServiceProvider serviceProvider)
        {
            Task.Run(() => serviceProvider.GetService(typeof(ConfigManager)));
        }

        internal void SetWindwsState(WindwsState windwsState)
        {
            switch (windwsState)
            {
                case WindwsState.WindowDragMove:
                    DragMove();
                    break;
                case WindwsState.WindowMinimize:
                    WindowState = WindowState.Minimized;
                    break;
                case WindwsState.WindowClose:
                    Close();
                    break;
                default: break;
            }
        }

        private void PageStateHandler(PageControlType pageControlType)
        {
            var call = pageControlType switch
            {
                PageControlType.TerminationTask or PageControlType.EndTask => default,
                PageControlType.Default or PageControlType.Translate => pageControlType.ToString(),
                _ => "SubPage",
            };
            this.CallStory(call);
        }
    }
}