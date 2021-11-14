using System.Collections.Generic;

namespace TranslateApi.Kate
{
    class RequestData
    {
        private readonly string value;

        public RequestData(string value) => this.value = value;        

        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs() => 
            new KeyValuePair<string, string>[] { new("info", value) };

    }
}
