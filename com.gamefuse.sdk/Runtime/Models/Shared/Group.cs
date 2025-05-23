using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models.Shared
{
    /// <summary>
    /// Represents the status of a group connection.
    /// </summary>
    public enum GroupConnectionStatus
    {
        /// <summary>
        /// The connection request is pending.
        /// </summary>
        Pending,
        
        /// <summary>
        /// The user has been invited to join the group.
        /// </summary>
        Invited,
        
        /// <summary>
        /// The connection request has been accepted.
        /// </summary>
        Accepted,
        
        /// <summary>
        /// The connection request has been declined.
        /// </summary>
        Declined,
        
        /// <summary>
        /// The invitation is pending acceptance from the invitee.
        /// </summary>
        PendingAcceptanceFromInvitee
    }
    
    /// <summary>
    /// Consolidated model that represents a group in GameFuse.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The type of the group.
        /// </summary>
        [JsonProperty("group_type")]
        public string GroupType { get; internal set; }

        /// <summary>
        /// Whether users can automatically join the group.
        /// </summary>
        [JsonProperty("can_auto_join")]
        public bool CanAutoJoin { get; internal set; }

        /// <summary>
        /// Whether the group is invite-only.
        /// </summary>
        [JsonProperty("is_invite_only")]
        public bool IsInviteOnly { get; internal set; }

        /// <summary>
        /// The maximum number of members the group can have.
        /// </summary>
        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; internal set; }

        /// <summary>
        /// Whether the group is searchable.
        /// </summary>
        [JsonProperty("searchable")]
        public bool Searchable { get; internal set; }

        /// <summary>
        /// Whether only admins can create attributes.
        /// </summary>
        [JsonProperty("admins_only_can_create_attributes")]
        public bool AdminsOnlyCanCreateAttributes { get; internal set; }

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        [JsonProperty("member_count")]
        public int MemberCount { get; internal set; }

        /// <summary>
        /// The members of the group.
        /// </summary>
        [JsonProperty("members", NullValueHandling = NullValueHandling.Ignore)]
        public List<Friend> Members { get; internal set; }

        /// <summary>
        /// The administrators of the group.
        /// </summary>
        [JsonProperty("admins", NullValueHandling = NullValueHandling.Ignore)]
        public List<Friend> Admins { get; internal set; }

        /// <summary>
        /// Pending join requests for this group.
        /// </summary>
        [JsonProperty("join_requests", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupConnectionResponse> JoinRequests { get; internal set; }

        /// <summary>
        /// Invitations sent out by this group.
        /// </summary>
        [JsonProperty("invites", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupConnectionResponse> Invites { get; internal set; }

        /// <summary>
        /// Creates a new Group instance with empty collections.
        /// </summary>
        public Group()
        {
            Members = new List<Friend>();
            Admins = new List<Friend>();
            JoinRequests = new List<GroupConnectionResponse>();
            Invites = new List<GroupConnectionResponse>();
        }

        /// <summary>
        /// Creates a summary view of this group with only essential information.
        /// </summary>
        /// <returns>A GroupSummary representing this group.</returns>
        public GroupSummary ToSummary()
        {
            return new GroupSummary
            {
                Id = this.Id,
                Name = this.Name,
                GroupType = this.GroupType,
                CanAutoJoin = this.CanAutoJoin,
                IsInviteOnly = this.IsInviteOnly,
                MaxGroupSize = this.MaxGroupSize,
                Searchable = this.Searchable,
                MemberCount = this.MemberCount
            };
        }
    }

    /// <summary>
    /// Represents a summary view of a group, used in lists and when full details aren't needed.
    /// </summary>
    public class GroupSummary
    {
        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The type of the group.
        /// </summary>
        [JsonProperty("group_type")]
        public string GroupType { get; internal set; }

        /// <summary>
        /// Whether users can automatically join the group.
        /// </summary>
        [JsonProperty("can_auto_join")]
        public bool CanAutoJoin { get; internal set; }

        /// <summary>
        /// Whether the group is invite-only.
        /// </summary>
        [JsonProperty("is_invite_only")]
        public bool IsInviteOnly { get; internal set; }

        /// <summary>
        /// The maximum number of members the group can have.
        /// </summary>
        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; internal set; }

        /// <summary>
        /// Whether the group is searchable.
        /// </summary>
        [JsonProperty("searchable")]
        public bool Searchable { get; internal set; }

        /// <summary>
        /// The number of members in the group.
        /// </summary>
        [JsonProperty("member_count")]
        public int MemberCount { get; internal set; }
    }

    /// <summary>
    /// Payload for creating a new group.
    /// </summary>
    public class CreateGroupPayload
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the group.
        /// </summary>
        [JsonProperty("group_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupType { get; set; }

        /// <summary>
        /// The maximum number of members the group can have.
        /// </summary>
        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; set; }

        /// <summary>
        /// Whether users can automatically join the group.
        /// </summary>
        [JsonProperty("can_auto_join", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CanAutoJoin { get; set; }

        /// <summary>
        /// Whether the group is invite-only.
        /// </summary>
        [JsonProperty("is_invite_only", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInviteOnly { get; set; }

        /// <summary>
        /// Whether the group is searchable.
        /// </summary>
        [JsonProperty("searchable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Searchable { get; set; }

        /// <summary>
        /// Whether only admins can create attributes.
        /// </summary>
        [JsonProperty("admins_only_can_create_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AdminsOnlyCanCreateAttributes { get; set; }
    }

    /// <summary>
    /// Response wrapper for fetching all groups.
    /// </summary>
    public class FetchAllGroupsResponse
    {
        /// <summary>
        /// The list of groups.
        /// </summary>
        [JsonProperty("groups")]
        public List<GroupSummary> Groups { get; set; }

        /// <summary>
        /// Creates a new FetchAllGroupsResponse with an empty list.
        /// </summary>
        public FetchAllGroupsResponse()
        {
            Groups = new List<GroupSummary>();
        }
    }

    /// <summary>
    /// Payload for creating a group connection (user requesting to join or admin inviting).
    /// </summary>
    public class SendGroupConnectionRequestPayload
    {
        /// <summary>
        /// The ID of the group.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        /// <summary>
        /// The ID of the user (the one requesting to join or being invited).
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }

    /// <summary>
    /// Response from creating/fetching a group connection.
    /// </summary>
    public class GroupConnectionResponse
    {
        /// <summary>
        /// The ID of the group connection.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The status of the connection.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// The ID of the user who initiated the connection.
        /// </summary>
        [JsonProperty("inviter_id")]
        public int? InviterId { get; set; }

        /// <summary>
        /// The user involved in the connection.
        /// </summary>
        [JsonProperty("user")]
        public Friend User { get; set; }

        /// <summary>
        /// The ID of the group.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
    }

    /// <summary>
    /// Payload for updating the status of a group connection.
    /// </summary>
    public class UpdateGroupConnectionStatusPayload
    {
        /// <summary>
        /// The new status for the connection.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Response from updating a group connection's status.
    /// </summary>
    public class GroupConnectionStatusUpdateResponse
    {
        /// <summary>
        /// The ID of the group connection that was updated.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The new status of the connection.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Represents a group invitation from the invited user's perspective.
    /// </summary>
    public class MyGroupInvite
    {
        /// <summary>
        /// The user who was invited.
        /// </summary>
        [JsonProperty("user")]
        public Friend InvitedUser { get; set; }

        /// <summary>
        /// The user who sent the invitation.
        /// </summary>
        [JsonProperty("inviter")]
        public Friend Inviter { get; set; }

        /// <summary>
        /// The group to which the user was invited.
        /// </summary>
        [JsonProperty("group")]
        public Group GroupDetails { get; set; }
    }

    /// <summary>
    /// Represents a single attribute item in the payload for creating group attributes.
    /// </summary>
    public class GroupAttributePayloadItem
    {
        /// <summary>
        /// The key of the attribute.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// The value of the attribute.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Whether users other than the creator can edit the attribute.
        /// </summary>
        [JsonProperty("others_can_edit", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OthersCanEdit { get; set; }
    }

    /// <summary>
    /// Payload for the Create Group Attribute request.
    /// </summary>
    public class CreateGroupAttributesPayload
    {
        /// <summary>
        /// The list of attributes to create.
        /// </summary>
        [JsonProperty("attributes")]
        public List<GroupAttributePayloadItem> Attributes { get; set; }

        /// <summary>
        /// Creates a new CreateGroupAttributesPayload with an empty list.
        /// </summary>
        public CreateGroupAttributesPayload()
        {
            Attributes = new List<GroupAttributePayloadItem>();
        }
    }

    /// <summary>
    /// Represents a single group attribute.
    /// </summary>
    public class GroupAttributeResponseItem
    {
        /// <summary>
        /// The ID of the attribute.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The key of the attribute.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// The value of the attribute.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// The ID of the user who created the attribute.
        /// </summary>
        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        /// <summary>
        /// Whether the current user can edit the attribute.
        /// </summary>
        [JsonProperty("can_edit")]
        public bool CanEdit { get; set; }
    }

    /// <summary>
    /// Response from the Create Group Attribute request.
    /// </summary>
    public class CreateGroupAttributesResponse
    {
        /// <summary>
        /// The list of created attributes.
        /// </summary>
        [JsonProperty("attributes")]
        public List<GroupAttributeResponseItem> Attributes { get; set; }

        /// <summary>
        /// Creates a new CreateGroupAttributesResponse with an empty list.
        /// </summary>
        public CreateGroupAttributesResponse()
        {
            Attributes = new List<GroupAttributeResponseItem>();
        }
    }

    /// <summary>
    /// Response from the Fetch Group Attributes request.
    /// </summary>
    public class FetchGroupAttributesResponse
    {
        /// <summary>
        /// The list of group attributes.
        /// </summary>
        [JsonProperty("attributes")]
        public List<GroupAttributeResponseItem> Attributes { get; set; }

        /// <summary>
        /// Creates a new FetchGroupAttributesResponse with an empty list.
        /// </summary>
        public FetchGroupAttributesResponse()
        {
            Attributes = new List<GroupAttributeResponseItem>();
        }
    }

    /// <summary>
    /// Payload for modifying an existing group attribute.
    /// </summary>
    public class ModifyGroupAttributePayload
    {
        /// <summary>
        /// The key of the attribute to modify.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// The new value for the attribute.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}