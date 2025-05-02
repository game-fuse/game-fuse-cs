using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GameRoundsResponse
    {
        [JsonProperty("game_rounds")]
        public GameRoundObject[] GameRounds { get; set; } = Array.Empty<GameRoundObject>();
    }
}