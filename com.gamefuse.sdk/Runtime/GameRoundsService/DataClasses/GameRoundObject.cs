using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Represents a game round object in the GameFuse API.
    /// </summary>
    [Serializable]
    public class GameRoundObject
    {
        /// <summary>
        /// The ID of the game round.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the game this round belongs to.
        /// </summary>
        [JsonProperty("game_id")]
        public int GameId { get; set; }

        /// <summary>
        /// The ID of the user to whom the game round belongs.
        /// </summary>
        [JsonProperty("game_user_id")]
        public int GameUserId { get; set; }

        /// <summary>
        /// The ID of the associated multiplayer game round, if applicable.
        /// </summary>
        [JsonProperty("multiplayer_game_round_id")]
        public int? MultiplayerGameRoundId { get; set; }

        /// <summary>
        /// The type of game being played.
        /// </summary>
        [JsonProperty("game_type")]
        public string GameType { get; set; }

        /// <summary>
        /// The place the user finished in during the game round (1st, 2nd, etc.).
        /// </summary>
        [JsonProperty("place")]
        public int Place { get; set; }

        /// <summary>
        /// The score achieved in the game round.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; set; }

        /// <summary>
        /// The start time of the game round.
        /// </summary>
        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// The end time of the game round.
        /// </summary>
        [JsonProperty("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// Additional metadata related to the game round.
        /// </summary>
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The timestamp when the game round was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// The timestamp when the game round was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        /// <summary>
        /// Player rankings for multiplayer game rounds.
        /// </summary>
        [JsonProperty("rankings")]
        public RankingsObject[] Rankings { get; set; }

        /// <summary>
        /// Indicates whether this is a multiplayer game round.
        /// Used only when creating a new multiplayer round.
        /// </summary>
        [JsonProperty("multiplayer")]
        public bool Multiplayer { get; set; }
    }
}