using System;
using NearExtend.WpfPrism.Messages;

namespace DotNetCorezhHans.Messages
{
    internal class Message<T> : MessageBase<Message<T>, MessageData<T>> { }

    internal class MessageData<T>
    {
        public MessageData() { }

        public MessageData(Action<T> action) => SetData = action;

        public T Data { get; init; }

        public string Target { get; init; }

        /// <summary>
        /// 是否为请求，否则为响应
        /// </summary>
        public bool IsRequest { get; init; }

        public Action<T> SetData { get; init; }

        public bool TrySet(T value)
        {
            SetData?.Invoke(value);
            return SetData is not null;
        }
    }
}
