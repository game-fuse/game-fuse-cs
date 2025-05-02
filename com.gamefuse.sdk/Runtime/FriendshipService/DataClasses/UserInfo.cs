using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class UserInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("display_email")]
        public string DisplayEmail { get; set; }

        [JsonProperty("credits")]
        public int Credits { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }
        
        [JsonProperty("is_new_user")]
        public bool IsNewUser { get; set; }
    }
}
