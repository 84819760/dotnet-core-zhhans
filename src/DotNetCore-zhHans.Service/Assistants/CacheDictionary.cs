using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DotNetCoreZhHans.Service
{
    /// <summary>
    /// 缓存单元
    /// </summary>
    internal class CacheDictionary<T>
    {
        private readonly ConcurrentDictionary<string, T> cache = new();

        public CacheDictionary() { }

        public ReaderWriterLockSlim LockData { get; } = new();

        public T[] Values => cache.Values.ToArray();

        public T this[string key]
        {
            get
            {
                TryGetValue(key, out var res);
                return res;
            }
            set
            {
                LockData.LockAndWrite(() => cache[key] = value);
            }
        }

        public bool TryGetValue(string key, out T cacheData)
        {
            LockData.EnterReadLock();
            try
            {
                return cache.TryGetValue(key, out cacheData);
            }
            finally
            {
                LockData.ExitReadLock();
            }
        }

        public bool ContainsKey(string key) => LockData.LockAndRead(() => cache.ContainsKey(key));

        internal void Clear() => cache.Clear();
    }
}
