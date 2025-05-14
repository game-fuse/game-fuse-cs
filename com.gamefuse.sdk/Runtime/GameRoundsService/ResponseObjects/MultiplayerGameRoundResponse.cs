using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class MultiplayerGameRoundResponse
    {
        [JsonProperty("multiplayer_game_round")]
        public GameRoundObject MultiplayerGameRound { get; set; }
        
        [JsonProperty("rankings")]
        public RankingsObject[] Rankings { get; set; } = Array.Empty<RankingsObject>();
    }
}