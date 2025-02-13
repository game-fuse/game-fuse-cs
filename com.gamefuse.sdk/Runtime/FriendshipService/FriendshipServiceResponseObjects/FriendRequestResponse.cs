using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendRequestResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("friendship_id")]
        public int FriendshipId { get; set; }
    }
}
