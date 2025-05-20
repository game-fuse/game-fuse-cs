// Models/GroupModels.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Payload for creating a new group.
    /// </summary>
    public class CreateGroupPayload
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("group_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupType { get; set; }

        // MaxGroupSize is considered required for creation based on your feedback.
        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; set; }

        [JsonProperty("can_auto_join", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CanAutoJoin { get; set; }

        [JsonProperty("is_invite_only", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInviteOnly { get; set; }

        [JsonProperty("searchable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Searchable { get; set; }

        [JsonProperty("admins_only_can_create_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AdminsOnlyCanCreateAttributes { get; set; }
    }

    /// <summary>
    /// Represents full details of a group.
    /// Used as the response for creating a group and fetching group details.
    /// </summary>
    public class Group
    {
        [JsonProperty("id")]
        public int Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("group_type")]
        public string GroupType { get; internal set; }

        [JsonProperty("can_auto_join")]
        public bool CanAutoJoin { get; internal set; }

        [JsonProperty("is_invite_only")]
        public bool IsInviteOnly { get; internal set; }

        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; internal set; }

        [JsonProperty("searchable")]
        public bool Searchable { get; internal set; }

        [JsonProperty("admins_only_can_create_attributes")]
        public bool AdminsOnlyCanCreateAttributes { get; internal set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; internal set; }

        [JsonProperty("members")]
        public List<UserSummary> Members { get; internal set; }

        [JsonProperty("admins")]
        public List<UserSummary> Admins { get; internal set; }

        /// <summary>
        /// List of pending join requests for this group. Each item is a GroupConnection.
        /// The 'User' property within each GroupConnectionResponse is the requester.
        /// The 'Status' property will likely be "pending".
        /// </summary>
        [JsonProperty("join_requests", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupConnectionResponse> JoinRequests { get; internal set; }

        /// <summary>
        /// List of active invitations sent out by this group. Each item is a GroupConnection.
        /// The 'User' property within each GroupConnectionResponse is the invitee.
        /// The 'Status' property might be "invited" or "pending_acceptance_from_invitee".
        /// The 'InviterId' will be the admin who sent the invite.
        /// </summary>
        [JsonProperty("invites", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupConnectionResponse> Invites { get; internal set; }

        public Group()
        {
            Members = new List<UserSummary>();
            Admins = new List<UserSummary>();
            JoinRequests = new List<GroupConnectionResponse>();
            Invites = new List<GroupConnectionResponse>();
        }
    }

    /// <summary>
    /// Represents a summary of a group, typically used in lists of all available groups.
    /// </summary>
    public class GroupSummary
    {
        [JsonProperty("id")]
        public int Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("group_type")]
        public string GroupType { get; internal set; }

        [JsonProperty("can_auto_join")]
        public bool CanAutoJoin { get; internal set; }

        [JsonProperty("is_invite_only")]
        public bool IsInviteOnly { get; internal set; }

        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; internal set; }

        [JsonProperty("searchable")]
        public bool Searchable { get; internal set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; internal set; }
    }

    /// <summary>
    /// Response wrapper for fetching all groups.
    /// </summary>
    public class FetchAllGroupsResponse
    {
        [JsonProperty("groups")]
        public List<GroupSummary> Groups { get; set; }

        public FetchAllGroupsResponse()
        {
            Groups = new List<GroupSummary>();
        }
    }

    /// <summary>
    /// A summarized representation of a user, often used in lists like group members or friends.
    /// </summary>
    public class UserSummary
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")] // System email as per API docs for similar contexts
        public string Email { get; set; }

        [JsonProperty("credits")]
        public int Credits { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        // If "display_email" is consistently present in these summaries, add it.
        // [JsonProperty("display_email")]
        // public string DisplayEmail { get; set; }
    }

    /// <summary>
    /// Payload for creating a group connection (user requesting to join or admin inviting).
    /// </summary>
    public class SendGroupConnectionRequestPayload
    {
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; } // User requesting to join OR user being invited by an admin
    }

    /// <summary>
    /// Response from creating/fetching a group connection.
    /// This model is versatile:
    /// - Response from POST /group_connections.
    /// - Represents items in Group.JoinRequests list.
    /// - Represents items in Group.Invites list (from group's perspective).
    /// </summary>
    public class GroupConnectionResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; } // The GroupConnection ID itself

        [JsonProperty("status")]
        public string Status { get; set; } // e.g., "pending", "accepted", "declined", "invited"

        [JsonProperty("inviter_id")]
        public int? InviterId { get; set; } // User ID of who initiated (could be self if requesting, or admin if inviting)

        [JsonProperty("user")]
        public UserSummary User { get; set; } // The user who this connection pertains to (requester or invitee)

        [JsonProperty("group_id")] // The ID of the group this connection is for
        public int GroupId { get; set; }

        // If the API response for group_connections (or join_requests/invites list items) includes timestamps like "created_at" or "updated_at"
        // for the connection itself, add them here.
        // [JsonProperty("created_at")]
        // public string CreatedAt { get; set; }
    }

    /// <summary>
    /// Payload for updating the status of a group connection (e.g., admin accepting/declining a join request).
    /// </summary>
    public class UpdateGroupConnectionStatusPayload
    {
        [JsonProperty("status")]
        public string Status { get; set; } // "accepted" or "declined"
    }

    /// <summary>
    /// Response from updating a group connection's status.
    /// </summary>
    public class GroupConnectionStatusUpdateResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; } // GroupConnection ID that was updated

        [JsonProperty("status")]
        public string Status { get; set; } // The new status
    }


    // --- Models for User's Perspective of Invites (from Sign-In response) ---
    // The structure shown in `signing game users in.md` for `group_invites` is more complex
    // than a simple GroupConnectionResponse. If you need to deserialize *that specific structure*
    // (e.g., when processing the sign-in response), you'd need a dedicated model for it.

    /// <summary>
    /// Represents a group invitation as seen from the invited user's perspective (e.g., in Sign-In response).
    /// This structure is based on the example in "signing game users in.md".
    /// </summary>
    public class MyGroupInvite
    {
        // The "invite": [] field in the example is peculiar and ignored here unless its content is defined.
        // It's assumed the main object in the "group_invites" array *is* the invite.

        [JsonProperty("user")] // The user who IS INVITED (this would be the authenticated user)
        public UserSummary InvitedUser { get; set; }

        [JsonProperty("inviter")] // The user WHO SENT THE INVITE (an admin or group representative)
        public UserSummary Inviter { get; set; }

        [JsonProperty("group")] // The group to which the user is invited
        public Group GroupDetails { get; set; } // Sign-in example shows full group details here

        // Timestamps like "invited_at" or "status" are not explicitly in the sign-in example's
        // group_invites structure but would be logical. If the API provides them, add them.
        // Also, a group_connection_id would be crucial for the invited user to accept/decline.
        // This might be implicitly known or part of an action URL not detailed.
        // For SDK purposes, if an action (accept/decline invite) needs an ID, that ID must come from somewhere.
    }

    /// <summary>
    /// Represents a single attribute item in the payload for creating group attributes.
    /// </summary>
    public class GroupAttributePayloadItem
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("others_can_edit", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OthersCanEdit { get; set; } // Defaults to false if not provided
    }

    /// <summary>
    /// Payload for the Create Group Attribute request.
    /// Contains a list of attributes to be added.
    /// </summary>
    public class CreateGroupAttributesPayload
    {
        // The API documentation shows: --data '{"attributes": [{"key": "theme", "value": "dark"}, ...]}'
        // So the top-level object sent to the API should have an "attributes" property which is a list.
        [JsonProperty("attributes")]
        public List<GroupAttributePayloadItem> Attributes { get; set; }

        public CreateGroupAttributesPayload()
        {
            Attributes = new List<GroupAttributePayloadItem>();
        }
    }

    /// <summary>
    /// Represents a single group attribute as returned by the API.
    /// </summary>
    public class GroupAttributeResponseItem // Renamed from GroupAttribute to avoid conflict if you have a general Attribute model
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("creator_id")]
        public int CreatorId { get; set; } // API doc says "group id not user" for this in the example. Let's clarify.
                                           // If it's truly group ID, the name "CreatorId" is misleading.
                                           // For now, sticking to "creator_id" as per JSON, but type might be GroupId.
                                           // The doc says "The ID of the group who created the attribute" - this is very unusual.
                                           // Typically, creator_id refers to a user.
                                           // Let's assume it IS the user ID of the creator for now, as that's standard.
                                           // If the API *really* returns the group's ID here, we'll adjust.

        [JsonProperty("can_edit")]
        public bool CanEdit { get; set; } // Whether the current authenticated user can edit this attribute
    }

    /// <summary>
    /// Response from the Create Group Attribute request.
    /// Contains a list of the created/updated attributes.
    /// </summary>
    public class CreateGroupAttributesResponse // Or simply List<GroupAttributeResponseItem> if the API returns a direct array
    {
        // The API documentation example response is: {"attributes": [{"id": 1, "key": "theme", ...}, ...]}
        [JsonProperty("attributes")]
        public List<GroupAttributeResponseItem> Attributes { get; set; }

        public CreateGroupAttributesResponse()
        {
            Attributes = new List<GroupAttributeResponseItem>();
        }
    }

    /// <summary>
    /// Response from the Fetch Group Attributes request.
    /// Contains a list of the group's attributes.
    /// </summary>
    public class FetchGroupAttributesResponse
    {
        // Assuming the API returns a structure like: {"attributes": [{"id": 1, "key": "theme", ...}, ...]}
        // similar to the Create response.
        [JsonProperty("attributes")]
        public List<GroupAttributeResponseItem> Attributes { get; set; }

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
        [JsonProperty("key")]
        public string Key { get; set; } // The key of the attribute to modify

        [JsonProperty("value")]
        public string Value { get; set; } // The new value for the attribute
    }

}