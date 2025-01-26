using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    [Serializable]
    [JsonConverter(typeof(GameRoundsResponseConverter))]
    public class GameRoundsResponse
    {
        [JsonProperty("game_rounds")]
        public GameRoundObject[] GameRounds { get; set; }

        public GameRoundsResponse()
        {
            GameRounds = Array.Empty<GameRoundObject>();
        }
    }

    public class GameRoundsResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GameRoundsResponse);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var response = new GameRoundsResponse();
                response.GameRounds = serializer.Deserialize<GameRoundObject[]>(reader);
                return response;
            }

            return serializer.Deserialize<GameRoundsResponse>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var response = (GameRoundsResponse)value;
            serializer.Serialize(writer, response.GameRounds);
        }
    }
}
