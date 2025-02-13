using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameFuseCSharp
{
    [Serializable]
    public class GetMessagesResponse
    {
        [JsonProperty("messages")]
        public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();
    }
}
