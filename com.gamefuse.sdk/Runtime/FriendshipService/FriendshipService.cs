using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace GameFuseCSharp
{
    /// <summary>
    /// Implementation of the Friendship API service.
    /// </summary>
    public class FriendshipService : AbstractService, IFriendshipService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendshipService"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="token">The authentication token.</param>
        public FriendshipService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        /// <summary>
        /// Sends a friend request to another user.
        /// </summary>
        /// <param name="username">The username of the player to send the friend request to.</param>
        /// <returns>A response containing a success message and friendship ID.</returns>
        /// <exception cref="ArgumentException">Thrown when username is null or empty.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<FriendRequestResponse> SendFriendRequestAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username is required", nameof(username));
            }

            string url = $"{_baseUrl}/friendships";
            var requestData = new FriendRequestData { Username = username };
            string jsonBody = SerializeRequest(requestData);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<FriendRequestResponse>(webRequest);
            }
        }

        /// <summary>
        /// Accepts a pending friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to accept.</param>
        /// <returns>A response containing a success message.</returns>
        /// <exception cref="ArgumentException">Thrown when friendshipId is invalid.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<FriendshipStatusResponse> AcceptFriendRequestAsync(int friendshipId)
        {
            if (friendshipId <= 0)
            {
                throw new ArgumentException("A valid friendship ID is required", nameof(friendshipId));
            }

            try
            {
                return await UpdateFriendRequestStatusAsync(friendshipId, FriendRequestStatus.accepted.ToString());
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Declines a pending friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to decline.</param>
        /// <returns>A response containing a success message.</returns>
        /// <exception cref="ArgumentException">Thrown when friendshipId is invalid.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<FriendshipStatusResponse> DeclineFriendRequestAsync(int friendshipId)
        {
            if (friendshipId <= 0)
            {
                throw new ArgumentException("A valid friendship ID is required", nameof(friendshipId));
            }

            try
            {
                return await UpdateFriendRequestStatusAsync(friendshipId, FriendRequestStatus.declined.ToString());
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the status of a friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to update.</param>
        /// <param name="status">The new status to set (accepted or declined).</param>
        /// <returns>A response containing a success message.</returns>
        private async Task<FriendshipStatusResponse> UpdateFriendRequestStatusAsync(int friendshipId, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("Status is required", nameof(status));
            }

            string url = $"{_baseUrl}/friendships/{friendshipId}";
            var statusData = new FriendshipStatusData { Status = status };
            string jsonBody = SerializeRequest(statusData);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.PUT, jsonBody))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        /// <summary>
        /// Cancels a previously sent friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request to cancel.</param>
        /// <returns>A response containing a success message.</returns>
        /// <exception cref="ArgumentException">Thrown when friendshipId is invalid.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId)
        {
            if (friendshipId <= 0)
            {
                throw new ArgumentException("A valid friendship ID is required", nameof(friendshipId));
            }

            string url = $"{_baseUrl}/friendships/{friendshipId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.DELETE))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        /// <summary>
        /// Removes a user from the current user's friend list.
        /// </summary>
        /// <param name="userId">The ID of the user to unfriend.</param>
        /// <returns>A response containing a success message.</returns>
        /// <exception cref="ArgumentException">Thrown when userId is invalid.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("A valid user ID is required", nameof(userId));
            }

            string url = $"{_baseUrl}/unfriend?user_id={userId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.DELETE))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        /// <summary>
        /// Retrieves the list of all accepted friends for the current user.
        /// </summary>
        /// <returns>A response containing a list of friends.</returns>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<FriendsResponse> GetFriendsAsync()
        {
            string url = $"{_baseUrl}/friends";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<FriendsResponse>(webRequest);
            }
        }

        /// <summary>
        /// Retrieves a list of all pending friend requests received by the current user.
        /// </summary>
        /// <returns>A response containing a list of incoming friend requests.</returns>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<IncomingFriendRequestsResponse> GetIncomingFriendRequestsAsync()
        {
            string url = $"{_baseUrl}/incoming_friend_requests";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<IncomingFriendRequestsResponse>(webRequest);
            }
        }

        /// <summary>
        /// Retrieves a list of all pending friend requests sent by the current user.
        /// </summary>
        /// <returns>A response containing a list of outgoing friend requests.</returns>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<OutgoingFriendRequestsResponse> GetOutgoingFriendRequestsAsync()
        {
            string url = $"{_baseUrl}/outgoing_friend_requests";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<OutgoingFriendRequestsResponse>(webRequest);
            }
        }
    }
}