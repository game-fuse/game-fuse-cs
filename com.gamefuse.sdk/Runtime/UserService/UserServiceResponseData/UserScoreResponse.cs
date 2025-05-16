using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class UserScoreResponse
    {
        [JsonProperty("score")]
        public int Score { get; set; }
    }
}