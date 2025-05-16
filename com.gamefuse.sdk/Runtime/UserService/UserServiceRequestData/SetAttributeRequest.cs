using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SetAttributeRequest
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}