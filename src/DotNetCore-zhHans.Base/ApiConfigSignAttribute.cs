using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCorezhHans.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiConfigSignAttribute : Attribute
    {
        public ApiConfigSignAttribute(string apiName)
        {
            ApiName = apiName;
        }

        public string ApiName { get; }

        public static ApiConfigSignAttribute GetApiConfigSign(Type type) => type
            .GetCustomAttributes(typeof(ApiConfigSignAttribute), false)
            .OfType<ApiConfigSignAttribute>()
            .FirstOrDefault();
    }
}
