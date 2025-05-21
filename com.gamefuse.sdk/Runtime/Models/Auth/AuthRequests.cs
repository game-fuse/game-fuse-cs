using Newtonsoft.Json;

namespace GameFuse.Models.Auth
{
    /// <summary>
    /// Request model for signing up a new user.
    /// </summary>
    public class SignUpRequest
    {
        /// <summary>
        /// The user's email address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmation of the user's password.
        /// </summary>
        [JsonProperty("password_confirmation")]
        public string PasswordConfirmation { get; set; }

        /// <summary>
        /// The user's desired username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("game_id")]
        public string GameId { get; set; }

        /// <summary>
        /// The API token for the game.
        /// </summary>
        [JsonProperty("game_token")]
        public string GameToken { get; set; }
    }

    /// <summary>
    /// Request model for signing in a user.
    /// </summary>
    public class SignInRequest
    {
        /// <summary>
        /// The user's email address or username.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("game_id")]
        public string GameId { get; set; }

        /// <summary>
        /// The API token for the game.
        /// </summary>
        [JsonProperty("game_token")]
        public string GameToken { get; set; }
    }


    /// <summary>
    /// Request model for resetting a user's password.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// The token provided in the reset password email.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// The user's new password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmation of the user's new password.
        /// </summary>
        [JsonProperty("password_confirmation")]
        public string PasswordConfirmation { get; set; }

        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("game_id")]
        public string GameId { get; set; }

        /// <summary>
        /// The API token for the game.
        /// </summary>
        [JsonProperty("game_token")]
        public string GameToken { get; set; }
    }
}