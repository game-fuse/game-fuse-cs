using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Models.Auth;
using GameFuse.Transport;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for authentication-related operations.
    /// </summary>
    public class AuthService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the AuthService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public AuthService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Signs up a new user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="username">The user's desired username.</param>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="gameApiKey">The API key for the game.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The newly created user.</returns>
        public async Task<User> SignUpAsync(string email, string password, string username, string gameId, string gameApiKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
            if (string.IsNullOrEmpty(gameApiKey)) throw new ArgumentNullException(nameof(gameApiKey));

            var request = new SignUpRequest
            {
                Email = email,
                Password = password,
                PasswordConfirmation = password,
                Username = username,
                GameId = gameId,
                GameToken = gameApiKey
            };

            try
            {
                return await _transport.PostAsync<SignUpRequest, User>("users", request, null, cancellationToken);
            }
            catch (GameFuseApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new GameFuseApiException("Failed to sign up. Game ID or Game API Key might be wrong.", ex.StatusCode, ex.ApiErrorCode);
                }
                throw;
            }
        }

        /// <summary>
        /// Signs in an existing user.
        /// </summary>
        /// <param name="emailOrUsername">The user's email address or username.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="gameApiKey">The API key for the game.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The signed-in user.</returns>
        public async Task<User> SignInAsync(string emailOrUsername, string password, string gameId, string gameApiKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(emailOrUsername)) throw new ArgumentNullException(nameof(emailOrUsername));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
            if (string.IsNullOrEmpty(gameApiKey)) throw new ArgumentNullException(nameof(gameApiKey));

            var request = new SignInRequest
            {
                Email = emailOrUsername,
                Password = password,
                GameId = gameId,
                GameToken = gameApiKey
            };

            try
            {
                return await _transport.PostAsync<SignInRequest, User>("sessions", request, null, cancellationToken);
            }
            catch (GameFuseApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new GameFuseApiException("User not found or incorrect password.", ex.StatusCode, ex.ApiErrorCode);
                }
                if (ex.StatusCode == HttpStatusCode.PaymentRequired)
                {
                    throw new GameFuseApiException("Game is disabled. Check the GameFuse dashboard.", ex.StatusCode, ex.ApiErrorCode);
                }
                throw;
            }
        }

        /// <summary>
        /// Initiates the forgot password process for a user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="gameApiKey">The API key for the game.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ForgotPasswordAsync(string email, string gameId, string gameApiKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
            if (string.IsNullOrEmpty(gameApiKey)) throw new ArgumentNullException(nameof(gameApiKey));

            // Using the correct API endpoint from documentation - GET request with query parameters
            string path = $"games/{gameId}/forget_password?email={email}&game_id={gameId}&game_token={gameApiKey}";
            await _transport.GetAsync<object>(path, null, cancellationToken);
        }

        /// <summary>
        /// Resets a user's password using a token provided in the forgot password email.
        /// </summary>
        /// <param name="token">The token provided in the forgot password email.</param>
        /// <param name="password">The new password.</param>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="gameApiKey">The API key for the game.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ResetPasswordAsync(string token, string password, string gameId, string gameApiKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
            if (string.IsNullOrEmpty(gameApiKey)) throw new ArgumentNullException(nameof(gameApiKey));

            var request = new ResetPasswordRequest
            {
                Token = token,
                Password = password,
                PasswordConfirmation = password,
                GameId = gameId,
                GameToken = gameApiKey
            };

            await _transport.PostAsync<ResetPasswordRequest>("password/reset", request, null, cancellationToken);
        }
    }
}