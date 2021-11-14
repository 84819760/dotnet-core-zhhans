using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCoreZhHans.Service
{
    /// <summary>
    /// 缓存数据
    /// </summary>
    internal class CacheData
    {
        private readonly SemaphoreSlim slim;

        public CacheData(string cacheSource)
        {
            slim = new(0, 1);
            CacheSource = cacheSource;
        }

        ~CacheData() => slim?.Dispose();

        /// <summary>
        /// <inheritdoc cref="CacheData"/>,用于创建 有值 缓存。
        /// </summary>
        public CacheData(string cacheSource, string value)
        {
            Value = value;
            CacheSource = cacheSource;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public string CacheSource { get; }

        public string Value { get; private set; }

        public int[] MissingContent { get; set; }

        public bool IsCompletion => Value is not null;

        /// <summary>
        /// 检查通过
        /// </summary>
        public bool IsCheckPassed => (MissingContent?.Length ?? 1) is 0;

        /// <summary>
        /// 值是否为API响应的内容
        /// </summary>
        public bool IsResponseValue { get; private set; }

        /// <summary>
        /// 响应的源
        /// </summary>
        public string ResponseSource { get; private set; }

        internal async Task Wait(CancellationToken token)
        {
            try
            {
                if (slim is null) return;
                await slim.WaitAsync(token);
                slim.Release();
            }
            catch (Exception)
            {
                if (token.IsCancellationRequested) return;
                throw;
            }
        }

        public CacheData SetValue(string value, string responseSource, bool isResponseValue)
        {
            if (Value is { Length: > 0 } && Value != value)
            {
                throw new NotImplementedException($"{nameof(CacheData)},数据重复？");
            }

            Value = value;
            IsResponseValue = isResponseValue;
            ResponseSource = responseSource;
            slim?.Release();
            return this;
        }

        public override string ToString() => $"Completion = {IsCompletion}, Value = {Value}";

        public CacheData CheckRowSymbol(string queryValue)
        {
            if (IsResponseValue)
            {
                var valueIds = GetIds(Value);
                var queryValueIds = GetIds(queryValue);
                MissingContent = queryValueIds.Except(valueIds).ToArray();
            }
            return this;
        }

        private static int[] GetIds(string value) => SymbolManager.GetIds(value);
    }
}
