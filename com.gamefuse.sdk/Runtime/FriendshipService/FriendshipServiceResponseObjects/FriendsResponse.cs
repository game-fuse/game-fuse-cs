
using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendsResponse
    {
        [JsonProperty("friends")]
        public UserInfo[] Friends { get; set; } = Array.Empty<UserInfo>();
    }
}
