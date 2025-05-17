using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameFuseCSharp
{
    /// <summary>
    /// Interface for the Game Rounds API service.
    /// </summary>
    public interface IGameRoundsService
    {
        /// <summary>
        /// Creates a new basic game round for a user with default values.
        /// </summary>
        /// <param name="gameUserId">The ID of the user to whom the game round belongs.</param>
        /// <returns>The created game round object.</returns>
        Task<GameRoundObject> CreateGameRoundAsync(int gameUserId);

        /// <summary>
        /// Creates a new game round with detailed information.
        /// </summary>
        /// <param name="gameRound">The game round data to create.</param>
        /// <returns>The created game round object.</returns>
        Task<GameRoundObject> CreateGameRoundAsync(GameRoundObject gameRound);

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
        Task<MultiplayerGameRoundResponse> CreateMultiplayerGameRoundAsync(string gameType, int creatorUserId, List<GameRoundObject> playerRounds);
        
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
        Task<GameRoundObject> AddPlayerToMultiplayerGameRoundAsync(int multiplayerGameRoundId, GameRoundObject playerRound);

        /// <summary>
        /// Updates an existing game round with new values.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to update.</param>
        /// <param name="gameRound">The game round with updated values.</param>
        /// <returns>The updated game round object.</returns>
        Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, GameRoundObject gameRound); 

        /// <summary>
        /// Retrieves a specific game round by ID.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to retrieve.</param>
        /// <returns>The game round object.</returns>
        Task<GameRoundObject> GetGameRoundAsync(int gameRoundId);

        /// <summary>
        /// Retrieves all game rounds for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose game rounds to retrieve.</param>
        /// <returns>A response containing an array of game rounds.</returns>
        Task<GameRoundsResponse> GetUserGameRoundsAsync(int userId);

        /// <summary>
        /// Retrieves a multiplayer game round by ID, including all player rankings.
        /// </summary>
        /// <param name="multiplayerGameRoundId">The ID of the multiplayer game round.</param>
        /// <returns>The multiplayer game round with player rankings.</returns>
        Task<MultiplayerGameRoundResponse> GetMultiplayerGameRoundAsync(int multiplayerGameRoundId);

        /// <summary>
        /// Deletes a specific game round.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to delete.</param>
        /// <returns>A response containing a success message.</returns>
        Task<MessageResponse> DeleteGameRoundAsync(int gameRoundId);
    }
}
