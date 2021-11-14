using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCorezhHans.Base
{
    public class GcManager
    {
        #region static

        private static volatile int count;
        static GcManager()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    SpinWait.SpinUntil(IsRun);
                    await Task.Delay(3000);
                    GC.Collect();
                    count--;
                }
            });
        }

        private static bool IsRun() => count > 0;

        public static void SetGc(int value = 10) => count = value;

        #endregion

        private TaskCompletionSource completionSource;
        private readonly List<Type> types = new();


        public Task Task => completionSource?.Task ?? Task.CompletedTask;

        public void Add(object vlaue)
        {
            completionSource ??= new();
            types.Add(vlaue.GetType());
        }

        public void Remove(object value)
        {
            types.Remove(value.GetType());
            if (types.Count is 0) completionSource.TrySetResult();
        }
    }
}
