using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models.Shared
{
    /// <summary>
    /// Represents the status of a friendship between users.
    /// </summary>
    public enum FriendshipStatus
    {
        /// <summary>
        /// The friendship is active.
        /// </summary>
        Active,
        
        /// <summary>
        /// The friendship request is pending acceptance.
        /// </summary>
        Pending,
        
        /// <summary>
        /// The friendship request was accepted.
        /// </summary>
        Accepted,
        
        /// <summary>
        /// The friendship request was declined.
        /// </summary>
        Declined,
        
        /// <summary>
        /// The friendship was canceled.
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Represents a friend of a user.
    /// </summary>
    public class Friend
    {
        /// <summary>
        /// The friend's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The friend's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The friend's system email (combination of ID and email).
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// The friend's actual email used for notifications and login.
        /// </summary>
        [JsonProperty("display_email", NullValueHandling = NullValueHandling.Ignore)]
        public string? DisplayEmail { get; internal set; }

        /// <summary>
        /// Number of credits the friend has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// The friend's score.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }
    }

    /// <summary>
    /// Represents a friend request.
    /// </summary>
    public class FriendRequest
    {
        /// <summary>
        /// The friend's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }
        
        /// <summary>
        /// The friend's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }
        
        /// <summary>
        /// The friend's actual email used for notifications and login.
        /// </summary>
        [JsonProperty("display_email", NullValueHandling = NullValueHandling.Ignore)]
        public string? DisplayEmail { get; internal set; }
        
        /// <summary>
        /// Number of credits the friend has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }
        
        /// <summary>
        /// The friend's score.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }
        
        /// <summary>
        /// The friendship request's unique identifier.
        /// </summary>
        [JsonProperty("friendship_id")]
        public int FriendshipId { get; internal set; }
        
        /// <summary>
        /// When the friend request was sent.
        /// </summary>
        [JsonProperty("requested_at")]
        public string RequestedAt { get; internal set; }
    }

    /// <summary>
    /// Payload for sending a friend request.
    /// </summary>
    public class SendFriendRequestPayload
    {
        /// <summary>
        /// The username of the user to send a friend request to.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }
    }

    /// <summary>
    /// Response from sending a friend request.
    /// </summary>
    public class FriendshipResponse
    {
        /// <summary>
        /// Message indicating the result of the operation.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// The ID of the friendship.
        /// </summary>
        [JsonProperty("friendship_id")]
        public int FriendshipId { get; set; }
    }

    /// <summary>
    /// Payload for updating friendship status.
    /// </summary>
    public class UpdateFriendshipStatusPayload
    {
        /// <summary>
        /// The new status of the friendship ("accepted" or "declined").
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
    /// <summary>
    /// Response from updating friendship status or canceling
    /// </summary>
    public class FriendshipStatusResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// Container for a list of friends.
    /// </summary>
    public class FriendsListResponse
    {
        /// <summary>
        /// The list of friends.
        /// </summary>
        [JsonProperty("friends")]
        public List<Friend> Friends { get; set; }

        /// <summary>
        /// Creates a new FriendsListResponse with an empty list.
        /// </summary>
        public FriendsListResponse()
        {
            Friends = new List<Friend>();
        }
    }

    /// <summary>
    /// Comprehensive friendship data response including friends and friend requests.
    /// </summary>
    public class FriendshipDataResponse
    {
        /// <summary>
        /// The list of friends.
        /// </summary>
        [JsonProperty("friends")]
        public List<Friend> Friends { get; set; }

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
        /// Creates a new FriendshipDataResponse with empty lists.
        /// </summary>
        public FriendshipDataResponse()
        {
            Friends = new List<Friend>();
            OutgoingFriendRequests = new List<FriendRequest>();
            IncomingFriendRequests = new List<FriendRequest>();
        }
    }
}