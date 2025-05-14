using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameFuseCSharp
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Creates a new basic game round for the current user.
        /// </summary>
        /// <returns>Response containing the created game round.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GameRoundObject> CreateGameRoundAsync()
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.CreateGameRoundAsync(this.id);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new game round with detailed information for the current user.
        /// </summary>
        /// <param name="gameRound">Game round details. The GameUserId will be set to the current user's ID.</param>
        /// <returns>Response containing the created game round.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GameRoundObject> CreateGameRoundAsync(GameRoundObject gameRound)
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                gameRound.GameUserId = this.id;
                return await gameRoundsService.CreateGameRoundAsync(gameRound);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new multiplayer game round with the current user as creator.
        /// </summary>
        /// <param name="gameType">The type of game being played</param>
        /// <param name="playerRounds">List of player game rounds to include in the multiplayer round</param>
        /// <returns>The created multiplayer game round with rankings</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<MultiplayerGameRoundResponse> CreateMultiplayerGameRoundAsync(string gameType, List<GameRoundObject> playerRounds)
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.CreateMultiplayerGameRoundAsync(gameType, this.id, playerRounds);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates an existing game round owned by the current user.
        /// </summary>
        /// <param name="gameRoundId">ID of the game round to update.</param>
        /// <param name="gameRound">game round with updated values</param>
        /// <returns>Response containing the updated game round.</returns>
        /// <exception cref="ApiException">Thrown when request fails or user doesn't own the game round.</exception>
        public async Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, GameRoundObject gameRound)
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.UpdateGameRoundAsync(gameRoundId, gameRound);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific game round by ID.
        /// </summary>
        /// <param name="gameRoundId">ID of the game round to retrieve.</param>
        /// <returns>Response containing the game round details.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GameRoundObject> GetGameRoundAsync(int gameRoundId)
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.GetGameRoundAsync(gameRoundId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves all game rounds for the current user.
        /// </summary>
        /// <returns>Response containing an array of game rounds.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GameRoundsResponse> GetMyGameRoundsAsync()
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.GetUserGameRoundsAsync(this.id);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves all game rounds for a specific user.
        /// </summary>
        /// <param name="userId">ID of the user whose game rounds to retrieve.</param>
        /// <returns>Response containing an array of game rounds.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GameRoundsResponse> GetUserGameRoundsAsync(int userId)
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.GetUserGameRoundsAsync(userId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a specific game round owned by the current user.
        /// </summary>
        /// <param name="gameRoundId">ID of the game round to delete.</param>
        /// <returns>Response confirming the deletion.</returns>
        /// <exception cref="ApiException">Thrown when request fails or user doesn't own the game round.</exception>
        public async Task<MessageResponse> DeleteGameRoundAsync(int gameRoundId)
        {
            try
            {
                IGameRoundsService gameRoundsService = new GameRoundsService(GameFuse.GetBaseURL(), authenticationToken);
                return await gameRoundsService.DeleteGameRoundAsync(gameRoundId);
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}