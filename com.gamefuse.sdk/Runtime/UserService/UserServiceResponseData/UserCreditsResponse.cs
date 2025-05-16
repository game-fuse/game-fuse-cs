using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class UserCreditsResponse
    {
        [JsonProperty("credits")]
        public int Credits { get; set; }
    }
}