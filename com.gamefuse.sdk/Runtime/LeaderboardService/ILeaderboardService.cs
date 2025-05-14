using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface ILeaderboardService
    {
        /// <summary>
        /// Adds a new leaderboard entry for the current user.
        /// </summary>
        /// <param name="userId">The user ID for which to add the leaderboard entry</param>
        /// <param name="leaderboardName">The name of the leaderboard</param>
        /// <param name="score">The score to add</param>
        /// <returns>A response confirming the entry was added</returns>
        Task<MessageResponse> AddLeaderboardEntryAsync(int userId, string leaderboardName, int score);
        
        /// <summary>
        /// Adds a new leaderboard entry with metadata for the current user.
        /// </summary>
        /// <param name="userId">The user ID for which to add the leaderboard entry</param>
        /// <param name="leaderboardName">The name of the leaderboard</param>
        /// <param name="score">The score to add</param>
        /// <param name="metadata">Additional metadata to store with the entry</param>
        /// <returns>A response confirming the entry was added</returns>
        Task<MessageResponse> AddLeaderboardEntryAsync(int userId, string leaderboardName, int score, object metadata);
        
        /// <summary>
        /// Clears all leaderboard entries for the specified user.
        /// </summary>
        /// <param name="userId">The user ID for which to clear entries</param>
        /// <returns>A response confirming the entries were cleared</returns>
        Task<MessageResponse> ClearLeaderboardEntriesAsync(int userId);
        
        /// <summary>
        /// Gets leaderboard entries for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to get entries for</param>
        /// <param name="limit">The maximum number of entries to return</param>
        /// <param name="leaderboardName">Optional name of the leaderboard to filter by</param>
        /// <param name="onePerUser">Whether to return only one entry per user</param>
        /// <returns>A list of leaderboard entries</returns>
        Task<LeaderboardEntriesResponse> GetUserLeaderboardEntriesAsync(int userId, int limit, string leaderboardName = null, bool onePerUser = false);
        
        /// <summary>
        /// Gets leaderboard entries for a specific game and leaderboard name.
        /// </summary>
        /// <param name="gameId">The game ID to get entries for</param>
        /// <param name="leaderboardName">The name of the leaderboard</param>
        /// <param name="limit">The maximum number of entries to return</param>
        /// <returns>A list of leaderboard entries</returns>
        Task<LeaderboardEntriesResponse> GetGameLeaderboardEntriesAsync(int gameId, string leaderboardName, int limit);
    }
}