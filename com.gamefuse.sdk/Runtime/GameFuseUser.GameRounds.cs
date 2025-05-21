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
        /// Creates a new non-multiplayer game round for the current user.
        /// </summary>
        /// <param name="gameType">Type of game being played.</param>
        /// <param name="startTime">Start time of the game round.</param>
        /// <param name="endTime">End time of the game round.</param>
        /// <param name="score">The score achieved in the game round.</param>
        /// <param name="place">The place the user finished in during the game round.</param>
        /// <param name="metadata">Additional metadata related to the game round.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created game round.</returns>
        public Task<GameRound> CreateGameRoundAsync(
            string gameType,
            string startTime = null,
            string endTime = null,
            int? score = null,
            int? place = null,
            Dictionary<string, object> metadata = null,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.CreateGameRoundAsync(
                Id, 
                gameType, 
                startTime, 
                endTime, 
                score, 
                place, 
                metadata,
                false, 
                null, 
                cancellationToken);
        }

        /// <summary>
        /// Creates a new multiplayer game round or joins an existing one.
        /// </summary>
        /// <param name="gameType">Type of game being played.</param>
        /// <param name="startTime">Start time of the game round.</param>
        /// <param name="endTime">End time of the game round.</param>
        /// <param name="score">The score achieved in the game round.</param>
        /// <param name="place">The place the user finished in during the game round.</param>
        /// <param name="metadata">Additional metadata related to the game round.</param>
        /// <param name="multiplayerGameRoundId">ID of an existing multiplayer game round to join. If null, a new multiplayer round will be created.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created game round.</returns>
        public Task<GameRound> CreateMultiplayerGameRoundAsync(
            string gameType,
            string startTime = null,
            string endTime = null,
            int? score = null,
            int? place = null,
            Dictionary<string, object> metadata = null,
            int? multiplayerGameRoundId = null,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            bool isNewMultiplayerGame = !multiplayerGameRoundId.HasValue;
            
            return _gameRoundService.CreateGameRoundAsync(
                Id, 
                gameType, 
                startTime, 
                endTime, 
                score, 
                place, 
                metadata,
                isNewMultiplayerGame, 
                multiplayerGameRoundId, 
                cancellationToken);
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
        /// Gets all game rounds for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose game rounds to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of game rounds.</returns>
        public Task<IReadOnlyList<GameRound>> GetGameRoundsForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            return _gameRoundService.GetGameRoundsForUserAsync(userId, cancellationToken);
        }

        /// <summary>
        /// Updates a game round.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to update.</param>
        /// <param name="startTime">The start time of the game round.</param>
        /// <param name="endTime">The end time of the game round.</param>
        /// <param name="score">The score achieved in the game round.</param>
        /// <param name="place">The place the user finished in during the game round.</param>
        /// <param name="gameType">The type of game being played.</param>
        /// <param name="metadata">Additional metadata related to the game round.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated game round.</returns>
        public Task<GameRound> UpdateGameRoundAsync(
            int gameRoundId,
            string startTime = null,
            string endTime = null,
            int? score = null,
            int? place = null,
            string gameType = null,
            Dictionary<string, object> metadata = null,
            CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.UpdateGameRoundAsync(
                gameRoundId, 
                startTime, 
                endTime, 
                score, 
                place, 
                gameType, 
                metadata, 
                cancellationToken);
        }

        /// <summary>
        /// Deletes a game round.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response indicating success.</returns>
        public Task<GameRoundDeleteResponse> DeleteGameRoundAsync(int gameRoundId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _gameRoundService.DeleteGameRoundAsync(gameRoundId, cancellationToken);
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