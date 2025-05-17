using System.Threading.Tasks;

namespace GameFuseCSharp
{
    /// <summary>
    /// Service interface for managing chats and messages within the GameFuse platform.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Retrieves a paginated list of chats (both direct and group chats) the user is part of.
        /// </summary>
        /// <param name="page">Page number for pagination. Defaults to 1 if not specified.</param>
        /// <returns>Response containing lists of direct and group chats.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        Task<GetChatsResponse> GetChatsAsync(int page = 1);

        /// <summary>
        /// Creates a new direct chat with specified users and sends the first message.
        /// </summary>
        /// <param name="usernames">List of usernames to start a direct chat with.</param>
        /// <param name="text">The initial message text to send.</param>
        /// <returns>Response containing the newly created chat.</returns>
        /// <exception cref="ApiException">Thrown when request fails or parameters are invalid.</exception>
        Task<Chat> CreateDirectChatAsync(string[] usernames, string text);

        /// <summary>
        /// Creates a new group chat for a specific group and sends the first message.
        /// </summary>
        /// <param name="groupId">ID of the group to create a chat for.</param>
        /// <param name="text">The initial message text to send.</param>
        /// <returns>Response containing the newly created chat.</returns>
        /// <exception cref="ApiException">Thrown when request fails or parameters are invalid.</exception>
        Task<Chat> CreateGroupChatAsync(int groupId, string text);

        /// <summary>
        /// Retrieves a paginated list of messages for a specific chat.
        /// </summary>
        /// <param name="chatId">The ID of the chat to fetch messages from.</param>
        /// <param name="page">Page number for pagination. Defaults to 1 if not specified.</param>
        /// <returns>Response containing list of messages in the chat.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        Task<GetMessagesResponse> GetMessagesAsync(int chatId, int page = 1);

        /// <summary>
        /// Sends a new message to an existing chat.
        /// </summary>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <param name="text">The message text to send.</param>
        /// <returns>Response containing the newly created message.</returns>
        /// <exception cref="ApiException">Thrown when request fails or parameters are invalid.</exception>
        Task<ChatMessage> SendMessageAsync(int chatId, string text);

        /// <summary>
        /// Marks a specific message as read by the current user.
        /// </summary>
        /// <param name="messageId">The ID of the message to mark as read.</param>
        /// <returns>Response confirming the message has been marked as read.</returns>
        /// <exception cref="ApiException">Thrown when request fails or message is already read.</exception>
        Task<MessageReadResponse> MarkMessageAsReadAsync(int messageId);
    }
}