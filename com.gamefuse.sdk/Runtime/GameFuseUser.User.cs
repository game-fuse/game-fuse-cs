using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Updates the current user's information.
        /// </summary>
        /// <param name="username">The new username, or null to keep the current one.</param>
        /// <param name="email">The new email, or null to keep the current one.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated user.</returns>
        public Task<User> UpdateUserAsync(string username = null, string email = null, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _userService.UpdateUserAsync(Id, username, email, cancellationToken);
        }

        /// <summary>
        /// Updates the current user's password.
        /// </summary>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdatePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _userService.UpdatePasswordAsync(Id, currentPassword, newPassword, cancellationToken);
        }

        /// <summary>
        /// Gets or creates a custom attribute for the current user.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created or updated attribute.</returns>
        public Task<UserAttribute> SetUserAttributeAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _userService.SetUserAttributeAsync(Id, key, value, cancellationToken);
        }

        /// <summary>
        /// Gets all attributes for the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of user attributes.</returns>
        public Task<IReadOnlyList<UserAttribute>> GetUserAttributesAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _userService.GetUserAttributesAsync(Id, cancellationToken);
        }

        /// <summary>
        /// Deletes a user attribute.
        /// </summary>
        /// <param name="attributeId">The ID of the attribute to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteUserAttributeAsync(int attributeId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _userService.DeleteUserAttributeAsync(Id, attributeId, cancellationToken);
        }
    }
}