using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class OutgoingFriendRequestsResponse
    {
        [JsonProperty("outgoing_friend_requests")]
        public FriendRequest[] OutgoingFriendRequests { get; set; } = Array.Empty<FriendRequest>();
    }
}
