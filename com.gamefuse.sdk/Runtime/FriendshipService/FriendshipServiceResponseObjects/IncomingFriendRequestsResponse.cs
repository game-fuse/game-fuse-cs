using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for the get incoming friend requests endpoint.
    /// </summary>
    [Serializable]
    public class IncomingFriendRequestsResponse
    {
        /// <summary>
        /// List of friend requests received by the user.
        /// </summary>
        [JsonProperty("incoming_friend_requests")]
        public FriendRequest[] IncomingFriendRequests { get; set; } = Array.Empty<FriendRequest>();
    }
}
