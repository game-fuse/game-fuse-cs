using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class MessageResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
