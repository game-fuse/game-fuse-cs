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

        [JsonProperty("only_can_edit_by_creator", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OnlyCanEditByCreator { get; set; }
    }
}
