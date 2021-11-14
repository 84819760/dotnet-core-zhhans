using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NearExtend.WpfPrism
{
    public static class JsonExtension
    {
        public static T Clone<T>(this T data)
        {
            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
