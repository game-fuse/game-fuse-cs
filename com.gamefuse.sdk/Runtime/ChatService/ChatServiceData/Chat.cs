using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class Chat
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        [JsonProperty("creator_type")]
        public string CreatorType { get; set; }

        [JsonProperty("messages")]
        public ChatMessage[] Messages { get; set; }

        [JsonProperty("participants")]
        public ChatParticipant[] Participants { get; set; }
    }
}
