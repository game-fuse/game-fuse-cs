using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
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

        /// <summary>
        /// Sends a friend request.
        /// </summary>
        /// <param name="friendId">The ID of the user to send the request to.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendFriendRequestAsync(int friendId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.SendFriendRequestAsync(Id, friendId, cancellationToken);
        }

        /// <summary>
        /// Gets all friend requests for the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A dictionary containing incoming and outgoing friend requests.</returns>
        public Task<(IReadOnlyList<FriendRequest> Incoming, IReadOnlyList<FriendRequest> Outgoing)> GetFriendRequestsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.GetFriendRequestsAsync(Id, cancellationToken);
        }

        /// <summary>
        /// Accepts a friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AcceptFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.AcceptFriendRequestAsync(friendshipId, cancellationToken);
        }

        /// <summary>
        /// Rejects a friend request.
        /// </summary>
        /// <param name="friendshipId">The ID of the friendship request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RejectFriendRequestAsync(int friendshipId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.RejectFriendRequestAsync(friendshipId, cancellationToken);
        }

        /// <summary>
        /// Removes a friend from the current user's friends list.
        /// </summary>
        /// <param name="friendId">The ID of the friend to remove.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RemoveFriendAsync(int friendId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.RemoveFriendAsync(Id, friendId, cancellationToken);
        }

        /// <summary>
        /// Searches for users by username.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of users matching the search criteria.</returns>
        public Task<IReadOnlyList<User>> SearchUsersAsync(string query, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _friendService.SearchUsersAsync(query, cancellationToken);
        }
    }
}