using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using NearCoreExtensions;

namespace DotNetCorezhHans
{
    public static class Extensions
    {

        #region String

        public static string Join(this IEnumerable<string> data, string separator = " ") =>
            string.Join(separator, data);

        public static string JoinBuilder(this IEnumerable<string> data)
        {
            var res = new StringBuilder();
            foreach (var item in data) res.Append(item);
            return res.ToString();
        }

        public static string GetValue(this ITranslateValue[] transResult
            , Func<ITranslateValue, string> func) => transResult.Select(x => func(x)).Join("\r\n");

        public static IEnumerable<string> ReplaceSpace(this IEnumerable<string> values) =>
            values.Select(ReplaceSpace);

        public static string ReplaceSpace(this string value)
        {
            value = value.Trim();
            while (value.IndexOf("  ") > -1) value = value.Replace("  ", " ");
            return value;
        }

        public static string[] SplitRemoveEmpty(this string value, params char[] separator) =>
            value.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitRemoveEmpty(this string value, string separator) =>
            value.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitCrlf(this string value) => value.SplitRemoveEmpty(new[] { '\r', '\n' });

        #endregion

        #region MD5
        public static byte[] Md5ComputeHash(this byte[] data) => MD5.Create().ComputeHash(data);

        #endregion

        #region xml      

        private static XmlDocument GetDocument(this XmlNode superior) =>
            superior is XmlDocument xDoc ? xDoc : superior.OwnerDocument;

        internal static XmlNode CreateAddNode(this XmlNode superior, Func<XmlDocument, XmlNode> func)
        {
            var res = func(superior.GetDocument());
            superior.AppendChild(res);
            return res;
        }

        internal static XmlNode CreateAddNode(this XmlNode superior, string name) =>
            CreateAddNode(superior, doc => doc.CreateElement(name));

        internal static XmlText CreateAddTextNode(this XmlNode superior, string value) =>
            CreateAddNode(superior, doc => doc.CreateTextNode(value)) as XmlText;

        #endregion

        #region BlockingCollection
        public static void Run(this BlockingCollection<IRun> bc)
        {
            Task.Run(() =>
            {
                while (!bc.IsCompleted) bc.Take().Start();
            });
        }
        #endregion

        #region IErrorValue
        public static Exception GetException(this IErrorValue errorValue)
        {
            return errorValue.ErrorMsg is { Length: > 0 }
            ? new Exception($"code:{errorValue.ErrorCode}, msg:{errorValue.ErrorMsg}")
            : default;
        }
        #endregion

        #region Json

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string Serialize(object obj) => JsonSerializer.Serialize(obj, jsonOptions);

        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, jsonOptions);
        #endregion

        #region Exception

        public static string GetErrorString(this Exception exception, StringBuilder sb = null)
        {
            sb ??= new();
            exception = exception.UnAggregateException();
            SetErrorString(exception, sb);
            return sb.ToString();
        }

        private static void SetErrorString(Exception exception, StringBuilder sb)
        {
            exception = exception.UnAggregateException();
            sb.Append(exception.Message);
            sb.Append("\r\n");

            if (exception.InnerException is not null)
                SetErrorString(exception.InnerException, sb);
        }
        #endregion
    }
}