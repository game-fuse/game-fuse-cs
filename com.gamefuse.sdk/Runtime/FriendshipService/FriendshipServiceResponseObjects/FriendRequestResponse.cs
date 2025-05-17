using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for the send friend request endpoint.
    /// </summary>
    [Serializable]
    public class FriendRequestResponse
    {
        /// <summary>
        /// Success message confirming the friend request.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// The ID of the created friendship request.
        /// </summary>
        [JsonProperty("friendship_id")]
        public int FriendshipId { get; set; }
    }
}
