using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Response object for the get multiplayer game round endpoint.
    /// </summary>
    [Serializable]
    public class MultiplayerGameRoundResponse
    {
        /// <summary>
        /// The multiplayer game round object.
        /// </summary>
        [JsonProperty("multiplayer_game_round")]
        public GameRoundObject MultiplayerGameRound { get; set; }
        
        /// <summary>
        /// Array of player rankings for the multiplayer game round.
        /// </summary>
        [JsonProperty("rankings")]
        public RankingsObject[] Rankings { get; set; } = Array.Empty<RankingsObject>();
    }
}