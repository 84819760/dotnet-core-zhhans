using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCorezhHans;
using DotNetCorezhHans.Base;
using DotNetCorezhHans.Base.Exceptions;
using DotNetCorezhHans.Base.Interfaces;


namespace DotNetCoreZhHans.Service.ApiRequests
{
    internal class ApiRequestItem
    {
        private readonly TranslateServiceBase translateServiceBase;
        private readonly ApiRequestProvider apiRequestProvider;
        private readonly ITransmitData transmits;

        public ApiRequestItem(ApiRequestProvider apiRequestProvider
            , TranslateServiceBase translateServiceBase
            , ITransmitData transmits)
        {
            this.translateServiceBase = translateServiceBase;
            this.apiRequestProvider = apiRequestProvider;
            this.transmits = transmits;
        }

        public ApiConfig ApiConfig => translateServiceBase.ApiConfig;

        public IMasterProgress Master { get; set; }

        public string Name => ApiConfig.Name;

        public int IntervalTime => ApiConfig.IntervalTime;

        private async Task<string> Send(string value, CancellationToken token)
        {
            await Task.Delay(IntervalTime, token);
            return await TrySend(value, token);
        }

        private async Task<string> TrySend(string value, CancellationToken token)
        {
            Exception exception = null;
            for (var i = 0; i < 5; i++)
            {
                if (token.IsCancellationRequested) return default;
                try
                {
                    return await SnedAsync(value);
                }
                catch (DecompositionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.Print($"异常:{ex.Message}\r\n请求内容:{value}");
                    await Task.Delay(1000, token);
                    exception = ex;
                }
                SetMasterTitle($"重试{i + 1}");
            }
            throw transmits.Interrupt = CreateInterruptException(exception);
        }


        private void SetMasterTitle(string value) => transmits.Progress.Master.Title = value;

        private Exception CreateInterruptException(Exception exception)
        {
            var error = $"{translateServiceBase.Name}Api超过重试次数，无法继续执行！\r\n{exception.Message}";
            transmits.CancellationTokenSource.Cancel();
            return new InterruptException(error, exception);
        }

        private Task<string> SnedAsync(string value) =>
             translateServiceBase.GetTranslateAsync(value);

        public async Task<string> SendRequest(string value, CancellationToken token)
        {
            try
            {
                return await GetResult(value, token);
            }
            catch (DecompositionException)
            {
                return await GetDecomposition(value, token);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task Reduction() => apiRequestProvider.Add(this);

        private async Task<string> GetDecomposition(string value, CancellationToken token)
        {           
            var res = new StringBuilder();
            var array = value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                SetMasterTitle($"分段重试({i}/{array.Length})");
                var item = array[i];
                res.Append(await GetResult(item, token));
                res.Append("\r\n");
            }
            return res.ToString();
        }

        private async Task<string> GetResult(string value, CancellationToken token)
        {
            Master.RequestStatus = $"请求 : {value.Length}".PadRight(9, ' ');
            var res = await Send(value, token);
            Master.ResponseStatus = $"响应 : {res?.Length}".PadRight(9, ' ');
            return res;
        }
    }
}
