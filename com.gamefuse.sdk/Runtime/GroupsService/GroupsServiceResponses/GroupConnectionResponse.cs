using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupConnectionResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("inviter_id")]
        public int InviterId { get; set; }

        [JsonProperty("user")]
        public UserInfo User { get; set; }
    }
}