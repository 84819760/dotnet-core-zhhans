using System.Windows;
using System.Windows.Controls;
using DotNetCorezhHans.ViewModels;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    /// <summary>
    /// ListItem.xaml 的交互逻辑
    /// </summary>
    public partial class UserFunctItem : UserControl
    {
        public UserFunctItem()
        {
            InitializeComponent();
            this.SetDesignData<UserFunctItemViewModel>();
        }


        #region DependencyProperty:Title 
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = "Title"
            .RegisterProperty<string, UserFunctItem>(callBack: SetTitle);

        private static void SetTitle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<UserFunctItemViewModel>(vm => vm.Title = (string)e.NewValue);
        }
        #endregion

        #region DependencyProperty:Subtitle 
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public static readonly DependencyProperty SubtitleProperty = "Subtitle"
            .RegisterProperty<string, UserFunctItem>(callBack: SetSubtitle);

        private static void SetSubtitle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<UserFunctItemViewModel>(vm => vm.Subtitle = (string)e.NewValue);
        }
        #endregion

        #region DependencyProperty:IconKind 
        public string IconKind
        {
            get => (string)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty = "IconKind"
            .RegisterProperty<string, UserFunctItem>(callBack: SetIconKind);

        private static void SetIconKind(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<UserFunctItemViewModel>(vm => vm.IconKind = (string)e.NewValue);
        }

        #endregion

        #region DependencyProperty:Target 
        public string Target
        {
            get => (string)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty TargetProperty = "Target"
            .RegisterProperty<string, UserFunctItem>(callBack: SetTarget);

        private static void SetTarget(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<UserFunctItemViewModel>(vm => vm.Target = (string)e.NewValue);
        }
        #endregion 
    }
}
