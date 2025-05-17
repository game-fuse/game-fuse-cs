using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    /// <summary>
    /// Implementation of the Game Rounds API service.
    /// </summary>
    public class GameRoundsService : AbstractService, IGameRoundsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameRoundsService"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="token">The authentication token.</param>
        public GameRoundsService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        /// <summary>
        /// Creates a new basic game round for a user with default values.
        /// </summary>
        /// <param name="gameUserId">The ID of the user to whom the game round belongs.</param>
        /// <returns>The created game round object.</returns>
        public async Task<GameRoundObject> CreateGameRoundAsync(int gameUserId)
        {
            var gameRound = new GameRoundObject
            {
                GameUserId = gameUserId,
                GameType = "default"  // Providing required game_type
            };
            return await CreateGameRoundAsync(gameRound);
        }

        /// <summary>
        /// Creates a new game round with detailed information.
        /// </summary>
        /// <remarks>
        /// To create a multiplayer game round, set the Multiplayer property to true.
        /// To join an existing multiplayer game round, set the MultiplayerGameRoundId property.
        /// </remarks>
        /// <param name="gameRound">The game round data to create.</param>
        /// <returns>The created game round object.</returns>
        /// <exception cref="ArgumentException">Thrown when the GameUserId is not set or invalid.</exception>
        public async Task<GameRoundObject> CreateGameRoundAsync(GameRoundObject gameRound)
        {
            if (gameRound.GameUserId == 0)
            {
                throw new ArgumentException("GameUserId is required for creating a game round", nameof(gameRound));
            }

            // Enforce required fields according to API documentation
            if (string.IsNullOrEmpty(gameRound.GameType))
            {
                throw new ArgumentException("GameType is required for creating a game round", nameof(gameRound));
            }

            string url = $"{_baseUrl}/game_rounds";
            string jsonBody = SerializeRequest(gameRound);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GameRoundObject>(webRequest);
            }
        }

        /// <summary>
        /// Updates an existing game round with new values.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to update.</param>
        /// <param name="gameRound">The game round with updated values.</param>
        /// <returns>The updated game round object.</returns>
        public async Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, GameRoundObject gameRound)
        {
            if (gameRoundId <= 0)
            {
                throw new ArgumentException("A valid game round ID is required", nameof(gameRoundId));
            }

            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";

            // According to the API docs, we shouldn't modify the game_type of multiplayer rounds
            // First, get the existing game round to check if it's multiplayer
            GameRoundObject existingRound;
            try 
            {
                existingRound = await GetGameRoundAsync(gameRoundId);
            }
            catch (ApiException ex)
            {
                Debug.LogError($"Failed to get existing game round for update: {ex.Message}");
                throw new ArgumentException($"Game round with ID {gameRoundId} not found", nameof(gameRoundId));
            }

            // Create update object with all allowed fields
            var updateData = new
            {
                // Don't include game_type if this is a multiplayer game
                game_type = existingRound.MultiplayerGameRoundId.HasValue ? null : gameRound.GameType,
                place = gameRound.Place,
                score = gameRound.Score,
                start_time = gameRound.StartTime,
                end_time = gameRound.EndTime,
                metadata = gameRound.Metadata
            };

            string jsonBody = SerializeRequest(updateData);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.PUT, jsonBody))
            {
                return await SendRequestAsync<GameRoundObject>(webRequest);
            }
        }

        /// <summary>
        /// Retrieves a specific game round by ID.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to retrieve.</param>
        /// <returns>The game round object.</returns>
        public async Task<GameRoundObject> GetGameRoundAsync(int gameRoundId)
        {
            if (gameRoundId <= 0)
            {
                throw new ArgumentException("A valid game round ID is required", nameof(gameRoundId));
            }

            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GameRoundObject>(webRequest);
            }
        }

        /// <summary>
        /// Retrieves a multiplayer game round by ID, including all player rankings.
        /// </summary>
        /// <param name="multiplayerGameRoundId">The ID of the multiplayer game round.</param>
        /// <returns>The multiplayer game round with player rankings.</returns>
        public async Task<MultiplayerGameRoundResponse> GetMultiplayerGameRoundAsync(int multiplayerGameRoundId)
        {
            if (multiplayerGameRoundId <= 0)
            {
                throw new ArgumentException("A valid multiplayer game round ID is required", nameof(multiplayerGameRoundId));
            }

            string url = $"{_baseUrl}/game_rounds/multiplayer_game_round/{multiplayerGameRoundId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<MultiplayerGameRoundResponse>(webRequest);
            }
        }

        /// <summary>
        /// Retrieves all game rounds for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose game rounds to retrieve.</param>
        /// <returns>A response containing an array of game rounds.</returns>
        public async Task<GameRoundsResponse> GetUserGameRoundsAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("A valid user ID is required", nameof(userId));
            }

            string url = $"{_baseUrl}/game_rounds?user_id={userId}";

            try
            {
                using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
                {
                    var response = await SendRequestAsync<GameRoundsResponse>(webRequest);
                    
                    // Ensure we have a valid array
                    if (response.GameRounds == null)
                    {
                        response.GameRounds = Array.Empty<GameRoundObject>();
                    }
                    
                    return response;
                }
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                // If the user has no game rounds, API may return 404.
                // In this case, return an empty response instead of throwing
                Debug.LogWarning($"No game rounds found for user {userId}, returning empty response.");
                return new GameRoundsResponse 
                { 
                    GameRounds = Array.Empty<GameRoundObject>() 
                };
            }
        }

        /// <summary>
        /// Deletes a specific game round.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to delete.</param>
        /// <returns>A response containing a success message.</returns>
        public async Task<MessageResponse> DeleteGameRoundAsync(int gameRoundId)
        {
            if (gameRoundId <= 0)
            {
                throw new ArgumentException("A valid game round ID is required", nameof(gameRoundId));
            }

            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";

            try
            {
                using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.DELETE))
                {
                    return await SendRequestAsync<MessageResponse>(webRequest);
                }
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                // If the game round doesn't exist (possibly already deleted), return a success message
                Debug.LogWarning($"Game round {gameRoundId} not found for deletion, it may have been already deleted.");
                return new MessageResponse 
                { 
                    Message = $"Game round {gameRoundId} destroyed successfully (not found)." 
                };
            }
        }
    }
}