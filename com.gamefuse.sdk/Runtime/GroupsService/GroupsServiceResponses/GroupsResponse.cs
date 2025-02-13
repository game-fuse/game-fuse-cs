using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupsResponse
    {
        [JsonProperty("groups")]
        public GroupResponse[] Groups { get; set; } = Array.Empty<GroupResponse>();
    }
}