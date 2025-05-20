using GameFuse.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameFuse.Config;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        

        /// <summary>
        /// Submits a new leaderboard entry for the current user.
        /// </summary>
        /// <param name="leaderboardName">Name of the leaderboard within the game.</param>
        /// <param name="score">Score for the leaderboard.</param>
        /// <param name="metadata">Optional metadata for the leaderboard entry.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The response with user details.</returns>
        public Task<SubmitLeaderboardEntryResponse> SubmitLeaderboardEntryAsync(
            string leaderboardName,
            double score,
            Dictionary<string, object> metadata = null,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _leaderboardService.SubmitLeaderboardEntryAsync(
                Id,
                leaderboardName,
                score,
                metadata,
                cancellationToken);
        }

        /// <summary>
        /// Clears all leaderboard entries for the current user for a specific leaderboard.
        /// </summary>
        /// <param name="leaderboardName">Name of the leaderboard to clear entries from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The response with user details.</returns>
        public Task<User> ClearLeaderboardEntriesAsync(
            string leaderboardName,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            if (string.IsNullOrEmpty(leaderboardName))
                throw new ArgumentException("Leaderboard name cannot be null or empty.", nameof(leaderboardName));
                
            return _leaderboardService.ClearLeaderboardEntriesAsync(Id, leaderboardName, cancellationToken);
        }

        /// <summary>
        /// Gets leaderboard entries for a specific leaderboard name.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="leaderboardName">Name of the leaderboard within the game.</param>
        /// <param name="limit">Limit the number of results. Must be >= 1.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of leaderboard entries.</returns>
        public Task<LeaderboardEntriesResponse> GetLeaderboardEntriesAsync(
            int gameId,
            string leaderboardName,
            int limit,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _leaderboardService.GetLeaderboardEntriesAsync(
                gameId,
                leaderboardName,
                limit,
                cancellationToken);
        }

        /// <summary>
        /// Gets all leaderboard entries for the current user.
        /// </summary>
        /// <param name="limit">Limit the number of results. Must be >= 1.</param>
        /// <param name="leaderboardName">Optional name of the leaderboard within the game. No value returns all leaderboard entries for the User.</param>
        /// <param name="onePerUser">If true, get only one result per player on the leaderboard.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of leaderboard entries.</returns>
        public Task<LeaderboardEntriesResponse> GetCurrentUserLeaderboardEntriesAsync(
            int limit,
            string leaderboardName = null,
            bool? onePerUser = null,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _leaderboardService.GetUserLeaderboardEntriesAsync(
                Id,
                limit,
                leaderboardName,
                onePerUser,
                cancellationToken);
        }

        /// <summary>
        /// Gets leaderboard entries for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose entries to retrieve.</param>
        /// <param name="limit">Limit the number of results. Must be >= 1.</param>
        /// <param name="leaderboardName">Optional name of the leaderboard within the game. No value returns all leaderboard entries for the User.</param>
        /// <param name="onePerUser">If true, get only one result per player on the leaderboard.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of leaderboard entries.</returns>
        public Task<LeaderboardEntriesResponse> GetUserLeaderboardEntriesAsync(
            int userId,
            int limit,
            string leaderboardName = null,
            bool? onePerUser = null,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            return _leaderboardService.GetUserLeaderboardEntriesAsync(
                userId,
                limit,
                leaderboardName,
                onePerUser,
                cancellationToken);
        }
    }
}