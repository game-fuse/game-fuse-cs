using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Tests.Common.Transport;
using NUnit.Framework;

namespace GameFuse.Tests.Editor
{
    public class UserServiceTests
    {
        private MockTransport _mockTransport;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockTransport = new MockTransport();
            _userService = new UserService(_mockTransport);
        }

        [Test]
        public async Task GetUser_Success_ReturnsUser()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""john.doe"",
                ""email"": ""_appid_1_john.doe@example.com"",
                ""display_email"": ""john.doe@example.com"",
                ""credits"": 100,
                ""score"": 500,
                ""last_login"": ""2024-07-21"",
                ""number_of_logins"": 10,
                ""authentication_token"": ""abc123"",
                ""events_total"": 50,
                ""events_current_month"": 20,
                ""game_sessions_total"": 15,
                ""game_sessions_current_month"": 5
            }";

            _mockTransport.RegisterResponse("GET", "users/1", responseJson);

            // Act
            var result = await _userService.GetUserAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("john.doe", result.Username);
            Assert.AreEqual("_appid_1_john.doe@example.com", result.Email);
            Assert.AreEqual("john.doe@example.com", result.DisplayEmail);
            Assert.AreEqual(100, result.Credits);
            Assert.AreEqual(500, result.Score);
            Assert.AreEqual("abc123", result.AuthenticationToken);
            Assert.AreEqual(50, result.EventsTotal);
            Assert.AreEqual(20, result.EventsCurrentMonth);
            Assert.AreEqual(15, result.GameSessionsTotal);
            Assert.AreEqual(5, result.GameSessionsCurrentMonth);
        }

        [Test]
        public async Task UpdateUser_Success_ReturnsUpdatedUser()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""username"": ""new_username"",
                ""email"": ""_appid_1_new_email@example.com"",
                ""display_email"": ""new_email@example.com"",
                ""credits"": 100,
                ""score"": 500,
                ""last_login"": ""2024-07-21"",
                ""number_of_logins"": 10,
                ""authentication_token"": ""abc123""
            }";

            _mockTransport.RegisterResponse("PUT", "users/1", responseJson);

            // Act
            var result = await _userService.UpdateUserAsync(1, "new_username", "new_email@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("new_username", result.Username);
            Assert.AreEqual("_appid_1_new_email@example.com", result.Email);
            Assert.AreEqual("new_email@example.com", result.DisplayEmail);
        }

        [Test]
        public async Task UpdatePassword_Success_CompletesWithNoErrors()
        {
            // Arrange
            _mockTransport.RegisterResponse("PUT", "users/1/password", "{}");

            // Act & Assert
            await _userService.UpdatePasswordAsync(1, "old_password", "new_password");

            // No exceptions means success
            Assert.Pass();
        }

        [Test]
        public async Task SetUserAttribute_Success_ReturnsAttribute()
        {
            // Arrange
            const string responseJson = @"{
                ""id"": 1,
                ""key"": ""test_key"",
                ""value"": ""test_value""
            }";

            _mockTransport.RegisterResponse("POST", "users/1/attributes", responseJson);

            // Act
            var result = await _userService.SetUserAttributeAsync(1, "test_key", "test_value");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("test_key", result.Key);
            Assert.AreEqual("test_value", result.Value);
        }

        [Test]
        public async Task GetUserAttributes_Success_ReturnsAttributesList()
        {
            // Arrange
            const string responseJson = @"[
                {
                    ""id"": 1,
                    ""key"": ""key1"",
                    ""value"": ""value1""
                },
                {
                    ""id"": 2,
                    ""key"": ""key2"",
                    ""value"": ""value2""
                }
            ]";

            _mockTransport.RegisterResponse("GET", "users/1/attributes", responseJson);

            // Act
            var result = await _userService.GetUserAttributesAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("key1", result[0].Key);
            Assert.AreEqual("value1", result[0].Value);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("key2", result[1].Key);
            Assert.AreEqual("value2", result[1].Value);
        }

        [Test]
        public async Task DeleteUserAttribute_Success_CompletesWithNoErrors()
        {
            // Arrange
            _mockTransport.RegisterResponse("DELETE", "users/1/attributes/2", "{}");

            // Act & Assert
            await _userService.DeleteUserAttributeAsync(1, 2);

            // No exceptions means success
            Assert.Pass();
        }
    }
}