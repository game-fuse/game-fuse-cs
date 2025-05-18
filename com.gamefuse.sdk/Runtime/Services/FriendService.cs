using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for friend-related operations.
    /// </summary>
    public class FriendService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the FriendService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public FriendService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Gets all friends for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of friends.</returns>
        public async Task<IReadOnlyList<Friend>> GetFriendsAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<Friend>>($"users/{userId}/friends", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Sends a friend request.
        /// </summary>
        /// <param name="userId">The ID of the user sending the request.</param>
        /// <param name="friendId">The ID of the user to send the request to.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendFriendRequestAsync(int userId, int friendId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (friendId <= 0) throw new ArgumentOutOfRangeException(nameof(friendId), "Friend ID must be positive.");
            
            var request = new Dictionary<string, int>
            {
                ["user_id"] = userId,
                ["friend_id"] = friendId
            };
            
            return _transport.PostAsync<Dictionary<string, int>>("friends/requests", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets all friend requests for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A dictionary containing incoming and outgoing friend requests.</returns>
        public async Task<(IReadOnlyList<FriendRequest> Incoming, IReadOnlyList<FriendRequest> Outgoing)> GetFriendRequestsAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<Dictionary<string, List<FriendRequest>>>($"users/{userId}/friend_requests", null, cancellationToken);
            
            List<FriendRequest> incoming = response.ContainsKey("incoming") ? response["incoming"] : new List<FriendRequest>();
            List<FriendRequest> outgoing = response.ContainsKey("outgoing") ? response["outgoing"] : new List<FriendRequest>();
            
            return (incoming.AsReadOnly(), outgoing.AsReadOnly());
        }

        /// <summary>
        /// Accepts a friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AcceptFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            if (friendshipId <= 0) throw new ArgumentOutOfRangeException(nameof(friendshipId), "Friendship ID must be positive.");
            
            return _transport.PutAsync<Dictionary<string, object>>($"friends/requests/{friendshipId}/accept", new Dictionary<string, object>(), null, cancellationToken);
        }

        /// <summary>
        /// Rejects a friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RejectFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            if (friendshipId <= 0) throw new ArgumentOutOfRangeException(nameof(friendshipId), "Friendship ID must be positive.");
            
            return _transport.DeleteAsync($"friends/requests/{friendshipId}", null, cancellationToken);
        }

        /// <summary>
        /// Removes a friend.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="friendId">The ID of the friend to remove.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RemoveFriendAsync(int userId, int friendId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (friendId <= 0) throw new ArgumentOutOfRangeException(nameof(friendId), "Friend ID must be positive.");
            
            return _transport.DeleteAsync($"users/{userId}/friends/{friendId}", null, cancellationToken);
        }

        /// <summary>
        /// Searches for users by username.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of users matching the search criteria.</returns>
        public async Task<IReadOnlyList<User>> SearchUsersAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));
            
            var response = await _transport.GetAsync<List<User>>($"users/search?query={Uri.EscapeDataString(query)}", null, cancellationToken);
            return response.AsReadOnly();
        }
    }
}