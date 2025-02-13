using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendRequestData
    {
        [JsonProperty("username", Required = Required.Always)]
        public string Username { get; set; }
    }
}