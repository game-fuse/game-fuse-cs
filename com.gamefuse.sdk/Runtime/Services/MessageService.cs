// Services/MessageService.cs
using GameFuse.Models;
using GameFuse.Transport;
using System;
using System.Collections.Generic; // For List
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    public class MessageService
    {
        private readonly ITransport _transport;

        public MessageService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Fetches paginated chats for the authenticated user.
        /// </summary>
        public async Task<PaginatedChatsResponse> FetchPaginatedChatsAsync(int page, CancellationToken cancellationToken = default)
        {
            // API Path: GET /api/v3/chats/page/{page}
            // Page defaults to 1 if not provided, but our signature requires it.
            string path = (page > 0) ? $"chats/page/{page}" : "chats/page/1"; // Or just "chats" if API supports no page for first
            var response = await _transport.GetAsync<PaginatedChatsResponse>(path, null, cancellationToken);

            if (response == null) return new PaginatedChatsResponse();
            response.DirectChats ??= new List<Chat>();
            response.GroupChats ??= new List<Chat>();
            return response;
        }

        /// <summary>
        /// Creates a new chat (direct or group) and sends the first message.
        /// </summary>
        public async Task<Chat> CreateChatAsync(CreateChatPayload payload, CancellationToken cancellationToken = default) // Return Chat directly
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrEmpty(payload.Text)) throw new ArgumentException("Initial message text cannot be empty.", nameof(payload.Text));
            if ((payload.Usernames == null || !payload.Usernames.Any()) && !payload.GroupId.HasValue)
                throw new ArgumentException("Either usernames or groupId must be provided.", nameof(payload));

            // API Path: POST /api/v3/chats
            // Expecting the Chat object directly as the response
            var chatResponseObject = await _transport.PostAsync<CreateChatPayload, Chat>("chats", payload, null, cancellationToken);

            if (chatResponseObject != null)
            {
                chatResponseObject.Messages ??= new List<Message>();
                chatResponseObject.Participants ??= new List<UserSummary>();
            }
            return chatResponseObject; // Return the Chat object
        }

        /// <summary>
        /// Fetches paginated messages for a specific chat.
        /// </summary>
        public async Task<PaginatedMessagesResponse> FetchPaginatedMessagesAsync(int chatId, int page, CancellationToken cancellationToken = default)
        {
            if (chatId <= 0) throw new ArgumentOutOfRangeException(nameof(chatId), "Chat ID must be positive.");

            // API Path: GET /api/v3/messages/page/{page}?chat_id={chat_id}
            string path = $"messages/page/{(page > 0 ? page : 1)}?chat_id={chatId}";
            var response = await _transport.GetAsync<PaginatedMessagesResponse>(path, null, cancellationToken);

            if (response == null) return new PaginatedMessagesResponse();
            response.Messages ??= new List<Message>();
            return response;
        }

        /// <summary>
        /// Sends a message to an existing chat.
        /// </summary>
        /// <returns>The newly created Message object.</returns>
        public async Task<Message> SendMessageAsync(SendMessagePayload payload, CancellationToken cancellationToken = default)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrEmpty(payload.Text)) throw new ArgumentException("Message text cannot be empty.", nameof(payload.Text));
            if (payload.ChatId <= 0) throw new ArgumentOutOfRangeException(nameof(payload.ChatId), "Chat ID must be positive.");

            // API Path: POST /api/v3/messages
            // Assuming the API returns the Message object directly, not wrapped.
            var response = await _transport.PostAsync<SendMessagePayload, Message>("messages", payload, null, cancellationToken);
            // If it's wrapped like {"message": MessageObject}, then the return type here would be MessageActionResponse
            // and we'd return response.MessageDetail. Based on doc wording, direct Message is likely.
            return response;
        }

        /// <summary>
        /// Marks a specific message in a chat as read by the current user.
        /// </summary>
        public async Task<MarkAsReadResponse> MarkMessageAsReadAsync(int messageId, CancellationToken cancellationToken = default)
        {
            if (messageId <= 0) throw new ArgumentOutOfRangeException(nameof(messageId), "Message ID must be positive.");

            // API Path: POST /api/v3/messages/{id}/mark_as_read
            // No payload.
            string path = $"messages/{messageId}/mark_as_read";
            return await _transport.PostAsync<object, MarkAsReadResponse>(path, null, null, cancellationToken); // Sending null as body
        }
    }
}