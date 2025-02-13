using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateGroupRequest
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("group_type")]
        public string GroupType { get; set; } = "default";  // Matches JS default

        [JsonProperty("max_group_size", Required = Required.Always)]
        public int MaxGroupSize { get; set; }

        [JsonProperty("can_auto_join", Required = Required.Always)]
        public bool CanAutoJoin { get; set; }

        [JsonProperty("is_invite_only", Required = Required.Always)]
        public bool IsInviteOnly { get; set; }

        [JsonProperty("searchable")]
        public bool? Searchable { get; set; }

        [JsonProperty("admins_only_can_create_attributes")]
        public bool? AdminsOnlyCanCreateAttributes { get; set; }
    }
}