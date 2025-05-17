using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for friendship status update endpoints.
    /// Used for accept, decline, cancel, and unfriend operations.
    /// </summary>
    [Serializable]
    public class FriendshipStatusResponse
    {
        /// <summary>
        /// Message confirming the friendship status update.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
