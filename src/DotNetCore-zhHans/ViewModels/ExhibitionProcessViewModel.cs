using System.ComponentModel;
using System.Windows;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Messages;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.ViewModels
{
    using ProcessMessage = Message<ExhibitionProcessViewModel>;

    /// <summary>
    /// 执行过程
    /// </summary>
    public class ExhibitionProcessViewModel : ViewModelBase<ExhibitionProcessViewModel>
        , IMasterProgress
    {
        private static readonly TaskbarItemInfoProgress progress = TaskbarItemInfoProgress.Instance;
        private readonly PageStateMessage pageStateMsg = PageStateMessage.Instance;
        private readonly ProcessMessage processMsg = ProcessMessage.Instance;

        public ExhibitionProcessViewModel()
        {
            processMsg.Subscribe(ProcessMsgHandler);
            pageStateMsg.Subscribe(PageStateHandler);
            InitDelayClear();
            PropertyChanged += PropEventHandler;
        }

        private void PropEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not "ProgressValue") return;
            progress.Publish((double)ProgressValue / 100);
        }

        private void InitDelayClear()
        {
            _ = new DelayClear()
            {
                GetValue = () => RequestStatus,
                SetValue = x => RequestStatus = "请求 : 0   "
            };

            _ = new DelayClear()
            {
                GetValue = () => ResponseStatus,
                SetValue = x => ResponseStatus = "响应 : 0   "
            };
        }

        public int ProgressValue { get; set; } = 50;
        public int FileProgressValue { get; set; } = 50;
        public string ProgressText { get; set; } = "10000";
        public string Title { get; set; } = "更新 : 1024";
        public string RequestStatus { get; set; } = "请求:9527";
        public string ResponseStatus { get; set; } = "响应:9527";
        public string Title2 { get; set; } = "缓存 : 0";
        public string Title3 { get; set; } = "等待 : 0";
        public string AppendInformationDescribe { get; set; } = "附加消息";
        public Visibility CacheAndUpdateXmlVisibility { get; set; } = Visibility.Visible;
        public Visibility AppendInformationVisibility { get; set; } = Visibility.Visible;
        public Visibility TranslState { get; set; } = Visibility.Hidden;

        private void PageStateHandler(PageControlType obj)
        {
            FileProgressValue = ProgressValue = 0;
            switch (obj)
            {
                case PageControlType.Setup:
                case PageControlType.Surprised:
                case PageControlType.EndTask:
                case PageControlType.AbnormalList:                  
                    return;
            }         
            ProgressText = "--";
            AppendInformationDescribe =
            RequestStatus = 
            ResponseStatus = 
            Title = "";
        }

        private void ProcessMsgHandler(MessageData<ExhibitionProcessViewModel> obj)
        {
            if (obj.TrySet(this) || obj.Target is not "RequestInstance") return;
            processMsg.Publish(new()
            {
                Target = "ResponseInstance",
                Data = this
            });
        }

        public void SetDisplayState(bool value) => TranslState = value
            ? Visibility.Visible
            : Visibility.Hidden;
    }
}