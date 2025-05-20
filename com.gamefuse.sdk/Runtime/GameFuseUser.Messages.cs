// GameFuseUser.Messages.cs
using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq; // For .Any()

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Fetches a paginated list of chats (both direct and group) this user is a part of.
        /// </summary>
        /// <param name="page">Page number for pagination (1-indexed).</param>
        public Task<PaginatedChatsResponse> FetchMyPaginatedChatsAsync(int page = 1, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.FetchPaginatedChatsAsync(page > 0 ? page : 1, cancellationToken);
        }

        /// <summary>
        /// Creates a new direct chat with the specified users and sends an initial message.
        /// </summary>
        /// <param name="usernames">A list of usernames to start the direct chat with.</param>
        /// <param name="initialMessageText">The first message to send.</param>
        public async Task<Chat> CreateDirectChatAsync(List<string> usernames, string initialMessageText, CancellationToken cancellationToken = default) // Return Chat
        {
            EnsureAuthenticated();
            if (usernames == null || !usernames.Any()) throw new System.ArgumentException("Usernames list cannot be null or empty for direct chat.", nameof(usernames));
            if (string.IsNullOrEmpty(initialMessageText)) throw new System.ArgumentNullException(nameof(initialMessageText), "Initial message text cannot be empty.");

            var payload = new CreateChatPayload { Usernames = usernames, Text = initialMessageText };
            // The service now returns Chat directly
            return await _messageService.CreateChatAsync(payload, cancellationToken);
        }

        public async Task<Chat> CreateGroupChatAsync(int groupId, string initialMessageText, CancellationToken cancellationToken = default) // Return Chat
        {
            EnsureAuthenticated();
            if (groupId <= 0) throw new System.ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (string.IsNullOrEmpty(initialMessageText)) throw new System.ArgumentNullException(nameof(initialMessageText), "Initial message text cannot be empty.");

            var payload = new CreateChatPayload { GroupId = groupId, Text = initialMessageText };
            // The service now returns Chat directly
            return await _messageService.CreateChatAsync(payload, cancellationToken);
        }

        /// <summary>
        /// Fetches a paginated list of messages for a specific chat ID.
        /// </summary>
        /// <param name="chatId">The ID of the chat to fetch messages for.</param>
        /// <param name="page">Page number for pagination (1-indexed).</param>
        public Task<PaginatedMessagesResponse> FetchPaginatedMessagesForChatAsync(int chatId, int page = 1, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.FetchPaginatedMessagesAsync(chatId, page > 0 ? page : 1, cancellationToken);
        }

        /// <summary>
        /// Sends a message to an existing chat.
        /// </summary>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <param name="messageText">The text of the message.</param>
        /// <returns>The newly created Message object.</returns>
        public Task<Message> SendMessageToChatAsync(int chatId, string messageText, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var payload = new SendMessagePayload { ChatId = chatId, Text = messageText };
            return _messageService.SendMessageAsync(payload, cancellationToken);
        }

        /// <summary>
        /// Marks a specific message as read by this user.
        /// </summary>
        /// <param name="messageId">The ID of the message to mark as read.</param>
        public Task<MarkAsReadResponse> MarkMessageAsReadAsync(int messageId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.MarkMessageAsReadAsync(messageId, cancellationToken);
        }
    }
}