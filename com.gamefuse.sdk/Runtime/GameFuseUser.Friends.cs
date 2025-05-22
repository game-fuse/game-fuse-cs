using GameFuse.Models.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Retrieves the list of incoming friend requests for this user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of incoming friend requests.</returns>
        public async Task<IReadOnlyList<FriendRequest>> GetIncomingFriendRequestsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            // Get the full friendship data that includes incoming requests
            FriendshipDataResponse friendshipData = await GetFriendshipDataAsync(cancellationToken);

            // Return the incoming requests
            return friendshipData.IncomingFriendRequests.AsReadOnly();
        }

        /// <summary>
        /// Retrieves the list of outgoing friend requests for this user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of outgoing friend requests.</returns>
        public async Task<IReadOnlyList<FriendRequest>> GetOutgoingFriendRequestsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            // Get the full friendship data that includes outgoing requests
            FriendshipDataResponse friendshipData = await GetFriendshipDataAsync(cancellationToken);

            // Return the outgoing requests
            return friendshipData.OutgoingFriendRequests.AsReadOnly();
        }
        /*
        /// <summary>
        /// Gets all friends for the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of friends.</returns>
        public Task<IReadOnlyList<Friend>> GetFriendsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.GetFriendsAsync(Id, cancellationToken);
        }
        */
        /// <summary>
        /// Sends a friend request to another user by their username.
        /// </summary>
        /// <param name="friendUsername">The username of the user to send the request to.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a message and the friendship ID.</returns>
        public async Task<FriendshipResponse> SendFriendRequestAsync(string friendUsername, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated(); // Ensures the current GameFuseUser instance is authenticated
            // The FriendService.SendFriendRequestAsync doesn't need the current user's ID,
            // as it's inferred from the authentication token provided by this GameFuseUser instance's transport.
            return await _friendService.SendFriendRequestAsync(friendUsername, cancellationToken);
        }

        /// <summary>
        /// Accepts a pending friend request. This action is performed by the current GameFuseUser instance.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to accept.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipStatusResponse> AcceptFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated(); // Ensures this GameFuseUser instance is authenticated
            // The FriendService.AcceptFriendRequestAsync uses the auth token of the calling GameFuseUser instance.
            return await _friendService.AcceptFriendRequestAsync(friendshipId, cancellationToken);
        }

        /// <summary>
        /// Declines a pending friend request. This action is performed by the current GameFuseUser instance.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to decline.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipStatusResponse> DeclineFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _friendService.DeclineFriendRequestAsync(friendshipId, cancellationToken);
        }

        /// <summary>
        /// Cancels a friend request previously sent by this user.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _friendService.CancelFriendRequestAsync(friendshipId, cancellationToken);
        }

        /// <summary>
        /// Removes a friend from this user's friend list.
        /// </summary>
        /// <param name="friendUserId">The ID of the user to unfriend.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a confirmation message.</returns>
        public async Task<FriendshipResponse> UnfriendPlayerAsync(int friendUserId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _friendService.UnfriendPlayerAsync(friendUserId, cancellationToken);
        }

        /// <summary>
        /// Retrieves the list of friends, outgoing, and incoming friendship requests for this user.
        /// Updates the internal state of this GameFuseUser instance with the fetched data.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing lists of friends, outgoing requests, and incoming requests.</returns>
        public async Task<FriendshipDataResponse> GetFriendshipDataAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated(); // Ensures this GameFuseUser instance is authenticated

            FriendshipDataResponse friendshipData = await _friendService.GetFriendshipDataAsync(cancellationToken);

            // Update the internal _userData state of this GameFuseUser instance
            if (friendshipData != null)
            {
                // Assuming _userData.Friends, _userData.OutgoingFriendRequests, etc. are writable lists or IReadOnlyList
                // If they are IReadOnlyList, you might need to re-assign the _userData object or have internal setters.
                // For simplicity, let's assume User model properties are settable internally or the model is replaced.
                // A common pattern is to have internal setters for these collections in the User model.
                _userData.Friends = friendshipData.Friends; // Or new ReadOnlyCollection<Friend>(friendshipData.Friends)
                _userData.OutgoingFriendRequests = friendshipData.OutgoingFriendRequests;
                _userData.IncomingFriendRequests = friendshipData.IncomingFriendRequests;
            }

            return friendshipData;
        }

        /// <summary>
        /// Retrieves the list of all accepted friends for this user.
        /// Updates the internal friends list of this GameFuseUser instance.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of friends.</returns>
        public async Task<IReadOnlyList<Friend>> GetFriendsListAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();

            FriendsListResponse response = await _friendService.GetFriendsListAsync(cancellationToken);

            if (response?.Friends != null)
            {
                _userData.Friends = response.Friends; // Update internal state
                return response.Friends.AsReadOnly();
            }

            _userData.Friends = new List<Friend>(); // Ensure it's an empty list if response or Friends is null
            return _userData.Friends;
        }

        /// <summary>
        /// Retrieves the list of all accepted friends for a SPECIFIED OTHER user.
        /// This GameFuseUser instance (authenticated user) is making the request.
        /// </summary>
        /// <param name="otherUserId">The ID of the user whose friends list to fetch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of the other user's friends.</returns>
        public async Task<IReadOnlyList<Friend>> GetFriendsListForOtherUserAsync(int otherUserId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated(); // The calling user must be authenticated
            if (otherUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(otherUserId), "Other User ID must be positive.");
            }

            FriendsListResponse response = await _friendService.GetFriendsListAsync(otherUserId, cancellationToken);

            // We don't update the current _userData.Friends here, as this is for another user.
            return response?.Friends?.AsReadOnly() ?? (IReadOnlyList<Friend>)new List<Friend>().AsReadOnly();
        }
    }
}