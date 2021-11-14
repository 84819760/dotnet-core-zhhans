using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DotNetCorezhHans.Base.Interfaces;
using MaterialDesignThemes.Wpf;
using NearExtend.WpfPrism;
using NearCoreExtensions;
using System.Diagnostics;
using Prism.Services.Dialogs;

namespace DotNetCorezhHans.ViewModels
{
    public class FileItemViewModel : ViewModelBase<FileItemViewModel>
        , IErrorHandler, IFileProgress
    {
        private readonly List<Exception> exceptions = new();
        private readonly IDialogService dialog;

        public event Action<Exception> AddErrorEvent;

        public FileItemViewModel()
        {

        }

        public FileItemViewModel(IDialogService dialog) => this.dialog = dialog;

        public int Index { get; init; }

        public string Path { get; init; } = "文件路径";

        public string Progress { get; set; } = "排队";

        public Visibility ProgressVisibility { get; set; } = Visibility.Visible;

        public PackIconKind IconKind { get; private set; } = PackIconKind.Alert;

        public Brush IconBrush { get; private set; } = new SolidColorBrush(Colors.Orange);

        public Visibility IconKindVisibility { get; set; } = Visibility.Collapsed;

        public Exception[] Exceptions => exceptions.ToArray();


        private void Show()
        {
            ProgressVisibility = Visibility.Collapsed;
            IconKindVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 设置显示警告
        /// </summary>
        public void ShowAlert()
        {
            Show();
            IconKind = PackIconKind.Alert;
            IconBrush = new SolidColorBrush(Colors.Orange);
        }

        public void ShowError()
        {
            Show();
            IconKind = PackIconKind.Close;
            IconBrush = new SolidColorBrush(Colors.Red);
        }

        public Exception AddError(Exception exception)
        {
            exceptions.Add(exception.UnAggregateException());
            AddErrorEvent?.Invoke(exception);
            return exception;
        }

        public void CallMethod()
        {
            if (exceptions.Count is 0) return;
            var param = new DialogParameters() { { "", exceptions } };
            dialog.ShowDialog("ErrorList", param, null);
        }
    }
}