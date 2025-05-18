using System.Net;
using System.Threading.Tasks;
using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Tests.Common.Transport;
using NUnit.Framework;

namespace GameFuse.Tests.Editor
{
    public class AuthServiceTests
    {
        private MockTransport _mockTransport;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _mockTransport = new MockTransport();
            _authService = new AuthService(_mockTransport);
        }

        [Test]
        public async Task SignUp_Success_ReturnsUser()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""some_username"",
                ""email"": ""_appid_1_john.doe@example.com"",
                ""display_email"": ""john.doe@example.com"",
                ""credits"": 0,
                ""score"": 0,
                ""last_login"": ""2024-07-21T14:23:37.457-04:00"",
                ""number_of_logins"": 0,
                ""authentication_token"": ""abc123"",
                ""events_total"": 0,
                ""events_current_month"": 0,
                ""game_sessions_total"": 0,
                ""game_sessions_current_month"": 0
            }";

            _mockTransport.RegisterResponse("POST", "users", responseJson);

            // Act
            var result = await _authService.SignUpAsync(
                "john.doe@example.com", 
                "password123", 
                "jdoe", 
                "1", 
                "abc123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("some_username", result.Username);
            Assert.AreEqual("_appid_1_john.doe@example.com", result.Email);
            Assert.AreEqual("john.doe@example.com", result.DisplayEmail);
            Assert.AreEqual(0, result.Credits);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual("abc123", result.AuthenticationToken);
        }

        [Test]
        public void SignUp_InvalidGameCredentials_ThrowsException()
        {
            // Arrange
            _mockTransport.RegisterErrorResponse(
                "POST", 
                "users", 
                "Failed to fetch game variables. gameId or gameToken might be wrong", 
                HttpStatusCode.NotFound);

            // Act & Assert
            var exception = Assert.ThrowsAsync<GameFuseApiException>(async () =>
                await _authService.SignUpAsync(
                    "john.doe@example.com",
                    "password123",
                    "jdoe",
                    "invalid_id",
                    "invalid_key"));

            Assert.AreEqual(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Test]
        public async Task SignIn_Success_ReturnsUser()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""john.doe"",
                ""email"": ""_appid_1_john.doe@example.com"",
                ""display_email"": ""john.doe@example.com"",
                ""credits"": 0,
                ""score"": 0,
                ""last_login"": ""2024-07-21"",
                ""number_of_logins"": 1,
                ""authentication_token"": ""abc123"",
                ""events_total"": 1,
                ""events_current_month"": 1,
                ""game_sessions_total"": 1,
                ""game_sessions_current_month"": 1,
                ""game_user_attributes"": [
                    {
                        ""id"": 0,
                        ""key"": ""this_key"",
                        ""value"": ""this_value""
                    },
                    {
                        ""id"": 1,
                        ""key"": ""other_key"",
                        ""value"": ""other_value""
                    }
                ],
                ""game_user_store_items"": [
                    {
                        ""id"": 337,
                        ""name"": ""test"",
                        ""cost"": 123,
                        ""description"": ""a test item"",
                        ""category"": ""generic"",
                        ""icon_url"": null
                    }
                ]
            }";

            _mockTransport.RegisterResponse("POST", "sessions", responseJson);

            // Act
            var result = await _authService.SignInAsync(
                "john.doe@example.com",
                "password123",
                "1",
                "abc123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("john.doe", result.Username);
            Assert.AreEqual("_appid_1_john.doe@example.com", result.Email);
            Assert.AreEqual("john.doe@example.com", result.DisplayEmail);
            Assert.AreEqual(0, result.Credits);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual("abc123", result.AuthenticationToken);
            Assert.AreEqual(2, result.GameUserAttributes.Count);
            Assert.AreEqual(1, result.GameUserStoreItems.Count);
        }

        [Test]
        public void SignIn_InvalidCredentials_ThrowsException()
        {
            // Arrange
            _mockTransport.RegisterErrorResponse(
                "POST",
                "sessions",
                "User not found or incorrect password.",
                HttpStatusCode.NotFound);

            // Act & Assert
            var exception = Assert.ThrowsAsync<GameFuseApiException>(async () =>
                await _authService.SignInAsync(
                    "john.doe@example.com",
                    "wrong_password",
                    "1",
                    "abc123"));

            Assert.AreEqual(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Test]
        public void SignIn_GameDisabled_ThrowsException()
        {
            // Arrange
            _mockTransport.RegisterErrorResponse(
                "POST",
                "sessions",
                "Game is disabled. Check the GameFuse dashboard.",
                HttpStatusCode.PaymentRequired);

            // Act & Assert
            var exception = Assert.ThrowsAsync<GameFuseApiException>(async () =>
                await _authService.SignInAsync(
                    "john.doe@example.com",
                    "password123",
                    "1",
                    "abc123"));

            Assert.AreEqual(HttpStatusCode.PaymentRequired, exception.StatusCode);
        }

        [Test]
        public async Task ForgotPassword_Success_CompletesWithNoErrors()
        {
            // Arrange
            _mockTransport.RegisterResponse("POST", "password/forgot", "{}");

            // Act & Assert
            await _authService.ForgotPasswordAsync(
                "john.doe@example.com",
                "1",
                "abc123");

            // No exceptions means success
            Assert.Pass();
        }

        [Test]
        public async Task ResetPassword_Success_CompletesWithNoErrors()
        {
            // Arrange
            _mockTransport.RegisterResponse("POST", "password/reset", "{}");

            // Act & Assert
            await _authService.ResetPasswordAsync(
                "reset_token_123",
                "new_password",
                "1",
                "abc123");

            // No exceptions means success
            Assert.Pass();
        }
    }
}