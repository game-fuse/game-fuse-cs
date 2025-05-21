// StoreItem.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Represents an item in the GameFuse store.
    /// </summary>
    public class StoreItem
    {
        [JsonProperty("id")]
        public int Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("cost")]
        public int Cost { get; internal set; }

        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("category")]
        public string Category { get; internal set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; internal set; }
    }

    /// <summary>
    /// Represents the API response when fetching all available store items for a game.
    /// </summary>
    public class GameStoreItemsResponse
    {
        [JsonProperty("store_items")]
        public List<StoreItem> StoreItems { get; set; }

        public GameStoreItemsResponse()
        {
            StoreItems = new List<StoreItem>();
        }
    }

    // ... UserStore, CreditTransaction, CreditBalance classes remain as they were ...
    public class UserStore
    {
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        [JsonProperty("store_items")]
        public List<StoreItem> StoreItems { get; internal set; }

        public UserStore()
        {
            StoreItems = new List<StoreItem>();
        }
    }


    public class PurchaseItemPayload
    {
        [JsonProperty("store_item_id")]
        public int StoreItemId { get; set; }
    }
}