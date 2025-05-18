using GameFuse.Exceptions;
using GameFuse.Models;
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
        /// <param name="userId">The ID of the user creating the game round.</param>
        /// <param name="level">The level identifier (optional).</param>
        /// <param name="customData">Custom data for the game round (optional).</param>
        /// <param name="variables">Variables for the game round (optional).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created game round.</returns>
        public Task<GameRound> CreateGameRoundAsync(int userId, string level = null, string customData = null, Dictionary<string, string> variables = null, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var request = new Dictionary<string, object>
            {
                ["user_id"] = userId
            };
            
            if (!string.IsNullOrEmpty(level))
            {
                request["level"] = level;
            }
            
            if (!string.IsNullOrEmpty(customData))
            {
                request["custom_data"] = customData;
            }
            
            if (variables != null && variables.Count > 0)
            {
                request["variables"] = variables;
            }
            
            return _transport.PostAsync<Dictionary<string, object>, GameRound>("game_rounds", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets a game round by ID.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The game round.</returns>
        public Task<GameRound> GetGameRoundAsync(int gameRoundId, CancellationToken cancellationToken = default)
        {
            if (gameRoundId <= 0) throw new ArgumentOutOfRangeException(nameof(gameRoundId), "Game Round ID must be positive.");
            
            return _transport.GetAsync<GameRound>($"game_rounds/{gameRoundId}", null, cancellationToken);
        }

        /// <summary>
        /// Gets all game rounds for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of game rounds.</returns>
        public async Task<IReadOnlyList<GameRound>> GetGameRoundsForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<GameRound>>($"users/{userId}/game_rounds", null, cancellationToken);
            return response.AsReadOnly();
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
            if (gameRoundId <= 0) throw new ArgumentOutOfRangeException(nameof(gameRoundId), "Game Round ID must be positive.");
            
            var request = new Dictionary<string, object>();
            
            if (score.HasValue)
            {
                request["score"] = score.Value;
            }
            
            if (!string.IsNullOrEmpty(customData))
            {
                request["custom_data"] = customData;
            }
            
            if (variables != null && variables.Count > 0)
            {
                request["variables"] = variables;
            }
            
            if (ended.HasValue && ended.Value)
            {
                request["ended_at"] = DateTime.UtcNow.ToString("o");
            }
            
            if (request.Count == 0)
            {
                throw new ArgumentException("At least one parameter must be provided to update.");
            }
            
            return _transport.PutAsync<Dictionary<string, object>, GameRound>($"game_rounds/{gameRoundId}", request, null, cancellationToken);
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
            
            var response = await _transport.GetAsync<List<LeaderboardEntry>>($"leaderboard?limit={limit}", null, cancellationToken);
            return response.AsReadOnly();
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