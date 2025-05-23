// GameFuseUserMessageIntegrationTests.cs
using GameFuse.Models.Shared;
using GameFuse.Models.TestSuite;
using GameFuse.Services; // Not directly used here, but good to have for consistency
using GameFuse.Transport; // For service transport setup
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFuse.Tests.Editor.IntegrationTests
{
    [TestFixture]
    public class GameFuseUserMessageIntegrationTests
    {
        private GameFuseUser _user1;
        private GameFuseUser _user2;
        private GameFuseUser _user3; // For group chat tests
        private Group _testGroupForChat; // Group created by user1, joined by user2, user3

        private TestSuiteService _testSuiteService;
        private CreateGameResponse _testGame;

        protected const string ApiBaseUrl = "https://gamefuse.co/api/v3";
        protected string AdminToken { get; private set; }
        protected string AdminName { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LoadTestConfig();
            var serviceTransport = new UnityWebRequestTransport(ApiBaseUrl, 3, 60);
            _testSuiteService = new TestSuiteService(serviceTransport);
        }

        [SetUp]
        public async Task SetupForEachTest()
        {
            _testGame = await _testSuiteService.CreateGameAsync(AdminToken, AdminName);
            Assert.IsNotNull(_testGame, "Test game creation failed.");

            _user1 = await GameFuseUser.SignUpAsync($"user1_{Guid.NewGuid().ToString("N").Substring(0, 6)}@gfmesg.com", "password", $"UserMsg1_{Guid.NewGuid().ToString("N").Substring(0, 6)}", _testGame.Id.ToString(), _testGame.Token);
            _user2 = await GameFuseUser.SignUpAsync($"user2_{Guid.NewGuid().ToString("N").Substring(0, 6)}@gfmesg.com", "password", $"UserMsg2_{Guid.NewGuid().ToString("N").Substring(0, 6)}", _testGame.Id.ToString(), _testGame.Token);
            _user3 = await GameFuseUser.SignUpAsync($"user3_{Guid.NewGuid().ToString("N").Substring(0, 6)}@gfmesg.com", "password", $"UserMsg3_{Guid.NewGuid().ToString("N").Substring(0, 6)}", _testGame.Id.ToString(), _testGame.Token);

            Assert.IsNotNull(_user1, "User1 sign up failed.");
            Assert.IsNotNull(_user2, "User2 sign up failed.");
            Assert.IsNotNull(_user3, "User3 sign up failed.");

            // Create a group for group chat testing (user1 is admin)
            _testGroupForChat = await _user1.CreateGroupAsync(new CreateGroupPayload { Name = "ChatTestGroup", MaxGroupSize = 10, CanAutoJoin = true });
            Assert.IsNotNull(_testGroupForChat, "Failed to create group for chat tests.");
            // User2 and User3 join the group
            await _user2.SendGroupConnectionRequestAsync(_testGroupForChat.Id); // Auto-join
            await _user3.SendGroupConnectionRequestAsync(_testGroupForChat.Id); // Auto-join
        }

        [TearDown]
        public async Task TearDownForEachTest()
        {
            _user1?.SignOut();
            _user2?.SignOut();
            _user3?.SignOut();
            // Group will be cleaned up with the game
            if (_testGame != null && _testGame.Id > 0)
            {
                await _testSuiteService.CleanUpTestAsync(_testGame.Id, AdminToken, AdminName);
            }
            GameFuseUser.SignOut(); // Static clear
        }

        // --- Test Methods will be added here one by one ---
        [Test, Order(1)]
        public async Task Test_CreateDirectChat_Succeeds()
        {
            string initialMessage = "Hello User2 from User1!";
            Debug.Log($"User1 ('{_user1.Username}') creating direct chat with User2 ('{_user2.Username}') with message: '{initialMessage}'");

            // Act: Facade now returns Chat directly
            Chat createdChat = await _user1.CreateDirectChatAsync(new List<string> { _user2.Username }, initialMessage);

            // Assert
            Assert.IsNotNull(createdChat, "Created Chat object is null."); // Changed from chatResponse
            Assert.IsTrue(createdChat.Id > 0, "Chat ID is invalid.");

            Assert.IsNotNull(createdChat.Messages, "Chat.Messages list is null.");
            Assert.AreEqual(1, createdChat.Messages.Count, "Should be 1 initial message.");
            Assert.AreEqual(initialMessage, createdChat.Messages.First().Text, "Initial message text mismatch.");
            Assert.AreEqual(_user1.Id, createdChat.Messages.First().UserId, "Initial message sender ID mismatch.");

            Assert.IsNotNull(createdChat.Participants, "Chat.Participants list is null.");
            Assert.AreEqual(2, createdChat.Participants.Count, "Should be 2 participants.");
            Assert.IsTrue(createdChat.Participants.Any(p => p.Id == _user1.Id), "Sender (User1) not in participants.");
            Assert.IsTrue(createdChat.Participants.Any(p => p.Id == _user2.Id), "Recipient (User2) not in participants.");

            Debug.Log($"Direct chat created successfully. Chat ID: {createdChat.Id}");
        }




        private void LoadTestConfig()
        {
            string configPath = Path.Combine(Application.dataPath, "TestConfiguration", "testConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                TestConfig config = JsonUtility.FromJson<TestConfig>(json);
                AdminToken = config.adminToken;
                AdminName = config.adminName;
                if (string.IsNullOrEmpty(AdminToken) || string.IsNullOrEmpty(AdminName))
                {
                    Assert.Inconclusive("AdminToken or AdminName is empty in testConfig.json.");
                }
            }
            else
            {
                Assert.Inconclusive($"Test configuration file (testConfig.json) not found in Assets/TestConfiguration/: {configPath}");
            }
        }
        [Serializable] private class TestConfig { public string adminToken; public string adminName; }
    }
}
