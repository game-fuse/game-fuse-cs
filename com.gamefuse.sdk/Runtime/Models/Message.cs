// Models/MessagingModels.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models
{
    /// <summary>
    /// Represents a single message.
    /// </summary>
    public class Message
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; } // ID of the user who sent the message

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; } // Timestamp

        [JsonProperty("read_by")]
        public List<int> ReadBy { get; set; } // List of user IDs who have read the message

        [JsonProperty("read")]
        public bool Read { get; set; } // Whether the current authenticated user has read this message

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
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        [JsonProperty("creator_type")]
        public string CreatorType { get; set; } // e.g., "User"

        [JsonProperty("messages")] // List of most recent messages
        public List<Message> Messages { get; set; }

        [JsonProperty("participants")] // List of users in the chat
        public List<UserSummary> Participants { get; set; } // Using existing UserSummary

        // For Group Chats, there might be a group_id or group_name,
        // The API doc is a bit sparse on distinguishing direct vs group chat structure here,
        // aside from them being in separate lists in the PaginatedChatsResponse.
        // If a group chat has specific group info attached, we'd add it here.
        // For now, assuming 'participants' covers who is in it.

        public Chat()
        {
            Messages = new List<Message>();
            Participants = new List<UserSummary>();
        }
    }

    /// <summary>
    /// Response for fetching paginated chats.
    /// </summary>
    public class PaginatedChatsResponse
    {
        [JsonProperty("direct_chats")]
        public List<Chat> DirectChats { get; set; }

        [JsonProperty("group_chats")]
        public List<Chat> GroupChats { get; set; }

        public PaginatedChatsResponse()
        {
            DirectChats = new List<Chat>();
            GroupChats = new List<Chat>();
        }
    }

    /// <summary>
    /// Payload for creating a new chat.
    /// </summary>
    public class CreateChatPayload
    {
        [JsonProperty("usernames", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Usernames { get; set; } // For direct chat

        [JsonProperty("group_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? GroupId { get; set; } // For group chat

        [JsonProperty("text")]
        public string Text { get; set; } // Initial message text
    }

    

    /// <summary>
    /// Response for fetching paginated messages for a chat.
    /// </summary>
    public class PaginatedMessagesResponse
    {
        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }

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
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("chat_id")]
        public int ChatId { get; set; }
    }

    /// <summary>
    /// Response from sending a message or marking a message as read.
    /// </summary>
    public class MessageActionResponse // Can be used for SendMessage and MarkAsRead
    {
        // Send Message returns the created "message" object
        [JsonProperty("message")]
        public Message MessageDetail { get; set; } // For SendMessage response

        // MarkAsRead only returns a simple "message": "Message marked as read"
        // This means this model might need to be flexible or we use two different models.
        // For simplicity, if MessageDetail is null, we check GeneralMessage.
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        private string GeneralMessageString { get; set; } // For MarkAsRead string response

        [JsonIgnore] // Not part of JSON, just a helper
        public string ConfirmationMessage => GeneralMessageString;

        // If SendMessage only returns Message object without a wrapper "message" key:
        // then SendMessage response is just 'Message' model, MarkAsRead is '{ "message": "string" }'
        // The doc for Send Message says "Response object: message (object) The newly created message object."
        // This implies {"message": { message_details }}
        // The doc for Mark Message as Read says "Response object: message (string) Success message..."
        // This implies {"message": "string_message"}
        // This is problematic for a single response model due to type conflict on "message" key.

        // Let's assume SendMessage returns the Message object directly without a "message" wrapper key.
        // If it *is* wrapped, we'll need to adjust or use dynamic parsing.
        // For now, let's assume:
        // - SendMessage returns Message
        // - MarkAsRead returns MarkAsReadResponse { string Message; }
    }

    // Specific response for SendMessage if it's just the Message object
    // (If API returns {"message": MessageObject}, then MessageActionResponse with MessageDetail is fine)

    // Specific response for MarkAsRead
    public class MarkAsReadResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}