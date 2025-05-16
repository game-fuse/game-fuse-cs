using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class UserStoreItemsResponse
    {
        [JsonProperty("game_user_store_items")]
        public List<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
        
        [JsonProperty("credits")]
        public int Credits { get; set; }
    }
}