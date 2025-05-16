using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class AddScoreRequest
    {
        [JsonProperty("score")]
        public int Score { get; set; }
    }
}