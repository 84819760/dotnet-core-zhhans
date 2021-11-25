using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCorezhHans.TranslTasks
{
    internal class LogHandler
    {
        private readonly Channel<Data> channel = Channel.CreateUnbounded<Data>();
        private readonly string logDir = CreateLoggingDirectory();
        private readonly IndexProvider indexProvider = new();
        private volatile int errorCount;
        private readonly Task task;

        private record Data(int Index, string Path, Exception Exception, int FileIndex)
        {
            public override string ToString() => Exception is ExceptionBase
                    ? Exception.ToString()
                    : @$"
Index : {Index}
文件 : {Path}
日期 : {DateTime.Now}
错误 : {Exception.Message}";
        }

        public LogHandler() => task = WhileWrite();

        private static string CreateLoggingDirectory()
        {
            var path = Directory.GetCurrentDirectory();
            return Path.Combine(path, "log", DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
        }

        public CancellationTokenSource CancellationTokenSource { get; } = new();

        private CancellationToken Token => CancellationTokenSource.Token;

        public async Task SetComplete()
        {
            channel.Writer.Complete();
            await CallEnd();
            CancellationTokenSource.Cancel();
            await task;
        }

        internal void AddError(Exception exception, IFilePath file, int index)
        {
            errorCount++;
            channel.Writer.TryWrite(new(errorCount, $"({index})\t{file.Path}", exception, indexProvider.GetId()));
        }

        private Task WhileWrite() => Task.Run(async () =>
        {
            while (await channel.Reader.WaitToReadAsync(Token))
            {
                var item = await channel.Reader.ReadAsync(Token);
                Write(item);
            }
        });

        private async Task CallEnd()
        {
            var items = channel.Reader.ReadAllAsync(Token);
            await foreach (var item in items) Write(item);
        }

        private void Write(Data item)
        {
            Directory.CreateDirectory(logDir);
            var path = Path.Combine(logDir, $"{item.Index}.log");
            File.WriteAllText(path, item.ToString());
        }
    }
}
