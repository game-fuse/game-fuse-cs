using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Transport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    

    /// <summary>
    /// Service for user-related operations.
    /// </summary>
    public class UserService
    {
        private readonly ITransport _transport;

        public UserService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public Task<User> GetUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            return _transport.GetAsync<User>($"users/{userId}", null, cancellationToken);
        }

        public Task<User> UpdateUserAsync(int userId, string username = null, string email = null, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");

            var requestPayload = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(username)) requestPayload["username"] = username;
            if (!string.IsNullOrEmpty(email)) requestPayload["email"] = email;

            if (requestPayload.Count == 0)
            {
                throw new ArgumentException("At least one of username or email must be provided for update.");
            }

            return _transport.PutAsync<Dictionary<string, object>, User>($"users/{userId}", requestPayload, null, cancellationToken);
        }

        public Task UpdatePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(currentPassword)) throw new ArgumentNullException(nameof(currentPassword));
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword));

            var requestPayload = new Dictionary<string, string>
            {
                ["current_password"] = currentPassword,
                ["password"] = newPassword,
                ["password_confirmation"] = newPassword
            };

            return _transport.PutAsync<Dictionary<string, string>>($"users/{userId}/password", requestPayload, null, cancellationToken);
        }

        public Task<UserAttributes> SetUserAttributeAsync(int userId, string key, string value, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var requestPayload = new Dictionary<string, string>
            {
                ["key"] = key,
                ["value"] = value
            };
            return _transport.PostAsync<Dictionary<string, string>, UserAttributes>($"users/{userId}/add_game_user_attribute", requestPayload, null, cancellationToken);
        }

        public Task<UserAttributes> GetUserAttributesAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            return  _transport.GetAsync<UserAttributes>($"users/{userId}/game_user_attributes", null, cancellationToken);
        }

        public Task<UserAttributes> DeleteUserAttributeAsync(int userId, string attributeKey, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(attributeKey)) throw new ArgumentNullException(nameof(attributeKey));

            string encodedKey = System.Uri.EscapeDataString(attributeKey);
            string pathWithQuery = $"users/{userId}/remove_game_user_attribute?game_user_attribute_key={encodedKey}";
            UnityEngine.Debug.Log($"[UserService.DeleteUserAttributeAsync] Requesting URL path: {pathWithQuery}"); // LOG THIS
            return _transport.GetAsync<UserAttributes>($"users/{userId}/remove_game_user_attribute?game_user_attribute_key={encodedKey}", null, cancellationToken);
        }

        public Task<User> AddScoreAsync(int userId, int scoreAmount, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            var payload = new { score = scoreAmount };
            UnityEngine.Debug.Log($"post with score amout {scoreAmount}");
            return _transport.PostAsync<object, User>($"users/{userId}/add_score", payload, null, cancellationToken);
        }

        public Task<User> SetScoreAsync(int userId, int newScore, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            var payload = new { score = newScore };
            return _transport.PostAsync<object, User>($"users/{userId}/set_score", payload, null, cancellationToken);
        }

        public Task<User> AddCreditsAsync(int userId, int creditsAmount, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            var payload = new { credits = creditsAmount };
            return _transport.PostAsync<object, User>($"users/{userId}/add_credits", payload, null, cancellationToken);
        }

        public Task<User> SetCreditsAsync(int userId, int newCredits, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            var payload = new { credits = newCredits };
            return _transport.PostAsync<object, User>($"users/{userId}/set_credits", payload, null, cancellationToken);
        }

        public Task<UserAttributes> SetUserAttributesBatchAsync(int userId, List<UserAttributePayload> attributes, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (attributes == null || attributes.Count == 0) throw new ArgumentNullException(nameof(attributes), "Attributes list cannot be null or empty.");

            var requestPayload = new { attributes = attributes };
            return _transport.PostAsync<object, UserAttributes>($"users/{userId}/add_game_user_attribute", requestPayload, null, cancellationToken);
        }

        public Task<UserStore> GetUserStoreAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            return _transport.GetAsync<UserStore>($"users/{userId}/game_user_store_items", null, cancellationToken);
        }

        public async Task<LeaderboardEntries> GetUserLeaderboardEntriesAsync(int userId, string leaderboardName = null, int? limit = null, bool? onePerUser = null, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(leaderboardName)) queryParams["leaderboard_name"] = leaderboardName;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            // Ensure boolean is string "true" or "false" for query param
            if (onePerUser.HasValue) queryParams["one_per_user"] = onePerUser.Value.ToString().ToLowerInvariant();

            string queryString = "";
            if (queryParams.Count > 0)
            {
                var parts = new List<string>();
                foreach (var kvp in queryParams)
                {
                    parts.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
                queryString = "?" + string.Join("&", parts);
            }

            var response = await _transport.GetAsync<LeaderboardEntries>($"users/{userId}/leaderboard_entries{queryString}", null, cancellationToken);
            return response;
            // The GameFuseUser layer will extract response.LeaderboardEntries
        }
    }

    
}