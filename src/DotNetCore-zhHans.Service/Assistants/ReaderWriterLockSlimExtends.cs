using System;
using System.Threading;

namespace DotNetCoreZhHans.Service
{
    internal static class ReaderWriterLockSlimExtends
    {
        /// <summary>
        /// 并行读取(配合 LockAndWrite 实现多个线程读取，单个线程写入)
        /// </summary>
        public static void LockAndRead(this ReaderWriterLockSlim cacheLock, Action action)
        {
            cacheLock.EnterReadLock();
            try { action(); }
            finally { cacheLock.ExitReadLock(); }
        }

        /// <summary>
        /// 并行读取(配合 LockAndWrite 实现多个线程读取，单个线程写入)
        /// </summary>
        public static T LockAndRead<T>(this ReaderWriterLockSlim cacheLock, Func<T> getValue)
        {
            cacheLock.EnterReadLock();
            try { return getValue(); }
            finally { cacheLock.ExitReadLock(); }
        }

        /// <summary>
        /// 并行读取(配合 LockAndRead 实现多个线程读取，单个线程写入)
        /// </summary>
        public static void LockAndWrite(this ReaderWriterLockSlim cacheLock, Action action)
        {
            cacheLock.EnterWriteLock();
            try { action(); }
            finally { cacheLock.ExitWriteLock(); }
        }

        /// <summary>
        /// 并行读取(配合 LockAndRead 实现多个线程读取，单个线程写入)
        /// </summary>
        public static T LockAndWrite<T>(this ReaderWriterLockSlim cacheLock, Func<T> getValue)
        {
            cacheLock.EnterWriteLock();
            try { return getValue(); }
            finally { cacheLock.ExitWriteLock(); }
        }
    }
}
