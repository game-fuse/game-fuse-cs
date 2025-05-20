using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Response model for a leaderboard entry submission.
    /// </summary>
    public class SubmitLeaderboardEntryResponse
    {
        /// <summary>
        /// User's ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// User's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// System email: a combination of ID and email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// User's actual email used for notifications and login.
        /// </summary>
        [JsonProperty("display_email")]
        public string DisplayEmail { get; internal set; }

        /// <summary>
        /// Number of credits the user has. These can be used in your in-game store.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// A generic score metric.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// Timestamp of last login.
        /// </summary>
        [JsonProperty("last_login")]
        public string LastLogin { get; internal set; }

        /// <summary>
        /// Total logins.
        /// </summary>
        [JsonProperty("number_of_logins")]
        public int NumberOfLogins { get; internal set; }

        /// <summary>
        /// Token that must be saved and added as a parameter to all authenticated requests.
        /// </summary>
        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; internal set; }

        /// <summary>
        /// Running API hits for this user.
        /// </summary>
        [JsonProperty("events_total")]
        public int EventsTotal { get; internal set; }

        /// <summary>
        /// Running API hits for this user for the current month.
        /// </summary>
        [JsonProperty("events_current_month")]
        public int EventsCurrentMonth { get; internal set; }

        /// <summary>
        /// Unique game session for this user.
        /// </summary>
        [JsonProperty("game_sessions_total")]
        public int GameSessionsTotal { get; internal set; }

        /// <summary>
        /// Unique game session for this user during the current month.
        /// </summary>
        [JsonProperty("game_sessions_current_month")]
        public int GameSessionsCurrentMonth { get; internal set; }
    }

    /// <summary>
    /// Represents an entry in a leaderboard.
    /// </summary>
    public class LeaderboardEntryModel
    {
        /// <summary>
        /// User's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// Score for the leaderboard entry.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// Name of the leaderboard within the game.
        /// </summary>
        [JsonProperty("leaderboard_name")]
        public string LeaderboardName { get; internal set; }

        /// <summary>
        /// The user ID associated with this leaderboard entry.
        /// </summary>
        [JsonProperty("game_user_id")]
        public int GameUserId { get; internal set; }

        /// <summary>
        /// Additional metadata related to the leaderboard entry.
        /// </summary>
        [JsonProperty("metadata")]
        public object Metadata { get; internal set; }

        /// <summary>
        /// When the leaderboard entry was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }
    }

    /// <summary>
    /// Container for a list of leaderboard entries.
    /// </summary>
    public class LeaderboardEntriesResponse
    {
        /// <summary>
        /// List of leaderboard entries.
        /// </summary>
        [JsonProperty("leaderboard_entries")]
        public List<LeaderboardEntryModel> LeaderboardEntries { get; internal set; } = new List<LeaderboardEntryModel>();
    }
}