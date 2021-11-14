using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Messages
{
    public abstract class SemaphoreMessageBase<T>
    {
        private readonly SemaphoreSlim slim = new(0, 1);

        public SemaphoreMessageBase(T value) => Value = value;

        public T Value { get; }

        public Task Complete => slim.WaitAsync();

        public void SetComplete() => slim.Release();

        ~SemaphoreMessageBase() => slim.Dispose();
    }
}
