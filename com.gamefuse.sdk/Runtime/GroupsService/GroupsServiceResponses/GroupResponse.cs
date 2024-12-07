using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("group_type")]
        public string GroupType { get; set; }

        [JsonProperty("can_auto_join")]
        public bool CanAutoJoin { get; set; }

        [JsonProperty("is_invite_only")]
        public bool IsInviteOnly { get; set; }

        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; set; }

        [JsonProperty("searchable")]
        public bool Searchable { get; set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        [JsonProperty("members")]
        public UserInfo[] Members { get; set; } = Array.Empty<UserInfo>();

        [JsonProperty("admins")]
        public UserInfo[] Admins { get; set; } = Array.Empty<UserInfo>();

        [JsonProperty("join_requests")]
        public GroupConnectionResponse[] JoinRequests { get; set; } = Array.Empty<GroupConnectionResponse>();

        [JsonProperty("invites")]
        public GroupConnectionResponse[] Invites { get; set; } = Array.Empty<GroupConnectionResponse>();
    }
}