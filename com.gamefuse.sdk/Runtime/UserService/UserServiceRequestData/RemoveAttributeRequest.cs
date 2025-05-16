using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class RemoveAttributeRequest
    {
        [JsonProperty("game_user_attribute_key")]
        public string Key { get; set; }
    }
}