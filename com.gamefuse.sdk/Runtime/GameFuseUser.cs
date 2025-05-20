// GameFuseUser.cs

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
        private User _userData; // Store the underlying User model

        /// <summary>
        /// The currently authenticated user. Set upon successful sign-in/sign-up and cleared on sign-out.
        /// </summary>
        public static GameFuseUser CurrentUser { get; private set; }

        // --- Properties delegating to _userData ---
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public int Id => _userData.Id;

        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username => _userData.Username;

        /// <summary>
        /// The system email of the user.
        /// </summary>
        public string Email => _userData.Email;

        /// <summary>
        /// The display email of the user.
        /// </summary>
        public string DisplayEmail => _userData.DisplayEmail;

        /// <summary>
        /// The number of credits the user has.
        /// </summary>
        public int Credits => _userData.Credits;

        /// <summary>
        /// The score of the user.
        /// </summary>
        public int Score => _userData.Score;

        /// <summary>
        /// The date and time of the user's last login.
        /// </summary>
        public string LastLogin => _userData.LastLogin;

        /// <summary>
        /// The number of times the user has logged in.
        /// </summary>
        public int NumberOfLogins => _userData.NumberOfLogins;

        /// <summary>
        /// Running API hits for this user.
        /// </summary>
        public int EventsTotal => _userData.EventsTotal;

        /// <summary>
        /// Running API hits for this user for the current month.
        /// </summary>
        public int EventsCurrentMonth => _userData.EventsCurrentMonth;

        /// <summary>
        /// Unique game sessions for this user.
        /// </summary>
        public int GameSessionsTotal => _userData.GameSessionsTotal;

        /// <summary>
        /// Unique game sessions for this user during the current month.
        /// </summary>
        public int GameSessionsCurrentMonth => _userData.GameSessionsCurrentMonth;

        /// <summary>
        /// Custom attributes associated with the user.
        /// Fetched separately, or potentially part of initial sign-in.
        /// </summary>
        public IReadOnlyList<UserAttribute> GameUserAttributes => _userData.GameUserAttributes;

        /// <summary>
        /// Store items purchased by the user.
        /// Fetched separately, or potentially part of initial sign-in.
        /// </summary>
        public IReadOnlyList<StoreItem> GameUserStoreItems => _userData.GameUserStoreItems;

        /// <summary>
        /// The user's friends.
        /// Fetched separately, or potentially part of initial sign-in.
        /// </summary>
        public IReadOnlyList<Friend> Friends => _userData.Friends;

        /// <summary>
        /// Outgoing friend requests sent by the user.
        /// </summary>
        public IReadOnlyList<FriendRequest> OutgoingFriendRequests => _userData.OutgoingFriendRequests;

        /// <summary>
        /// Incoming friend requests received by the user.
        /// </summary>
        public IReadOnlyList<FriendRequest> IncomingFriendRequests => _userData.IncomingFriendRequests;

        /// <summary>
        /// Groups that the user is a member of.
        /// </summary>
        public IReadOnlyList<GroupSummary> Groups => _userData.Groups;

        /// <summary>
        /// Group join requests sent by the user.
        /// </summary>
        public IReadOnlyList<GroupConnectionResponse> GroupJoinRequests => _userData.GroupJoinRequests;

        /// <summary>
        /// Group invites received by the user.
        /// </summary>
        public IReadOnlyList<MyGroupInvite> GroupInvites => _userData.GroupInvites;


        /// <summary>
        /// The authentication token for the user.
        /// </summary>
        internal string AuthenticationToken => _userData.AuthenticationToken;

        private readonly UserService _userService;
        private readonly GameRoundService _gameRoundService;
        private readonly StoreService _storeService;
        private readonly FriendService _friendService;
        private readonly GroupService _groupService;
        private readonly MessageService _messageService;

        public GameFuseUser(User user)
        {
            _userData = user ?? throw new ArgumentNullException(nameof(user));

            _transport.SetAuthHeaderProvider(() => new Dictionary<string, string>
            {
                ["authentication-token"] = _userData.AuthenticationToken // Use token from User model
            });

            _userService = new UserService(_transport);
            _gameRoundService = new GameRoundService(_transport);
            _storeService = new StoreService(_transport);
            _friendService = new FriendService(_transport);
            _groupService = new GroupService(_transport);
            _messageService = new MessageService(_transport);
        }

        
        // --- Static Methods (SignUp, SignIn, etc.) remain largely the same ---
        // They will construct a new GameFuseUser with the User object returned from AuthService

        /// <summary>
        /// Signs up a new user.
        /// </summary>
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
            var userModel = await authService.SignUpAsync(email, password, username, gameId, gameApiKey, cancellationToken);
            CurrentUser = new GameFuseUser(userModel); // Pass the full User model
            return new GameFuseUser(userModel);
        }

        /// <summary>
        /// Signs in an existing user.
        /// </summary>
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
            var userModel = await authService.SignInAsync(emailOrUsername, password, gameId, gameApiKey, cancellationToken);
            CurrentUser = new GameFuseUser(userModel); // Pass the full User model
            return new GameFuseUser(userModel);
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        public static void SignOut(CancellationToken cancellationToken = default)
        {
            CurrentUser = null;
           
        }

        public void SignOut()
        {
            _userData.AuthenticationToken = null;
        }

        /// <summary>
        /// Initiates the forgot password process for a user.
        /// </summary>
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

            var transport = new UnityWebRequestTransport(); // Create a new transport for this static call
            var authService = new AuthService(transport);
            return authService.ForgotPasswordAsync(email, gameId, gameApiKey, cancellationToken);
        }

        /// <summary>
        /// Resets a user's password using a token provided in the forgot password email.
        /// </summary>
        public static Task ResetPasswordAsync(string token, string password, string gameId = null, string gameApiKey = null, CancellationToken cancellationToken = default)
        {
            // As before, check API docs for actual reset password endpoint.
            // This method might not directly map if the reset is purely web-based.
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

            var transport = new UnityWebRequestTransport(); // Create a new transport for this static call
            var authService = new AuthService(transport);
            return authService.ResetPasswordAsync(token, password, gameId, gameApiKey, cancellationToken);
        }

        /// <summary>
        /// Checks if the user is authenticated.
        /// </summary>
        public bool IsAuthenticated()
        {
            return _userData?.AuthenticationToken != null;
        }

        /// <summary>
        /// Ensures that the user is authenticated. Throws an exception if not.
        /// </summary>
        private void EnsureAuthenticated()
        {
            if (_userData?.AuthenticationToken == null)
            {
                throw new GameFuseNotAuthenticatedException();
            }
        }

        private void SetInternalScore(int newScore)
        {
            _userData.Score = newScore;
        }

        private void SetInternalCredits(int newCredits)
        {
            _userData.Credits = newCredits;
        }
    }
}