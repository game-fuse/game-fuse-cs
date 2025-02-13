using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupAttributesResponse
    {
        [JsonProperty("attributes")]
        public GroupAttribute[] Attributes { get; set; } = Array.Empty<GroupAttribute>();
    }
}