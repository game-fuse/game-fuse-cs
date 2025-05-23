using Newtonsoft.Json;

namespace GameFuse.Models.TestSuite
{
    /// <summary>
    /// Request model for creating a test game.
    /// </summary>
    public class CreateGameRequest
    {
        // No parameters required for this request
    }

    /// <summary>
    /// Response model for creating a test game.
    /// </summary>
    public class CreateGameResponse
    {
        /// <summary>
        /// The ID of the created game.
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// The name of the created game.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The API token for the created game.
        /// </summary>
        [JsonProperty("Token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// Request model for creating a test user.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// The ID of the game to create the user in.
        /// </summary>
        [JsonProperty("game_id")]
        public int GameId { get; set; }

        /// <summary>
        /// The username for the test user.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// The email for the test user.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
    }


    /// <summary>
    /// Request model for cleaning up a test game.
    /// </summary>
    public class CleanUpGameRequest
    {
        /// <summary>
        /// The ID of the game to clean up.
        /// </summary>
        [JsonProperty("game_id")]
        public int GameId { get; set; }
    }

    /// <summary>
    /// Response model for cleaning up a test game.
    /// </summary>
    public class CleanUpResponse
    {
        /// <summary>
        /// The message indicating the result of the clean up operation.
        /// </summary>
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}