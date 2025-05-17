using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for signing in a user.
    /// </summary>
    [System.Serializable]
    public class SignInResponse
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// The user's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }
        
        /// <summary>
        /// The system email address, which is a combination of ID and actual email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        
        /// <summary>
        /// The user's actual email address used for login and notifications.
        /// </summary>
        [JsonProperty("display_email")]
        public string DisplayEmail { get; set; }
        
        /// <summary>
        /// The number of credits the user has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; set; }
        
        /// <summary>
        /// The user's score in the game.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; set; }
        
        /// <summary>
        /// The timestamp of the user's last login.
        /// </summary>
        [JsonProperty("last_login")]
        public string LastLogin { get; set; }
        
        /// <summary>
        /// The number of times the user has logged in.
        /// </summary>
        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; set; }
        
        /// <summary>
        /// The authentication token for the user session.
        /// </summary>
        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; set; }
        
        /// <summary>
        /// The total number of API events for this user.
        /// </summary>
        [JsonProperty("events_total")]
        public int EventsTotal { get; set; }
        
        /// <summary>
        /// The number of API events for this user in the current month.
        /// </summary>
        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; set; }
        
        /// <summary>
        /// The total number of unique game sessions for this user.
        /// </summary>
        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; set; }
        
        /// <summary>
        /// The number of unique game sessions for this user in the current month.
        /// </summary>
        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; set; }
        
        /// <summary>
        /// The list of user attributes.
        /// </summary>
        [JsonProperty("game_user_attributes")]
        public List<UserAttribute> GameUserAttributes { get; set; }
        
        /// <summary>
        /// The list of store items purchased by the user.
        /// </summary>
        [JsonProperty("game_user_store_items")]
        public List<StoreItem> GameUserStoreItems { get; set; }
        
        /// <summary>
        /// The list of user's friends.
        /// </summary>
        [JsonProperty("friends")]
        public List<UserInfo> Friends { get; set; }
        
        /// <summary>
        /// The list of outgoing friend requests.
        /// </summary>
        [JsonProperty("outgoing_friend_requests")]
        public List<FriendRequest> OutgoingFriendRequests { get; set; }
        
        /// <summary>
        /// The list of incoming friend requests.
        /// </summary>
        [JsonProperty("incoming_friend_requests")]
        public List<FriendRequest> IncomingFriendRequests { get; set; }
        
        /// <summary>
        /// The list of groups the user belongs to.
        /// </summary>
        [JsonProperty("groups")]
        public List<GroupResponse> Groups { get; set; }
        
        /// <summary>
        /// The list of pending group join requests.
        /// </summary>
        [JsonProperty("group_join_requests")]
        public List<JoinRequest> GroupJoinRequests { get; set; }
        
        /// <summary>
        /// The list of group invites for the user.
        /// </summary>
        [JsonProperty("group_invites")]
        public List<GroupInvite> GroupInvites { get; set; }
    }
    
    /// <summary>
    /// Represents a user attribute with a key-value pair.
    /// </summary>
    [System.Serializable]
    public class UserAttribute
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
    }
    
    /// <summary>
    /// Represents a store item in the game.
    /// </summary>
    [System.Serializable]
    public class StoreItem
    {
        /// <summary>
        /// The ID of the store item.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// The name of the store item.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The category of the store item.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }
        
        /// <summary>
        /// The description of the store item.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The cost of the store item in credits.
        /// </summary>
        [JsonProperty("cost")]
        public int Cost { get; set; }
        
        /// <summary>
        /// The URL of the store item's icon.
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
    }
    
    /// <summary>
    /// Represents an invitation to join a group.
    /// </summary>
    [System.Serializable]
    public class GroupInvite
    {
        /// <summary>
        /// The ID of the invite.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// The ID of the group.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
        
        /// <summary>
        /// The status of the invite.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
        
        /// <summary>
        /// The ID of the user who sent the invite.
        /// </summary>
        [JsonProperty("inviter_id")]
        public int InviterId { get; set; }
    }
    
    /// <summary>
    /// Represents a request to join a group.
    /// </summary>
    [System.Serializable]
    public class JoinRequest
    {
        /// <summary>
        /// The ID of the request.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// The ID of the group.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
        
        /// <summary>
        /// The status of the request.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}