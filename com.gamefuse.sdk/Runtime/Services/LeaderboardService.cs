using GameFuse.Models;
using GameFuse.Models.Shared;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for leaderboard related operations.
    /// </summary>
    public class LeaderboardService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the LeaderboardService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public LeaderboardService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Submits a new leaderboard entry for a user.
        /// </summary>
        /// <param name="userId">The ID of the user submitting the entry.</param>
        /// <param name="leaderboardName">Name of the leaderboard within the game.</param>
        /// <param name="score">Score for the leaderboard.</param>
        /// <param name="metadata">Optional metadata for the leaderboard entry.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The response with user details.</returns>
        public Task<User> SubmitLeaderboardEntryAsync(
            int userId,
            string leaderboardName,
            double score,
            Dictionary<string, object> metadata = null,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(leaderboardName)) throw new ArgumentException("Leaderboard name cannot be null or empty.", nameof(leaderboardName));
            
            var request = new Dictionary<string, object>
            {
                ["leaderboard_name"] = leaderboardName,
                ["score"] = score
            };
            
            if (metadata != null && metadata.Count > 0)
            {
                request["metadata"] = metadata;
            }
            
            return _transport.PostAsync<Dictionary<string, object>, User>($"users/{userId}/add_leaderboard_entry", request, null, cancellationToken);
        }

        /// <summary>
        /// Clears all leaderboard entries for a specific user and leaderboard.
        /// </summary>
        /// <param name="userId">The ID of the user whose entries will be cleared.</param>
        /// <param name="leaderboardName">Name of the leaderboard to clear entries from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The response with user details.</returns>
        public Task<User> ClearLeaderboardEntriesAsync(
            int userId,
            string leaderboardName,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(leaderboardName)) throw new ArgumentException("Leaderboard name cannot be null or empty.", nameof(leaderboardName));
            
            var request = new Dictionary<string, object>
            {
                ["leaderboard_name"] = leaderboardName
            };
            
            return _transport.PostAsync<Dictionary<string, object>, User>($"users/{userId}/clear_my_leaderboard_entries", request, null, cancellationToken);
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
            if (gameId <= 0) throw new ArgumentOutOfRangeException(nameof(gameId), "Game ID must be positive.");
            if (string.IsNullOrEmpty(leaderboardName)) throw new ArgumentException("Leaderboard name cannot be null or empty.", nameof(leaderboardName));
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be positive.");
            
            return _transport.GetAsync<LeaderboardEntriesResponse>($"games/{gameId}/leaderboard_entries?leaderboard_name={Uri.EscapeDataString(leaderboardName)}&limit={limit}", null, cancellationToken);
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
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be positive.");
            
            var queryParams = $"limit={limit}";
            if (!string.IsNullOrEmpty(leaderboardName))
            {
                queryParams += $"&leaderboard_name={Uri.EscapeDataString(leaderboardName)}";
            }
            if (onePerUser.HasValue)
            {
                queryParams += $"&one_per_user={onePerUser.Value.ToString().ToLower()}";
            }
            
            return _transport.GetAsync<LeaderboardEntriesResponse>($"users/{userId}/leaderboard_entries?{queryParams}", null, cancellationToken);
        }
    }
}