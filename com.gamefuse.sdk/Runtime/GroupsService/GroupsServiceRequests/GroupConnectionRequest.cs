using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupConnectionRequest
    {
        [JsonProperty("group_id", Required = Required.Always)]
        public int GroupId { get; set; }

        [JsonProperty("user_id", Required = Required.Always)]
        public int UserId { get; set; }
    }
}
