using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;

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
                
                string leaderboardName = "test_leaderboard";
                int score = 1000;
                
                // Add a leaderboard entry
                var response = await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, score);
                
                Assert.NotNull(response);
                Assert.NotNull(response.Message);
                Assert.That(response.Message.Contains("Leaderboard entry added"));
                
                // Verify the entry was added by retrieving it
                var entries = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10, leaderboardName);
                
                Assert.NotNull(entries);
                Assert.NotNull(entries.LeaderboardEntries);
                Assert.IsTrue(entries.LeaderboardEntries.Length > 0);
                
                var entry = entries.LeaderboardEntries.FirstOrDefault(e => e.LeaderboardName == leaderboardName);
                Assert.NotNull(entry);
                Assert.AreEqual(score, entry.Score);
                Assert.AreEqual(_user.Username, entry.Username);
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
                
                string leaderboardName = "test_metadata_leaderboard";
                int score = 1500;
                var metadata = new
                {
                    level = "hard",
                    time = 120,
                    achievements = new[] { "headshot", "speedrun" }
                };
                
                // Add a leaderboard entry with metadata
                var response = await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, score, metadata);
                
                Assert.NotNull(response);
                Assert.NotNull(response.Message);
                
                // Verify the entry was added with metadata
                var entries = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10, leaderboardName);
                
                Assert.NotNull(entries);
                Assert.NotNull(entries.LeaderboardEntries);
                Assert.GreaterOrEqual(entries.LeaderboardEntries.Length, 1);
                
                var entry = entries.LeaderboardEntries.FirstOrDefault(e => e.LeaderboardName == leaderboardName);
                Assert.NotNull(entry);
                Assert.AreEqual(score, entry.Score);
                Assert.NotNull(entry.Metadata);
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
                
                string leaderboardName = "test_game_leaderboard";
                
                // Add entries for different users
                await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, 1000);
                await leaderboardService2.AddLeaderboardEntryAsync(user2.Id, leaderboardName, 1500);
                await leaderboardService3.AddLeaderboardEntryAsync(user3.Id, leaderboardName, 800);
                
                // Get game-specific leaderboard entries
                var entries = await _leaderboardService.GetGameLeaderboardEntriesAsync(_testGameId, leaderboardName, 10);
                
                Assert.NotNull(entries);
                Assert.NotNull(entries.LeaderboardEntries);
                Assert.GreaterOrEqual(entries.LeaderboardEntries.Length, 3);
                
                // Verify entries are sorted by score (descending)
                Assert.AreEqual(1500, entries.LeaderboardEntries[0].Score);
                Assert.AreEqual(1000, entries.LeaderboardEntries[1].Score);
                Assert.AreEqual(800, entries.LeaderboardEntries[2].Score);
                
                // Verify usernames
                Assert.AreEqual(user2.Username, entries.LeaderboardEntries[0].Username);
                Assert.AreEqual(_user.Username, entries.LeaderboardEntries[1].Username);
                Assert.AreEqual(user3.Username, entries.LeaderboardEntries[2].Username);
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
                
                string leaderboardName = "test_clear_leaderboard";
                
                // Add a few leaderboard entries
                await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, leaderboardName, 1000);
                await _leaderboardService.AddLeaderboardEntryAsync(_user.Id, "other_leaderboard", 500);
                
                // Clear leaderboard entries
                var response = await _leaderboardService.ClearLeaderboardEntriesAsync(_user.Id);
                
                Assert.NotNull(response);
                Assert.NotNull(response.Message);
                Assert.That(response.Message.Contains("Leaderboard entries cleared"));
                
                // Verify entries were removed
                var entries = await _leaderboardService.GetUserLeaderboardEntriesAsync(_user.Id, 10);
                
                Assert.NotNull(entries);
                Assert.NotNull(entries.LeaderboardEntries);
                Assert.AreEqual(0, entries.LeaderboardEntries.Length);
            }
            finally
            {
                await TearDownAsync();
            }
        }
    }
}