using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using UnityEngine.TestTools;
using System.Collections;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class FriendshipServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private FriendshipService _friendshipService;
        private string _adminToken;
        private string _serviceKeyName;

        private int _gameId;
        private CreateUserResponse _user1;
        private CreateUserResponse _user2;

        private string _gameToken;

        private GameFuse gameFuse;

        [Serializable]
        private class TestConfig
        {
            public string testToken;
            public string testName;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Read configuration from JSON file
            string configPath = Path.Combine(Application.dataPath, "TestConfiguration", "testConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                TestConfig config = JsonUtility.FromJson<TestConfig>(json);
                _adminToken = config.testToken;
                _serviceKeyName = config.testName;
            }
            else
            {
                Debug.LogWarning($"Test configuration file not found at {configPath}. Using default values.");
                _adminToken = "default_token";
                _serviceKeyName = "default_service_key";
            }
        }

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            // Initialization code...
            yield return null; // Wait for Start() methods to execute
            gameFuse = GameObject.FindAnyObjectByType<GameFuse>();
            if(gameFuse != null)
            {
                Debug.Log("Found the game fuse object!");
            }
            else
            {
                Debug.Log("GameFuse not found!");
            }
        }



        public async Task SetUpAsync()
        {
            // Initialize the admin service
            _adminService = new SystemAdminTestSuiteService("https://gamefuse.co/api/v3", _adminToken, _serviceKeyName);

            // Create a new game
            var gameResponse = await _adminService.CreateGameAsync();
            Assert.IsNotNull(gameResponse, "Failed to create a game.");
            _gameId = gameResponse.id;
            _gameToken = gameResponse.token;
            //GameFuse.SetUpGame(_gameId.ToString(), _gameToken);

            // Create two users
            string user1Name = $"user1_{Guid.NewGuid()}";
            string user1Email = $"user1_{Guid.NewGuid()}@email.com";
            _user1 = await _adminService.CreateUserAsync(_gameId, user1Name, user1Email);
            Assert.IsNotNull(_user1, "Failed to create user1.");
            //GameFuse.SignUp(user1Email, "password123", "password123", user1Name);

            string user2Name = $"user2_{Guid.NewGuid()}";
            string user2Email = $"user2_{Guid.NewGuid()}@email.com";
            _user2 = await _adminService.CreateUserAsync(_gameId, user2Name, user2Email);
            Assert.IsNotNull(_user2, "Failed to create user2.");
            //GameFuse.SignUp(user2Email, "password123", "password123", user2Name);

           

            // Initialize FriendshipService instance
            _friendshipService = new FriendshipService("https://gamefuse.co/api/v3", _gameToken);
           
        }


        public async Task TearDownAsync()
        {
            if (_gameId != 0)
            {
                var cleanupResponse = await _adminService.CleanUpTestAsync(_gameId);
                Assert.AreEqual("everything should have been destroyed!", cleanupResponse.message, "Failed to clean up the game.");
            }
        }

        [Test, Order(1)]
        public async Task SendFriendRequest_ShouldSucceed()
        {
            await SetUpAsync();

            // User1 sends a friend request to User2
            var sendRequestResponse = await _friendshipService.SendFriendRequestAsync(_user2.username);
            Assert.IsNotNull(sendRequestResponse, "SendFriendRequestAsync returned null.");
            Assert.AreEqual("Friend request sent to friend_username", sendRequestResponse.message, "Unexpected response message.");
            Assert.Greater(sendRequestResponse.friendshipId, 0, "Invalid friendshipId returned.");

            await TearDownAsync();
        }

    }
}