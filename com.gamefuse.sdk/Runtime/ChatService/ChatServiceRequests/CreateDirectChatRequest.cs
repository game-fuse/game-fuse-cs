using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateDirectChatRequest
    {
        // The API requires "usernames" parameter as an array
        [JsonProperty("usernames")]
        public string[] Usernames { get; set; }

        // Message text to send
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}