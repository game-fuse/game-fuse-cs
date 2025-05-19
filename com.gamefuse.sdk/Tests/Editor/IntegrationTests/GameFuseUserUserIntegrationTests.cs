using GameFuse.Models;
using GameFuse.Models.TestSuite;
using GameFuse.Services;
using GameFuse.Transport;
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
    public class GameFuseUserUserIntegrationTests
    {
        private GameFuseUser _testUser; // This will be our signed-in user for most tests
        private TestSuiteService _testSuiteService;
        private CreateGameResponse _testGame; // Store the created game for cleanup

        protected const string ApiBaseUrl = "https://gamefuse.co/api/v3"; // Or from config
        protected string AdminToken { get; private set; }
        protected string AdminName { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LoadTestConfig();
            var serviceTransport = new UnityWebRequestTransport(ApiBaseUrl, 3, 60); // Explicit namespace
            _testSuiteService = new TestSuiteService(serviceTransport);
        }

        [SetUp]
        public async Task SetupForEachTest()
        {
            // Create a new game and a new user for each test to ensure isolation
            _testGame = await _testSuiteService.CreateGameAsync(AdminToken, AdminName);
            Assert.IsNotNull(_testGame, "Test game creation failed.");
            Assert.IsNotNull(_testGame.Id, "Test game ID is null."); // Assuming Id is int not int?
            Assert.IsFalse(string.IsNullOrEmpty(_testGame.Token), "Test game token is null or empty.");


            // Sign up a new user for this game
            string userEmail = $"testuser_{DateTime.UtcNow.Ticks}@gamefuse.com";
            string userName = $"TestUser_{DateTime.UtcNow.Ticks}";
            string password = "password1234";

            _testUser = await GameFuseUser.SignUpAsync(userEmail, password, userName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_testUser, "Test user sign up failed.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "User should be authenticated after sign up.");
        }

        [TearDown]
        public async Task TearDownForEachTest()
        {
            if (_testGame != null && _testGame.Id > 0) // Assuming Id is int and > 0 for valid game
            {
                await _testSuiteService.CleanUpTestAsync(_testGame.Id, AdminToken, AdminName);
            }
            _testUser = null; // Clear the user
            GameFuseUser.SignOut(); // Ensure static CurrentUser is cleared
        }

        [Test]
        public async Task Test_SetAndGetMyUserAttribute_Succeeds()
        {
            string key = $"my_attr_key_{DateTime.UtcNow.Ticks}";
            string value = $"my_attr_value_{DateTime.UtcNow.Ticks}";

            // Set attribute for current user
            await _testUser.SetUserAttributeAsync(key, value);

            // Get attributes for current user (this will refresh _testUser._userData)
            UserAttributes userAttributes = await _testUser.GetUserAttributesAsync(); 

            Assert.IsNotNull(userAttributes.Attributes, "GameUserAttributes should not be null after fetching.");
            var retrievedAttribute = userAttributes.Attributes.FirstOrDefault(a => a.Key == key);
            Assert.IsNotNull(retrievedAttribute, $"Attribute with key {key} not found.");
            Assert.AreEqual(value, retrievedAttribute.Value, "Attribute value mismatch.");
            Debug.Log($"Successfully set and retrieved attribute: {key} = {value}");
        }

        [Test]
        public async Task Test_SetAndGetUserAttributesBatch_Succeeds()
        {
            var attributesToSet = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>($"batch_key1_{DateTime.UtcNow.Ticks}", $"batch_value1_{DateTime.UtcNow.Ticks}"),
                new KeyValuePair<string, string>($"batch_key2_{DateTime.UtcNow.Ticks}", $"batch_value2_{DateTime.UtcNow.Ticks}")
            };

            await _testUser.SetUserAttributesBatchAsync(attributesToSet);
            UserAttributes userAttributes = await _testUser.GetUserAttributesAsync(); 

            Assert.IsNotNull(userAttributes.Attributes);
            foreach (var kvp in attributesToSet)
            {
                var attr = userAttributes.Attributes.FirstOrDefault(a => a.Key == kvp.Key);
                Assert.IsNotNull(attr, $"Batch attribute {kvp.Key} not found.");
                Assert.AreEqual(kvp.Value, attr.Value, $"Batch attribute {kvp.Key} value mismatch.");
            }
            Debug.Log("Successfully set and verified batch attributes.");
        }

        [Test]
        public async Task Test_DeleteMyUserAttribute_Succeeds()
        {
            string key = $"delete_attr_key_{DateTime.UtcNow.Ticks}";
            string value = "to_be_deleted";

            await _testUser.SetUserAttributeAsync(key, value); // Set it first
            UserAttributes setAttributes = await _testUser.GetUserAttributesAsync();
            Assert.IsNotNull(setAttributes.Attributes.FirstOrDefault(a => a.Key == key), "Attribute not set prior to delete.");

            await _testUser.DeleteUserAttributeAsync(key); // Delete it
            UserAttributes afterDeleteAttributes = await _testUser.GetUserAttributesAsync(); // Refresh again

            Assert.IsNull(afterDeleteAttributes.Attributes.FirstOrDefault(a => a.Key == key), $"Attribute {key} should be deleted.");
            Debug.Log($"Successfully deleted attribute: {key}");
        }

        [Test]
        public async Task Test_GetUserAttributesForOtherUser_Succeeds()
        {
            // Sign up a second user in the same game
            string otherUserEmail = $"otheruser_{DateTime.UtcNow.Ticks}@gamefuse.com";
            string otherUserName = $"OtherUser_{DateTime.UtcNow.Ticks}";
            GameFuseUser otherUser = await GameFuseUser.SignUpAsync(otherUserEmail, "password1234", otherUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(otherUser, "Failed to sign up second user.");

            string key = $"other_user_attr_{DateTime.UtcNow.Ticks}";
            string value = "other_value";
            await otherUser.SetUserAttributeAsync(key, value); // otherUser sets their attribute

            // Current _testUser fetches attributes of otherUser
            UserAttributes otherUserAttributes = await _testUser.GetUserAttributesAsync(otherUser.Id);
            Assert.IsNotNull(otherUserAttributes, "Fetched attributes for other user should not be null.");
            var retrievedAttribute = otherUserAttributes.Attributes.FirstOrDefault(a => a.Key == key);
            Assert.IsNotNull(retrievedAttribute, $"Attribute {key} for other user not found.");
            Assert.AreEqual(value, retrievedAttribute.Value, "Other user attribute value mismatch.");
            Debug.Log($"Successfully fetched attributes for other user {otherUser.Id}.");

            // Clean up second user (optional, TearDown handles primary _testUser)
            otherUser.SignOut(); // Sign out otherUser if their session is active statically
        }

        // --- Score Tests ---
        [Test]
        public async Task Test_AddAndSetMyScore_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser is null at start of test.");
            Assert.IsFalse(string.IsNullOrEmpty(_testUser.AuthenticationToken), "AuthenticationToken IS NULL OR EMPTY at start of Test_AddAndSetMyScore_Succeeds!"); // CRUCIAL CHECK


            int initialScore = _testUser.Score; // Score from sign-up (usually 0)

            // Add Score
            int scoreToAdd = 50;
            User userAfterAdd = await _testUser.AddScoreAsync(scoreToAdd);
            Assert.AreEqual(initialScore + scoreToAdd, userAfterAdd.Score, "AddScore did not update score correctly.");
            Assert.AreEqual(initialScore + scoreToAdd, _testUser.Score, "_testUser instance score mismatch after AddMyScoreAsync.");
            Debug.Log($"Score after adding {scoreToAdd}: {_testUser.Score}");

            // Set Score
            int scoreToSet = 123;
            User userAfterSet = await _testUser.SetScoreAsync(scoreToSet);
            Assert.AreEqual(scoreToSet, userAfterSet.Score, "SetScore did not update score correctly.");
            Assert.AreEqual(scoreToSet, _testUser.Score, "_testUser instance score mismatch after SetMyScoreAsync.");
            Debug.Log($"Score after setting to {scoreToSet}: {_testUser.Score}");
        }

        // --- Credits Tests ---
        [Test]
        public async Task Test_AddAndSetMyCredits_Succeeds()
        {
            int initialCredits = _testUser.Credits; // Credits from sign-up (usually 0)

            // Add Credits
            int creditsToAdd = 100;
            User userAfterAdd = await _testUser.AddCreditsAsync(creditsToAdd);
            Assert.AreEqual(initialCredits + creditsToAdd, userAfterAdd.Credits, "AddCredits did not update credits correctly.");
            Assert.AreEqual(initialCredits + creditsToAdd, _testUser.Credits, "_testUser instance credits mismatch after AddMyCreditsAsync.");
            Debug.Log($"Credits after adding {creditsToAdd}: {_testUser.Credits}");

            // Set Credits
            int creditsToSet = 500;
            User userAfterSet = await _testUser.SetCreditsAsync(creditsToSet);
            Assert.AreEqual(creditsToSet, userAfterSet.Credits, "SetCredits did not update credits correctly.");
            Assert.AreEqual(creditsToSet, _testUser.Credits, "_testUser instance credits mismatch after SetMyCreditsAsync.");
            Debug.Log($"Credits after setting to {creditsToSet}: {_testUser.Credits}");
        }

        [Test]
        public async Task Test_GetUserDetailsForOtherUser_Succeeds()
        {
            string otherUserEmail = $"details_other_{DateTime.UtcNow.Ticks}@gamefuse.com";
            string otherUserName = $"detailsother_{DateTime.UtcNow.Ticks}";
            // Sign up another user (their details will be fetched by _testUser)
            // Need to sign them in to get their GameFuseUser object if we need their ID easily,
            // or get ID from TestSuiteService if it supports user creation/querying.
            // For simplicity, sign them up and get their ID.
            GameFuseUser otherSignedUpUser = await GameFuseUser.SignUpAsync(otherUserEmail, "password1234", otherUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(otherSignedUpUser, "Failed to sign up user for detail fetching.");
            int otherUserId = otherSignedUpUser.Id;

            // _testUser (current authenticated user) fetches details of otherSignedUpUser
            User fetchedUser = await _testUser.GetUserDetailsAsync(otherUserId);

            Assert.IsNotNull(fetchedUser, "Fetched user details should not be null.");
            Assert.AreEqual(otherUserId, fetchedUser.Id, "Fetched user ID mismatch.");
            Assert.AreEqual(otherUserName, fetchedUser.Username, "Fetched user username mismatch.");
            Debug.Log($"Successfully fetched details for user ID: {otherUserId}, Username: {fetchedUser.Username}");

        }

        [Test]
        public async Task Test_GetMyStoreItems_ReturnsEmptyForNewUser()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "User should be authenticated.");

            // Call the instance method for the current user
            UserStore myStore = await _testUser.GetUserStoreItemsAsync();

            Assert.IsNotNull(myStore, "UserStore response should not be null.");
            Assert.AreEqual(_testUser.Credits, myStore.Credits, "UserStore credits should match current user's credits.");
            Assert.IsNotNull(myStore.StoreItems, "StoreItems list within UserStore should not be null.");
            Assert.IsEmpty(myStore.StoreItems, "StoreItems list should be empty for a new user.");
            Debug.Log($"Successfully fetched current user's store. Credits: {myStore.Credits}, Items Count: {myStore.StoreItems.Count}");
        }

        [Test]
        public async Task Test_GetUserStoreItemsForOtherUser_ReturnsEmptyForNewUser()
        {
            Assert.IsNotNull(_testUser, "_testUser (requesting user) was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "Requesting user should be authenticated.");

            // Sign up a second user (the one whose store items we'll fetch)
            string otherUserEmail = $"otherstoreuser_{DateTime.UtcNow.Ticks}@gamefuse.com";
            string otherUserName = $"otherstoreuser_{DateTime.UtcNow.Ticks}";
            GameFuseUser otherUser = await GameFuseUser.SignUpAsync(otherUserEmail, "password1234", otherUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(otherUser, "Failed to sign up second user.");
            int otherUserId = otherUser.Id;
            int otherUserInitialCredits = otherUser.Credits; // Should be 0
            GameFuseUser.SignOut(); // Sign out otherUser to ensure _testUser is CurrentUser for the next call

            // Re-authenticate _testUser if necessary, or ensure its session is still primary
            // For simplicity, we assume _testUser's session is implicitly active for its methods
            // If _testUser was overwritten by otherUser becoming CurrentUser, we'd need to re-login _testUser.
            // Let's assume _testUser instance methods correctly use its own token.
            // If GameFuseUser.CurrentUser static is critical, then after otherUser.SignUp,
            // _testUser might need to sign in again if SignUpAsync always sets CurrentUser.
            // Given your setup, _testUser is the primary instance from SetupForEachTest.

            // _testUser (current authenticated user) fetches store items of otherUser
            UserStore otherUserStore = await _testUser.GetUserStoreItemsAsync(otherUserId);

            Assert.IsNotNull(otherUserStore, "UserStore response for other user should not be null.");
            Assert.AreEqual(otherUserInitialCredits, otherUserStore.Credits, "Other user's UserStore credits should match their initial credits.");
            Assert.IsNotNull(otherUserStore.StoreItems, "StoreItems list for other user should not be null.");
            Assert.IsEmpty(otherUserStore.StoreItems, "StoreItems list for other new user should be empty.");
            Debug.Log($"Successfully fetched other user's ({otherUserId}) store. Credits: {otherUserStore.Credits}, Items Count: {otherUserStore.StoreItems.Count}");
        }

        [Test]
        public async Task Test_GetMyLeaderboardEntries_ReturnsEmptyForNewUser()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "User should be authenticated.");
            string testLeaderboardName = "test_lb_for_new_user";

            // Call the instance method for the current user
            LeaderboardEntries myEntries = await _testUser.GetUserLeaderboardEntriesAsync(leaderboardName: testLeaderboardName, limit: 10);

            Assert.IsNotNull(myEntries, "LeaderboardEntries response should not be null.");
            Assert.IsNotNull(myEntries.Entries, "Entries list within LeaderboardEntries should not be null.");
            Assert.IsEmpty(myEntries.Entries, "Leaderboard entries list should be empty for a new user on a test leaderboard.");
            Debug.Log($"Successfully fetched current user's leaderboard entries for '{testLeaderboardName}'. Entries Count: {myEntries.Entries.Count}");
        }

        [Test]
        public async Task Test_GetUserLeaderboardEntriesForOtherUser_ReturnsEmptyForNewUser()
        {
            Assert.IsNotNull(_testUser, "_testUser (requesting user) was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "Requesting user should be authenticated.");
            string testLeaderboardName = "other_user_test_lb";

            // Sign up a second user
            string otherUserEmail = $"otherlbuser_{DateTime.UtcNow.Ticks}@gamefuse.com";
            string otherUserName = $"OtherLbUser_{DateTime.UtcNow.Ticks}";
            GameFuseUser otherUser = await GameFuseUser.SignUpAsync(otherUserEmail, "password1234", otherUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(otherUser, "Failed to sign up second user.");
            int otherUserId = otherUser.Id;
            otherUser.SignOut(); // Sign out otherUser

            // _testUser fetches leaderboard entries of otherUser
            LeaderboardEntries otherUserEntries = await _testUser.GetUserLeaderboardEntriesAsync(userIdToFetch: otherUserId, leaderboardName: testLeaderboardName, limit: 10);

            Assert.IsNotNull(otherUserEntries, "LeaderboardEntries response for other user should not be null.");
            Assert.IsNotNull(otherUserEntries.Entries, "Entries list for other user should not be null.");
            Assert.IsEmpty(otherUserEntries.Entries, "Leaderboard entries list for other new user should be empty.");
            Debug.Log($"Successfully fetched other user's ({otherUserId}) leaderboard entries for '{testLeaderboardName}'. Entries Count: {otherUserEntries.Entries.Count}");
        }



        private void LoadTestConfig()
        {
            // Load configuration from a JSON file
            string configPath = Path.Combine(Application.dataPath, "TestConfiguration", "testConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                TestConfig config = JsonUtility.FromJson<TestConfig>(json);
                AdminToken = config.adminToken;
                AdminName = config.adminName;
                Debug.Log($"Loaded admin credentials: Name={AdminName}, Token={AdminToken?.Substring(0, 5)}...");
            }
            else
            {
                Debug.LogError("Test configuration file not found. Create a testConfig.json file with adminToken and adminName properties.");
                Assert.Inconclusive("Test configuration file not found. Create a testConfig.json file with adminToken and adminName properties.");
            }
        }

        // Model for test configuration
        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
        }

        
    }
}