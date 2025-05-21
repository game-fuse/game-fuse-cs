using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models.Shared
{
    /// <summary>
    /// Represents a GameFuse user with comprehensive user data.
    /// This model consolidates the previous User and SubmitLeaderboardEntryResponse models.
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
        [JsonProperty("display_email", NullValueHandling = NullValueHandling.Ignore)]
        public string? DisplayEmail { get; internal set; }

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
        /// Timestamp of the user's last login.
        /// </summary>
        [JsonProperty("last_login")]
        public string LastLogin { get; internal set; }

        /// <summary>
        /// The number of times the user has logged in.
        /// </summary>
        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; internal set; }

        /// <summary>
        /// The user's authentication token.
        /// </summary>
        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; internal set; }

        /// <summary>
        /// Total number of events for this user.
        /// </summary>
        [JsonProperty("events_total")]
        public int EventsTotal { get; internal set; }

        /// <summary>
        /// Number of events for this user in the current month.
        /// </summary>
        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; internal set; }

        /// <summary>
        /// Total number of game sessions for this user.
        /// </summary>
        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; internal set; }

        /// <summary>
        /// Number of game sessions for this user in the current month.
        /// </summary>
        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; internal set; }

        /// <summary>
        /// The user's custom attributes.
        /// </summary>
        [JsonProperty("game_user_attributes")]
        public IReadOnlyList<UserAttribute> GameUserAttributes { get; internal set; }

        /// <summary>
        /// Store items owned by the user.
        /// </summary>
        [JsonProperty("game_user_store_items")]
        public IReadOnlyList<StoreItem> GameUserStoreItems { get; internal set; }

        /// <summary>
        /// The user's friends.
        /// </summary>
        [JsonProperty("friends")]
        public IReadOnlyList<Friend> Friends { get; internal set; }

        /// <summary>
        /// Friend requests sent by the user.
        /// </summary>
        [JsonProperty("outgoing_friend_requests")]
        public IReadOnlyList<FriendRequest> OutgoingFriendRequests { get; internal set; }

        /// <summary>
        /// Friend requests received by the user.
        /// </summary>
        [JsonProperty("incoming_friend_requests")]
        public IReadOnlyList<FriendRequest> IncomingFriendRequests { get; internal set; }

        /// <summary>
        /// Groups that the user is a member of.
        /// </summary>
        [JsonProperty("groups")]
        public IReadOnlyList<GroupSummary> Groups { get; internal set; }

        /// <summary>
        /// Group join requests sent by this user that are currently pending.
        /// </summary>
        [JsonProperty("group_join_requests")]
        public IReadOnlyList<GroupConnectionResponse> GroupJoinRequests { get; internal set; }

        /// <summary>
        /// Group invites received by this user that are currently pending their action.
        /// </summary>
        [JsonProperty("group_invites")]
        public IReadOnlyList<MyGroupInvite> GroupInvites { get; internal set; }

        /// <summary>
        /// Creates a new User instance with empty collections.
        /// </summary>
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

    /// <summary>
    /// Container for a list of user attributes.
    /// </summary>
    public class UserAttributes
    {
        /// <summary>
        /// The list of user attributes.
        /// </summary>
        [JsonProperty("game_user_attributes")]
        public List<UserAttribute> Attributes { get; set; }

        /// <summary>
        /// Creates a new UserAttributes instance with an empty list.
        /// </summary>
        public UserAttributes()
        {
            Attributes = new List<UserAttribute>();
        }
    }

    /// <summary>
    /// Payload for creating or updating a user attribute.
    /// </summary>
    public class UserAttributePayload
    {
        /// <summary>
        /// The attribute's key.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// The attribute's value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Creates a new UserAttributePayload with the specified key and value.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        public UserAttributePayload(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}