using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    [JsonConverter(typeof(GetMessagesResponseConverter))]
    public class GetMessagesResponse
    {
        [JsonProperty("messages")]
        public ChatMessage[] Messages { get; set; }

        // Required parameterless constructor
        public GetMessagesResponse()
        {
            Messages = Array.Empty<ChatMessage>();
        }
    }

    public class GetMessagesResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GetMessagesResponse);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                // Handle direct array response
                var messages = serializer.Deserialize<ChatMessage[]>(reader);
                return new GetMessagesResponse { Messages = messages };
            }

            // If it's not an array, try to deserialize normally as an object
            return serializer.Deserialize<GetMessagesResponse>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var response = (GetMessagesResponse)value;
            serializer.Serialize(writer, response);
        }
    }
}
