using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Represents a message sent between users in GameFuse.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The message's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The ID of the user who sent the message.
        /// </summary>
        [JsonProperty("sender_id")]
        public int SenderId { get; internal set; }

        /// <summary>
        /// The username of the user who sent the message.
        /// </summary>
        [JsonProperty("sender_username")]
        public string SenderUsername { get; internal set; }

        /// <summary>
        /// The ID of the user who received the message.
        /// </summary>
        [JsonProperty("recipient_id")]
        public int RecipientId { get; internal set; }

        /// <summary>
        /// The username of the user who received the message.
        /// </summary>
        [JsonProperty("recipient_username")]
        public string RecipientUsername { get; internal set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; internal set; }

        /// <summary>
        /// Whether the message has been read.
        /// </summary>
        [JsonProperty("read")]
        public bool Read { get; internal set; }

        /// <summary>
        /// When the message was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }

        /// <summary>
        /// When the message was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; internal set; }
    }

    /// <summary>
    /// Represents a group message in GameFuse.
    /// </summary>
    public class GroupMessage
    {
        /// <summary>
        /// The message's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The ID of the user who sent the message.
        /// </summary>
        [JsonProperty("sender_id")]
        public int SenderId { get; internal set; }

        /// <summary>
        /// The username of the user who sent the message.
        /// </summary>
        [JsonProperty("sender_username")]
        public string SenderUsername { get; internal set; }

        /// <summary>
        /// The ID of the group the message was sent to.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; internal set; }

        /// <summary>
        /// The name of the group the message was sent to.
        /// </summary>
        [JsonProperty("group_name")]
        public string GroupName { get; internal set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; internal set; }

        /// <summary>
        /// When the message was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }

        /// <summary>
        /// When the message was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; internal set; }

        /// <summary>
        /// The list of users who have read the message.
        /// </summary>
        [JsonProperty("read_by")]
        public IReadOnlyList<int> ReadBy { get; internal set; }
    }

    /// <summary>
    /// Represents a conversation between users.
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// The ID of the other user in the conversation.
        /// </summary>
        [JsonProperty("other_user_id")]
        public int OtherUserId { get; internal set; }

        /// <summary>
        /// The username of the other user in the conversation.
        /// </summary>
        [JsonProperty("other_user_username")]
        public string OtherUserUsername { get; internal set; }

        /// <summary>
        /// The most recent message in the conversation.
        /// </summary>
        [JsonProperty("last_message")]
        public Message LastMessage { get; internal set; }

        /// <summary>
        /// The number of unread messages in the conversation.
        /// </summary>
        [JsonProperty("unread_count")]
        public int UnreadCount { get; internal set; }
    }
}