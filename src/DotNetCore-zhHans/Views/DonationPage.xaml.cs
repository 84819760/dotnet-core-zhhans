//using System.Windows;
//using System.Windows.Controls;
//using DotNetCorezhHans.Messages;

//namespace DotNetCorezhHans.Views
//{
//    /// <summary>
//    /// Donation.xaml 的交互逻辑
//    /// </summary>
//    public partial class DonationPage : UserControl
//    {
//        private readonly NotificationMessage message = NotificationMessage.Instance;
//        public DonationPage()
//        {
//            InitializeComponent();
//            message.Subscribe(SetPleasantly, x => x is Notification.ToPleasantlyPage);
//            message.Subscribe(SetDefault, x => x is Notification.ToDefault);
//        }

//        private void SetDefault(Notification _)
//        {
//            Cover.Visibility = Visibility.Visible;
//            ContentTextBlock.Visibility = Visibility.Collapsed;
//        }

//        private void SetPleasantly(Notification _)
//        {
//            ContentTextBlock.Visibility = Visibility.Visible;
//            Cover.Visibility = Visibility.Collapsed;
//        }
//    }
//}
