using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendshipStatusResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
