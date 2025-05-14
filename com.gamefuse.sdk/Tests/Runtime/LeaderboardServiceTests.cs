using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class LeaderboardServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private ISessionsService _sessionsService;
        private ILeaderboardService _leaderboardService;
        private string _adminToken;
        private string _adminName;
        private int _testGameId;
        private string _testGameToken;
        private SignInResponse _user;

        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
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
                Assert.Fail("Test configuration file not found.");
            }
        }

        private async Task SetUpAsync()
        {
            _adminService = new SystemAdminTestSuiteService("https://gamefuse.co/api/v3", _adminToken, _adminName);
            _userService = new UserService("https://gamefuse.co/api/v3");
            _sessionsService = new SessionsService("https://gamefuse.co/api/v3");

            var gameResponse = await _adminService.CreateGameAsync();
            _testGameId = gameResponse.Id;
            _testGameToken = gameResponse.Token;

            _user = await CreateAndSignInUser("testuser");
            _leaderboardService = new LeaderboardService("https://gamefuse.co/api/v3", _user.AuthenticationToken);
        }

        private async Task TearDownAsync()
        {
            if (_testGameId != 0)
            {
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
        public async Task AddLeaderboardEntry_AddsEntrySuccessfully()
        {
            try
            {
                await SetUpAsync();
                
                string leaderboardName = $"test_leaderboard_{DateTime.Now.Ticks}";
                int score = 1000;
                
                Debug.Log($"Creating leaderboard entry with name: {leaderboardName} and score: {score}");
                
                // Add a leaderboard entry with a unique name to avoid conflicts
                var response = await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, score);
                
                Assert.NotNull(response, "Response should not be null");
                Assert.NotNull(response.Message, "Response message should not be null");
                
                // Give some time for the entry to be processed
                await Task.Delay(1000);
                
                // Verify the entry was added by retrieving it
                var entries = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10, leaderboardName);
                
                Assert.NotNull(entries, "Entries response should not be null");
                Assert.NotNull(entries.LeaderboardEntries, "Leaderboard entries array should not be null");
                
                Debug.Log($"Found {entries.LeaderboardEntries.Length} entries for leaderboard {leaderboardName}");
                
                // Find the entry with the matching leaderboard name
                var entry = entries.LeaderboardEntries.FirstOrDefault(e => e.LeaderboardName == leaderboardName);
                
                Assert.NotNull(entry, $"Entry for {leaderboardName} should exist");
                Assert.AreEqual(score, entry.Score, "Score should match");
                Assert.AreEqual(_user.Username, entry.Username, "Username should match");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task AddLeaderboardEntryWithMetadata_AddsEntryWithMetadataSuccessfully()
        {
            try
            {
                await SetUpAsync();
                
                string leaderboardName = $"test_metadata_leaderboard_{DateTime.Now.Ticks}";
                int score = 1500;
                var metadata = new Dictionary<string, object>
                {
                    { "level", "hard" },
                    { "time", 120 },
                    { "achievements", new[] { "headshot", "speedrun" } }
                };
                
                Debug.Log($"Creating leaderboard entry with name: {leaderboardName}, score: {score}, and metadata");
                
                // Add a leaderboard entry with metadata
                var response = await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, score, metadata);
                
                Assert.NotNull(response, "Response should not be null");
                Assert.NotNull(response.Message, "Response message should not be null");
                
                // Give some time for the entry to be processed
                await Task.Delay(1000);
                
                // Verify the entry was added with metadata
                var entries = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10, leaderboardName);
                
                Assert.NotNull(entries, "Entries response should not be null");
                Assert.NotNull(entries.LeaderboardEntries, "Leaderboard entries array should not be null");
                
                Debug.Log($"Found {entries.LeaderboardEntries.Length} entries for leaderboard {leaderboardName}");
                
                var entry = entries.LeaderboardEntries.FirstOrDefault(e => e.LeaderboardName == leaderboardName);
                Assert.NotNull(entry, $"Entry for {leaderboardName} should exist");
                Assert.AreEqual(score, entry.Score, "Score should match");
                Assert.NotNull(entry.Metadata, "Metadata should not be null");
                
                // Additional debug information to see what metadata is returned
                foreach (var item in entry.Metadata)
                {
                    Debug.Log($"Metadata: {item.Key} = {item.Value}");
                }
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetGameLeaderboardEntries_ReturnsEntriesForGame()
        {
            try
            {
                await SetUpAsync();
                
                // Create multiple users with leaderboard entries
                var user2 = await CreateAndSignInUser("leaderboarduser2");
                var user3 = await CreateAndSignInUser("leaderboarduser3");
                
                var leaderboardService2 = new LeaderboardService("https://gamefuse.co/api/v3", user2.AuthenticationToken);
                var leaderboardService3 = new LeaderboardService("https://gamefuse.co/api/v3", user3.AuthenticationToken);
                
                string leaderboardName = $"test_game_leaderboard_{DateTime.Now.Ticks}";
                
                Debug.Log($"Creating leaderboard entries for multiple users with name: {leaderboardName}");
                
                // Add entries for different users with different scores
                await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, 1000);
                await leaderboardService2.AddLeaderboardEntryAsync(user2.Id, leaderboardName, 1500);
                await leaderboardService3.AddLeaderboardEntryAsync(user3.Id, leaderboardName, 800);
                
                // Give some time for the entries to be processed
                await Task.Delay(1000);
                
                // Get game-specific leaderboard entries
                var entries = await _leaderboardService.GetGameLeaderboardEntriesAsync(_testGameId, leaderboardName, 10);
                
                Assert.NotNull(entries, "Entries response should not be null");
                Assert.NotNull(entries.LeaderboardEntries, "Leaderboard entries array should not be null");
                
                Debug.Log($"Found {entries.LeaderboardEntries.Length} entries for game leaderboard {leaderboardName}");
                
                // Confirm we have at least 3 entries
                Assert.GreaterOrEqual(entries.LeaderboardEntries.Length, 3, "Should have at least 3 entries");
                
                // Log all entries for debugging
                for (int i = 0; i < entries.LeaderboardEntries.Length; i++)
                {
                    var entry = entries.LeaderboardEntries[i];
                    Debug.Log($"Entry {i}: Username={entry.Username}, Score={entry.Score}");
                }
                
                // Verify entries are sorted by score (descending)
                Assert.GreaterOrEqual(entries.LeaderboardEntries[0].Score, entries.LeaderboardEntries[1].Score, 
                    "First entry should have highest score");
                Assert.GreaterOrEqual(entries.LeaderboardEntries[1].Score, entries.LeaderboardEntries[2].Score, 
                    "Second entry should have higher score than third");
                
                // Count entries by username - not required for the test to pass but useful for verification
                int user1EntryCount = entries.LeaderboardEntries.Count(e => e.Username == _user.Username);
                int user2EntryCount = entries.LeaderboardEntries.Count(e => e.Username == user2.Username);
                int user3EntryCount = entries.LeaderboardEntries.Count(e => e.Username == user3.Username);
                
                Debug.Log($"Entry counts - User1: {user1EntryCount}, User2: {user2EntryCount}, User3: {user3EntryCount}");
                
                Assert.GreaterOrEqual(user1EntryCount, 1, "Should have at least one entry for user 1");
                Assert.GreaterOrEqual(user2EntryCount, 1, "Should have at least one entry for user 2");
                Assert.GreaterOrEqual(user3EntryCount, 1, "Should have at least one entry for user 3");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task ClearLeaderboardEntries_RemovesAllEntriesForUser()
        {
            try
            {
                await SetUpAsync();
                
                string leaderboardName = $"test_clear_leaderboard_{DateTime.Now.Ticks}";
                string otherLeaderboardName = $"other_leaderboard_{DateTime.Now.Ticks}";
                
                Debug.Log($"Adding leaderboard entries before clearing: {leaderboardName} and {otherLeaderboardName}");
                
                // Add a few leaderboard entries
                await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, 1000);
                await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, otherLeaderboardName, 500);
                
                // Give some time for the entries to be processed
                await Task.Delay(1000);
                
                // Verify the entries were added
                var entriesBefore = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10);
                Debug.Log($"Before clearing: Found {entriesBefore.LeaderboardEntries.Length} entries for user");
                
                // Clear leaderboard entries
                var response = await _leaderboardService.ClearLeaderboardEntriesAsync(_user.Id);
                
                Assert.NotNull(response, "Response should not be null");
                Assert.NotNull(response.Message, "Response message should not be null");
                
                Debug.Log($"Clear response message: {response.Message}");
                
                // Give some time for the entries to be cleared
                await Task.Delay(1000);
                
                // Verify entries were removed
                var entries = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10);
                
                Assert.NotNull(entries, "Entries response should not be null");
                Assert.NotNull(entries.LeaderboardEntries, "Leaderboard entries array should not be null");
                
                Debug.Log($"After clearing: Found {entries.LeaderboardEntries.Length} entries for user");
                
                // Either there should be no entries, or if entries exist, they should not include our test entries
                if (entries.LeaderboardEntries.Length > 0)
                {
                    var clearedEntry1 = entries.LeaderboardEntries.FirstOrDefault(e => 
                        e.LeaderboardName == leaderboardName && e.GameUserId == _user.Id);
                    var clearedEntry2 = entries.LeaderboardEntries.FirstOrDefault(e => 
                        e.LeaderboardName == otherLeaderboardName && e.GameUserId == _user.Id);
                    
                    Assert.IsNull(clearedEntry1, $"Entry for {leaderboardName} should be cleared");
                    Assert.IsNull(clearedEntry2, $"Entry for {otherLeaderboardName} should be cleared");
                }
                else
                {
                    // If API properly cleared all entries, there should be none
                    Assert.AreEqual(0, entries.LeaderboardEntries.Length, "All entries should be cleared");
                }
            }
            finally
            {
                await TearDownAsync();
            }
        }
    }
}