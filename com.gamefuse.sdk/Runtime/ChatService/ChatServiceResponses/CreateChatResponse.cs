using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateChatResponse
    {
        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("chat_users")]
        public List<ChatParticipant> ChatUsers { get; set; }
    }
}