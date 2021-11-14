using System.Windows;
using System.Windows.Controls;
using DotNetCorezhHans.ViewModels;
using NearExtend.WpfPrism;

namespace DotNetCorezhHans.Views
{
    public partial class ListModule : UserControl
    {
        public ListModule()
        {
            InitializeComponent();
            this.SetDesignData<ListModuleViewModel>();
        }

        #region DependencyProperty:Icon 
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty = "Icon"
            .RegisterProperty<string, ListModule>(callBack: SetIcon);

        private static void SetIcon(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<ListModuleViewModel>(vm => vm.Icon = (string)e.NewValue);
        }
        #endregion

        #region DependencyProperty:RegionName 
        public string RegionName
        {
            get => (string)GetValue(RegionNameProperty);
            set => SetValue(RegionNameProperty, value);
        }

        public static readonly DependencyProperty RegionNameProperty = "RegionName"
            .RegisterProperty<string, ListModule>(callBack: SetRegionName);

        private static void SetRegionName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<ListModuleViewModel>(vm => vm.RegionName = (string)e.NewValue);
        }
        #endregion

        #region DependencyProperty:ContentHorizontalAlignment 
        public HorizontalAlignment ContentHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ContentHorizontalAlignmentProperty);
            set => SetValue(ContentHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ContentHorizontalAlignmentProperty = "ContentHorizontalAlignment"
            .RegisterProperty<HorizontalAlignment, ListModule>(callBack: SetContentHorizontalAlignment);

        private static void SetContentHorizontalAlignment(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<ListModuleViewModel>(vm => vm.ContentHorizontalAlignment = (HorizontalAlignment)e.NewValue);
        }
        #endregion

        #region DependencyProperty:Title 
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = "Title"
            .RegisterProperty<string, ListModule>(callBack: SetTitle);

        private static void SetTitle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            d.SetViewModelValue<ListModuleViewModel>(vm => vm.Title = (string)e.NewValue);
        }
        #endregion 

        public void SetShow(object value) => ControlData.Content = value;
    }
}