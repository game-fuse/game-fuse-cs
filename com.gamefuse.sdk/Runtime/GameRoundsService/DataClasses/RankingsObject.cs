using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class RankingsObject
    {
        [JsonProperty("place")]
        public int Place { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("end_time")]
        public string EndTime { get; set; }

        [JsonProperty("user")]
        public UserInfo User { get; set; }
    }
}