using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GameFuseCSharp
{
    [Serializable]
    public class LeaderboardEntryObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("game_user_id")]
        public int GameUserId { get; set; }
        
        [JsonProperty("leaderboard_name")]
        public string LeaderboardName { get; set; }
        
        [JsonProperty("score")]
        public int Score { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("extra_attributes")]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        
        [JsonProperty("metadata")]
        private string MetadataJson
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        var metadataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
                        if (metadataDict != null)
                        {
                            foreach (var kvp in metadataDict)
                            {
                                Metadata[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Debug.LogWarning($"Failed to parse metadata JSON: {ex.Message}");
                    }
                }
            }
        }
        
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
    }
}