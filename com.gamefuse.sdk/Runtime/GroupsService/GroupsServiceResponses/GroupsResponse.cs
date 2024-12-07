using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupsResponse
    {
        [JsonProperty("groups")]
        public GroupResponse[] Groups { get; set; } = Array.Empty<GroupResponse>();
    }
}
