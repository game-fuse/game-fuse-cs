using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class SendMessageRequest
    {
        [JsonProperty("chat_id")]
        public int ChatId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}