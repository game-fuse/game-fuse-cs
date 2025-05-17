using System.Threading.Tasks;

namespace GameFuseCSharp
{
    /// <summary>
    /// Interface for the Friendship API service.
    /// </summary>
    public interface IFriendshipService
    {
        /// <summary>
        /// Sends a friend request to another user.
        /// </summary>
        /// <param name="username">The username of the player to send the friend request to.</param>
        /// <returns>A response containing a success message and friendship ID.</returns>
        Task<FriendRequestResponse> SendFriendRequestAsync(string username);
        
        /// <summary>
        /// Accepts a pending friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to accept.</param>
        /// <returns>A response containing a success message.</returns>
        Task<FriendshipStatusResponse> AcceptFriendRequestAsync(int friendshipId);
        
        /// <summary>
        /// Declines a pending friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to decline.</param>
        /// <returns>A response containing a success message.</returns>
        Task<FriendshipStatusResponse> DeclineFriendRequestAsync(int friendshipId);
        
        /// <summary>
        /// Cancels a previously sent friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to cancel.</param>
        /// <returns>A response containing a success message.</returns>
        Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId);
        
        /// <summary>
        /// Removes a user from the current user's friend list.
        /// </summary>
        /// <param name="userId">The ID of the user to unfriend.</param>
        /// <returns>A response containing a success message.</returns>
        Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId);
        
        /// <summary>
        /// Retrieves the list of all accepted friends for the current user.
        /// </summary>
        /// <returns>A response containing a list of friends.</returns>
        Task<FriendsResponse> GetFriendsAsync();
        
        /// <summary>
        /// Retrieves a list of all pending friend requests received by the current user.
        /// </summary>
        /// <returns>A response containing a list of incoming friend requests.</returns>
        Task<IncomingFriendRequestsResponse> GetIncomingFriendRequestsAsync();
        
        /// <summary>
        /// Retrieves a list of all pending friend requests sent by the current user.
        /// </summary>
        /// <returns>A response containing a list of outgoing friend requests.</returns>
        Task<OutgoingFriendRequestsResponse> GetOutgoingFriendRequestsAsync();
    }
}
