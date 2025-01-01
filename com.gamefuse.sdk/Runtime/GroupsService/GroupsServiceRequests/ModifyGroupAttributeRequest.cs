using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class ModifyGroupAttributeRequest
    {
        [JsonProperty("key", Required = Required.Always)]
        public string Key { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public string Value { get; set; }
    }
}
