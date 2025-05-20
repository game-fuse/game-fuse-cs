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
        /// The user's system email (combination of ID and email).
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// The user's actual email used for notifications and login.
        /// </summary>
        [JsonProperty("display_email")]
        public string DisplayEmail { get; internal set; }

        /// <summary>
        /// Number of credits the user has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// A generic score metric.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }

        /// <summary>
        /// Timestamp of last login.
        /// </summary>
        [JsonProperty("last_login")]
        public string LastLogin { get; internal set; }

        /// <summary>
        /// Total number of logins.
        /// </summary>
        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; internal set; }

        /// <summary>
        /// Token used for authenticated requests.
        /// </summary>
        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; internal set; }

        /// <summary>
        /// Running API hits for this user.
        /// </summary>
        [JsonProperty("events_total")]
        public int EventsTotal { get; internal set; }

        /// <summary>
        /// Running API hits for this user for the current month.
        /// </summary>
        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; internal set; }

        /// <summary>
        /// Unique game sessions for this user.
        /// </summary>
        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; internal set; }

        /// <summary>
        /// Unique game sessions for this user during the current month.
        /// </summary>
        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; internal set; }

        /// <summary>
        /// Custom attributes associated with the user.
        /// </summary>
        [JsonProperty("game_user_attributes")]
        public IReadOnlyList<UserAttribute> GameUserAttributes { get; internal set; }

        /// <summary>
        /// Store items purchased by the user.
        /// </summary>
        [JsonProperty("game_user_store_items")]
        public IReadOnlyList<StoreItem> GameUserStoreItems { get; internal set; }

        /// <summary>
        /// The user's friends.
        /// </summary>
        [JsonProperty("friends")]
        public IReadOnlyList<Friend> Friends { get; internal set; }

        /// <summary>
        /// Outgoing friend requests sent by the user.
        /// </summary>
        [JsonProperty("outgoing_friend_requests")]
        public IReadOnlyList<FriendRequest> OutgoingFriendRequests { get; internal set; }

        /// <summary>
        /// Incoming friend requests received by the user.
        /// </summary>
        [JsonProperty("incoming_friend_requests")]
        public IReadOnlyList<FriendRequest> IncomingFriendRequests { get; internal set; }

        /// <summary>
        /// Groups that the user is a member of.
        /// </summary>
        [JsonProperty("groups")]
        public IReadOnlyList<GroupSummary> Groups { get; internal set; }

        /// <summary>
        /// Group join requests sent by the user.
        /// </summary>
        [JsonProperty("group_join_requests")]
        public IReadOnlyList<GroupJoinRequest> GroupJoinRequests { get; internal set; }

        /// <summary>
        /// Group invites received by the user.
        /// </summary>
        [JsonProperty("group_invites")]
        public IReadOnlyList<GroupInvite> GroupInvites { get; internal set; }
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