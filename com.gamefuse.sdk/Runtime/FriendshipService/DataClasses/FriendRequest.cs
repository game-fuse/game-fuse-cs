using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Represents a friend request with additional information about the request.
    /// Inherits from <see cref="UserInfo"/> to include user details.
    /// </summary>
    [Serializable]
    public class FriendRequest : UserInfo
    {
        /// <summary>
        /// The unique identifier for the friendship/request.
        /// </summary>
        [JsonProperty("friendship_id")]
        public int FriendshipId { get; set; }

        /// <summary>
        /// The timestamp when the friend request was created.
        /// </summary>
        [JsonProperty("requested_at")]
        public string RequestedAt { get; set; }
    }
}