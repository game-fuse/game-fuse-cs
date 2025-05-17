using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for the get outgoing friend requests endpoint.
    /// </summary>
    [Serializable]
    public class OutgoingFriendRequestsResponse
    {
        /// <summary>
        /// List of friend requests sent by the user.
        /// </summary>
        [JsonProperty("outgoing_friend_requests")]
        public FriendRequest[] OutgoingFriendRequests { get; set; } = Array.Empty<FriendRequest>();
    }
}
