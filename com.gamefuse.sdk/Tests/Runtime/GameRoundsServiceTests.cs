using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class GameRoundsServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private ISessionsService _sessionsService;
        private IGameRoundsService _gameRoundsService;
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
            _testGameId = gameResponse.id;
            _testGameToken = gameResponse.token;

            _user = await CreateAndSignInUser("testuser");
            _gameRoundsService = new GameRoundsService("https://gamefuse.co/api/v3", _user.authentication_token);
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
        public async Task CreateGameRound_WithMinimalData_CreatesSuccessfully()
        {
            try
            {
                await SetUpAsync();

                var gameRound = await _gameRoundsService.CreateGameRoundAsync(_user.id);

                Assert.NotNull(gameRound);
                Assert.Greater(gameRound.Id, 0);
                Assert.AreEqual(_user.id, gameRound.GameUserId);
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task CreateGameRound_WithFullData_CreatesSuccessfully()
        {
            try
            {
                await SetUpAsync();

                var gameRound = new GameRoundObject
                {
                    GameUserId = _user.id,
                    GameType = "solo_adventure",
                    Score = 1000.0,
                    Place = 1,
                    StartTime = "2024-01-25T05:00:00.000-05:00",
                    EndTime = "2024-01-25T05:30:00.000-05:00",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Medium" }
                    }
                };

                var createdRound = await _gameRoundsService.CreateGameRoundAsync(gameRound);

                Assert.NotNull(createdRound);
                Assert.Greater(createdRound.Id, 0);
                Assert.AreEqual(_user.id, createdRound.GameUserId);
                Assert.AreEqual("solo_adventure", createdRound.GameType);
                Assert.AreEqual(1000.0, createdRound.Score);
                Assert.AreEqual(1, createdRound.Place);
                Assert.NotNull(createdRound.Metadata);
                Assert.AreEqual("Medium", createdRound.Metadata["difficulty"]);
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task CreateGameRound_WithoutGameUserId_ThrowsException()
        {
            try
            {
                await SetUpAsync();

                var gameRound = new GameRoundObject
                {
                    GameType = "solo_adventure",
                    Score = 1000.0
                };

                var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                    await _gameRoundsService.CreateGameRoundAsync(gameRound)
                );

                Assert.That(ex.Message.Contains("GameUserId is required"));
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task CreateMultiplayerGameRound_AndAddPlayer_CreatesSuccessfully()
        {
            try
            {
                await SetUpAsync();
                var secondUser = await CreateAndSignInUser("secondplayer");
                var secondUserService = new GameRoundsService("https://gamefuse.co/api/v3", secondUser.authentication_token);

                // Create multiplayer game round
                var multiplayerRound = new GameRoundObject
                {
                    GameUserId = _user.id,
                    StartTime = "2024-01-25T10:00:00Z",
                    EndTime = "2024-01-25T10:30:00Z",
                    Score = 1200.0,
                    Place = 1,
                    GameType = "multiplayer_battle",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Hard" }
                    },
                    MultiplayerGameRoundId = null,
                    Multiplayer = true
                };

                var createdMultiplayerRound = await _gameRoundsService.CreateGameRoundAsync(multiplayerRound);
                Assert.NotNull(createdMultiplayerRound, "Created multiplayer round should not be null");
                Debug.Log($"Created multiplayer round ID: {createdMultiplayerRound.Id}");
                Debug.Log($"Multiplayer game round ID: {createdMultiplayerRound.MultiplayerGameRoundId}");

                // Add second player's round
                var secondPlayerRound = new GameRoundObject
                {
                    GameUserId = secondUser.id,
                    StartTime = "2024-01-25T10:30:00Z",
                    EndTime = "2024-01-25T11:00:00Z",
                    Score = 1100.0,
                    Place = 2,
                    GameType = "multiplayer_battle",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Easy" }
                    },
                    MultiplayerGameRoundId = createdMultiplayerRound.MultiplayerGameRoundId
                };

                var addedPlayerRound = await secondUserService.CreateGameRoundAsync(secondPlayerRound);
                Assert.NotNull(addedPlayerRound, "Added player round should not be null");
                Debug.Log($"Added player round ID: {addedPlayerRound.Id}");

                // Verify multiplayer rounds using GetUserGameRoundsAsync
                var userRounds = await _gameRoundsService.GetUserGameRoundsAsync(_user.id);
                var foundMultiplayerRound = userRounds.GameRounds.FirstOrDefault(r => r.Id == createdMultiplayerRound.Id);

                Assert.NotNull(foundMultiplayerRound, "Should find the multiplayer round");
                Assert.NotNull(foundMultiplayerRound.Rankings, "Rankings should not be null");
                Assert.AreEqual(2, foundMultiplayerRound.Rankings.Length, "Should have 2 rankings");
                Assert.That(foundMultiplayerRound.Rankings.Any(r => r.User.Id == _user.id), "Should contain first player");
                Assert.That(foundMultiplayerRound.Rankings.Any(r => r.User.Id == secondUser.id), "Should contain second player");
            }
            finally
            {
                await TearDownAsync();
            }
        }


        [Test]
        public async Task UpdateGameRound_ModifiesExistingRound()
        {
            try
            {
                await SetUpAsync();

                // Create initial round
                var initialRound = await _gameRoundsService.CreateGameRoundAsync(_user.id);
                Assert.NotNull(initialRound);

                // Update the round
               
                int newScore = 2000;
                int newPlace = 2;

                var updatedRound = await _gameRoundsService.UpdateGameRoundAsync(initialRound.Id, newScore, newPlace);

                Assert.NotNull(updatedRound);
                Assert.AreEqual(initialRound.Id, updatedRound.Id);
                Assert.AreEqual(newScore, updatedRound.Score);
                Assert.AreEqual(newPlace, updatedRound.Place);
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetGameRound_ReturnsCorrectRound()
        {
            try
            {
                await SetUpAsync();

                // Create a round
                var createdRound = await _gameRoundsService.CreateGameRoundAsync(_user.id);
                Assert.NotNull(createdRound);

                // Get the round
                var retrievedRound = await _gameRoundsService.GetGameRoundAsync(createdRound.Id);

                Assert.NotNull(retrievedRound);
                Assert.AreEqual(createdRound.Id, retrievedRound.Id);
                Assert.AreEqual(createdRound.GameUserId, retrievedRound.GameUserId);
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetUserGameRounds_ReturnsAllUserRounds()
        {
            try
            {
                await SetUpAsync();

                // Create multiple rounds
                await _gameRoundsService.CreateGameRoundAsync(_user.id);
                await _gameRoundsService.CreateGameRoundAsync(_user.id);

                // Get all rounds
                var response = await _gameRoundsService.GetUserGameRoundsAsync(_user.id);

                Assert.NotNull(response);
                Assert.NotNull(response.GameRounds);
                Assert.GreaterOrEqual(response.GameRounds.Length, 2);
                Assert.That(response.GameRounds.All(r => r.GameUserId == _user.id));
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task DeleteGameRound_RemovesRoundSuccessfully()
        {
            try
            {
                await SetUpAsync();

                // Create a round
                var createdRound = await _gameRoundsService.CreateGameRoundAsync(_user.id);
                Assert.NotNull(createdRound);

                // Delete the round
                var deleteResponse = await _gameRoundsService.DeleteGameRoundAsync(createdRound.Id);

                Assert.NotNull(deleteResponse);
                Assert.NotNull(deleteResponse.Message);
                Assert.That(deleteResponse.Message.Contains("destroyed successfully"));

                // Verify round is deleted
                var response = await _gameRoundsService.GetUserGameRoundsAsync(_user.id);
                Assert.That(!response.GameRounds.Any(r => r.Id == createdRound.Id));
            }
            finally
            {
                await TearDownAsync();
            }
        }
    }
}