using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Creates a new game round for the current user.
        /// </summary>
        /// <param name="level">The level identifier (optional).</param>
        /// <param name="customData">Custom data for the game round (optional).</param>
        /// <param name="variables">Variables for the game round (optional).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created game round.</returns>
        public Task<GameRound> CreateGameRoundAsync(string level = null, string customData = null, Dictionary<string, string> variables = null, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.CreateGameRoundAsync(Id, level, customData, variables, cancellationToken);
        }

        /// <summary>
        /// Gets a game round by ID.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The game round.</returns>
        public Task<GameRound> GetGameRoundAsync(int gameRoundId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.GetGameRoundAsync(gameRoundId, cancellationToken);
        }

        /// <summary>
        /// Gets all game rounds for the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of game rounds.</returns>
        public Task<IReadOnlyList<GameRound>> GetCurrentUserGameRoundsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.GetGameRoundsForUserAsync(Id, cancellationToken);
        }

        /// <summary>
        /// Updates a game round.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to update.</param>
        /// <param name="score">The new score (optional).</param>
        /// <param name="customData">New custom data (optional).</param>
        /// <param name="variables">New variables (optional).</param>
        /// <param name="ended">Whether the game round has ended (optional).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated game round.</returns>
        public Task<GameRound> UpdateGameRoundAsync(int gameRoundId, int? score = null, string customData = null, Dictionary<string, string> variables = null, bool? ended = null, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.UpdateGameRoundAsync(gameRoundId, score, customData, variables, ended, cancellationToken);
        }

        /// <summary>
        /// Gets the leaderboard for the game.
        /// </summary>
        /// <param name="limit">The maximum number of entries to return.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of leaderboard entries.</returns>
        public Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int limit = 100, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.GetLeaderboardAsync(limit, cancellationToken);
        }

        /// <summary>
        /// Gets the current user's rank in the leaderboard.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The user's leaderboard entry.</returns>
        public Task<LeaderboardEntry> GetUserRankAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.GetUserRankAsync(Id, cancellationToken);
        }
    }
}