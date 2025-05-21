// In a file like Models/FriendshipModels.cs or alongside your User model
using Newtonsoft.Json;
using System.Collections.Generic; // For List in other response models

namespace GameFuse.Models
{
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
        /// </summary>
        [JsonProperty("display_email")]
        public string DisplayEmail { get; internal set; }
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

    // Payload for sending a friend request
    public class SendFriendRequestPayload
    {
        [JsonProperty("username")]
        public string Username { get; set; }
    }

    // Response from sending a friend request
    public class SendFriendRequestResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("friendship_id")]
        public int FriendshipId { get; set; }
    }

    // Payload for updating friendship status (accept/decline)
    public class UpdateFriendshipStatusPayload
    {
        [JsonProperty("status")]
        public string Status { get; set; } // "accepted" or "declined"
    }

    // Response from updating friendship status or canceling
    public class FriendshipStatusResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    // --- Other models you'll need for subsequent friend endpoints ---
    // (Not strictly required for SendFriendRequest, but good to have in mind)

    

    // For GET /api/v3/friends
    public class FriendsListResponse
    {
        [JsonProperty("friends")]
        public List<Friend> Friends { get; set; }
    }


    public class FriendshipDataResponse
    {
        [JsonProperty("friends")]
        public List<Friend> Friends { get; set; }

        [JsonProperty("outgoing_friend_requests")]
        public List<FriendRequest> OutgoingFriendRequests { get; set; }

        [JsonProperty("incoming_friend_requests")]
        public List<FriendRequest> IncomingFriendRequests { get; set; }

        public FriendshipDataResponse()
        {
            Friends = new List<Friend>();
            OutgoingFriendRequests = new List<FriendRequest>();
            IncomingFriendRequests = new List<FriendRequest>();
        }
    }


}
