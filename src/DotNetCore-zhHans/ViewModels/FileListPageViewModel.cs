using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Messages;
using Prism.Services.Dialogs;

namespace DotNetCorezhHans.ViewModels
{
    using ProcessPageMsg = Message<ExhibitionProcessViewModel>;
    using FileListPageMsg = Message<FileListPageViewModel>;

    public class FileListPageViewModel : PrimaryAndSecondaryViewModel, IFilesProgress
    {
        private readonly FileListPageMsg fileListPageMsg = FileListPageMsg.Instance;
        private readonly ProcessPageMsg processPageMsg = ProcessPageMsg.Instance;
        private readonly PageStateMessage pageState = PageStateMessage.Instance;
        private readonly HashSet<string> fiels = new();
        private readonly IDialogService dialog;

        public FileListPageViewModel(IDialogService dialog)
        {
            this.dialog = dialog;
            ShowPrimary();
            fileListPageMsg.Subscribe(FileListPageMsgHandler);
            pageState.Subscribe(PageStateHandler);
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public int CheckCount { get; set; }

        /// <summary>
        /// 异常数量
        /// </summary>
        public int AlertCount { get; set; }

        /// <summary>
        /// 错误数量
        /// </summary>
        public int ErrorCount { get; set; }

        public string ScanContent { get; set; } = "扫描内容";

        public ObservableCollection<FileItemViewModel> Items { get; private set; } = new();

        IEnumerable<IFileProgress> IFilesProgress.FileItems => Items;

        public void ShowScan(bool value)
        {
            if (value) ShowPrimary();
            else ShowSecondary();
        }

        public void AddFile(string path)
        {
            fiels.Add(path);
            processPageMsg.Publish(new(x => x.ProgressText = $"{fiels.Count}"));
        }

        private void PageStateHandler(PageControlType obj)
        {
            if (obj is not PageControlType.Translate) return;
            ShowPrimary();
            fiels.Clear();
            App.Invoke(() => Items.Clear());
            fileListPageMsg.Publish(new() { Data = this, IsRequest = false });
            AlertCount = CheckCount = ErrorCount = Count = 0;
        }

        private void FileListPageMsgHandler(MessageData<FileListPageViewModel> obj)
        {
            if (obj.TrySet(this) || obj.Target != "RequestInstance") return;
            fileListPageMsg.Publish(new()
            {
                Target = "ResponseInstance",
                Data = this
            });
        }

        public void Clear() => App.Invoke(() => Items.Clear());

        public void InitFileItems() => App.Invoke(() =>
        {
            Items = new(GetFileItemViewModels());
            Count = Items.Count;
            fiels.Clear();
        });

        private IEnumerable<FileItemViewModel> GetFileItemViewModels() =>
            fiels.Select(GetFileItemViewModel);

        private FileItemViewModel GetFileItemViewModel(string path, int index) =>
            new(dialog) { Path = path, Index = index + 1 };

        public void ItemHandler(IFileProgress fileProgress) => App.Invoke(() =>
        {
            var item = fileProgress as FileItemViewModel;
            Items.Remove(item);
            if (item.Exceptions.Length is 0)
            {
                CheckCount += 1;
            }
            else
            {
                SetDisplay(item);
                Items.Add(item);
            }
        });

        private void SetDisplay(FileItemViewModel item)
        {
            if (IsError(item.Exceptions))
            {
                ErrorCount += 1;
                item.ShowError();
            }
            else
            {
                AlertCount += 1;
                item.ShowAlert();
            }
        }

        private static bool IsError(IEnumerable<Exception> exceptions)
        {
            var items = exceptions.OfType<ExceptionBase>().ToArray();
            return items.Length is 0 || items.Any(x => x.IsError);
        }
    }
}