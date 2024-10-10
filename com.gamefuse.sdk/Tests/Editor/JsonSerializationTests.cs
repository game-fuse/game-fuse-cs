using UnityEngine;
using NUnit.Framework;
using System;

namespace GameFuseCSharp.Tests
{
    [TestFixture]
    public class JsonSerializationTests : MonoBehaviour
    {
        [Test]
        public void CreateUserRequest_SerializesCorrectly()
        {
            // Arrange
            CreateUserRequest createUserRequest = new CreateUserRequest
            {
                game_id = 10,
                username = "dave",
                email = "dave@email.com"
            };

            // Act
            string createUserJson = JsonUtility.ToJson(createUserRequest);

            // Expected JSON output
            string expectedJson = "{\"game_id\":10,\"username\":\"dave\",\"email\":\"dave@email.com\"}";

            // Assert
            Assert.AreEqual(expectedJson, createUserJson);
        }

        [Test]
        public void CreateUserResponse_SerializesCorrectly()
        {
            // Arrange
            CreateUserResponse createUserResponse = new CreateUserResponse
            {
                id = 1,
                username = "dave",
                email = "server_dave@email.com",
                display_email = "dave@email.com"
            };

            // Act
            string createUserResponseJson = JsonUtility.ToJson(createUserResponse);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"username\":\"dave\",\"email\":\"server_dave@email.com\",\"display_email\":\"dave@email.com\"}";

            // Assert
            Assert.AreEqual(expectedJson, createUserResponseJson);
        }


        [Test]
        public void CreateStoreItemRequest_SerializesCorrectly()
        {
            // Arrange
            CreateStoreItemRequest createStoreItemRequest = new CreateStoreItemRequest
            {
                game_id = 10,
                name = "Sword",
                description = "A powerful sword",
                category = "Weapons",
                cost = 100
            };

            // Act
            string createStoreItemRequestJson = JsonUtility.ToJson(createStoreItemRequest);

            // Expected JSON output
            string expectedJson = "{\"game_id\":10,\"name\":\"Sword\",\"description\":\"A powerful sword\",\"category\":\"Weapons\",\"cost\":100}";

            // Assert
            Assert.AreEqual(expectedJson, createStoreItemRequestJson);
        }

        [Test]
        public void CreateStoreItemResponse_SerializesCorrectly()
        {
            // Arrange
            CreateStoreItemResponse createStoreItemResponse = new CreateStoreItemResponse
            {
                id = 1,
                game_id = 10,
                name = "Sword",
                description = "A powerful sword",
                category = "Weapons",
                cost = 100
            };

            // Act
            string createStoreItemResponseJson = JsonUtility.ToJson(createStoreItemResponse);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"game_id\":10,\"name\":\"Sword\",\"description\":\"A powerful sword\",\"category\":\"Weapons\",\"cost\":100}";

            // Assert
            Assert.AreEqual(expectedJson, createStoreItemResponseJson);
        }

        [Test]
        public void CreateGameResponse_SerializesCorrectly()
        {
            // Arrange
            CreateGameResponse createGameResponse = new CreateGameResponse
            {
                id = 1,
                name = "AdventureQuest",
                token = "abcd1234"
            };

            // Act
            string createGameResponseJson = JsonUtility.ToJson(createGameResponse);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"name\":\"AdventureQuest\",\"token\":\"abcd1234\"}";

            // Assert
            Assert.AreEqual(expectedJson, createGameResponseJson);
        }

        [Test]
        public void CleanUpGameRequest_SerializesCorrectly()
        {
            // Arrange
            CleanUpGameRequest cleanUpGameRequest = new CleanUpGameRequest
            {
                game_id = 10
            };

            // Act
            string cleanUpGameRequestJson = JsonUtility.ToJson(cleanUpGameRequest);

            // Expected JSON output
            string expectedJson = "{\"game_id\":10}";

            // Assert
            Assert.AreEqual(expectedJson, cleanUpGameRequestJson);
        }

        [Test]
        public void CleanUpResponse_SerializesCorrectly()
        {
            // Arrange
            CleanUpResponse cleanUpResponse = new CleanUpResponse
            {
                message = "Game cleaned up successfully"
            };

            // Act
            string cleanUpResponseJson = JsonUtility.ToJson(cleanUpResponse);

            // Expected JSON output
            string expectedJson = "{\"message\":\"Game cleaned up successfully\"}";

            // Assert
            Assert.AreEqual(expectedJson, cleanUpResponseJson);
        }
    }
}
