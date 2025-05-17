using System.Threading.Tasks;

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
        /// <remarks>
        /// To create a multiplayer game round, set the Multiplayer property to true.
        /// To join an existing multiplayer game round, set the MultiplayerGameRoundId property.
        /// </remarks>
        /// <param name="gameRound">The game round data to create.</param>
        /// <returns>The created game round object.</returns>
        Task<GameRoundObject> CreateGameRoundAsync(GameRoundObject gameRound);

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
