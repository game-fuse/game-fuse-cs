using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SetCreditsRequest
    {
        [JsonProperty("credits")]
        public int Credits { get; set; }
    }
}