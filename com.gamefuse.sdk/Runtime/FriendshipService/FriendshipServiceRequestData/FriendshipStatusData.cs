using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Request data for updating the status of a friend request.
    /// </summary>
    [Serializable]
    public class FriendshipStatusData
    {
        /// <summary>
        /// The new status to set for the friendship request (accepted or declined).
        /// </summary>
        [JsonProperty("status", Required = Required.Always)]
        public string Status { get; set; }
    }
}
