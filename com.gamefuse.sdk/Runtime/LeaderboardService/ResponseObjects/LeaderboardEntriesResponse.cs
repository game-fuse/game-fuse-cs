using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class LeaderboardEntriesResponse
    {
        [JsonProperty("leaderboard_entries")]
        public LeaderboardEntryObject[] LeaderboardEntries { get; set; } = Array.Empty<LeaderboardEntryObject>();
    }
}