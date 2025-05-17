using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateDirectChatRequest
    {
        [JsonProperty("usernames")]
        public string[] Usernames { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}