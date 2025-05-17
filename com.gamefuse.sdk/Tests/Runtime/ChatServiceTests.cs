
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class ChatServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private ISessionsService _sessionsService;
        private IChatService _chatService1;
        private IChatService _chatService2;
        private IGroupsService _groupsService;
        private string _adminToken;
        private string _adminName;
        private int _testGameId;
        private string _testGameToken;
        private SignInResponse _user1;
        private SignInResponse _user2;
        private SignInResponse _user3;

        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
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
                _adminToken = config.adminToken;
                _adminName = config.adminName;
            }
            else
            {
                Debug.LogError($"Test configuration file not found at {configPath}. Please ensure the file exists and contains valid admin credentials.");
                Assert.Fail("Test configuration file not found.");
            }

            if (string.IsNullOrEmpty(_adminToken) || string.IsNullOrEmpty(_adminName))
            {
                Debug.LogError("Admin token or name is null or empty. Please check your testConfig.json file.");
                Assert.Fail("Admin credentials are invalid.");
            }

            Debug.Log($"Admin Name: {_adminName}, Admin Token: {_adminToken.Substring(0, 5)}...");
        }

        private async Task SetUpAsync()
        {
            _adminService = new SystemAdminTestSuiteService("https://gamefuse.co/api/v3", _adminToken, _adminName);
            _userService = new UserService("https://gamefuse.co/api/v3");
            _sessionsService = new SessionsService("https://gamefuse.co/api/v3");

            // Create a test game
            Debug.Log("Creating test game...");
            var gameResponse = await _adminService.CreateGameAsync();
            _testGameId = gameResponse.Id;
            _testGameToken = gameResponse.Token;
            Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

            // Sign up and sign in two test users
            _user1 = await CreateAndSignInUser("testuser1");
            _user2 = await CreateAndSignInUser("testuser2");
            _user3 = await CreateAndSignInUser("testuser3");

            // Initialize ChatServices for both users
            _chatService1 = new ChatService("https://gamefuse.co/api/v3", _user1.AuthenticationToken);
            _chatService2 = new ChatService("https://gamefuse.co/api/v3", _user2.AuthenticationToken);
            _groupsService = new GroupsService("https://gamefuse.co/api/v3", _user1.AuthenticationToken);
        }

        private async Task TearDownAsync()
        {
            if (_testGameId != 0)
            {
                Debug.Log($"Cleaning up test game with ID: {_testGameId}");
                await _adminService.CleanUpTestAsync(_testGameId);
            }
        }

        private async Task<SignInResponse> CreateAndSignInUser(string username)
        {
            string userEmail = $"{username}_{UnityEngine.Random.Range(1, 1001)}@example.com";
            string password = "testpassword123";

            SignUpRequest signUpRequest = new SignUpRequest
            {
                Email = userEmail,
                Password = password,
                PasswordConfirmation = password,
                Username = username,
                GameId = _testGameId,
                GameToken = _testGameToken
            };

            await _userService.SignUpAsync(signUpRequest);

            SignInRequest signInRequest = new SignInRequest
            {
                Email = userEmail,
                Password = password,
                GameId = _testGameId,
                GameToken = _testGameToken
            };

            return await _sessionsService.SignInAsync(signInRequest);
        }

        [Test]
        public async Task CreateDirectChat_SendMessage_VerifyMessageDelivered()
        {
            try
            {
                await SetUpAsync();

                // Create direct chat between user1 and user2
                string initialMessage = "Hello, this is a test message!";
                var chat = await _chatService1.CreateDirectChatAsync(new[] { _user2.Username }, initialMessage);

                // Verify chat creation
                Assert.NotNull(chat, "Chat should not be null");
                Assert.Greater(chat.Id, 0, "Chat ID should be greater than 0");
                Assert.AreEqual(2, chat.Participants.Length, "Chat should have exactly two participants");
                Assert.AreEqual(_user1.Id, chat.CreatorId, "Creator ID should match user1");

                // Verify initial message
                Assert.NotNull(chat.Messages, "Messages array should not be null");
                Assert.Greater(chat.Messages.Length, 0, "Should have at least one message");
                Assert.AreEqual(initialMessage, chat.Messages[0].Text, "Message text should match");
                Assert.AreEqual(_user1.Id, chat.Messages[0].UserId, "Message sender should be user1");

                // Get messages using second user
                var messages = await _chatService2.GetMessagesAsync(chat.Id);
                Assert.NotNull(messages, "Messages response should not be null");
                Assert.NotNull(messages.Messages, "Messages array should not be null");
                Assert.Greater(messages.Messages.Length, 0, "Should have at least one message");
                Assert.AreEqual(initialMessage, messages.Messages[0].Text, "Message text should match for user2");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task CreateGroupChat_SendMessage_VerifyMessageDelivered()
        {
            try
            {
                await SetUpAsync();

                // First create a group
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Chat Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false
                };

                var group = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(group, "Group should not be null");

                Debug.Log($"User 1 id {_user1.Id} Group Created {group.Id}");
                // Create group chat
                string initialMessage = "Hello group members!";
                var chat = await _chatService1.CreateGroupChatAsync(group.Id, initialMessage);

                // Verify chat creation
                Assert.NotNull(chat, "Chat should not be null");
                Assert.Greater(chat.Id, 0, "Chat ID should be greater than 0");
                Assert.AreEqual(group.Id, chat.CreatorId, "Creator ID should match user1");

                // Verify initial message
                Assert.NotNull(chat.Messages, "Messages array should not be null");
                Assert.Greater(chat.Messages.Length, 0, "Should have at least one message");
                Assert.AreEqual(initialMessage, chat.Messages[0].Text, "Message text should match");
                Assert.AreEqual(_user1.Id, chat.Messages[0].UserId, "Message sender should be user1");

                // Get messages to verify
                var messages = await _chatService1.GetMessagesAsync(chat.Id);
                Assert.NotNull(messages, "Messages response should not be null");
                Assert.NotNull(messages.Messages, "Messages array should not be null");
                Assert.Greater(messages.Messages.Length, 0, "Should have at least one message");
                Assert.AreEqual(initialMessage, messages.Messages[0].Text, "Message text should match");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetChats_ReturnsAllUserChats()
        {
            try
            {
                await SetUpAsync();

                // Create multiple chats
                string message1 = "First direct chat";
                string message2 = "Second direct chat";

                var chat1 = await _chatService1.CreateDirectChatAsync(new[] { _user2.Username }, message1);
                var chat2 = await _chatService1.CreateDirectChatAsync(new[] { _user3.Username }, message2);

                // Get all chats for user1
                var chatsResponse = await _chatService1.GetChatsAsync();

                // Verify chats list
                Assert.NotNull(chatsResponse, "Chats response should not be null");
                Assert.NotNull(chatsResponse.DirectChats, "Direct chats array should not be null");
                Assert.GreaterOrEqual(chatsResponse.DirectChats.Length, 2, "Should have at least two direct chats");

                // Verify each chat exists in the response
                Assert.That(chatsResponse.DirectChats.Any(c => c.Id == chat1.Id), "First chat should be in the list");
                Assert.That(chatsResponse.DirectChats.Any(c => c.Id == chat2.Id), "Second chat should be in the list");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task SendMessage_AndMarkAsRead_VerifyReadStatus()
        {
            try
            {
                await SetUpAsync();

                // Create a direct chat
                string initialMessage = "Initial message";
                var chat = await _chatService1.CreateDirectChatAsync(new[] { _user2.Username }, initialMessage);

                // Send a new message
                string newMessage = "This is a follow-up message";
                var sentMessage = await _chatService1.SendMessageAsync(chat.Id, newMessage);

                Assert.NotNull(sentMessage, "Sent message should not be null");
                Assert.Greater(sentMessage.Id, 0, "Message ID should be greater than 0");
                Assert.AreEqual(newMessage, sentMessage.Text, "Message text should match");
                Assert.AreEqual(_user1.Id, sentMessage.UserId, "Message sender should be user1");

                // Mark message as read with second user
                var readResponse = await _chatService2.MarkMessageAsReadAsync(sentMessage.Id);
                Assert.NotNull(readResponse, "Read response should not be null");
                Assert.NotNull(readResponse.Message, "Response message should not be null");

                // Get messages to verify read status
                var messages = await _chatService2.GetMessagesAsync(chat.Id);
                var readMessage = messages.Messages.FirstOrDefault(m => m.Id == sentMessage.Id);
                Assert.NotNull(readMessage, "Message should be found");
                Assert.IsTrue(readMessage.Read, "Message should be marked as read for user2");
                Assert.That(readMessage.ReadBy.Contains(_user2.Id.ToString()), "User2 should be in the read by list");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetMessages_PaginationWorks()
        {
            try
            {
                await SetUpAsync();

                // Create a direct chat
                var chat = await _chatService1.CreateDirectChatAsync(new[] { _user2.Username }, "Initial message");

                // Send multiple messages
                for (int i = 1; i <= 5; i++)
                {
                    await _chatService1.SendMessageAsync(chat.Id, $"Test message {i}");
                }

                // Get first page of messages
                var firstPage = await _chatService1.GetMessagesAsync(chat.Id, 1);
                Assert.NotNull(firstPage, "First page response should not be null");
                Assert.NotNull(firstPage.Messages, "First page messages should not be null");
                Assert.Greater(firstPage.Messages.Length, 0, "Should have messages on first page");

                // Get second page of messages
                var secondPage = await _chatService1.GetMessagesAsync(chat.Id, 2);
                Assert.NotNull(secondPage, "Second page response should not be null");
                Assert.NotNull(secondPage.Messages, "Second page messages should not be null");

                // Verify different messages on different pages
                var firstPageIds = firstPage.Messages.Select(m => m.Id).ToList();
                var secondPageIds = secondPage.Messages.Select(m => m.Id).ToList();
                Assert.IsFalse(firstPageIds.Intersect(secondPageIds).Any(), "Pages should contain different messages");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }
    }
}