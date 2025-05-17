using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Represents a player's ranking in a multiplayer game round.
    /// </summary>
    [Serializable]
    public class RankingsObject
    {
        /// <summary>
        /// The ID of the corresponding game round.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The place the player finished in during the game round (1st, 2nd, etc.).
        /// </summary>
        [JsonProperty("place")]
        public int Place { get; set; }

        /// <summary>
        /// The score achieved by the player in the game round.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; set; }

        /// <summary>
        /// The start time of the player's game round.
        /// </summary>
        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// The end time of the player's game round.
        /// </summary>
        [JsonProperty("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// Information about the user who participated in the game round.
        /// </summary>
        [JsonProperty("user")]
        public UserInfo User { get; set; }
    }
}