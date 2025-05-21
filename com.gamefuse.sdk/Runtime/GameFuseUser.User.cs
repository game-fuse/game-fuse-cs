using GameFuse.Models.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameFuse.Services; // Required for UserAttributePayload if used directly here

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Retrieves the most up-to-date information for the currently authenticated user
        /// from the server and updates the CurrentUser's internal data.
        /// </summary>
        /// <param name="cancellationToken">Optional token to cancel the operation.</param>
        /// <returns>The refreshed <see cref="User"/> record now stored internally.</returns>
        public async Task<User> GetUserDetailsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            await _userService.GetUserAsync(this.Id, cancellationToken); //ToDo Figure out what to update
            return _userData;
        }

        /// <summary>
        /// Fetches details for a specific user by their ID, using the current authenticated session.
        /// API: GET /api/v3/users/{id}
        /// </summary>
        /// <param name="userIdToFetch">The ID of the user to fetch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The User object for the specified user ID.</returns>
        public async Task<User> GetUserDetailsAsync(int userIdToFetch, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _userService.GetUserAsync(userIdToFetch, cancellationToken);
        }

        /// <summary>
        /// Updates the current user's profile information (e.g., username, email) on the server
        /// and refreshes the CurrentUser's internal data.
        /// Note: This assumes the V3 API has a corresponding endpoint that returns the full User object.
        /// </summary>
        /// <param name="username">The new username, or null to keep the current one.</param>
        /// <param name="email">The new email, or null to keep the current one.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data after the operation.</returns>
        public async Task<User> UpdateProfileAsync(string username = null, string email = null, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            // Assuming _userService.UpdateUserAsync returns the FULL updated User model.
            // If not, this should just await the service call and then call RefreshUserDataAsync.
            var updatedUserFromService = await _userService.UpdateUserAsync(this.Id, username, email, cancellationToken);
            return _userData;
        }

        /// <summary>
        /// Updates the current user's password on the server.
        /// After a successful password update, user data is refreshed.
        /// Note: This assumes the V3 API has a corresponding endpoint.
        /// </summary>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data after refreshing.</returns>
        public async Task<User> UpdatePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            await _userService.UpdatePasswordAsync(this.Id, currentPassword, newPassword, cancellationToken);
            return await GetUserDetailsAsync(cancellationToken);
        }

        /// <summary>
        /// Sets a single custom attribute for the current user on the server.
        /// After successfully setting the attribute, the user's data is refreshed.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data after refreshing.</returns>
        public async Task<User> SetUserAttributeAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            await _userService.SetUserAttributeAsync(this.Id, key, value, cancellationToken);
            return await GetUserDetailsAsync(cancellationToken);
        }

        /// <summary>
        /// Sets a batch of custom attributes for the current user on the server.
        /// After successfully setting the attributes, the user's data is refreshed.
        /// </summary>
        /// <param name="attributesToSet">A list of KeyValuePair representing attributes to set.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data after refreshing.</returns>
        public async Task<User> SetUserAttributesBatchAsync(List<KeyValuePair<string, string>> attributesToSet, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            if (attributesToSet == null || attributesToSet.Count == 0)
            {
                // Optionally, just return current _userData or throw ArgumentNullException
                return _userData;
            }

            var payload = new List<UserAttributePayload>();
            foreach (var kvp in attributesToSet)
            {
                payload.Add(new UserAttributePayload(kvp.Key, kvp.Value));
            }

            await _userService.SetUserAttributesBatchAsync(this.Id, payload, cancellationToken);
            return await GetUserDetailsAsync(cancellationToken);
        }

        /// <summary>
        /// Gets all custom attributes for the current user by refreshing all user data.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data containing all user attributes.</returns>
        public async Task<UserAttributes> GetUserAttributesAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            // _userService.GetUserAttributesAsync returns IReadOnlyList<UserAttribute> directly
            return await _userService.GetUserAttributesAsync(this.Id, cancellationToken);
        }

        /// <summary>
        /// Fetches custom game attributes for a specific user by their ID, using the current authenticated session.
        /// API: GET /api/v3/users/{id}/game_user_attributes
        /// </summary>
        /// <param name="userIdToFetch">The ID of the user whose attributes to fetch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of user attributes for the specified user ID.</returns>
        public async Task<UserAttributes> GetUserAttributesAsync(int userIdToFetch, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            // _userService.GetUserAttributesAsync returns IReadOnlyList<UserAttribute> directly
            return await _userService.GetUserAttributesAsync(userIdToFetch, cancellationToken);
        }

        /// <summary>
        /// Deletes a user attribute by its key on the server.
        /// After successfully deleting the attribute, the user's data is refreshed.
        /// </summary>
        /// <param name="attributeKey">The key of the attribute to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data after refreshing.</returns>
        public async Task<User> DeleteUserAttributeAsync(string attributeKey, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            await _userService.DeleteUserAttributeAsync(this.Id, attributeKey, cancellationToken);
            return await GetUserDetailsAsync(cancellationToken);
        }

        // --- Score Management Methods ---

        /// <summary>
        /// Adds a specified amount to the user's current score.
        /// Updates local user data with the response from the server.
        /// </summary>
        /// <param name="scoreAmount">The amount to add to the score (can be negative).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data.</returns>
        public async Task<User> AddScoreAsync(int scoreAmount, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var userResponse = await _userService.AddScoreAsync(this.Id, scoreAmount, cancellationToken);
            if (userResponse != null)
            {
                this.SetInternalScore(userResponse.Score);
            }
            return _userData;
        }

        /// <summary>
        /// Sets the user's score to an absolute value.
        /// Updates local user data with the response from the server.
        /// </summary>
        /// <param name="newScore">The new absolute score for the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data.</returns>
        public async Task<User> SetScoreAsync(int scoreAmount, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            User userResponse = await _userService.SetScoreAsync(this.Id, scoreAmount, cancellationToken);
            if (userResponse != null)
            {
                this.SetInternalScore(userResponse.Score);
            }
            return _userData; // Return the (partially) updated local user data
        }

        // --- Credits Management Methods ---

        /// <summary>
        /// Adds a specified amount to the user's current credits.
        /// Updates local user data with the response from the server.
        /// </summary>
        /// <param name="creditsAmount">The amount to add to credits (can be negative).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data.</returns>
        public async Task<User> AddCreditsAsync(int creditsAmount, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var userResponse = await _userService.AddCreditsAsync(this.Id, creditsAmount, cancellationToken);
            if(userResponse != null)
            {
                SetInternalCredits(userResponse.Credits);
            }
            return _userData;
        }

        /// <summary>
        /// Sets the user's credits to an absolute value.
        /// Updates local user data with the response from the server.
        /// </summary>
        /// <param name="newCredits">The new absolute credits for the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated local user data.</returns>
        public async Task<User> SetCreditsAsync(int newCredits, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var userResponse = await _userService.SetCreditsAsync(this.Id, newCredits, cancellationToken);
            if(userResponse != null)
            {
                SetInternalCredits(userResponse.Credits);
            }
            return _userData;
        }

        public async Task<UserStore> GetUserStoreItemsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _userService.GetUserStoreAsync(Id, cancellationToken);
        }

        /// <summary>
        /// Fetches store items purchased by a specific user by their ID, using the current authenticated session.
        /// API: GET /api/v3/users/{id}/game_user_store_items
        /// </summary>
        /// <param name="userIdToFetch">The ID of the user whose store items to fetch.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>An object containing the specified user's credits and their list of purchased store items.</returns>
        public async Task<UserStore> GetUserStoreItemsAsync(int userIdToFetch, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _userService.GetUserStoreAsync(userIdToFetch, cancellationToken);
        }

        public async Task<LeaderboardEntries> GetUserLeaderboardEntriesAsync(string leaderboardName = null, int? limit = null, bool? onePerUser = null, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _userService.GetUserLeaderboardEntriesAsync(Id, leaderboardName, limit, onePerUser, cancellationToken);
        }

        /// <summary>
        /// Fetches leaderboard entries for a specific user by their ID, using the current authenticated session.
        /// API: GET /api/v3/users/{id}/leaderboard_entries
        /// </summary>
        /// <param name="userIdToFetch">The ID of the user whose leaderboard entries to fetch.</param>
        /// <param name="leaderboardName">Optional: Filter by leaderboard name.</param>
        /// <param name="limit">Optional: Limit the number of results.</param>
        /// <param name="onePerUser">Optional: If true, get only one result per player on the leaderboard.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of leaderboard entries for the specified user.</returns>
        public async Task<LeaderboardEntries> GetUserLeaderboardEntriesAsync(int userIdToFetch, string leaderboardName = null, int? limit = null, bool? onePerUser = null, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return await _userService.GetUserLeaderboardEntriesAsync(userIdToFetch, leaderboardName, limit, onePerUser,cancellationToken);
        }
    }
}