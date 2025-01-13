using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class ChatParticipant : UserInfo
    {
        [JsonProperty("is_new_user")]
        public bool IsNewUser { get; set; }
    }
}
