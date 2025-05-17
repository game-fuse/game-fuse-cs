using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Request object for signing in a user.
    /// </summary>
    [System.Serializable]
    public class SignInRequest
    {
        /// <summary>
        /// User's email, used for login and forgot password functionality.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        
        /// <summary>
        /// User's password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
        
        /// <summary>
        /// The game ID found on your GameFuse.co dashboard.
        /// </summary>
        [JsonProperty("game_id")]
        public int GameId { get; set; }
        
        /// <summary>
        /// API token found on your GameFuse.co dashboard.
        /// </summary>
        [JsonProperty("game_token")]
        public string GameToken { get; set; }
    }
}