using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SetScoreRequest
    {
        [JsonProperty("score")]
        public int Score { get; set; }
    }
}