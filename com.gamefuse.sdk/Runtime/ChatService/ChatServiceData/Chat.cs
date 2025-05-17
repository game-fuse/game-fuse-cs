using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Represents a chat in the GameFuse system.
    /// </summary>
    [Serializable]
    public class Chat
    {
        /// <summary>
        /// The unique identifier for the chat.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier of the creator of the chat.
        /// </summary>
        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        /// <summary>
        /// The type of the creator (e.g., "User" or "Group").
        /// </summary>
        [JsonProperty("creator_type")]
        public string CreatorType { get; set; }

        /// <summary>
        /// The messages in the chat.
        /// </summary>
        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        /// <summary>
        /// The participants in the chat. This is populated from the chat_users field in the response.
        /// </summary>
        [JsonIgnore] // This field is not directly in the JSON, it gets populated manually
        public ChatParticipant[] Participants { get; set; }
        
        /// <summary>
        /// Debugging property to dump raw JSON.
        /// </summary>
        [JsonIgnore]
        public string RawJson { get; set; }
    }
}
