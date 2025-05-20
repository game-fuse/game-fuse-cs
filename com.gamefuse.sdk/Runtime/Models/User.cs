using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GameFuse.Tests.Editor")]
namespace GameFuse.Models
{
    /// <summary>
    /// Represents a GameFuse user.
    /// </summary>
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; internal set; }

        [JsonProperty("username")]
        public string Username { get; internal set; }

        [JsonProperty("email")]
        public string Email { get; internal set; }

        [JsonProperty("display_email")]
        public string DisplayEmail { get; internal set; }

        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        [JsonProperty("score")]
        public int Score { get; internal set; }

        [JsonProperty("last_login")]
        public string LastLogin { get; internal set; }

        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; internal set; }

        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; internal set; }

        [JsonProperty("events_total")]
        public int EventsTotal { get; internal set; }

        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; internal set; }

        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; internal set; }

        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; internal set; }

        [JsonProperty("game_user_attributes")]
        public IReadOnlyList<UserAttribute> GameUserAttributes { get; internal set; }

        [JsonProperty("game_user_store_items")]
        public IReadOnlyList<StoreItem> GameUserStoreItems { get; internal set; } // Assuming StoreItem is defined

        [JsonProperty("friends")]
        public IReadOnlyList<Friend> Friends { get; internal set; } // Assuming Friend is defined

        [JsonProperty("outgoing_friend_requests")]
        public IReadOnlyList<FriendRequest> OutgoingFriendRequests { get; internal set; } // Assuming FriendRequest is defined

        [JsonProperty("incoming_friend_requests")]
        public IReadOnlyList<FriendRequest> IncomingFriendRequests { get; internal set; }

        /// <summary>
        /// Groups that the user is a member of.
        /// The sign-in response shows a list of group summary objects.
        /// </summary>
        [JsonProperty("groups")]
        public IReadOnlyList<GroupSummary> Groups { get; internal set; }

        /// <summary>
        /// Group join requests SENT BY this user that are currently pending.
        /// Each item represents a group_connection initiated by this user.
        /// </summary>
        [JsonProperty("group_join_requests")]
        public IReadOnlyList<GroupConnectionResponse> GroupJoinRequests { get; internal set; } // Changed from placeholder

        /// <summary>
        /// Group invites RECEIVED BY this user that are currently pending their action.
        /// This structure is based on the 'signing game users in.md' example.
        /// </summary>
        [JsonProperty("group_invites")]
        public IReadOnlyList<MyGroupInvite> GroupInvites { get; internal set; } // Changed from placeholder

        // Constructor to initialize lists to prevent null issues if API omits empty ones
        public User()
        {
            GameUserAttributes = new List<UserAttribute>().AsReadOnly();
            GameUserStoreItems = new List<StoreItem>().AsReadOnly();
            Friends = new List<Friend>().AsReadOnly();
            OutgoingFriendRequests = new List<FriendRequest>().AsReadOnly();
            IncomingFriendRequests = new List<FriendRequest>().AsReadOnly();
            Groups = new List<GroupSummary>().AsReadOnly();
            GroupJoinRequests = new List<GroupConnectionResponse>().AsReadOnly();
            GroupInvites = new List<MyGroupInvite>().AsReadOnly();
        }
    }
    /// <summary>
    /// Represents a custom attribute for a user.
    /// </summary>
    public class UserAttribute
    {
        /// <summary>
        /// The attribute's identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The attribute's key.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; internal set; }

        /// <summary>
        /// The attribute's value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; internal set; }
    }

    public class UserAttributes
    {
        [JsonProperty("game_user_attributes")]
        public List<UserAttribute> Attributes { get; set; }

        UserAttributes()
        {
            Attributes = new List<UserAttribute>();
        }
    }

    public class UserAttributePayload
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public UserAttributePayload(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}