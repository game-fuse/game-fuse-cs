using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GameRoundObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("game_id")]
        public int GameId { get; set; }

        [JsonProperty("game_user_id")]
        public int GameUserId { get; set; }

        [JsonProperty("multiplayer_game_round_id")]
        public int? MultiplayerGameRoundId { get; set; }

        [JsonProperty("game_type")]
        public string GameType { get; set; }

        [JsonProperty("place")]
        public int Place { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("end_time")]
        public string EndTime { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("rankings")]
        public RankingsObject[] Rankings { get; set; }

        [JsonProperty("multiplayer")]
        public bool Multiplayer { get; set; }
    }
}