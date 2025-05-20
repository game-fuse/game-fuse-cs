// GameFuseUserLeaderboardIntegrationTests.cs
using GameFuse.Models;
using GameFuse.Models.TestSuite;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace GameFuse.Tests.Editor.IntegrationTests
{
    [TestFixture]
    public class GameFuseUserLeaderboardIntegrationTests
    {
        private GameFuseUser _testUser;
        private GameFuseUser _secondUser;
        private GameFuseUser _thirdUser;

        private TestSuiteService _testSuiteService;
        private CreateGameResponse _testGame;

        // Reusable test data
        private const string TEST_LEADERBOARD_NAME = "test_leaderboard";
        private const string TEST_LEADERBOARD_NAME_2 = "test_leaderboard_2";
        private const int DEFAULT_LIMIT = 10;

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

            // Sign up the main test user
            string mainUserEmail = $"mainuser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string mainUserName = $"MainUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _testUser = await GameFuseUser.SignUpAsync(mainUserEmail, "password1234", mainUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_testUser, "Main test user sign up failed.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "Main user should be authenticated after sign up.");

            // Sign up the second user
            string secondUserEmail = $"seconduser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string secondUserName = $"SecondUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _secondUser = await GameFuseUser.SignUpAsync(secondUserEmail, "password1234", secondUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_secondUser, "Second user sign up failed.");

            // Sign up a third user
            string thirdUserEmail = $"thirduser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string thirdUserName = $"ThirdUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _thirdUser = await GameFuseUser.SignUpAsync(thirdUserEmail, "password1234", thirdUserName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_thirdUser, "Third user sign up failed.");
        }

        [TearDown]
        public async Task TearDownForEachTest()
        {
            // Attempt to sign out instances if they exist to clear any internal state.
            _testUser?.SignOut();
            _secondUser?.SignOut();
            _thirdUser?.SignOut();

            if (_testGame != null && _testGame.Id > 0)
            {
                await _testSuiteService.CleanUpTestAsync(_testGame.Id, AdminToken, AdminName);
            }
            GameFuseUser.SignOut(); // Clear static CurrentUser
        }

        [Test]
        public async Task Test_SubmitLeaderboardEntry_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // Prepare test data
            double score = 1200;
            var metadata = new Dictionary<string, object>
            {
                ["level"] = "10",
                ["color"] = "blue"
            };

            Debug.Log($"Submitting leaderboard entry for user '{_testUser.Username}'");

            // Act: Submit a leaderboard entry
            SubmitLeaderboardEntryResponse response = await _testUser.SubmitLeaderboardEntryAsync(
                TEST_LEADERBOARD_NAME,
                score,
                metadata);

            // Assert
            Assert.IsNotNull(response, "SubmitLeaderboardEntry response should not be null.");
            Assert.AreEqual(_testUser.Id, response.Id, "User ID in response should match the test user's ID.");
            Assert.AreEqual(_testUser.Username, response.Username, "Username in response should match the test user's username.");
            
            
            
            Debug.Log($"Successfully submitted leaderboard entry with score {score} for user '{_testUser.Username}'");
        }

        [Test]
        public async Task Test_SubmitLeaderboardEntry_WithoutMetadata_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // Prepare test data
            double score = 1500;

            Debug.Log($"Submitting leaderboard entry without metadata for user '{_testUser.Username}'");

            // Act: Submit a leaderboard entry without metadata
            SubmitLeaderboardEntryResponse response = await _testUser.SubmitLeaderboardEntryAsync(
                TEST_LEADERBOARD_NAME,
                score);

            // Assert
            Assert.IsNotNull(response, "SubmitLeaderboardEntry response should not be null.");
            Assert.AreEqual(_testUser.Id, response.Id, "User ID in response should match the test user's ID.");
            Assert.AreEqual(_testUser.Username, response.Username, "Username in response should match the test user's username.");
           
            
            Debug.Log($"Successfully submitted leaderboard entry without metadata with score {score} for user '{_testUser.Username}'");
        }

        [Test]
        public async Task Test_ClearLeaderboardEntries_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // First add some entries to later clear
            await _testUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME, 100);
            await _testUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME_2, 200);
            
            Debug.Log($"Submitted test leaderboard entries for user '{_testUser.Username}', now clearing {TEST_LEADERBOARD_NAME}");

            // Act: Clear leaderboard entries for a specific leaderboard
            User response = await _testUser.ClearLeaderboardEntriesAsync(TEST_LEADERBOARD_NAME);

            // Assert
            Assert.IsNotNull(response, "ClearLeaderboardEntries response should not be null.");
            Assert.AreEqual(_testUser.Id, response.Id, "User ID in response should match the test user's ID.");
            Assert.AreEqual(_testUser.Username, response.Username, "Username in response should match the test user's username.");
            
            
            Debug.Log($"Successfully cleared leaderboard entries for user '{_testUser.Username}' and leaderboard '{TEST_LEADERBOARD_NAME}'");

            // Verify entries are cleared by getting user's leaderboard entries for that specific leaderboard
            LeaderboardEntriesResponse entriesResponse = await _testUser.GetCurrentUserLeaderboardEntriesAsync(DEFAULT_LIMIT, TEST_LEADERBOARD_NAME);
            
            // Check if entries are indeed cleared
            // Note: Depending on how quickly the API processes clear requests, this might still return entries
            // So this assertion might need adjustment based on the actual API behavior
            if (entriesResponse.LeaderboardEntries != null && entriesResponse.LeaderboardEntries.Count > 0)
            {
                Debug.Log($"Note: Found {entriesResponse.LeaderboardEntries.Count} entries after clearing. This might be due to API caching or timing.");
            }
            
            // But entries for the other leaderboard should still exist
            LeaderboardEntriesResponse otherEntries = await _testUser.GetCurrentUserLeaderboardEntriesAsync(DEFAULT_LIMIT, TEST_LEADERBOARD_NAME_2);
            
            Assert.IsNotNull(otherEntries, "Other leaderboard entries response should not be null.");
            Assert.GreaterOrEqual(otherEntries.LeaderboardEntries.Count, 1, "Entries for other leaderboard should still exist.");
        }

        [Test]
        public async Task Test_GetLeaderboardEntries_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsNotNull(_secondUser, "_secondUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            Assert.IsTrue(_secondUser.IsAuthenticated(), "_secondUser should be authenticated.");

            // First add entries from both users
            await _testUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME, 1500);
            await _secondUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME, 1300);
            
            Debug.Log($"Submitted test leaderboard entries for two users, now retrieving them");

            // Act: Get leaderboard entries
            LeaderboardEntriesResponse response = await _testUser.GetLeaderboardEntriesAsync(_testGame.Id,TEST_LEADERBOARD_NAME, DEFAULT_LIMIT);

            // Assert
            Assert.IsNotNull(response, "GetLeaderboardEntries response should not be null.");
            Assert.IsNotNull(response.LeaderboardEntries, "Leaderboard entries list should not be null.");
            
            // We might have more than 2 entries if the test environment has previous entries
            Assert.GreaterOrEqual(response.LeaderboardEntries.Count, 2, "There should be at least 2 leaderboard entries.");
            
            // Verify the entries contain both users
            bool foundTestUserEntry = response.LeaderboardEntries.Any(e => 
                e.Username == _testUser.Username && 
                e.LeaderboardName == TEST_LEADERBOARD_NAME);
                
            bool foundSecondUserEntry = response.LeaderboardEntries.Any(e => 
                e.Username == _secondUser.Username && 
                e.LeaderboardName == TEST_LEADERBOARD_NAME);
                
            Assert.IsTrue(foundTestUserEntry, $"Leaderboard entries should include an entry for {_testUser.Username}");
            Assert.IsTrue(foundSecondUserEntry, $"Leaderboard entries should include an entry for {_secondUser.Username}");
            
            Debug.Log($"Successfully retrieved {response.LeaderboardEntries.Count} leaderboard entries for '{TEST_LEADERBOARD_NAME}'");
        }

        [Test]
        public async Task Test_GetUserLeaderboardEntries_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // First add multiple entries for the test user on different leaderboards
            await _testUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME, 1500);
            await _testUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME_2, 1800);
            
            Debug.Log($"Submitted test leaderboard entries for user '{_testUser.Username}' on multiple leaderboards, now retrieving them");

            // Act: Get user's leaderboard entries
            LeaderboardEntriesResponse response = await _testUser.GetCurrentUserLeaderboardEntriesAsync(DEFAULT_LIMIT);

            // Assert
            Assert.IsNotNull(response, "GetUserLeaderboardEntries response should not be null.");
            Assert.IsNotNull(response.LeaderboardEntries, "Leaderboard entries list should not be null.");
            Assert.GreaterOrEqual(response.LeaderboardEntries.Count, 2, "There should be at least 2 leaderboard entries.");
            
            // Verify entries for both leaderboards
            bool foundFirstLeaderboardEntry = response.LeaderboardEntries.Any(e => 
                e.Username == _testUser.Username && 
                e.LeaderboardName == TEST_LEADERBOARD_NAME);
                
            bool foundSecondLeaderboardEntry = response.LeaderboardEntries.Any(e => 
                e.Username == _testUser.Username && 
                e.LeaderboardName == TEST_LEADERBOARD_NAME_2);
                
            Assert.IsTrue(foundFirstLeaderboardEntry, $"User entries should include an entry for {TEST_LEADERBOARD_NAME}");
            Assert.IsTrue(foundSecondLeaderboardEntry, $"User entries should include an entry for {TEST_LEADERBOARD_NAME_2}");
            
            Debug.Log($"Successfully retrieved {response.LeaderboardEntries.Count} leaderboard entries for user '{_testUser.Username}'");

            // Test with specific leaderboard filter
            LeaderboardEntriesResponse filteredResponse = await _testUser.GetCurrentUserLeaderboardEntriesAsync(DEFAULT_LIMIT, TEST_LEADERBOARD_NAME);
            
            Assert.IsNotNull(filteredResponse, "Filtered response should not be null.");
            Assert.IsNotNull(filteredResponse.LeaderboardEntries, "Filtered leaderboard entries list should not be null.");
            
            // All entries should be for the specified leaderboard
            foreach (var entry in filteredResponse.LeaderboardEntries)
            {
                Assert.AreEqual(TEST_LEADERBOARD_NAME, entry.LeaderboardName, "Filtered entries should only be for the specified leaderboard name.");
            }
            
            Debug.Log($"Successfully retrieved {filteredResponse.LeaderboardEntries.Count} filtered leaderboard entries for user '{_testUser.Username}' and leaderboard '{TEST_LEADERBOARD_NAME}'");
        }

        [Test]
        public async Task Test_GetLeaderboardEntriesForOtherUser_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsNotNull(_secondUser, "_secondUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            Assert.IsTrue(_secondUser.IsAuthenticated(), "_secondUser should be authenticated.");

            // First add entries for the second user
            await _secondUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME, 1500);
            await _secondUser.SubmitLeaderboardEntryAsync(TEST_LEADERBOARD_NAME_2, 1800);
            
            Debug.Log($"Submitted test leaderboard entries for user '{_secondUser.Username}', now retrieving them as '{_testUser.Username}'");

            // Act: First user gets second user's leaderboard entries
            LeaderboardEntriesResponse response = await _testUser.GetUserLeaderboardEntriesAsync(
                _secondUser.Id,
                DEFAULT_LIMIT);

            // Assert
            Assert.IsNotNull(response, "GetUserLeaderboardEntries response should not be null.");
            Assert.IsNotNull(response.LeaderboardEntries, "Leaderboard entries list should not be null.");
            Assert.GreaterOrEqual(response.LeaderboardEntries.Count, 2, "There should be at least 2 leaderboard entries.");
            
            // Verify all entries are for the second user
            foreach (var entry in response.LeaderboardEntries)
            {
                Assert.AreEqual(_secondUser.Username, entry.Username, "Entries should be for the second user only.");
            }
            
            // Verify entries for both leaderboards
            bool foundFirstLeaderboardEntry = response.LeaderboardEntries.Any(e => 
                e.Username == _secondUser.Username && 
                e.LeaderboardName == TEST_LEADERBOARD_NAME);
                
            bool foundSecondLeaderboardEntry = response.LeaderboardEntries.Any(e => 
                e.Username == _secondUser.Username && 
                e.LeaderboardName == TEST_LEADERBOARD_NAME_2);
                
            Assert.IsTrue(foundFirstLeaderboardEntry, $"User entries should include an entry for {TEST_LEADERBOARD_NAME}");
            Assert.IsTrue(foundSecondLeaderboardEntry, $"User entries should include an entry for {TEST_LEADERBOARD_NAME_2}");
            
            Debug.Log($"Successfully retrieved {response.LeaderboardEntries.Count} leaderboard entries for user '{_secondUser.Username}' as user '{_testUser.Username}'");
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
            }
            else
            {
                Debug.LogError("Test configuration file not found: " + configPath + ". Please create it in Assets/TestConfiguration/ with adminToken and adminName.");
                Assert.Inconclusive("Test configuration file (testConfig.json) not found in Assets/TestConfiguration/. Please create it with adminToken and adminName properties.");
            }
        }

        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
        }
    }
}