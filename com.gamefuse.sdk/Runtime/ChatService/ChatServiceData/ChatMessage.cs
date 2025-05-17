using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Represents a message within a chat.
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        /// <summary>
        /// The unique identifier for the message.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The user ID of the sender.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// The timestamp when the message was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// List of user IDs who have read the message.
        /// </summary>
        [JsonProperty("read_by")]
        public List<int> ReadBy { get; set; } = new List<int>();

        /// <summary>
        /// Whether the message has been read by the current user.
        /// </summary>
        [JsonProperty("read")]
        public bool Read { get; set; }
    }
}
