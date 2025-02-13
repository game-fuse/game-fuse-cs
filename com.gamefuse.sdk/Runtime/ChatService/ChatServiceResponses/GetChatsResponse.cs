using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GetChatsResponse
    {
        [JsonProperty("direct_chats")]
        public Chat[] DirectChats { get; set; }

        [JsonProperty("group_chats")]
        public Chat[] GroupChats { get; set; }
    }
}
