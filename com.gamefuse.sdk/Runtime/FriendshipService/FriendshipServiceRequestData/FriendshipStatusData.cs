using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendshipStatusData
    {
        [JsonProperty("status", Required = Required.Always)]
        public string Status { get; set; }
    }
}
