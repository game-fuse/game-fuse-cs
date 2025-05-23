using GameFuse.Exceptions;
using GameFuse.Models.Shared;
using GameFuse.Transport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for game round related operations.
    /// </summary>
    public class GameRoundService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the GameRoundService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public GameRoundService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Creates a new game round.
        /// </summary>
        /// <param name="gameUserId">The ID of the user creating the game round.</param>
        /// <param name="gameType">The type of game being played.</param>
        /// <param name="startTime">The start time of the game round.</param>
        /// <param name="endTime">The end time of the game round.</param>
        /// <param name="score">The score achieved in the game round.</param>
        /// <param name="place">The place the user finished in during the game round.</param>
        /// <param name="metadata">Additional metadata related to the game round.</param>
        /// <param name="multiplayer">If true, create or join a multiplayer round.</param>
        /// <param name="multiplayerGameRoundId">ID of the associated multiplayer game round if applicable.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created game round.</returns>
        public Task<GameRound> CreateGameRoundAsync(
            int gameUserId, 
            string gameType, 
            string startTime = null, 
            string endTime = null, 
            int? score = null, 
            int? place = null, 
            Dictionary<string, object> metadata = null, 
            bool? multiplayer = null, 
            int? multiplayerGameRoundId = null, 
            CancellationToken cancellationToken = default)
        {
            if (gameUserId <= 0) throw new ArgumentOutOfRangeException(nameof(gameUserId), "Game User ID must be positive.");
            if (string.IsNullOrEmpty(gameType)) throw new ArgumentException("Game type cannot be null or empty.", nameof(gameType));
            
            var request = new Dictionary<string, object>
            {
                ["game_user_id"] = gameUserId,
                ["game_type"] = gameType
            };
            
            if (!string.IsNullOrEmpty(startTime)) request["start_time"] = startTime;
            if (!string.IsNullOrEmpty(endTime)) request["end_time"] = endTime;
            if (score.HasValue) request["score"] = score.Value;
            if (place.HasValue) request["place"] = place.Value;
            if (metadata != null) request["metadata"] = metadata;
            if (multiplayer.HasValue) request["multiplayer"] = multiplayer.Value;
            if (multiplayerGameRoundId.HasValue) request["multiplayer_game_round_id"] = multiplayerGameRoundId.Value;
            
            return _transport.PostAsync<Dictionary<string, object>, GameRound>("game_rounds", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets a game round by ID.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The game round. For multiplayer rounds, includes rankings of all participants.</returns>
        /// <remarks>
        /// When retrieving a multiplayer game round, the response includes a rankings array with all participants.
        /// </remarks>
        public Task<GameRound> GetGameRoundAsync(int gameRoundId, CancellationToken cancellationToken = default)
        {
            if (gameRoundId <= 0) throw new ArgumentOutOfRangeException(nameof(gameRoundId), "Game Round ID must be positive.");
            
            return _transport.GetAsync<GameRound>($"game_rounds/{gameRoundId}", null, cancellationToken);
        }

        /// <summary>
        /// Gets all game rounds for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="perPage">Number of game rounds per page (default and max 100).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of game rounds.</returns>
        /// <remarks>
        /// Note that multiplayer game rounds retrieved in bulk will not have rankings attached.
        /// To retrieve rankings, use GetGameRoundAsync to fetch individual game rounds.
        /// </remarks>
        public async Task<IReadOnlyList<GameRound>> GetGameRoundsForUserAsync(
            int userId, 
            int page = 1, 
            int perPage = 100, 
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page number must be positive.");
            if (perPage <= 0 || perPage > 100) throw new ArgumentOutOfRangeException(nameof(perPage), "Per page must be between 1 and 100.");
            
            var response = await _transport.GetAsync<GameRoundListResponse>(
                $"game_rounds?user_id={userId}&page={page}&per_page={perPage}", 
                null, 
                cancellationToken);
            return response.GameRounds?.AsReadOnly() ?? new List<GameRound>().AsReadOnly();
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
            if (gameRoundId <= 0) throw new ArgumentOutOfRangeException(nameof(gameRoundId), "Game Round ID must be positive.");
            
            var request = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(startTime)) request["start_time"] = startTime;
            if (!string.IsNullOrEmpty(endTime)) request["end_time"] = endTime;
            if (score.HasValue) request["score"] = score.Value;
            if (place.HasValue) request["place"] = place.Value;
            if (!string.IsNullOrEmpty(gameType)) request["game_type"] = gameType;
            if (metadata != null) request["metadata"] = metadata;
            
            if (request.Count == 0)
            {
                throw new ArgumentException("At least one parameter must be provided to update.");
            }
            
            return _transport.PutAsync<Dictionary<string, object>, GameRound>($"game_rounds/{gameRoundId}", request, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a game round.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response indicating success.</returns>
        public Task<GameRoundDeleteResponse> DeleteGameRoundAsync(int gameRoundId, CancellationToken cancellationToken = default)
        {
            if (gameRoundId <= 0) throw new ArgumentOutOfRangeException(nameof(gameRoundId), "Game Round ID must be positive.");
            
            return _transport.DeleteAsync<GameRoundDeleteResponse>($"game_rounds/{gameRoundId}", null, cancellationToken);
        }

        /// <summary>
        /// Gets the leaderboard for the game.
        /// </summary>
        /// <param name="limit">The maximum number of entries to return.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of leaderboard entries.</returns>
        public async Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int limit = 100, CancellationToken cancellationToken = default)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be positive.");
            
            var response = await _transport.GetAsync<LeaderboardEntries>($"leaderboard?limit={limit}", null, cancellationToken);
            return response.Entries?.AsReadOnly() ?? new List<LeaderboardEntry>().AsReadOnly();
        }

        /// <summary>
        /// Gets a user's rank in the leaderboard.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The user's leaderboard entry.</returns>
        public Task<LeaderboardEntry> GetUserRankAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            return _transport.GetAsync<LeaderboardEntry>($"leaderboard/users/{userId}", null, cancellationToken);
        }
    }
}