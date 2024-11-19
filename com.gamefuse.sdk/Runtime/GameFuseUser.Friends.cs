using System.Threading.Tasks;
namespace GameFuseCSharp
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Sends a friend request to another user.
        /// </summary>
        /// <param name="otherUserName">Username of the player to send friend request to</param>
        /// <returns>Response containing friendship_id and confirmation message</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendRequestResponse> SendFriendRequestAsync(string otherUserName)
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                return await friendshipService.SendFriendRequestAsync(otherUserName);

            }
            catch (ApiException)
            {
                throw;
            }
        }
        /// <summary>
        /// Cancels a pending friend request that was previously sent.
        /// Only the request sender can cancel it.
        /// </summary>
        /// <param name="friedshipId">ID of the friendship request to cancel</param>
        /// <returns>Response confirming cancellation</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friedshipId)
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                return await friendshipService.CancelFriendRequestAsync(friedshipId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Accepts a pending friend request.
        /// Only the request recipient can accept it.
        /// </summary>
        /// <param name="friendshipId">ID of the friendship request to accept</param>
        /// <returns>Response confirming acceptance</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendshipStatusResponse> AcceptFriendRequestAsync(int friendshipId)
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                return await friendshipService.AcceptFriendRequestAsync(friendshipId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Declines a pending friend request.
        /// Only the request recipient can decline it.
        /// </summary>
        /// <param name="friendshipId">ID of the friendship request to decline</param>
        /// <returns>Response confirming decline</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendshipStatusResponse> DeclineFriendRequestAsync(int friendshipId)
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                return await friendshipService.DeclineFriendRequestAsync(friendshipId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the list of current friends for this user.
        /// </summary>
        /// <returns>Array of UserInfo objects containing friend details</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<UserInfo[]> GetFriendsAsync()
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                FriendsResponse friendsResponse = await friendshipService.GetFriendsAsync();
                return friendsResponse.friends;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the list of pending friend requests sent to this user.
        /// </summary>
        /// <returns>Array of FriendRequest objects containing incoming request details</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendRequest[]> GetIncomingFriendRequestsAsync()
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                IncomingFriendRequestsResponse friendRequestsResponse = await friendshipService.GetIncomingFriendRequestsAsync();
                return friendRequestsResponse.incoming_friend_requests;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the list of pending friend requests sent by this user.
        /// </summary>
        /// <returns>Array of FriendRequest objects containing outgoing request details</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendRequest[]> GetOutgoingFriendRequestsAsync()
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                OutgoingFriendRequestsResponse friendRequestsResponse = await friendshipService.GetOutgoingFriendRequestsAsync();
                return friendRequestsResponse.outgoing_friend_requests;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Removes a friend from the user's friend list.
        /// </summary>
        /// <param name="userId">ID of the user to unfriend</param>
        /// <returns>Response confirming the unfriend action</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<FriendshipStatusResponse> UnFriendAsync(int userId)
        {
            try
            {
                IFriendshipService friendshipService = new FriendshipService(GameFuse.GetBaseURL(), authenticationToken);
                return await friendshipService.UnfriendPlayerAsync(userId);
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}
