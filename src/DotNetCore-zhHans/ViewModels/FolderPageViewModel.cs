using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NearExtend.WpfPrism;
using System.Collections.ObjectModel;
using Prism.Events;
using System.Windows;
using DotNetCorezhHans.Messages;

namespace DotNetCorezhHans.ViewModels
{
    public class FolderPageViewModel : ViewModelBase<FolderPageViewModel>
    {
        //private readonly NotificationMessage message = NotificationMessage.Instance;
        private const MessageBoxButton ButtonYesNo = MessageBoxButton.YesNo;
        private const MessageBoxImage Question = MessageBoxImage.Question;
        private const MessageBoxResult No = MessageBoxResult.No;

        public FolderPageViewModel()
        {
            if (IsDesignMode) return;
            var config = App.GetConfigManager();
            Datas = config.Directorys;
            SetDefault();
        }

        public string SelectedItem { get; set; }

        public ObservableCollection<string> Datas { get; set; }

        public Visibility DeleteForeverState { get; set; }

        private void SetDefault() => DeleteForeverState = Visibility.Hidden;

        //选择项目
        public void SelectionChanged() => DeleteForeverState = Visibility.Visible;

        //失去焦点
        public void LostFocus() => SetDefault();

        //获得焦点
        public void GotFocus()
        {
            if (SelectedItem?.Length > 0) SelectionChanged();
        }

        //执行删除
        public void DeleteItem()
        {
            if (!IsDelete()) return;
            Datas.Remove(SelectedItem);
            SetDefault();
        }

        private bool IsDelete()
        {
            var msg = $"是否删除?\r\n{SelectedItem}";
            var res = MessageBox.Show(msg, "删除确认", ButtonYesNo, Question, No);
            return res == MessageBoxResult.Yes;
        }

        public void AddItem()
        {
            if (!TryBrowserDialog(out var path)) return;
            Datas.Remove(path);
            Datas.Add(path);
            SetDefault();
        }

        private static bool TryBrowserDialog(out string path)
        {
            path = default;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return false;
            path = dialog.SelectedPath;
            return true;
        }
    }
}
