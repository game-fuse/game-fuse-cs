using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class AddCreditsRequest
    {
        [JsonProperty("credits")]
        public int Credits { get; set; }
    }
}