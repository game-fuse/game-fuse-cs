// Models/GroupModels.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    public class CreateGroupPayload
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("group_type", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupType { get; set; } // e.g., "Public", "Private"

        [JsonProperty("max_group_size", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxGroupSize { get; set; }

        [JsonProperty("can_auto_join", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CanAutoJoin { get; set; }

        [JsonProperty("is_invite_only", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInviteOnly { get; set; }

        [JsonProperty("searchable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Searchable { get; set; } // Not in doc but good to have if API supports

        [JsonProperty("admins_only_can_create_attributes", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AdminsOnlyCanCreateAttributes { get; set; }
    }

    // This will be the comprehensive Group model, also used as response for CreateGroup
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

        // The API doc for Create Group response shows members and admins.
        // We can use the existing Friend/User model for members/admins if the structure is similar.
        // Assuming 'Friend' model (id, username, email, credits, score) is suitable here.
        [JsonProperty("members")]
        public List<UserSummary> Members { get; internal set; } // Using a UserSummary or Friend-like model

        [JsonProperty("admins")]
        public List<UserSummary> Admins { get; internal set; } // Using a UserSummary or Friend-like model

        // These might not be populated on create, but are part of a full group object
        [JsonProperty("join_requests", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupJoinRequest> JoinRequests { get; internal set; }

        [JsonProperty("invites", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupInvite> Invites { get; internal set; }

        // Constructor to initialize lists
        public Group()
        {
            Members = new List<UserSummary>();
            Admins = new List<UserSummary>();
            JoinRequests = new List<GroupJoinRequest>();
            Invites = new List<GroupInvite>();
        }
    }

    /// <summary>
    /// Represents a summary of a group, typically used in lists.
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

        // admins_only_can_create_attributes might not be in summary, check API actual response
        // For now, assuming it's not, to keep summary distinct from full Group.

        [JsonProperty("member_count")]
        public int MemberCount { get; internal set; }
    }

    public class FetchAllGroupsResponse
    {
        [JsonProperty("groups")]
        public List<GroupSummary> Groups { get; set; }

        public FetchAllGroupsResponse()
        {
            Groups = new List<GroupSummary>();
        }
    }

    // A simplified user model for listings like members/admins, if different from full User/Friend
    public class UserSummary // Or reuse Friend if appropriate
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("email")] // System email
        public string Email { get; set; }
        // display_email might not be in this summary
        [JsonProperty("credits")]
        public int Credits { get; set; }
        [JsonProperty("score")]
        public int Score { get; set; }
    }

    public class SendGroupConnectionRequestPayload
    {
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; } // The ID of the user requesting or being connected
    }

    public class GroupConnectionResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; } // This is the GroupConnection ID

        [JsonProperty("status")]
        public string Status { get; set; } // e.g., "pending", "accepted"

        [JsonProperty("inviter_id")] // Could be self if requesting, or an admin if invited
        public int? InviterId { get; set; } // Nullable if not applicable

        [JsonProperty("user")] // The user involved in the connection
        public UserSummary User { get; set; } // Assuming UserSummary is appropriate here
    }

    public class UpdateGroupConnectionStatusPayload
    {
        [JsonProperty("status")]
        public string Status { get; set; } // "accepted" or "declined"
    }

    public class GroupConnectionStatusUpdateResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; } // GroupConnection ID that was updated

        [JsonProperty("status")]
        public string Status { get; set; } // The new status ("accepted" or "declined")
    }

    // Placeholder for more detailed models if needed later
    public class GroupJoinRequest { /* ... */ }
    public class GroupInvite { /* ... */ }
}