using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Sends a message to another user.
        /// </summary>
        /// <param name="recipientId">The ID of the recipient.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The sent message.</returns>
        public Task<Message> SendMessageAsync(int recipientId, string content, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.SendMessageAsync(Id, recipientId, content, cancellationToken);
        }

        /// <summary>
        /// Gets a conversation between the current user and another user.
        /// </summary>
        /// <param name="otherUserId">The ID of the other user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of messages in the conversation.</returns>
        public Task<IReadOnlyList<Message>> GetConversationAsync(int otherUserId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.GetConversationAsync(Id, otherUserId, cancellationToken);
        }

        /// <summary>
        /// Gets all conversations for the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of conversations.</returns>
        public Task<IReadOnlyList<Conversation>> GetConversationsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.GetConversationsAsync(Id, cancellationToken);
        }

        /// <summary>
        /// Marks a message as read.
        /// </summary>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated message.</returns>
        public Task<Message> MarkMessageAsReadAsync(int messageId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.MarkMessageAsReadAsync(messageId, cancellationToken);
        }

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.DeleteMessageAsync(messageId, cancellationToken);
        }

        /// <summary>
        /// Sends a message to a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The sent message.</returns>
        public Task<GroupMessage> SendGroupMessageAsync(int groupId, string content, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.SendGroupMessageAsync(Id, groupId, content, cancellationToken);
        }

        /// <summary>
        /// Gets all messages for a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of group messages.</returns>
        public Task<IReadOnlyList<GroupMessage>> GetGroupMessagesAsync(int groupId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.GetGroupMessagesAsync(groupId, cancellationToken);
        }

        /// <summary>
        /// Marks a group message as read by the current user.
        /// </summary>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated message.</returns>
        public Task<GroupMessage> MarkGroupMessageAsReadAsync(int messageId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.MarkGroupMessageAsReadAsync(messageId, Id, cancellationToken);
        }

        /// <summary>
        /// Deletes a group message.
        /// </summary>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteGroupMessageAsync(int messageId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _messageService.DeleteGroupMessageAsync(messageId, cancellationToken);
        }
    }
}