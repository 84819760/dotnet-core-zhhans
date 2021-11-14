using System;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    public class UserFunctItemViewModel : ViewModelBase<UserFunctItemViewModel>
    {
        private readonly PageStateMessage pageState = PageStateMessage.Instance;

        public string Title { get; set; } = "标题";

        public string Subtitle { get; set; } = "副标题";

        public string IconKind { get; set; } = "AlertDecagram";

        public string Target { get; set; }

        public void Call()
        {
            var pageControlType = Enum.Parse<PageControlType>(Target);
            pageState.Publish(pageControlType);
        }
    }
}
