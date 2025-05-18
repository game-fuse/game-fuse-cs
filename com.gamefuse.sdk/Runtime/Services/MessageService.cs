using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for message-related operations.
    /// </summary>
    public class MessageService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the MessageService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public MessageService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Sends a message to another user.
        /// </summary>
        /// <param name="senderId">The ID of the sender.</param>
        /// <param name="recipientId">The ID of the recipient.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The sent message.</returns>
        public Task<Message> SendMessageAsync(int senderId, int recipientId, string content, CancellationToken cancellationToken = default)
        {
            if (senderId <= 0) throw new ArgumentOutOfRangeException(nameof(senderId), "Sender ID must be positive.");
            if (recipientId <= 0) throw new ArgumentOutOfRangeException(nameof(recipientId), "Recipient ID must be positive.");
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException(nameof(content));
            
            var request = new Dictionary<string, object>
            {
                ["sender_id"] = senderId,
                ["recipient_id"] = recipientId,
                ["content"] = content
            };
            
            return _transport.PostAsync<Dictionary<string, object>, Message>("messages", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets a conversation between two users.
        /// </summary>
        /// <param name="userId">The ID of the first user.</param>
        /// <param name="otherUserId">The ID of the second user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of messages in the conversation.</returns>
        public async Task<IReadOnlyList<Message>> GetConversationAsync(int userId, int otherUserId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (otherUserId <= 0) throw new ArgumentOutOfRangeException(nameof(otherUserId), "Other User ID must be positive.");
            
            var response = await _transport.GetAsync<List<Message>>($"users/{userId}/conversations/{otherUserId}", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Gets all conversations for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of conversations.</returns>
        public async Task<IReadOnlyList<Conversation>> GetConversationsAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<Conversation>>($"users/{userId}/conversations", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Marks a message as read.
        /// </summary>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated message.</returns>
        public Task<Message> MarkMessageAsReadAsync(int messageId, CancellationToken cancellationToken = default)
        {
            if (messageId <= 0) throw new ArgumentOutOfRangeException(nameof(messageId), "Message ID must be positive.");
            
            var request = new Dictionary<string, bool>
            {
                ["read"] = true
            };
            
            return _transport.PutAsync<Dictionary<string, bool>, Message>($"messages/{messageId}", request, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken = default)
        {
            if (messageId <= 0) throw new ArgumentOutOfRangeException(nameof(messageId), "Message ID must be positive.");
            
            return _transport.DeleteAsync($"messages/{messageId}", null, cancellationToken);
        }

        /// <summary>
        /// Sends a message to a group.
        /// </summary>
        /// <param name="senderId">The ID of the sender.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The sent message.</returns>
        public Task<GroupMessage> SendGroupMessageAsync(int senderId, int groupId, string content, CancellationToken cancellationToken = default)
        {
            if (senderId <= 0) throw new ArgumentOutOfRangeException(nameof(senderId), "Sender ID must be positive.");
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException(nameof(content));
            
            var request = new Dictionary<string, object>
            {
                ["sender_id"] = senderId,
                ["group_id"] = groupId,
                ["content"] = content
            };
            
            return _transport.PostAsync<Dictionary<string, object>, GroupMessage>("group_messages", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets all messages for a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of group messages.</returns>
        public async Task<IReadOnlyList<GroupMessage>> GetGroupMessagesAsync(int groupId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            
            var response = await _transport.GetAsync<List<GroupMessage>>($"groups/{groupId}/messages", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Marks a group message as read by a user.
        /// </summary>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="userId">The ID of the user marking the message as read.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated message.</returns>
        public Task<GroupMessage> MarkGroupMessageAsReadAsync(int messageId, int userId, CancellationToken cancellationToken = default)
        {
            if (messageId <= 0) throw new ArgumentOutOfRangeException(nameof(messageId), "Message ID must be positive.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var request = new Dictionary<string, int>
            {
                ["user_id"] = userId
            };
            
            return _transport.PutAsync<Dictionary<string, int>, GroupMessage>($"group_messages/{messageId}/read", request, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a group message.
        /// </summary>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteGroupMessageAsync(int messageId, CancellationToken cancellationToken = default)
        {
            if (messageId <= 0) throw new ArgumentOutOfRangeException(nameof(messageId), "Message ID must be positive.");
            
            return _transport.DeleteAsync($"group_messages/{messageId}", null, cancellationToken);
        }
    }
}