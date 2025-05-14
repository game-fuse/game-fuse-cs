using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameFuseCSharp
{
    public interface IGameRoundsService
    {
        /// <summary>
        /// Creates a new basic game round for a user.
        /// </summary>
        Task<GameRoundObject> CreateGameRoundAsync(int gameUserId);

        /// <summary>
        /// Creates a new game round with detailed information.
        /// </summary>
        Task<GameRoundObject> CreateGameRoundAsync(GameRoundObject gameRound);

        /// <summary>
        /// Creates a new multiplayer game round.
        /// </summary>
        /// <param name="gameType">The type of game being played</param>
        /// <param name="playerRounds">List of player game rounds to include in the multiplayer round</param>
        /// <returns>The created multiplayer game round with rankings</returns>
        Task<MultiplayerGameRoundResponse> CreateMultiplayerGameRoundAsync(string gameType, List<GameRoundObject> playerRounds);

        /// <summary>
        /// Updates an existing game round with new score and place values.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to update</param>
        /// <param name="gameRound">The game round with updated values</param>
        /// <returns>The updated game round object</returns>
        Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, GameRoundObject gameRound); 

        /// <summary>
        /// Retrieves a specific game round by ID.
        /// </summary>
        Task<GameRoundObject> GetGameRoundAsync(int gameRoundId);

        /// <summary>
        /// Retrieves all game rounds for a specific user.
        /// </summary>
        Task<GameRoundsResponse> GetUserGameRoundsAsync(int userId);

        /// <summary>
        /// Retrieves a multiplayer game round by ID, including all player rankings.
        /// </summary>
        /// <param name="multiplayerGameRoundId">The ID of the multiplayer game round</param>
        /// <returns>The multiplayer game round with player rankings</returns>
        Task<MultiplayerGameRoundResponse> GetMultiplayerGameRoundAsync(int multiplayerGameRoundId);

        /// <summary>
        /// Deletes a specific game round.
        /// </summary>
        Task<MessageResponse> DeleteGameRoundAsync(int gameRoundId);
    }
}
