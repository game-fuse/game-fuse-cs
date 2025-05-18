using GameFuse.Exceptions;
using GameFuse.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameFuse.Transport;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for user-related operations.
    /// </summary>
    public class UserService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the UserService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public UserService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Gets information about the current user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The user information.</returns>
        public Task<User> GetUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            return _transport.GetAsync<User>($"users/{userId}", null, cancellationToken);
        }

        /// <summary>
        /// Updates a user's information.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="username">The new username, or null to keep the current one.</param>
        /// <param name="email">The new email, or null to keep the current one.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated user information.</returns>
        public Task<User> UpdateUserAsync(int userId, string username = null, string email = null, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var request = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(username))
            {
                request["username"] = username;
            }
            
            if (!string.IsNullOrEmpty(email))
            {
                request["email"] = email;
            }
            
            if (request.Count == 0)
            {
                throw new ArgumentException("At least one of username or email must be provided.");
            }
            
            return _transport.PutAsync<Dictionary<string, object>, User>($"users/{userId}", request, null, cancellationToken);
        }

        /// <summary>
        /// Updates a user's password.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="currentPassword">The user's current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdatePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(currentPassword)) throw new ArgumentNullException(nameof(currentPassword));
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword));
            
            var request = new Dictionary<string, string>
            {
                ["current_password"] = currentPassword,
                ["password"] = newPassword,
                ["password_confirmation"] = newPassword
            };
            
            return _transport.PutAsync<Dictionary<string, string>>($"users/{userId}/password", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets or creates a custom attribute for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created or updated attribute.</returns>
        public Task<UserAttribute> SetUserAttributeAsync(int userId, string key, string value, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            
            var request = new Dictionary<string, string>
            {
                ["key"] = key,
                ["value"] = value
            };
            
            return _transport.PostAsync<Dictionary<string, string>, UserAttribute>($"users/{userId}/attributes", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets all attributes for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of user attributes.</returns>
        public async Task<IReadOnlyList<UserAttribute>> GetUserAttributesAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<UserAttribute>>($"users/{userId}/attributes", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Deletes a user attribute.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="attributeId">The ID of the attribute to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteUserAttributeAsync(int userId, int attributeId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (attributeId <= 0) throw new ArgumentOutOfRangeException(nameof(attributeId), "Attribute ID must be positive.");
            
            return _transport.DeleteAsync($"users/{userId}/attributes/{attributeId}", null, cancellationToken);
        }
    }
}