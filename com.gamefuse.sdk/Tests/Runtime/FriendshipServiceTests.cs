using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using GameFuseCSharp;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class FriendshipServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private ISessionsService _sessionsService;
        private IFriendshipService _friendshipService1;
        private IFriendshipService _friendshipService2;
        private string _adminToken;
        private string _adminName;
        private int _testGameId;
        private string _testGameToken;
        private SignInResponse _user1;
        private SignInResponse _user2;

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
            _testGameId = gameResponse.id;
            _testGameToken = gameResponse.token;
            Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

            // Sign up and sign in two test users
            _user1 = await CreateAndSignInUser("testuser1");
            _user2 = await CreateAndSignInUser("testuser2");

            // Initialize FriendshipServices for both users
            _friendshipService1 = new FriendshipService("https://gamefuse.co/api/v3", _user1.authentication_token);
            _friendshipService2 = new FriendshipService("https://gamefuse.co/api/v3", _user2.authentication_token);
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
                email = userEmail,
                password = password,
                password_confirmation = password,
                username = username,
                game_id = _testGameId,
                game_token = _testGameToken
            };

            await _userService.SignUpAsync(signUpRequest);

            SignInRequest signInRequest = new SignInRequest
            {
                email = userEmail,
                password = password,
                game_id = _testGameId,
                game_token = _testGameToken
            };

            return await _sessionsService.SignInAsync(signInRequest);
        }

        [Test]
        public async Task SendFriendRequest_AcceptFriendRequest_SuccessfullyCreatesAndAcceptsFriendship()
        {
            try
            {
                await SetUpAsync();

                // User1 sends a friend request to User2
                var sendRequestResponse = await _friendshipService1.SendFriendRequestAsync(_user2.username);
                Assert.IsNotNull(sendRequestResponse);
                Assert.IsTrue(sendRequestResponse.friendship_id > 0);

                // User2 accepts the friend request
                var acceptRequestResponse = await _friendshipService2.UpdateFriendRequestStatusAsync(sendRequestResponse.friendship_id, "accepted");
                Assert.IsNotNull(acceptRequestResponse);
                Assert.AreEqual("you have successfully accepted this friend request", acceptRequestResponse.message);

                // Verify friendship data for both users
                var user1FriendshipData = await _friendshipService1.GetFriendshipDataAsync();
                var user2FriendshipData = await _friendshipService2.GetFriendshipDataAsync();

                Assert.IsTrue(user1FriendshipData.friends.Length > 0);
                Assert.IsTrue(user2FriendshipData.friends.Length > 0);
                Assert.AreEqual(_user2.username, user1FriendshipData.friends[0].username);
                Assert.AreEqual(_user1.username, user2FriendshipData.friends[0].username);
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
        public async Task SendFriendRequest_DeclineFriendRequest_SuccessfullyCreatesAndDeclinesFriendship()
        {
            try
            {
                await SetUpAsync();

                // User1 sends a friend request to User2
                var sendRequestResponse = await _friendshipService1.SendFriendRequestAsync(_user2.username);
                Assert.IsNotNull(sendRequestResponse);
                Assert.IsTrue(sendRequestResponse.friendship_id > 0);

                // User2 declines the friend request
                var declineRequestResponse = await _friendshipService2.UpdateFriendRequestStatusAsync(sendRequestResponse.friendship_id, "declined");
                Assert.IsNotNull(declineRequestResponse);
                Assert.AreEqual("you have successfully declined this friend request", declineRequestResponse.message);

                // Verify friendship data for both users
                var user1FriendshipData = await _friendshipService1.GetFriendshipDataAsync();
                var user2FriendshipData = await _friendshipService2.GetFriendshipDataAsync();

                Assert.IsTrue(user1FriendshipData.friends.Length == 0);
                Assert.IsTrue(user2FriendshipData.friends.Length == 0);
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
        public async Task SendFriendRequest_CancelFriendRequest_SuccessfullyCancelsRequest()
        {
            try
            {
                await SetUpAsync();

                // User1 sends a friend request to User2
                var sendRequestResponse = await _friendshipService1.SendFriendRequestAsync(_user2.username);
                Assert.IsNotNull(sendRequestResponse);
                Assert.IsTrue(sendRequestResponse.friendship_id > 0);

                // User1 cancels the friend request
                var cancelRequestResponse = await _friendshipService1.CancelFriendRequestAsync(sendRequestResponse.friendship_id);
                Assert.IsNotNull(cancelRequestResponse);
                Assert.AreEqual("friend request destroyed successfully", cancelRequestResponse.message);

                // Verify friendship data for both users
                var user1FriendshipData = await _friendshipService1.GetFriendshipDataAsync();
                var user2FriendshipData = await _friendshipService2.GetFriendshipDataAsync();

                Assert.IsTrue(user1FriendshipData.outgoing_friend_requests.Length == 0);
                Assert.IsTrue(user2FriendshipData.incoming_friend_requests.Length == 0);
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
        public async Task AcceptFriendRequest_Unfriend_SuccessfullyRemovesFriendship()
        {
            try
            {
                await SetUpAsync();

                // User1 sends a friend request to User2
                var sendRequestResponse = await _friendshipService1.SendFriendRequestAsync(_user2.username);
                Assert.IsNotNull(sendRequestResponse);
                Assert.IsTrue(sendRequestResponse.friendship_id > 0);

                // User2 accepts the friend request
                var acceptRequestResponse = await _friendshipService2.UpdateFriendRequestStatusAsync(sendRequestResponse.friendship_id, "accepted");
                Assert.IsNotNull(acceptRequestResponse);
                Assert.AreEqual("you have successfully accepted this friend request", acceptRequestResponse.message);

                // User1 unfriends User2
                var unfriendResponse = await _friendshipService1.UnfriendPlayerAsync(_user2.id);
                Assert.IsNotNull(unfriendResponse);
                Assert.AreEqual("user has been unfriended successfully", unfriendResponse.message);

                // Verify friendship data for both users
                var user1FriendshipData = await _friendshipService1.GetFriendshipDataAsync();
                var user2FriendshipData = await _friendshipService2.GetFriendshipDataAsync();

                Assert.IsTrue(user1FriendshipData.friends.Length == 0);
                Assert.IsTrue(user2FriendshipData.friends.Length == 0);
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