using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for the get friends endpoint.
    /// </summary>
    [Serializable]
    public class FriendsResponse
    {
        /// <summary>
        /// List of friends with basic user info.
        /// </summary>
        [JsonProperty("friends")]
        public UserInfo[] Friends { get; set; } = Array.Empty<UserInfo>();
    }
}
