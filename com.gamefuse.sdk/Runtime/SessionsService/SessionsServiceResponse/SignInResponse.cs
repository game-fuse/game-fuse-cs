using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class SignInResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("credits")]
        public int Credits { get; set; }
        
        [JsonProperty("score")]
        public int Score { get; set; }
        
        [JsonProperty("last_login")]
        public string LastLogin { get; set; }
        
        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; set; }
        
        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; set; }
        
        [JsonProperty("events_total")]
        public int EventsTotal { get; set; }
        
        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; set; }
        
        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; set; }
        
        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; set; }
    }
}