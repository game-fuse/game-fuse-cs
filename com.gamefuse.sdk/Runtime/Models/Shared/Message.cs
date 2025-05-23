using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models.Shared
{
    /// <summary>
    /// Represents the type of a chat.
    /// </summary>
    public enum ChatType
    {
        /// <summary>
        /// A direct chat between two users.
        /// </summary>
        Direct,
        
        /// <summary>
        /// A group chat involving multiple users.
        /// </summary>
        Group
    }

    /// <summary>
    /// Represents a single message in a chat.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The message's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The text content of the message.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The ID of the user who sent the message.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// When the message was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// The IDs of users who have read the message.
        /// </summary>
        [JsonProperty("read_by")]
        public List<int> ReadBy { get; set; }

        /// <summary>
        /// Whether the current user has read the message.
        /// </summary>
        [JsonProperty("read")]
        public bool Read { get; set; }

        /// <summary>
        /// Creates a new Message instance with an empty ReadBy list.
        /// </summary>
        public Message()
        {
            ReadBy = new List<int>();
        }
    }

    /// <summary>
    /// Represents a chat conversation (direct or group).
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// The chat's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the user or entity who created the chat.
        /// </summary>
        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        /// <summary>
        /// The type of entity that created the chat (e.g., "User").
        /// </summary>
        [JsonProperty("creator_type")]
        public string CreatorType { get; set; }

        /// <summary>
        /// The most recent messages in the chat.
        /// </summary>
        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }

        /// <summary>
        /// The users participating in the chat.
        /// </summary>
        [JsonProperty("participants")]
        public List<Friend> Participants { get; set; }

        /// <summary>
        /// Gets the type of the chat based on context.
        /// </summary>
        [JsonIgnore]
        public ChatType ChatType { get; internal set; }

        /// <summary>
        /// The ID of the group, if this is a group chat.
        /// </summary>
        [JsonProperty("group_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? GroupId { get; set; }

        /// <summary>
        /// Creates a new Chat instance with empty collections.
        /// </summary>
        public Chat()
        {
            Messages = new List<Message>();
            Participants = new List<Friend>();
        }
    }

    /// <summary>
    /// Response for fetching paginated chats.
    /// </summary>
    public class PaginatedChatsResponse
    {
        /// <summary>
        /// Direct chats between two users.
        /// </summary>
        [JsonProperty("direct_chats")]
        public List<Chat> DirectChats { get; set; }

        /// <summary>
        /// Group chats involving multiple users.
        /// </summary>
        [JsonProperty("group_chats")]
        public List<Chat> GroupChats { get; set; }

        /// <summary>
        /// Creates a new PaginatedChatsResponse with empty collections.
        /// </summary>
        public PaginatedChatsResponse()
        {
            DirectChats = new List<Chat>();
            GroupChats = new List<Chat>();
            
            // Set the chat type for each chat
            foreach (var chat in DirectChats)
            {
                chat.ChatType = ChatType.Direct;
            }
            
            foreach (var chat in GroupChats)
            {
                chat.ChatType = ChatType.Group;
            }
        }

        /// <summary>
        /// Gets all chats (both direct and group).
        /// </summary>
        /// <returns>A combined list of all chats.</returns>
        public List<Chat> GetAllChats()
        {
            var allChats = new List<Chat>();
            allChats.AddRange(DirectChats);
            allChats.AddRange(GroupChats);
            return allChats;
        }
    }

    /// <summary>
    /// Payload for creating a new chat.
    /// </summary>
    public class CreateChatPayload
    {
        /// <summary>
        /// The usernames of users to include in a direct chat.
        /// </summary>
        [JsonProperty("usernames", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Usernames { get; set; }

        /// <summary>
        /// The ID of the group for a group chat.
        /// </summary>
        [JsonProperty("group_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? GroupId { get; set; }

        /// <summary>
        /// The text of the initial message.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// Response for fetching paginated messages for a chat.
    /// </summary>
    public class PaginatedMessagesResponse
    {
        /// <summary>
        /// The list of messages.
        /// </summary>
        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }

        /// <summary>
        /// Creates a new PaginatedMessagesResponse with an empty list.
        /// </summary>
        public PaginatedMessagesResponse()
        {
            Messages = new List<Message>();
        }
    }

    /// <summary>
    /// Payload for sending a message to an existing chat.
    /// </summary>
    public class SendMessagePayload
    {
        /// <summary>
        /// The text of the message.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The ID of the chat to send the message to.
        /// </summary>
        [JsonProperty("chat_id")]
        public int ChatId { get; set; }
    }

    /// <summary>
    /// Response from marking a message as read.
    /// </summary>
    public class MarkAsReadResponse
    {
        /// <summary>
        /// A message indicating the result of the operation.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}