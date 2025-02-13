using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Gets a paginated list of chats (both direct and group chats) for the current user.
        /// </summary>
        /// <param name="page">Page number for pagination. Defaults to 1 if not specified.</param>
        /// <returns>Response containing lists of direct and group chats.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GetChatsResponse> GetChatsAsync(int page = 1)
        {
            try
            {
                IChatService chatService = new ChatService(GameFuse.GetBaseURL(), authenticationToken);
                return await chatService.GetChatsAsync(page);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new direct chat with specified users and sends the first message.
        /// </summary>
        /// <param name="usernames">List of usernames to start a direct chat with.</param>
        /// <param name="text">The initial message text to send.</param>
        /// <returns>Response containing the newly created chat.</returns>
        /// <exception cref="ApiException">Thrown when request fails or parameters are invalid.</exception>
        public async Task<Chat> CreateDirectChatAsync(string[] usernames, string text)
        {
            try
            {
                IChatService chatService = new ChatService(GameFuse.GetBaseURL(), authenticationToken);
                return await chatService.CreateDirectChatAsync(usernames, text);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new group chat for a specific group and sends the first message.
        /// </summary>
        /// <param name="groupId">ID of the group to create a chat for.</param>
        /// <param name="text">The initial message text to send.</param>
        /// <returns>Response containing the newly created chat.</returns>
        /// <exception cref="ApiException">Thrown when request fails or parameters are invalid.</exception>
        public async Task<Chat> CreateGroupChatAsync(int groupId, string text)
        {
            try
            {
                IChatService chatService = new ChatService(GameFuse.GetBaseURL(), authenticationToken);
                return await chatService.CreateGroupChatAsync(groupId, text);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves a paginated list of messages for a specific chat.
        /// </summary>
        /// <param name="chatId">The ID of the chat to fetch messages from.</param>
        /// <param name="page">Page number for pagination. Defaults to 1 if not specified.</param>
        /// <returns>Response containing list of messages in the chat.</returns>
        /// <exception cref="ApiException">Thrown when request fails.</exception>
        public async Task<GetMessagesResponse> GetMessagesAsync(int chatId, int page = 1)
        {
            try
            {
                IChatService chatService = new ChatService(GameFuse.GetBaseURL(), authenticationToken);
                return await chatService.GetMessagesAsync(chatId, page);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends a new message to an existing chat.
        /// </summary>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <param name="text">The message text to send.</param>
        /// <returns>Response containing the newly created message.</returns>
        /// <exception cref="ApiException">Thrown when request fails or parameters are invalid.</exception>
        public async Task<ChatMessage> SendMessageAsync(int chatId, string text)
        {
            try
            {
                IChatService chatService = new ChatService(GameFuse.GetBaseURL(), authenticationToken);
                return await chatService.SendMessageAsync(chatId, text);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Marks a specific message as read by the current user.
        /// </summary>
        /// <param name="messageId">The ID of the message to mark as read.</param>
        /// <returns>Response confirming the message has been marked as read.</returns>
        /// <exception cref="ApiException">Thrown when request fails or message is already read.</exception>
        public async Task<MessageReadResponse> MarkMessageAsReadAsync(int messageId)
        {
            try
            {
                IChatService chatService = new ChatService(GameFuse.GetBaseURL(), authenticationToken);
                return await chatService.MarkMessageAsReadAsync(messageId);
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}