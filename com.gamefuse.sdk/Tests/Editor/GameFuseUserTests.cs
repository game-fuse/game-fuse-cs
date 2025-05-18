using System;
using System.Reflection;
using System.Threading.Tasks;
using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Tests.Common.Transport;
using GameFuse.Transport;
using NUnit.Framework;

namespace GameFuse.Tests.Editor
{
    public class GameFuseUserTests
    {
        private MockTransport _mockTransport;

        [SetUp]
        public void Setup()
        {
            _mockTransport = new MockTransport();
            
            // Set the mock transport in GameFuseUser using reflection since it's a private static field
            var fieldInfo = typeof(GameFuseUser).GetField("_transport", BindingFlags.NonPublic | BindingFlags.Static);
            fieldInfo.SetValue(null, _mockTransport);
            
            // Make sure CurrentUser is null at the start of each test
            typeof(GameFuseUser).GetProperty("CurrentUser").SetValue(null, null);
        }

        [Test]
        public async Task SignUp_Success_SetsCurrentUser()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""test_user"",
                ""email"": ""_appid_1_test@example.com"",
                ""display_email"": ""test@example.com"",
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
            var result = await GameFuseUser.SignUpAsync(
                "test@example.com",
                "password123",
                "test_user",
                "1",
                "abc123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test_user", result.Username);
            Assert.AreEqual(1, result.Id);
            Assert.IsNotNull(GameFuseUser.CurrentUser);
            Assert.AreEqual(result.Id, GameFuseUser.CurrentUser.Id);
            Assert.IsTrue(GameFuseUser.IsAuthenticated());
        }

        [Test]
        public async Task SignIn_Success_SetsCurrentUser()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""test_user"",
                ""email"": ""_appid_1_test@example.com"",
                ""display_email"": ""test@example.com"",
                ""credits"": 100,
                ""score"": 200,
                ""last_login"": ""2024-07-21T14:23:37.457-04:00"",
                ""number_of_logins"": 5,
                ""authentication_token"": ""abc123"",
                ""events_total"": 10,
                ""events_current_month"": 5,
                ""game_sessions_total"": 3,
                ""game_sessions_current_month"": 1
            }";

            _mockTransport.RegisterResponse("POST", "sessions", responseJson);

            // Act
            var result = await GameFuseUser.SignInAsync(
                "test@example.com",
                "password123",
                "1",
                "abc123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test_user", result.Username);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(100, result.Credits);
            Assert.AreEqual(200, result.Score);
            Assert.IsNotNull(GameFuseUser.CurrentUser);
            Assert.AreEqual(result.Id, GameFuseUser.CurrentUser.Id);
            Assert.IsTrue(GameFuseUser.IsAuthenticated());
        }

        [Test]
        public void SignOut_Success_ClearsCurrentUser()
        {
            // Arrange - Sign in first
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""test_user"",
                ""email"": ""_appid_1_test@example.com"",
                ""display_email"": ""test@example.com"",
                ""credits"": 100,
                ""score"": 200,
                ""last_login"": ""2024-07-21T14:23:37.457-04:00"",
                ""number_of_logins"": 5,
                ""authentication_token"": ""abc123""
            }";

            _mockTransport.RegisterResponse("POST", "sessions", responseJson);
            GameFuseUser.SignInAsync("test@example.com", "password123", "1", "abc123").Wait();

            // Act
            GameFuseUser.SignOutAsync().Wait();

            // Assert
            Assert.IsFalse(GameFuseUser.IsAuthenticated());
        }

        [Test]
        public void GameFuseUser_Methods_RequireAuthentication()
        {
            // Arrange - No sign in, user is not authenticated

            // Act & Assert - Various methods should throw when not authenticated
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await GameFuseUser.GetCurrentUserAsync());
            
            // To test instance methods, we need to create a GameFuseUser instance via reflection
            var user = CreateGameFuseUserViaReflection();
            
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await user.UpdateUserAsync("new_username"));
            
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await user.UpdatePasswordAsync("old_pass", "new_pass"));
            
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await user.SetUserAttributeAsync("key", "value"));
            
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await user.GetUserAttributesAsync());
            
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await user.CreateGameRoundAsync());
            
            Assert.ThrowsAsync<GameFuseNotAuthenticatedException>(async () => 
                await user.GetFriendsAsync());
        }

        // Helper method to create a GameFuseUser instance using reflection
        private GameFuseUser CreateGameFuseUserViaReflection()
        {
            var user = new User
            {
                Id = 1,
                Username = "test_user",
                Email = "test@example.com",
                AuthenticationToken = "test_token"
            };
            
            var ctor = typeof(GameFuseUser).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new[] { typeof(User) }, 
                null);
            
            return (GameFuseUser)ctor.Invoke(new object[] { user });
        }
    }
}