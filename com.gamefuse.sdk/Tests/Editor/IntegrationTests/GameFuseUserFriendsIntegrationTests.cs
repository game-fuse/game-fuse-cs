// GameFuseUserFriendsIntegrationTests.cs
using GameFuse.Models;
using GameFuse.Models.TestSuite;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log and JsonUtility in LoadTestConfig
using System.Linq; // For FirstOrDefault
using System.Collections.Generic;

namespace GameFuse.Tests.Editor.IntegrationTests
{
    [TestFixture]
    public class GameFuseUserFriendsIntegrationTests
    {
        private GameFuseUser _testUser; // User initiating friend requests
        private GameFuseUser _friendCandidateUser; // User to be friended
        private GameFuseUser _thirdUser;

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
            Assert.IsTrue(_testGame.Id > 0, "Test game ID is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(_testGame.Token), "Test game token is null or empty.");

            // Sign up the main test user (the one sending requests)
            string mainUserEmail = $"mainuser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string mainUserName = $"MainUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _testUser = await GameFuseUser.SignUpAsync(mainUserEmail, "password1234", mainUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_testUser, "Main test user sign up failed.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "Main user should be authenticated after sign up.");

            // Sign up the friend candidate user (the one receiving requests)
            string friendCandidateEmail = $"friendcand_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string friendCandidateName = $"FriendCand_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            // Sign up this user. If SignUpAsync modifies GameFuseUser.CurrentUser, _testUser's static context might be affected.
            // However, instance methods should use the token stored within the _testUser instance.
            _friendCandidateUser = await GameFuseUser.SignUpAsync(friendCandidateEmail, "password1234", friendCandidateName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_friendCandidateUser, "Friend candidate user sign up failed.");

            // Sign up a third user for specific tests
            string thirdUserEmail = $"thirduser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string thirdUserName = $"ThirdUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _thirdUser = await GameFuseUser.SignUpAsync(thirdUserEmail, "password1234", thirdUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_thirdUser, "Third user sign up failed.");

            // Crucial: Ensure _testUser is considered the "active" user if static CurrentUser is used by instance methods.
            // A robust way is to ensure the instance methods _always_ use the token from their internal _userData.
            // Your GameFuseUser constructor seems to do this:
            // _transport.SetAuthHeaderProvider(() => new Dictionary<string, string> { ["authentication-token"] = _userData.AuthenticationToken });
            // This is good. If not, you might need to re-sign-in _testUser here to reset GameFuseUser.CurrentUser.
            // For now, assuming instance methods are self-contained regarding authentication.
        }

        [TearDown]
        public async Task TearDownForEachTest()
        {
            // Attempt to sign out instances if they exist to clear any internal state.
            _testUser?.SignOut();
            _friendCandidateUser?.SignOut();
            _thirdUser?.SignOut();

            if (_testGame != null && _testGame.Id > 0)
            {
                await _testSuiteService.CleanUpTestAsync(_testGame.Id, AdminToken, AdminName);
            }
            GameFuseUser.SignOut(); // Clear static CurrentUser
        }

        [Test]
        public async Task Test_SendFriendRequest_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            Assert.AreNotEqual(_testUser.Id, _friendCandidateUser.Id, "Test user and friend candidate cannot be the same user.");

            Debug.Log($"Attempting to send friend request from '{_testUser.Username}' to '{_friendCandidateUser.Username}'.");

            // Act: _testUser sends a friend request to _friendCandidateUser by username
            SendFriendRequestResponse response = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);

            // Assert
            Assert.IsNotNull(response, "SendFriendRequest response should not be null.");
            Assert.IsTrue(response.FriendshipId > 0, "FriendshipId in response should be a positive integer.");
            Debug.Log($"Friend request sent successfully. Message: '{response.Message}', Friendship ID: {response.FriendshipId}");
        }

        [Test]
        public async Task Test_AcceptFriendRequest_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized for accept test.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized for accept test.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            // _friendCandidateUser also needs to be authenticated to accept a request.
            // SignUpAsync already makes them authenticated with their own instance.

