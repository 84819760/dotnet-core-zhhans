using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans
{
    internal class DelayClear
    {
        public DelayClear() => Task.Run(Run);

        public Action<string> SetValue { get; init; }

        public Func<string> GetValue { get; init; }

        public async void Run()
        {
            string data = null;
            while (true)
            {
                await Task.Delay(2000);
                var value = GetValue?.Invoke();
                if (data != value) data = value;
                else SetValue(string.Empty);
            }
        }

    }
}
