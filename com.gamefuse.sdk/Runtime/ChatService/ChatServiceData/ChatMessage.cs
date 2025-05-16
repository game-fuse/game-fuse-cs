using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class ChatMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("read_by")]
        public int[] ReadBy { get; set; }

        [JsonProperty("read")]
        public bool Read { get; set; }
    }
}
