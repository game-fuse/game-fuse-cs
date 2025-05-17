using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Request data for sending a friend request.
    /// </summary>
    [Serializable]
    public class FriendRequestData
    {
        /// <summary>
        /// The username of the player you want to send a friend request to.
        /// </summary>
        [JsonProperty("username", Required = Required.Always)]
        public string Username { get; set; }
    }
}