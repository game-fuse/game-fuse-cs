using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

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
        /// Creates a new multiplayer game round for the creator only.
        /// </summary>
        /// <remarks>
        /// According to the API design, each player must create their own game round
        /// using their own authentication token. This method only creates the multiplayer round
        /// container and adds the creator as the first player.
        /// 
        /// Other players should then add their own rounds by calling AddPlayerToMultiplayerGameRoundAsync
        /// with the multiplayer game round ID returned by this method.
        /// </remarks>
        /// <param name="gameType">The type of game being played.</param>
        /// <param name="creatorUserId">The ID of the user creating the multiplayer game round (must match the authenticated user).</param>
        /// <param name="playerRounds">List containing ONLY the creator's round data.</param>
        /// <returns>The created multiplayer game round with rankings.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<MultiplayerGameRoundResponse> CreateMultiplayerGameRoundAsync(string gameType, int creatorUserId, List<GameRoundObject> playerRounds)
        {
            if (string.IsNullOrEmpty(gameType))
            {
                throw new ArgumentException("GameType is required for multiplayer game rounds", nameof(gameType));
            }

            if (creatorUserId <= 0)
            {
                throw new ArgumentException("A valid creator user ID is required", nameof(creatorUserId));
            }

            if (playerRounds == null || playerRounds.Count == 0)
            {
                throw new ArgumentException("At least one player round is required", nameof(playerRounds));
            }

            try
            {
                // Verify the first player round is for the creator - we can only create rounds for the authenticated user
                var creatorRound = playerRounds.FirstOrDefault(r => r.GameUserId == creatorUserId);
                if (creatorRound == null)
                {
                    throw new ArgumentException($"The first player round must be for the creator (user ID {creatorUserId})", nameof(playerRounds));
                }

                // Set the multiplayer flag on the creator's round
                creatorRound.Multiplayer = true;
                
                // Ensure the game type is set
                if (string.IsNullOrEmpty(creatorRound.GameType))
                {
                    creatorRound.GameType = gameType;
                }

                // Create the creator's round, which will create the multiplayer container
                string url = $"{_baseUrl}/game_rounds";
                string jsonBody = SerializeRequest(creatorRound);
                Debug.Log($"Creating multiplayer game round for creator ID {creatorUserId}: {jsonBody}");

                GameRoundObject createdCreatorRound;
                using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
                {
                    createdCreatorRound = await SendRequestAsync<GameRoundObject>(webRequest);
                }

                // The multiplayer_game_round_id will be set in the created round
                if (!createdCreatorRound.MultiplayerGameRoundId.HasValue)
                {
                    throw new ApiException(500, "Failed to create multiplayer game round - no multiplayer ID was returned", "");
                }

                int multiplayerGameRoundId = createdCreatorRound.MultiplayerGameRoundId.Value;
                Debug.Log($"Created multiplayer game round with ID: {multiplayerGameRoundId}");

                // Allow some time for the server to process
                await Task.Delay(1000);

                // Get the complete multiplayer game round with rankings
                // Note: In a real application, other players would now create their own rounds using their own auth tokens
                return await GetMultiplayerGameRoundAsync(multiplayerGameRoundId);
            }
            catch (ApiException ex)
            {
                Debug.LogError($"Error in CreateMultiplayerGameRoundAsync: {ex.Message}, Status: {ex.StatusCode}, Response: {ex.ResponseBody}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in CreateMultiplayerGameRoundAsync: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Adds a player's round to an existing multiplayer game round.
        /// </summary>
        /// <remarks>
        /// This should be called by each player using their own authentication token.
        /// The player can only add themselves to the multiplayer game round.
        /// </remarks>
        /// <param name="multiplayerGameRoundId">The ID of the multiplayer game round to join.</param>
        /// <param name="playerRound">The player's game round data.</param>
        /// <returns>The created game round.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="ApiException">Thrown when the API returns an error.</exception>
        public async Task<GameRoundObject> AddPlayerToMultiplayerGameRoundAsync(int multiplayerGameRoundId, GameRoundObject playerRound)
        {
            if (multiplayerGameRoundId <= 0)
            {
                throw new ArgumentException("A valid multiplayer game round ID is required", nameof(multiplayerGameRoundId));
            }
            
            if (playerRound == null)
            {
                throw new ArgumentException("Player round data is required", nameof(playerRound));
            }
            
            try
            {
                // Set the multiplayer game round ID
                playerRound.MultiplayerGameRoundId = multiplayerGameRoundId;
                
                // Create the player's round
                return await CreateGameRoundAsync(playerRound);
            }
            catch (ApiException ex)
            {
                Debug.LogError($"Error adding player to multiplayer game round: {ex.Message}, Status: {ex.StatusCode}, Response: {ex.ResponseBody}");
                throw;
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