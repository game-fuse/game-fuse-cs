using GameFuse.Exceptions;
using GameFuse.Models.Shared;
using GameFuse.Models.TestSuite;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for test suite operations used for integration testing and staging workflows.
    /// </summary>
    public class TestSuiteService
    {
        private readonly ITransport _transport;
        private const string BASE_PATH = "test_suite";

        /// <summary>
        /// Creates a new instance of the TestSuiteService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public TestSuiteService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Creates the admin authentication headers required for test suite API calls.
        /// </summary>
        /// <param name="serviceKeyToken">The admin service key token.</param>
        /// <param name="serviceKeyName">The admin service key name.</param>
        /// <returns>A dictionary of headers.</returns>
        private Dictionary<string, string> CreateAdminHeaders(string serviceKeyToken, string serviceKeyName)
        {
            return new Dictionary<string, string>
            {
                ["service-key-token"] = serviceKeyToken,
                ["service-key-name"] = serviceKeyName,
                ["Content-Type"] = "application/json"
            };
        }

        /// <summary>
        /// Creates a new test game for integration testing.
        /// </summary>
        /// <param name="serviceKeyToken">The admin service key token.</param>
        /// <param name="serviceKeyName">The admin service key name.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created test game information.</returns>
        public async Task<CreateGameResponse> CreateGameAsync(string serviceKeyToken, string serviceKeyName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(serviceKeyToken)) throw new ArgumentNullException(nameof(serviceKeyToken));
            if (string.IsNullOrEmpty(serviceKeyName)) throw new ArgumentNullException(nameof(serviceKeyName));

            var headers = CreateAdminHeaders(serviceKeyToken, serviceKeyName);

            try
            {
                var request = new CreateGameRequest();
                return await _transport.PostAsync<CreateGameRequest, CreateGameResponse>($"{BASE_PATH}/create_game", request, headers, cancellationToken);
            }
            catch (GameFuseApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new GameFuseApiException("Unauthorized. Invalid service key token or name.", ex.StatusCode, ex.ApiErrorCode);
                }
                throw;
            }
        }

        /// <summary>
        /// Creates a new test user in a test game.
        /// </summary>
        /// <param name="gameId">The ID of the test game.</param>
        /// <param name="username">The username for the test user.</param>
        /// <param name="email">The email for the test user.</param>
        /// <param name="serviceKeyToken">The admin service key token.</param>
        /// <param name="serviceKeyName">The admin service key name.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created test user information.</returns>
        public async Task<Friend> CreateUserAsync(int gameId, string username, string email, string serviceKeyToken, string serviceKeyName, CancellationToken cancellationToken = default)
        {
            if (gameId <= 0) throw new ArgumentOutOfRangeException(nameof(gameId), "Game ID must be positive.");
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(serviceKeyToken)) throw new ArgumentNullException(nameof(serviceKeyToken));
            if (string.IsNullOrEmpty(serviceKeyName)) throw new ArgumentNullException(nameof(serviceKeyName));

            var headers = CreateAdminHeaders(serviceKeyToken, serviceKeyName);

            var request = new CreateUserRequest
            {
                GameId = gameId,
                Username = username,
                Email = email
            };

            try
            {
                return await _transport.PostAsync<CreateUserRequest, Friend>($"{BASE_PATH}/create_user", request, headers, cancellationToken);
            }
            catch (GameFuseApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new GameFuseApiException("Game not found. Invalid game ID.", ex.StatusCode, ex.ApiErrorCode);
                }
                if (ex.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new GameFuseApiException("Invalid request. Check that username and email are valid.", ex.StatusCode, ex.ApiErrorCode);
                }
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new GameFuseApiException("Unauthorized. Invalid service key token or name.", ex.StatusCode, ex.ApiErrorCode);
                }
                throw;
            }
        }

        /// <summary>
        /// Cleans up all resources created for a test game.
        /// </summary>
        /// <param name="gameId">The ID of the test game to clean up.</param>
        /// <param name="serviceKeyToken">The admin service key token.</param>
        /// <param name="serviceKeyName">The admin service key name.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A clean up response message.</returns>
        public async Task<CleanUpResponse> CleanUpTestAsync(int gameId, string serviceKeyToken, string serviceKeyName, CancellationToken cancellationToken = default)
        {
            if (gameId <= 0) throw new ArgumentOutOfRangeException(nameof(gameId), "Game ID must be positive.");
            if (string.IsNullOrEmpty(serviceKeyToken)) throw new ArgumentNullException(nameof(serviceKeyToken));
            if (string.IsNullOrEmpty(serviceKeyName)) throw new ArgumentNullException(nameof(serviceKeyName));

            var headers = CreateAdminHeaders(serviceKeyToken, serviceKeyName);
            
            var request = new CleanUpGameRequest
            {
                GameId = gameId
            };

            try
            {
                // API update: Using DELETE with a request body for the clean-up operation
                string route = $"{BASE_PATH}/clean_up_test";
                UnityEngine.Debug.Log($"attempting to clean up GameId: {request.GameId}, route: {route} using DELETE with body");
                var response = await _transport.DeleteAsync<CleanUpGameRequest, CleanUpResponse>($"{BASE_PATH}/clean_up_test", request, headers, cancellationToken);
                UnityEngine.Debug.Log($"response {response.Message}");
                return response;
            }
            catch (GameFuseApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new GameFuseApiException("Game not found. Invalid game ID.", ex.StatusCode, ex.ApiErrorCode);
                }
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new GameFuseApiException("Unauthorized. Invalid service key token or name.", ex.StatusCode, ex.ApiErrorCode);
                }
                throw;
            }
        }
    }
}