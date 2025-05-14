using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameFuseCSharp
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Adds a new leaderboard entry for the current user.
        /// </summary>
        /// <param name="leaderboardName">The name of the leaderboard</param>
        /// <param name="score">The score to add</param>
        /// <returns>A response confirming the entry was added</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<MessageResponse> AddLeaderboardEntryAsync(string leaderboardName, int score)
        {
            try
            {
                ILeaderboardService leaderboardService = new LeaderboardService(GameFuse.GetBaseURL(), authenticationToken);
                return await leaderboardService.AddLeaderboardEntryAsync(this.id, leaderboardName, score);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds a new leaderboard entry with metadata for the current user.
        /// </summary>
        /// <param name="leaderboardName">The name of the leaderboard</param>
        /// <param name="score">The score to add</param>
        /// <param name="metadata">Additional metadata to store with the entry. 
        /// Can be a Dictionary<string, object> or an anonymous object with properties.
        /// Will be serialized to JSON for storage.</param>
        /// <returns>A response confirming the entry was added</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        /// <remarks>
        /// Metadata will be serialized to JSON before being sent to the API.
        /// The API expects metadata to be a JSON string that can be parsed into a key-value structure.
        /// Example usage:
        /// <code>
        /// var metadata = new Dictionary&lt;string, object&gt;
        /// {
        ///     { "level", "hard" },
        ///     { "time", 120 },
        ///     { "achievements", new[] { "headshot", "speedrun" } }
        /// };
        /// await user.AddLeaderboardEntryAsync("arcade_mode", 1500, metadata);
        /// </code>
        /// </remarks>
        public async Task<MessageResponse> AddLeaderboardEntryAsync(string leaderboardName, int score, object metadata)
        {
            try
            {
                ILeaderboardService leaderboardService = new LeaderboardService(GameFuse.GetBaseURL(), authenticationToken);
                return await leaderboardService.AddLeaderboardEntryAsync(this.id, leaderboardName, score, metadata);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Clears all leaderboard entries for the current user.
        /// </summary>
        /// <returns>A response confirming the entries were cleared</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        /// <remarks>
        /// This method removes all leaderboard entries associated with the current user across all leaderboards.
        /// Use this method with caution as it cannot be undone and will permanently delete the user's leaderboard history.
        /// 
        /// The operation is performed at the user level rather than for specific leaderboards.
        /// Example usage:
        /// <code>
        /// // Clear all of the current user's leaderboard entries
        /// var response = await user.ClearLeaderboardEntriesAsync();
        /// Debug.Log($"Leaderboard entries cleared: {response.message}");
        /// </code>
        /// </remarks>
        public async Task<MessageResponse> ClearLeaderboardEntriesAsync()
        {
            try
            {
                ILeaderboardService leaderboardService = new LeaderboardService(GameFuse.GetBaseURL(), authenticationToken);
                return await leaderboardService.ClearLeaderboardEntriesAsync(this.id);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets leaderboard entries for the current user.
        /// </summary>
        /// <param name="limit">The maximum number of entries to return</param>
        /// <param name="leaderboardName">Optional name of the leaderboard to filter by</param>
        /// <param name="onePerUser">Whether to return only one entry per user</param>
        /// <returns>A list of leaderboard entries</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        /// <remarks>
        /// This method retrieves leaderboard entries for the currently authenticated user.
        /// 
        /// If <paramref name="leaderboardName"/> is provided, only entries from that specific leaderboard will be returned.
        /// If null or empty, entries from all leaderboards will be returned.
        /// 
        /// The <paramref name="onePerUser"/> parameter is typically set to false when retrieving entries for a single user (as in this method),
        /// since all entries already belong to the same user. It's more relevant when retrieving game-wide entries.
        /// 
        /// Results are sorted by score in descending order (highest scores first).
        /// 
        /// Example usage:
        /// <code>
        /// // Get the current user's top 10 scores on the "weekly_challenge" leaderboard
        /// var entries = await user.GetMyLeaderboardEntriesAsync(10, "weekly_challenge");
        /// 
        /// // Display the entries
        /// foreach (var entry in entries.leaderboardEntries)
        /// {
        ///     Debug.Log($"Score: {entry.score}, Date: {entry.createdAt}");
        ///     if (entry.Metadata.Count > 0)
        ///     {
        ///         Debug.Log($"Metadata: {string.Join(", ", entry.Metadata.Select(kv => $"{kv.Key}={kv.Value}"))}");
        ///     }
        /// }
        /// </code>
        /// </remarks>
        public async Task<LeaderboardEntriesResponse> GetMyLeaderboardEntriesAsync(int limit, string leaderboardName = null, bool onePerUser = false)
        {
            try
            {
                ILeaderboardService leaderboardService = new LeaderboardService(GameFuse.GetBaseURL(), authenticationToken);
                return await leaderboardService.GetUserLeaderboardEntriesAsync(this.id, limit, leaderboardName, onePerUser);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets leaderboard entries for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to get entries for</param>
        /// <param name="limit">The maximum number of entries to return</param>
        /// <param name="leaderboardName">Optional name of the leaderboard to filter by</param>
        /// <param name="onePerUser">Whether to return only one entry per user</param>
        /// <returns>A list of leaderboard entries</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        /// <remarks>
        /// This method retrieves leaderboard entries for a specific user identified by <paramref name="userId"/>.
        /// It requires the current user to be authenticated but can retrieve entries for any user in the game.
        /// 
        /// If <paramref name="leaderboardName"/> is provided, only entries from that specific leaderboard will be returned.
        /// If null or empty, entries from all leaderboards will be returned.
        /// 
        /// The <paramref name="onePerUser"/> parameter is typically set to false when retrieving entries for a single user,
        /// since all entries already belong to the same user. It's more relevant when retrieving game-wide entries.
        /// 
        /// Results are sorted by score in descending order (highest scores first).
        /// 
        /// Example usage:
        /// <code>
        /// // Get another user's top 5 scores from any leaderboard
        /// int friendUserId = 12345;
        /// var entries = await user.GetUserLeaderboardEntriesAsync(friendUserId, 5);
        /// 
        /// if (entries.leaderboardEntries.Count > 0)
        /// {
        ///     Debug.Log($"User {friendUserId}'s top score: {entries.leaderboardEntries[0].score}");
        /// }
        /// else
        /// {
        ///     Debug.Log($"User {friendUserId} has no leaderboard entries");
        /// }
        /// </code>
        /// </remarks>
        public async Task<LeaderboardEntriesResponse> GetUserLeaderboardEntriesAsync(int userId, int limit, string leaderboardName = null, bool onePerUser = false)
        {
            try
            {
                ILeaderboardService leaderboardService = new LeaderboardService(GameFuse.GetBaseURL(), authenticationToken);
                return await leaderboardService.GetUserLeaderboardEntriesAsync(userId, limit, leaderboardName, onePerUser);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets game-wide leaderboard entries for the current game.
        /// </summary>
        /// <param name="leaderboardName">The name of the leaderboard</param>
        /// <param name="limit">The maximum number of entries to return</param>
        /// <returns>A list of leaderboard entries</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        /// <remarks>
        /// This method retrieves the top leaderboard entries for the entire game, across all users.
        /// It's useful for displaying global rankings and high score tables.
        /// 
        /// The <paramref name="leaderboardName"/> parameter is required for this method, as game-wide entries
        /// must be filtered to a specific leaderboard.
        /// 
        /// Results are sorted by score in descending order (highest scores first), and include user information
        /// for each entry, allowing you to display usernames alongside scores.
        /// 
        /// This method requires that GameFuse.SetGameId() has been called to set the current game ID.
        /// If no game ID is set, an ApiException will be thrown.
        /// 
        /// Example usage:
        /// <code>
        /// // Get the top 100 scores on the "all_time_best" leaderboard
        /// var leaderboard = await user.GetGameLeaderboardEntriesAsync("all_time_best", 100);
        /// 
        /// // Display the top 3 players
        /// for (int i = 0; i < Math.Min(3, leaderboard.leaderboardEntries.Count); i++)
        /// {
        ///     var entry = leaderboard.leaderboardEntries[i];
        ///     Debug.Log($"{i+1}. {entry.user.username}: {entry.score} points");
        /// }
        /// 
        /// // Find current user's position
        /// var myEntry = leaderboard.leaderboardEntries.FirstOrDefault(e => e.user.id == user.id);
        /// if (myEntry != null)
        /// {
        ///     int rank = leaderboard.leaderboardEntries.IndexOf(myEntry) + 1;
        ///     Debug.Log($"Your rank: {rank} with score {myEntry.score}");
        /// }
        /// </code>
        /// </remarks>
        public async Task<LeaderboardEntriesResponse> GetGameLeaderboardEntriesAsync(string leaderboardName, int limit)
        {
            try
            {
                string gameIdStr = GameFuse.GetGameId();
                if (string.IsNullOrEmpty(gameIdStr))
                {
                    throw new ApiException(400, "Game ID not set", "No game ID was provided");
                }
                
                if (!int.TryParse(gameIdStr, out int gameId))
                {
                    throw new ApiException(400, "Invalid Game ID format", $"Game ID '{gameIdStr}' is not a valid integer");
                }
                ILeaderboardService leaderboardService = new LeaderboardService(GameFuse.GetBaseURL(), authenticationToken);
                return await leaderboardService.GetGameLeaderboardEntriesAsync(gameId, leaderboardName, limit);
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}