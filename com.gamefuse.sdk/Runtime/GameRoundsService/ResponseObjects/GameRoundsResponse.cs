using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for the get user game rounds endpoint.
    /// </summary>
    [Serializable]
    public class GameRoundsResponse
    {
        /// <summary>
        /// Array of game round objects for the requested user.
        /// </summary>
        [JsonProperty("game_rounds")]
        public GameRoundObject[] GameRounds { get; set; } = Array.Empty<GameRoundObject>();
    }
}