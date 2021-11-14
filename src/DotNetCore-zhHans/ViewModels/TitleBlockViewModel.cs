using System;
using System.Windows.Input;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;
using Prism.Commands;

namespace DotNetCorezhHans.ViewModels
{
    public class TitleBlockViewModel : ViewModelBase<TitleBlockViewModel>
    {
        private readonly WindwsStateMessage winMessage = WindwsStateMessage.Instance;
        private readonly TitleBlockMessage titleBlock = TitleBlockMessage.Instance;
        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public TitleBlockViewModel()
        {
            CmdDragMove = new(DragMove);
            CmdMinimize = new(Minimize);
            CmdClose = new(Close);
            EventHandler();
            pageState.Subscribe(PageStateHandler);
        }

        private void EventHandler()
        {
            titleBlock.Subscribe(x => Title = x);
            CmdReduction = new DelegateCommand(() =>
            {
                pageState.Publish(PageControlType.Default);
                Title = "";
            });
        }

        public DelegateCommand<MouseEventArgs> CmdDragMove { get; set; }

        public DelegateCommand CmdMinimize { get; set; }

        public DelegateCommand CmdClose { get; set; }

        public DelegateCommand CmdReduction { get; set; }

        public string Title { get; set; }

        private void DragMove(MouseEventArgs e)
        {
            if (e.LeftButton is MouseButtonState.Pressed)
                winMessage.Publish(WindwsState.WindowDragMove);
        }

        private void Minimize() => winMessage.Publish(WindwsState.WindowMinimize);

        private void Close() => winMessage.Publish(WindwsState.WindowClose);

        private void PageStateHandler(PageControlType obj)
        {
            Title = obj switch
            {
                PageControlType.Setup => "配置",
                PageControlType.AbnormalList => "异常",
                _ => "",
            };
        }
    }
}