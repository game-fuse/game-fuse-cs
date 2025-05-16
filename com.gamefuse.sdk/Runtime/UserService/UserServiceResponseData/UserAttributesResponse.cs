using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [System.Serializable]
    public class UserAttributesResponse
    {
        [JsonProperty("game_user_attributes")]
        public List<UserAttribute> Attributes { get; set; } = new List<UserAttribute>();
    }
}