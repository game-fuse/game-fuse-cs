using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class MessageReadResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
