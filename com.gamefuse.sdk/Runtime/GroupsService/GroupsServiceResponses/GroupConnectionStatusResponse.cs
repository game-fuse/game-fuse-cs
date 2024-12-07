using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupConnectionStatusResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
