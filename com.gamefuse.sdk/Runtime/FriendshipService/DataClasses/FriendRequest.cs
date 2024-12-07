using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendRequest : UserInfo
    {
        [JsonProperty("friendship_id")]
        public int FriendshipId { get; set; }

        [JsonProperty("requested_at")]
        public string RequestedAt { get; set; }
    }
}