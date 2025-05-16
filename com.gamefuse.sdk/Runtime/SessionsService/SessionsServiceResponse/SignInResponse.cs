using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SignInResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("display_email")]
        public string DisplayEmail { get; set; }
        
        [JsonProperty("credits")]
        public int Credits { get; set; }
        
        [JsonProperty("score")]
        public int Score { get; set; }
        
        [JsonProperty("last_login")]
        public string LastLogin { get; set; }
        
        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; set; }
        
        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; set; }
        
        [JsonProperty("events_total")]
        public int EventsTotal { get; set; }
        
        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; set; }
        
        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; set; }
        
        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; set; }
        
        [JsonProperty("game_user_attributes")]
        public List<UserAttribute> GameUserAttributes { get; set; }
        
        [JsonProperty("game_user_store_items")]
        public List<StoreItem> GameUserStoreItems { get; set; }
        
        [JsonProperty("friends")]
        public List<UserInfo> Friends { get; set; }
        
        [JsonProperty("outgoing_friend_requests")]
        public List<FriendRequest> OutgoingFriendRequests { get; set; }
        
        [JsonProperty("incoming_friend_requests")]
        public List<FriendRequest> IncomingFriendRequests { get; set; }
        
        [JsonProperty("groups")]
        public List<GroupResponse> Groups { get; set; }
        
        [JsonProperty("group_join_requests")]
        public List<JoinRequest> GroupJoinRequests { get; set; }
        
        [JsonProperty("group_invites")]
        public List<GroupInvite> GroupInvites { get; set; }
    }
    
    [System.Serializable]
    public class UserAttribute
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    
    [System.Serializable]
    public class StoreItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("category")]
        public string Category { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("cost")]
        public int Cost { get; set; }
        
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
    }
    
    [System.Serializable]
    public class GroupInvite
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("inviter_id")]
        public int InviterId { get; set; }
    }
    
    [System.Serializable]
    public class JoinRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}