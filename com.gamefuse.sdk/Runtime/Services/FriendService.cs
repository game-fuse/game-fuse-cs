using GameFuse.Exceptions;
using GameFuse.Models.Shared;
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

        /*
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
        */
        /// <summary>
        /// Sends a friend request to a user specified by their username.
        /// The current authenticated user (implicit from auth token) sends the request.
        /// </summary>
        /// <param name="targetUsername">The username of the player to send a friend request to.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a message and the friendship ID.</returns>
        public async Task<FriendshipResponse> SendFriendRequestAsync(string targetUsername, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(targetUsername))
            {
                throw new ArgumentException("Target username cannot be null or empty.", nameof(targetUsername));
            }

            var payload = new SendFriendRequestPayload
            {
                Username = targetUsername
            };

            // API Path: POST /api/v3/friendships
            // The _baseUrl in UnityWebRequestTransport is "https://gamefuse.co/api/v3" (or configured).
            // So the path here should be "friendships".
            return await _transport.PostAsync<SendFriendRequestPayload, FriendshipResponse>("friendships", payload, null, cancellationToken);
        }

        /// <summary>
        /// Accepts a pending friend request.
        /// The current authenticated user (implicit from auth token) accepts the request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to accept.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipStatusResponse> AcceptFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            if (friendshipId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(friendshipId), "Friendship ID must be positive.");
            }

            var payload = new UpdateFriendshipStatusPayload
            {
                Status = "accepted"
            };

            // API Path: PUT /api/v3/friendships/{id}
            return await _transport.PutAsync<UpdateFriendshipStatusPayload, FriendshipStatusResponse>($"friendships/{friendshipId}", payload, null, cancellationToken);
        }

        /// <summary>
        /// Declines a pending friend request.
        /// The current authenticated user (implicit from auth token) declines the request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to decline.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipStatusResponse> DeclineFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            if (friendshipId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(friendshipId), "Friendship ID must be positive.");
            }

            var payload = new UpdateFriendshipStatusPayload
            {
                Status = "declined" // Key change here
            };

            // API Path: PUT /api/v3/friendships/{id}
            return await _transport.PutAsync<UpdateFriendshipStatusPayload, FriendshipStatusResponse>($"friendships/{friendshipId}", payload, null, cancellationToken);
        }

        /// <summary>
        /// Cancels a pending friend request that was previously sent by the current authenticated user.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            if (friendshipId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(friendshipId), "Friendship ID must be positive.");
            }

            // API Path: DELETE /api/v3/friendships/{id}
            // No payload for DELETE in this case.
            return await _transport.DeleteAsync<FriendshipStatusResponse>($"friendships/{friendshipId}", null, cancellationToken);
        }

        // ... _transport field and constructor ...
        // ... SendFriendRequestAsync, AcceptFriendRequestAsync, DeclineFriendRequestAsync, CancelFriendRequestAsync ...

        /// <summary>
        /// Removes an accepted friend from the current authenticated user's friend list.
        /// </summary>
        /// <param name="friendUserId">The ID of the user to unfriend.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipResponse> UnfriendPlayerAsync(int friendUserId, CancellationToken cancellationToken = default)
        {
            if (friendUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(friendUserId), "Friend User ID must be positive.");
            }

            // API Path: DELETE /api/v3/unfriend?user_id={friendUserId}
            // The user_id is a query parameter.
            string path = $"unfriend?user_id={friendUserId}";

            // No payload for this DELETE request.
            // The _transport.DeleteAsync should handle query parameters in the path correctly.
            return await _transport.DeleteAsync<FriendshipResponse>(path, null, cancellationToken);
        }

        /// <summary>
        /// Retrieves the list of friends, outgoing, and incoming friendship requests for the current authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing lists of friends, outgoing requests, and incoming requests.</returns>
        public async Task<FriendshipDataResponse> GetFriendshipDataAsync(CancellationToken cancellationToken = default)
        {
            // API Path: GET /api/v3/friendships
            // The user ID is implicit from the authentication token.
            var response = await _transport.GetAsync<FriendshipDataResponse>("friendships", null, cancellationToken);

            // Ensure lists are not null even if API omits them when empty
            if (response == null) return new FriendshipDataResponse(); // Return empty initialized object
            response.Friends ??= new System.Collections.Generic.List<Friend>();
            response.OutgoingFriendRequests ??= new System.Collections.Generic.List<FriendRequest>();
            response.IncomingFriendRequests ??= new System.Collections.Generic.List<FriendRequest>();

            return response;
        }

        /// <summary>
        /// Retrieves the list of all accepted friends for the current authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a list of friends.</returns>
        public async Task<FriendsListResponse> GetFriendsListAsync(CancellationToken cancellationToken = default)
        {
            // API Path: GET /api/v3/friends
            // User ID is implicit from the authentication token.
            var response = await _transport.GetAsync<FriendsListResponse>("friends", null, cancellationToken);

            // Ensure Friends list is not null
            if (response == null) return new FriendsListResponse(); // Return empty initialized object
            response.Friends ??= new List<Friend>();

            return response;
        }

        /// <summary>
        /// Retrieves the list of all accepted friends.
        /// If targetUserId is provided and > 0, retrieves for that user.
        /// Otherwise, retrieves for the current authenticated user.
        /// </summary>
        /// <param name="targetUserId">Optional ID of the user whose friends list to fetch. If 0 or less, fetches for the authenticated user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a list of friends.</returns>
        public async Task<FriendsListResponse> GetFriendsListAsync(int targetUserId = 0, CancellationToken cancellationToken = default)
        {
            string path = "friends";
            if (targetUserId > 0)
            {
                path += $"?user_id={targetUserId}";
            }

            // API Path: GET /api/v3/friends OR /api/v3/friends?user_id={targetUserId}
            var response = await _transport.GetAsync<FriendsListResponse>(path, null, cancellationToken);

            if (response == null) return new FriendsListResponse();
            response.Friends ??= new List<Friend>();

            return response;
        }



        /*
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
        */
    }
}