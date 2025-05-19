using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{

    /// <summary>
    /// Represents the API response when fetching a user's purchased store items.
    /// Contains the user's current credits and a list of their purchased items.
    /// </summary>
    public class UserStore
    {
        /// <summary>
        /// The user's remaining credits after any transactions or as a current balance.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// A list of store items purchased by the user.
        /// </summary>
        [JsonProperty("store_items")]
        public List<StoreItem> StoreItems { get; internal set; }

        public UserStore()
        {
            StoreItems = new List<StoreItem>(); // Initialize to prevent null reference if API returns empty list
        }
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
        /// The item's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The cost of the item in credits.
        /// </summary>
        [JsonProperty("cost")]
        public int Cost { get; internal set; }

        /// <summary>
        /// A description of the item.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// <summary>
        /// The category the item belongs to.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; internal set; }

        /// <summary>
        /// The URL to the item's icon, if available.
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; internal set; }
    }

    /// <summary>
    /// Represents a credit transaction in GameFuse.
    /// </summary>
    public class CreditTransaction
    {
        /// <summary>
        /// The transaction's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The number of credits involved in the transaction.
        /// </summary>
        [JsonProperty("credits_amount")]
        public int CreditsAmount { get; internal set; }

        /// <summary>
        /// The type of transaction (e.g., "purchase", "reward").
        /// </summary>
        [JsonProperty("transaction_type")]
        public string TransactionType { get; internal set; }

        /// <summary>
        /// A description of the transaction.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// <summary>
        /// When the transaction occurred.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }
    }

    /// <summary>
    /// Represents a user's credit balance.
    /// </summary>
    public class CreditBalance
    {
        /// <summary>
        /// The user's current credit balance.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// The user's ID.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; internal set; }
    }
}