using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class SendMessageResponse
    {
        [JsonProperty("message")]
        public ChatMessage Message { get; set; }
    }
}