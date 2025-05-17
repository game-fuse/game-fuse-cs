using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateGroupChatRequest
    {
        [JsonProperty("usernames")]
        public string[] Usernames { get; set; }

        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}