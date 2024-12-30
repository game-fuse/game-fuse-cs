using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    [JsonConverter(typeof(GroupAttributesResponseConverter))]
    public class GroupAttributesResponse
    {
        [JsonProperty]
        public GroupAttribute[] Attributes { get; set; }

        // Required parameterless constructor
        public GroupAttributesResponse()
        {
            Attributes = Array.Empty<GroupAttribute>();
        }
    }

    public class GroupAttributesResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GroupAttributesResponse);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var response = new GroupAttributesResponse();
                response.Attributes = serializer.Deserialize<GroupAttribute[]>(reader);
                return response;
            }

            // If it's not an array, try to deserialize normally
            return serializer.Deserialize<GroupAttributesResponse>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var response = (GroupAttributesResponse)value;
            serializer.Serialize(writer, response.Attributes);
        }
    }
}
/*
[Serializable]
public class GroupAttributesResponse
{
    [JsonProperty("attributes")]
    public GroupAttribute[] Attributes { get; set; } = Array.Empty<GroupAttribute>();
}*/