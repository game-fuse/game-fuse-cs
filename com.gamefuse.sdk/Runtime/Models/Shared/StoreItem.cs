using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models.Shared
{
    /// <summary>
    /// Represents the status of a store item in relation to a user.
    /// </summary>
    public enum StoreItemStatus
    {
        /// <summary>
        /// The item is available for purchase.
        /// </summary>
        Available,
        
        /// <summary>
        /// The item has been purchased by the user.
        /// </summary>
        Purchased,
        
        /// <summary>
        /// The item is not available for purchase.
        /// </summary>
        Unavailable
    }

    /// <summary>
    /// Represents an item in the GameFuse store.
    /// </summary>
    public class StoreItem
    {
        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The cost of the item in credits.
        /// </summary>
        [JsonProperty("cost")]
        public int Cost { get; internal set; }

        /// <summary>
        /// The description of the item.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// <summary>
        /// The category of the item.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; internal set; }

        /// <summary>
        /// The URL of the item's icon.
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; internal set; }

        /// <summary>
        /// The status of the item in relation to the current user.
        /// This is not part of the API response, but is set based on context.
        /// </summary>
        [JsonIgnore]
        public StoreItemStatus Status { get; internal set; } = StoreItemStatus.Available;

        /// <summary>
        /// Determines if the item has been purchased by the current user.
        /// </summary>
        [JsonIgnore]
        public bool IsPurchased => Status == StoreItemStatus.Purchased;
    }

    /// <summary>
    /// Represents the API response when fetching all available store items for a game.
    /// </summary>
    public class GameStoreItemsResponse
    {
        /// <summary>
        /// The list of store items.
        /// </summary>
        [JsonProperty("store_items")]
        public List<StoreItem> StoreItems { get; set; }

        /// <summary>
        /// Creates a new GameStoreItemsResponse with an empty list.
        /// </summary>
        public GameStoreItemsResponse()
        {
            StoreItems = new List<StoreItem>();
        }
    }

    /// <summary>
    /// Represents a user's store, including their credits and purchased items.
    /// </summary>
    public class UserStore
    {
        /// <summary>
        /// The number of credits the user has.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// The list of store items owned by the user.
        /// </summary>
        [JsonProperty("game_user_store_items")]
        public List<StoreItem> StoreItems { get; internal set; }

        /// <summary>
        /// Creates a new UserStore with an empty list of store items.
        /// </summary>
        public UserStore()
        {
            StoreItems = new List<StoreItem>();
        }
    }

    /// <summary>
    /// Payload for purchasing an item from the store.
    /// </summary>
    public class PurchaseItemPayload
    {
        /// <summary>
        /// The ID of the store item to purchase.
        /// </summary>
        [JsonProperty("store_item_id")]
        public int StoreItemId { get; set; }
    }
}