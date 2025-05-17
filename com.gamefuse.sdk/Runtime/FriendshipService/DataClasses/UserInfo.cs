using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Represents basic information about a user in the system.
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The user's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// The system email address, which is a combination of ID and actual email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user's actual email address used for login and notifications.
        /// </summary>
        [JsonProperty("display_email")]
        public string DisplayEmail { get; set; }

        /// <summary>
        /// The number of credits the user has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; set; }

        /// <summary>
        /// The user's score in the game.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; set; }
        
        /// <summary>
        /// Indicates whether this is a new user.
        /// </summary>
        [JsonProperty("is_new_user")]
        public bool IsNewUser { get; set; }
    }
}
