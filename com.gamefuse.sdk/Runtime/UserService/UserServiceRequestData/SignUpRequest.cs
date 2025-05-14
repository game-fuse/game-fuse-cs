using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SignUpRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
        
        [JsonProperty("password_confirmation")]
        public string PasswordConfirmation { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("game_id")]
        public int GameId { get; set; }
        
        [JsonProperty("game_token")]
        public string GameToken { get; set; }
    }
}
