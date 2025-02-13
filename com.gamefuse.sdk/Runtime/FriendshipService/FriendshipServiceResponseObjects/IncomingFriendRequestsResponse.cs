using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class IncomingFriendRequestsResponse
    {
        [JsonProperty("incoming_friend_requests")]
        public FriendRequest[] IncomingFriendRequests { get; set; } = Array.Empty<FriendRequest>();
    }
}
