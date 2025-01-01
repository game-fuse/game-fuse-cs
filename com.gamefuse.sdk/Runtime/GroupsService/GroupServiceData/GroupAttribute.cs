using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupAttribute
    {
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }  // This replaces CreatorId

        [JsonProperty("others_can_edit")]
        public bool OthersCanEdit { get; set; }  // This replaces CanEdit

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
    }
}
