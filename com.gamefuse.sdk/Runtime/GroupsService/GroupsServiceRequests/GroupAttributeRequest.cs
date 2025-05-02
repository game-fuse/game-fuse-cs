using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupAttributeRequest
    {
        [JsonProperty("key", Required = Required.Always)]
        public string Key { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public string Value { get; set; }

        [JsonProperty("others_can_edit", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OthersCanEdit { get; set; }
    }
}
