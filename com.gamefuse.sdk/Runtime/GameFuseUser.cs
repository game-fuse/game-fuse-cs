using GameFuse.Config;
using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFuse
{
    /// <summary>
    /// Facade for the GameFuse SDK. Provides a user-centric entry point for all GameFuse functionality.
    /// </summary>
    public partial class GameFuseUser
    {
        private ITransport _transport = new UnityWebRequestTransport();

        /// <summary>
        /// The currently authenticated user. Set upon successful sign-in/sign-up and cleared on sign-out.
        /// </summary>
        public static GameFuseUser CurrentUser { get; private set; }

        /// <summary>
        /// The ID of the user.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// The display email of the user.
        /// </summary>
        public string DisplayEmail { get; private set; }

        /// <summary>
        /// The authentication token for the user.
        /// </summary>
        internal string AuthenticationToken { get; }

        /// <summary>
        /// The number of credits the user has.
        /// </summary>
        public int Credits { get; private set; }

        /// <summary>
        /// The score of the user.
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// The date and time of the user's last login.
        /// </summary>
        public string LastLogin { get; }

        /// <summary>
        /// The number of times the user has logged in.
        /// </summary>
        public int NumberOfLogins { get; }

        
        private readonly UserService _userService;
        private readonly GameRoundService _gameRoundService;
        private readonly StoreService _storeService;
        private readonly FriendService _friendService;
        private readonly GroupService _groupService;
        private readonly MessageService _messageService;

        public GameFuseUser(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            DisplayEmail = user.DisplayEmail;
            AuthenticationToken = user.AuthenticationToken;
            Credits = user.Credits;
            Score = user.Score;
            LastLogin = user.LastLogin;
            NumberOfLogins = user.NumberOfLogins;

            _transport.SetAuthHeaderProvider(() => new Dictionary<string, string>
            {
                ["authentication-token"] = user.AuthenticationToken
            });


            _userService = new UserService(_transport);
            _gameRoundService = new GameRoundService(_transport);
            _storeService = new StoreService(_transport);
            _friendService = new FriendService(_transport);
            _groupService = new GroupService(_transport);
            _messageService = new MessageService(_transport);
        }

        public void UpdateUser(User user)
        {
            Email = user.Email;
            DisplayEmail = user.DisplayEmail;
            Credits = user.Credits;
            Score = user.Score;
        }

        /// <summary>
        /// Signs up a new user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="username">The user's desired username.</param>
        /// <param name="gameId">The ID of the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="gameApiKey">The API key for the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The newly created GameFuseUser.</returns>
        public static async Task<GameFuseUser> SignUpAsync(string email, string password, string username, string gameId = null, string gameApiKey = null, CancellationToken cancellationToken = default)
        {
            gameId ??= GameFuseSettings.Settings?.GameId;
            gameApiKey ??= GameFuseSettings.Settings?.GameApiKey;

            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentNullException(nameof(gameId), "Game ID must be provided explicitly or through GameFuseSettings.");
            }

            if (string.IsNullOrEmpty(gameApiKey))
            {
                throw new ArgumentNullException(nameof(gameApiKey), "Game API Key must be provided explicitly or through GameFuseSettings.");
            }

            var transport = new UnityWebRequestTransport();
            var authService = new AuthService(transport);
            var user = await authService.SignUpAsync(email, password, username, gameId, gameApiKey, cancellationToken);
            CurrentUser = new GameFuseUser(user);
            return CurrentUser;
        }

        /// <summary>
        /// Signs in an existing user.
        /// </summary>
        /// <param name="emailOrUsername">The user's email address or username.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="gameId">The ID of the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="gameApiKey">The API key for the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The signed-in GameFuseUser.</returns>
        public static async Task<GameFuseUser> SignInAsync(string emailOrUsername, string password, string gameId = null, string gameApiKey = null, CancellationToken cancellationToken = default)
        {
            gameId ??= GameFuseSettings.Settings?.GameId;
            gameApiKey ??= GameFuseSettings.Settings?.GameApiKey;

            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentNullException(nameof(gameId), "Game ID must be provided explicitly or through GameFuseSettings.");
            }

            if (string.IsNullOrEmpty(gameApiKey))
            {
                throw new ArgumentNullException(nameof(gameApiKey), "Game API Key must be provided explicitly or through GameFuseSettings.");
            }

            var transport = new UnityWebRequestTransport();
            var authService = new AuthService(transport);
            var user = await authService.SignInAsync(emailOrUsername, password, gameId, gameApiKey, cancellationToken);
            CurrentUser = new GameFuseUser(user);
            return CurrentUser;
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            CurrentUser = null;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Initiates the forgot password process for a user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="gameId">The ID of the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="gameApiKey">The API key for the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task ForgotPasswordAsync(string email, string gameId = null, string gameApiKey = null, CancellationToken cancellationToken = default)
        {
            gameId ??= GameFuseSettings.Settings?.GameId;
            gameApiKey ??= GameFuseSettings.Settings?.GameApiKey;

            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentNullException(nameof(gameId), "Game ID must be provided explicitly or through GameFuseSettings.");
            }

            if (string.IsNullOrEmpty(gameApiKey))
            {
                throw new ArgumentNullException(nameof(gameApiKey), "Game API Key must be provided explicitly or through GameFuseSettings.");
            }

            var transport = new UnityWebRequestTransport();
            var authService = new AuthService(transport);
            return authService.ForgotPasswordAsync(email, gameId, gameApiKey, cancellationToken);
        }

        /// <summary>
        /// Resets a user's password using a token provided in the forgot password email.
        /// </summary>
        /// <param name="token">The token provided in the forgot password email.</param>
        /// <param name="password">The new password.</param>
        /// <param name="gameId">The ID of the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="gameApiKey">The API key for the game. If null, uses the value from GameFuseSettings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task ResetPasswordAsync(string token, string password, string gameId = null, string gameApiKey = null, CancellationToken cancellationToken = default)
        {
            gameId ??= GameFuseSettings.Settings?.GameId;
            gameApiKey ??= GameFuseSettings.Settings?.GameApiKey;

            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentNullException(nameof(gameId), "Game ID must be provided explicitly or through GameFuseSettings.");
            }

            if (string.IsNullOrEmpty(gameApiKey))
            {
                throw new ArgumentNullException(nameof(gameApiKey), "Game API Key must be provided explicitly or through GameFuseSettings.");
            }

            var transport = new UnityWebRequestTransport();
            var authService = new AuthService(transport);
            return authService.ResetPasswordAsync(token, password, gameId, gameApiKey, cancellationToken);
        }

        /// <summary>
        /// Checks if the user is authenticated.
        /// </summary>
        /// <returns>True if the user is authenticated, false otherwise.</returns>
        public static bool IsAuthenticated()
        {
            return CurrentUser != null;
        }

        /// <summary>
        /// Ensures that the user is authenticated. Throws an exception if not.
        /// </summary>
        /// <exception cref="GameFuseNotAuthenticatedException">Thrown if the user is not authenticated.</exception>
        private static void EnsureAuthenticated()
        {
            if (CurrentUser == null)
            {
                throw new GameFuseNotAuthenticatedException();
            }
        }
    }
}