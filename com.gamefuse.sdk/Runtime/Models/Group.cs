using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Represents a group in GameFuse.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The group's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The type of group (e.g., "Public", "Private").
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
        /// The maximum number of members allowed in the group.
        /// </summary>
        [JsonProperty("max_group_size")]
        public int MaxGroupSize { get; internal set; }

        /// <summary>
        /// Whether the group can be found in searches.
        /// </summary>
        [JsonProperty("searchable")]
        public bool Searchable { get; internal set; }

        /// <summary>
        /// The current number of members in the group.
        /// </summary>
        [JsonProperty("member_count")]
        public int MemberCount { get; internal set; }

        /// <summary>
        /// List of members in the group (may be null if not provided).
        /// </summary>
        [JsonProperty("members")]
        public IReadOnlyList<GroupMember> Members { get; internal set; }

        /// <summary>
        /// List of admin members in the group (may be null if not provided).
        /// </summary>
        [JsonProperty("admins")]
        public IReadOnlyList<GroupMember> Admins { get; internal set; }
    }

    /// <summary>
    /// Represents a member of a group.
    /// </summary>
    public class GroupMember
    {
        /// <summary>
        /// The member's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The member's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The member's email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// Number of credits the member has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// The member's score.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }
    }

    /// <summary>
    /// Represents a request to join a group.
    /// </summary>
    public class GroupJoinRequest
    {
        /// <summary>
        /// The request's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The group associated with the request.
        /// </summary>
        [JsonProperty("group")]
        public Group Group { get; internal set; }

        /// <summary>
        /// When the request was made.
        /// </summary>
        [JsonProperty("requested_at")]
        public string RequestedAt { get; internal set; }
    }

    /// <summary>
    /// Represents an invitation to join a group.
    /// </summary>
    public class GroupInvite
    {
        /// <summary>
        /// The invite details.
        /// </summary>
        [JsonProperty("invite")]
        public object Invite { get; internal set; }

        /// <summary>
        /// The user being invited.
        /// </summary>
        [JsonProperty("user")]
        public InviteUser User { get; internal set; }

        /// <summary>
        /// The user sending the invitation.
        /// </summary>
        [JsonProperty("inviter")]
        public InviteUser Inviter { get; internal set; }

        /// <summary>
        /// The group associated with the invitation.
        /// </summary>
        [JsonProperty("group")]
        public Group Group { get; internal set; }
    }

    /// <summary>
    /// Represents a user in the context of a group invite.
    /// </summary>
    public class InviteUser
    {
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The user's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The user's email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// The user's display email.
        /// </summary>
        [JsonProperty("display_email")]
        public string DisplayEmail { get; internal set; }

        /// <summary>
        /// Number of credits the user has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// The user's score.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }

        /// <summary>
        /// When the request was made.
        /// </summary>
        [JsonProperty("requested_at")]
        public string RequestedAt { get; internal set; }
    }
}