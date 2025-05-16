using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class RemoveGroupMemberResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("group")]
        public Group Group { get; set; }
    }
}