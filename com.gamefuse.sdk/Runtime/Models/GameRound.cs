using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Represents a game round in GameFuse.
    /// </summary>
    public class GameRound
    {
        /// <summary>
        /// The game round's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The ID of the user who created the game round.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; internal set; }

        /// <summary>
        /// The date and time when the game round was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }

        /// <summary>
        /// The date and time when the game round was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; internal set; }

        /// <summary>
        /// When the game round ended, if it has ended.
        /// </summary>
        [JsonProperty("ended_at")]
        public string EndedAt { get; internal set; }

        /// <summary>
        /// The score achieved in the game round.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }

        /// <summary>
        /// Custom data associated with the game round.
        /// </summary>
        [JsonProperty("custom_data")]
        public string CustomData { get; internal set; }

        /// <summary>
        /// The name or identifier of the level played in the game round.
        /// </summary>
        [JsonProperty("level")]
        public string Level { get; internal set; }

        /// <summary>
        /// Variables associated with the game round.
        /// </summary>
        [JsonProperty("variables")]
        public Dictionary<string, string> Variables { get; internal set; }
    }

    /// <summary>
    /// Represents a leaderboard entry.
    /// </summary>
    public class LeaderboardEntry
    {
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; internal set; }

        /// <summary>
        /// The user's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The user's score.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; internal set; }

        /// <summary>
        /// The user's rank in the leaderboard.
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; internal set; }

        /// <summary>
        /// Custom data associated with the leaderboard entry.
        /// </summary>
        [JsonProperty("custom_data")]
        public string CustomData { get; internal set; }

        /// <summary>
        /// When the leaderboard entry was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }

        /// <summary>
        /// The ID of the game round associated with this entry.
        /// </summary>
        [JsonProperty("game_round_id")]
        public int GameRoundId { get; internal set; }
    }
}