using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class PurchaseStoreItemRequest
    {
        [JsonProperty("store_item_id")]
        public int StoreItemId { get; set; }
    }
}