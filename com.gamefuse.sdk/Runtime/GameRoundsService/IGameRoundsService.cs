using System.Threading.Tasks;

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
        /// Updates an existing game round with new score and place values.
        /// </summary>
        /// <param name="gameRoundId">The ID of the game round to update</param>
        /// <param name="score">The new score for the game round</param>
        /// <param name="place">The new place/ranking for the game round</param>
        /// <returns>The updated game round object</returns>
        Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, int score, int place);

        /// <summary>
        /// Retrieves a specific game round by ID.
        /// </summary>
        Task<GameRoundObject> GetGameRoundAsync(int gameRoundId);

        /// <summary>
        /// Retrieves all game rounds for a specific user.
        /// </summary>
        Task<GameRoundsResponse> GetUserGameRoundsAsync(int userId);

        /// <summary>
        /// Deletes a specific game round.
        /// </summary>
        Task<MessageResponse> DeleteGameRoundAsync(int gameRoundId);
    }
}