            // Arrange: _testUser sends a friend request to _friendCandidateUser
            Debug.Log($"Arrange: '{_testUser.Username}' sending friend request to '{_friendCandidateUser.Username}'.");
            SendFriendRequestResponse sendResponse = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);
            Assert.IsNotNull(sendResponse, "Failed to send friend request in arrange step.");
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Invalid FriendshipId from send request.");
            int friendshipIdToAccept = sendResponse.FriendshipId;
            Debug.Log($"Friend request sent with FriendshipID: {friendshipIdToAccept}. '{_friendCandidateUser.Username}' will now accept.");

            // Act: _friendCandidateUser accepts the friend request
            // Ensure _friendCandidateUser is the one making the call.
            // The GameFuseUser instance methods use their own authentication token.
            FriendshipStatusResponse acceptResponse = await _friendCandidateUser.AcceptFriendRequestAsync(friendshipIdToAccept);

            // Assert
            Assert.IsNotNull(acceptResponse, "AcceptFriendRequest response should not be null.");
            // According to docs, message is: "You have successfully accepted this friend request"
            // Let's be a bit flexible in case the exact wording changes slightly.
            StringAssert.Contains("accepted", acceptResponse.Message.ToLower(), "Response message did not indicate acceptance.");
            Debug.Log($"Friend request accepted successfully by '{_friendCandidateUser.Username}'. Message: '{acceptResponse.Message}'");

        }

        [Test]
        public async Task Test_DeclineFriendRequest_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized for decline test.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized for decline test.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            // _friendCandidateUser also needs to be authenticated to decline a request.

            // Arrange: _testUser sends a friend request to _friendCandidateUser
            Debug.Log($"Arrange: '{_testUser.Username}' sending friend request to '{_friendCandidateUser.Username}' for decline test.");
            SendFriendRequestResponse sendResponse = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);
            Assert.IsNotNull(sendResponse, "Failed to send friend request in arrange step for decline test.");
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Invalid FriendshipId from send request for decline test.");
            int friendshipIdToDecline = sendResponse.FriendshipId;
            Debug.Log($"Friend request sent with FriendshipID: {friendshipIdToDecline}. '{_friendCandidateUser.Username}' will now decline.");

            // Act: _friendCandidateUser declines the friend request
            FriendshipStatusResponse declineResponse = await _friendCandidateUser.DeclineFriendRequestAsync(friendshipIdToDecline);

            // Assert
            Assert.IsNotNull(declineResponse, "DeclineFriendRequest response should not be null.");
            // The API doc for "Accept or Decline" PUT /api/v3/friendships/{id}
            // suggests a generic success message. Let's check for "declined" or a general success indicator.
            // The example response for PUT (accept) is: "You have successfully accepted this friend request"
            // We can anticipate a similar message for decline.
            StringAssert.Contains("declined", declineResponse.Message.ToLower(), "Response message did not indicate decline or expected success format.");
            Debug.Log($"Friend request declined successfully by '{_friendCandidateUser.Username}'. Message: '{declineResponse.Message}'");

        }

        [Test]
        public async Task Test_CancelFriendRequest_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized for cancel test.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized for cancel test."); // Recipient
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser (sender) should be authenticated.");

            // Arrange: _testUser (sender) sends a friend request to _friendCandidateUser
            Debug.Log($"Arrange: '{_testUser.Username}' sending friend request to '{_friendCandidateUser.Username}' for cancel test.");
            SendFriendRequestResponse sendResponse = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);
            Assert.IsNotNull(sendResponse, "Failed to send friend request in arrange step for cancel test.");
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Invalid FriendshipId from send request for cancel test.");
            int friendshipIdToCancel = sendResponse.FriendshipId;
            Debug.Log($"Friend request sent with FriendshipID: {friendshipIdToCancel}. '{_testUser.Username}' will now cancel it.");

            // Act: _testUser (sender) cancels the friend request they sent
            FriendshipStatusResponse cancelResponse = await _testUser.CancelFriendRequestAsync(friendshipIdToCancel);

            // Assert
            Assert.IsNotNull(cancelResponse, "CancelFriendRequest response should not be null.");
            // API doc example response: { "message": "Friend request destroyed successfully" }
            StringAssert.Contains("destroyed successfully", cancelResponse.Message.ToLower(), "Response message did not indicate successful cancellation or expected format.");
            Debug.Log($"Friend request cancelled successfully by '{_testUser.Username}'. Message: '{cancelResponse.Message}'");
        }

        [Test]
        public async Task Test_UnfriendPlayer_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized for unfriend test.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized for unfriend test.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            // _friendCandidateUser also needs to be authenticated for the accept step.

            // Arrange: Establish a friendship first
            // 1. _testUser sends request to _friendCandidateUser
            Debug.Log($"Arrange [Unfriend]: '{_testUser.Username}' sending friend request to '{_friendCandidateUser.Username}'.");
            SendFriendRequestResponse sendResponse = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);
            Assert.IsNotNull(sendResponse, "Failed to send friend request in arrange (unfriend test).");
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Invalid FriendshipId from send request (unfriend test).");
            int friendshipId = sendResponse.FriendshipId;

            // 2. _friendCandidateUser accepts the request
            Debug.Log($"Arrange [Unfriend]: '{_friendCandidateUser.Username}' accepting request from '{_testUser.Username}'.");
            FriendshipStatusResponse acceptResponse = await _friendCandidateUser.AcceptFriendRequestAsync(friendshipId);
            Assert.IsNotNull(acceptResponse, "Failed to accept friend request in arrange (unfriend test).");
            StringAssert.Contains("accepted", acceptResponse.Message.ToLower(), "Acceptance message incorrect (unfriend test).");
            Debug.Log($"Friendship established. '{_testUser.Username}' will now unfriend '{_friendCandidateUser.Username}'.");

            // Act: _testUser unfriends _friendCandidateUser
            // The user being unfriended is specified by their ID.
            FriendshipStatusResponse unfriendResponse = await _testUser.UnfriendPlayerAsync(_friendCandidateUser.Id);

            // Assert
            Assert.IsNotNull(unfriendResponse, "UnfriendPlayer response should not be null.");
            // API doc example response: { "message": "User has been unfriended successfully" }
            StringAssert.Contains("unfriended successfully", unfriendResponse.Message.ToLower(), "Response message did not indicate successful unfriend or expected format.");
            Debug.Log($"Player unfriended successfully by '{_testUser.Username}'. Message: '{unfriendResponse.Message}'");

        }

        [Test]
        public async Task Test_GetFriendshipData_ForNewUser_ReturnsEmptyLists()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            Debug.Log($"Fetching friendship data for new user: '{_testUser.Username}'.");

            // Act
            FriendshipDataResponse friendshipData = await _testUser.GetFriendshipDataAsync();

            // Assert
            Assert.IsNotNull(friendshipData, "FriendshipDataResponse should not be null.");

            Assert.IsNotNull(friendshipData.Friends, "Friends list should not be null.");
            Assert.IsEmpty(friendshipData.Friends, "Friends list should be empty for a new user.");

            Assert.IsNotNull(friendshipData.OutgoingFriendRequests, "OutgoingFriendRequests list should not be null.");
            Assert.IsEmpty(friendshipData.OutgoingFriendRequests, "OutgoingFriendRequests list should be empty for a new user.");

            Assert.IsNotNull(friendshipData.IncomingFriendRequests, "IncomingFriendRequests list should not be null.");
            Assert.IsEmpty(friendshipData.IncomingFriendRequests, "IncomingFriendRequests list should be empty for a new user.");

            Debug.Log("Successfully fetched friendship data for new user, all lists are empty as expected.");
        }

        [Test]
        public async Task Test_GetFriendshipData_AfterSendingRequest_ShowsOutgoingRequest()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // Arrange: _testUser sends a request to _friendCandidateUser
            Debug.Log($"Arrange: '{_testUser.Username}' sending request to '{_friendCandidateUser.Username}'.");
            SendFriendRequestResponse sendResponse = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Failed to send friend request.");
            int sentFriendshipId = sendResponse.FriendshipId;

            // Act: _testUser fetches their friendship data
            Debug.Log($"Fetching friendship data for '{_testUser.Username}' after sending request.");
            FriendshipDataResponse testUserData = await _testUser.GetFriendshipDataAsync();

            // Assert for _testUser
            Assert.IsNotNull(testUserData, "testUserData should not be null.");
            Assert.IsNotNull(testUserData.OutgoingFriendRequests, "testUser's OutgoingFriendRequests list should not be null.");
            var outgoingRequest = testUserData.OutgoingFriendRequests.FirstOrDefault(fr => fr.FriendshipId == sentFriendshipId);
            Assert.IsNotNull(outgoingRequest, $"Outgoing request with FriendshipId {sentFriendshipId} not found for sender.");
            Assert.AreEqual(_friendCandidateUser.Username, outgoingRequest.Username, "Username mismatch in outgoing request.");
            Assert.IsEmpty(testUserData.Friends, "Sender's friends list should still be empty.");
            Assert.IsEmpty(testUserData.IncomingFriendRequests, "Sender's incoming requests list should be empty.");
            Debug.Log($"Verified outgoing request for '{_testUser.Username}'.");

            // Act: _friendCandidateUser (recipient) fetches their friendship data
            Debug.Log($"Fetching friendship data for recipient '{_friendCandidateUser.Username}'.");
            FriendshipDataResponse recipientData = await _friendCandidateUser.GetFriendshipDataAsync();

            // Assert for _friendCandidateUser
            Assert.IsNotNull(recipientData, "recipientData should not be null.");
            Assert.IsNotNull(recipientData.IncomingFriendRequests, "Recipient's IncomingFriendRequests list should not be null.");
            var incomingRequest = recipientData.IncomingFriendRequests.FirstOrDefault(fr => fr.FriendshipId == sentFriendshipId);
            Assert.IsNotNull(incomingRequest, $"Incoming request with FriendshipId {sentFriendshipId} not found for recipient.");
            Assert.AreEqual(_testUser.Username, incomingRequest.Username, "Username mismatch in incoming request (should be sender).");
            Assert.IsEmpty(recipientData.Friends, "Recipient's friends list should still be empty.");
            Assert.IsEmpty(recipientData.OutgoingFriendRequests, "Recipient's outgoing requests list should be empty.");
            Debug.Log($"Verified incoming request for '{_friendCandidateUser.Username}'.");
        }

        [Test]
        public async Task Test_GetFriendsList_AfterFriendshipEstablished_ShowsFriend()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // Arrange: Establish friendship
            // 1. _testUser sends request
            Debug.Log($"Arrange: '{_testUser.Username}' sending request to '{_friendCandidateUser.Username}'.");
            SendFriendRequestResponse sendResponse = await _testUser.SendFriendRequestAsync(_friendCandidateUser.Username);
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Failed to send friend request.");

            // 2. _friendCandidateUser accepts
            Debug.Log($"Arrange: '{_friendCandidateUser.Username}' accepting request.");
            await _friendCandidateUser.AcceptFriendRequestAsync(sendResponse.FriendshipId);
            Debug.Log("Friendship established.");

            // Act: _testUser fetches their friends list
            Debug.Log($"Fetching friends list for '{_testUser.Username}'.");
            IReadOnlyList<Friend> testUserFriends = await _testUser.GetFriendsListAsync();

            // Assert for _testUser
            Assert.IsNotNull(testUserFriends, "_testUser's friends list should not be null.");
            Assert.AreEqual(1, testUserFriends.Count, "_testUser should have 1 friend.");
            var foundFriendForTestUser = testUserFriends.FirstOrDefault(f => f.Id == _friendCandidateUser.Id);
            Assert.IsNotNull(foundFriendForTestUser, $"{_friendCandidateUser.Username} not found in {_testUser.Username}'s friends list.");
            Assert.AreEqual(_friendCandidateUser.Username, foundFriendForTestUser.Username, "Friend's username mismatch.");
            Debug.Log($"'{_testUser.Username}' correctly sees '{_friendCandidateUser.Username}' in friends list.");

            // Act: _friendCandidateUser fetches their friends list
            Debug.Log($"Fetching friends list for '{_friendCandidateUser.Username}'.");
            IReadOnlyList<Friend> friendCandidateFriends = await _friendCandidateUser.GetFriendsListAsync();

            // Assert for _friendCandidateUser
            Assert.IsNotNull(friendCandidateFriends, "_friendCandidateUser's friends list should not be null.");
            Assert.AreEqual(1, friendCandidateFriends.Count, "_friendCandidateUser should have 1 friend.");
            var foundFriendForCandidate = friendCandidateFriends.FirstOrDefault(f => f.Id == _testUser.Id);
            Assert.IsNotNull(foundFriendForCandidate, $"{_testUser.Username} not found in {_friendCandidateUser.Username}'s friends list.");
            Assert.AreEqual(_testUser.Username, foundFriendForCandidate.Username, "Friend's username mismatch.");
            Debug.Log($"'{_friendCandidateUser.Username}' correctly sees '{_testUser.Username}' in friends list.");
        }

        // --- Helper for loading test config ---
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
                    Assert.Inconclusive("AdminToken or AdminName is empty in testConfig.json. Please provide valid credentials.");
                }
                // Debug.Log($"Loaded admin credentials: Name='{AdminName}', Token (start)='{AdminToken?.Substring(0, Math.Min(AdminToken.Length, 5))}...'");
            }
            else
            {
                Debug.LogError("Test configuration file not found: " + configPath + ". Please create it in Assets/TestConfiguration/ with adminToken and adminName.");
                Assert.Inconclusive("Test configuration file (testConfig.json) not found in Assets/TestConfiguration/. Please create it with adminToken and adminName properties.");
            }
        }

        [Test]
        public async Task Test_GetFriendsListForOtherUser_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser (observer) was not initialized.");
            Assert.IsNotNull(_friendCandidateUser, "_friendCandidateUser (target) was not initialized.");
            Assert.IsNotNull(_thirdUser, "_thirdUser (friend of target) was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser (observer) should be authenticated.");

            // Arrange: Establish a friendship between _friendCandidateUser and _thirdUser
            // 1. _friendCandidateUser sends request to _thirdUser
            Debug.Log($"Arrange: '{_friendCandidateUser.Username}' sending request to '{_thirdUser.Username}'.");
            SendFriendRequestResponse sendResponse = await _friendCandidateUser.SendFriendRequestAsync(_thirdUser.Username);
            Assert.IsTrue(sendResponse.FriendshipId > 0, "Failed to send friend request between target and third user.");

            // 2. _thirdUser accepts the request
            Debug.Log($"Arrange: '{_thirdUser.Username}' accepting request from '{_friendCandidateUser.Username}'.");
            await _thirdUser.AcceptFriendRequestAsync(sendResponse.FriendshipId);
            Debug.Log($"Friendship established between '{_friendCandidateUser.Username}' and '{_thirdUser.Username}'.");

            // Act: _testUser (observer) fetches the friends list of _friendCandidateUser (target)
            Debug.Log($"Action: '{_testUser.Username}' fetching friends list for '{_friendCandidateUser.Username}' (ID: {_friendCandidateUser.Id}).");
            IReadOnlyList<Friend> targetUserFriends = await _testUser.GetFriendsListForOtherUserAsync(_friendCandidateUser.Id);

            // Assert
            Assert.IsNotNull(targetUserFriends, $"Friends list for other user ({_friendCandidateUser.Username}) should not be null.");
            Assert.AreEqual(1, targetUserFriends.Count, $"{_friendCandidateUser.Username} should have 1 friend.");

            var foundFriend = targetUserFriends.FirstOrDefault(f => f.Id == _thirdUser.Id);
            Assert.IsNotNull(foundFriend, $"{_thirdUser.Username} not found in {_friendCandidateUser.Username}'s friends list when fetched by observer.");
            Assert.AreEqual(_thirdUser.Username, foundFriend.Username, "Friend's username mismatch in other user's list.");

            Debug.Log($"'{_testUser.Username}' successfully fetched friends list for '{_friendCandidateUser.Username}' and found '{_thirdUser.Username}'.");
        }

        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
        }
    }
}