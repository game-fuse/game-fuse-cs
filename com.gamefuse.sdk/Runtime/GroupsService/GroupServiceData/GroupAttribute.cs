using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupAttribute
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        [JsonProperty("can_edit")]
        public bool CanEdit { get; set; }
    }
}
