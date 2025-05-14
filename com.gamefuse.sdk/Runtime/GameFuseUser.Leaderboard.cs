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
        /// <param name="metadata">Additional metadata to store with the entry</param>
        /// <returns>A response confirming the entry was added</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
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
        public async Task<LeaderboardEntriesResponse> GetGameLeaderboardEntriesAsync(string leaderboardName, int limit)
        {
            try
            {
                int gameId = GameFuse.GetGameId() ?? throw new ApiException("Game ID not set", 400);
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