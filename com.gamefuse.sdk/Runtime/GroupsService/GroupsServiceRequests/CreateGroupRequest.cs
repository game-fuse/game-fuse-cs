using System;
using Newtonsoft.Json;

[Serializable]
public class CreateGroupRequest
{
    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty("group_type", NullValueHandling = NullValueHandling.Ignore)]
    public string GroupType { get; set; }

    [JsonProperty("max_group_size", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxGroupSize { get; set; }

    [JsonProperty("can_auto_join", NullValueHandling = NullValueHandling.Ignore)]
    public bool? CanAutoJoin { get; set; }

    [JsonProperty("is_invite_only", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsInviteOnly { get; set; }

    [JsonProperty("searchable", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Searchable { get; set; }

    [JsonProperty("admins_only_can_create_attributes", NullValueHandling = NullValueHandling.Ignore)]
    public bool? AdminsOnlyCanCreateAttributes { get; set; }
}

