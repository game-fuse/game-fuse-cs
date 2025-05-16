using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class RemoveStoreItemRequest
    {
        [JsonProperty("store_item_id")]
        public int StoreItemId { get; set; }
        
        [JsonProperty("reimburse")]
        public bool Reimburse { get; set; }
    }
}