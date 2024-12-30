using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    public class GroupAttributesRequest
    {
        [JsonProperty("attributes", Required = Required.Always)]
        public GroupAttributeRequest[] Attributes { get; set; }
    }
}
