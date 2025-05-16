using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SetAttributesRequest
    {
        [JsonProperty("attributes")]
        public List<AttributeItem> Attributes { get; set; } = new List<AttributeItem>();
    }
    
    [System.Serializable]
    public class AttributeItem
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}