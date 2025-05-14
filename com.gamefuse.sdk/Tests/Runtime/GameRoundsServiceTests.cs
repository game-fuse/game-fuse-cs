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
            _testGameId = gameResponse.Id;
            _testGameToken = gameResponse.Token;

            _user = await CreateAndSignInUser("testuser");
            _gameRoundsService = new GameRoundsService("https://gamefuse.co/api/v3", _user.AuthenticationToken);
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
        public async Task CreateGameRound_WithMinimalData_CreatesSuccessfully()
        {
            try
            {
                await SetUpAsync();

                var gameRound = await _gameRoundsService.CreateGameRoundAsync(_user.Id);

                Assert.NotNull(gameRound);
                Assert.Greater(gameRound.Id, 0);
                Assert.AreEqual(_user.Id, gameRound.GameUserId);
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
                    GameUserId = _user.Id,
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
                Assert.AreEqual(_user.Id, createdRound.GameUserId);
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
                var secondUserService = new GameRoundsService("https://gamefuse.co/api/v3", secondUser.AuthenticationToken);

                // Create multiplayer game round using CreateMultiplayerGameRoundAsync
                var player1Round = new GameRoundObject
                {
                    GameUserId = _user.Id,
                    StartTime = "2024-01-25T10:00:00Z",
                    EndTime = "2024-01-25T10:30:00Z",
                    Score = 1200.0,
                    Place = 1,
                    GameType = "multiplayer_battle",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Hard" }
                    }
                };

                var player2Round = new GameRoundObject
                {
                    GameUserId = secondUser.Id,
                    StartTime = "2024-01-25T10:00:00Z",
                    EndTime = "2024-01-25T10:30:00Z",
                    Score = 1100.0,
                    Place = 2,
                    GameType = "multiplayer_battle",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Medium" }
                    }
                };

                // Use the new method to create a multiplayer game round with both player rounds
                var playerRounds = new System.Collections.Generic.List<GameRoundObject>
                {
                    player1Round,
                    player2Round
                };

                var multiplayerGameRound = await _gameRoundsService.CreateMultiplayerGameRoundAsync("multiplayer_battle", playerRounds);
                Assert.NotNull(multiplayerGameRound, "Multiplayer game round response should not be null");
                Assert.NotNull(multiplayerGameRound.MultiplayerGameRound, "Multiplayer game round should not be null");
                Assert.NotNull(multiplayerGameRound.Rankings, "Rankings should not be null");
                Assert.AreEqual(2, multiplayerGameRound.Rankings.Length, "Should have 2 rankings");
                
                // Get multiplayer game round details
                var retrievedMultiplayerRound = await _gameRoundsService.GetMultiplayerGameRoundAsync(multiplayerGameRound.MultiplayerGameRound.Id);
                Assert.NotNull(retrievedMultiplayerRound, "Retrieved multiplayer round should not be null");
                Assert.NotNull(retrievedMultiplayerRound.Rankings, "Rankings should not be null");
                Assert.AreEqual(2, retrievedMultiplayerRound.Rankings.Length, "Should have 2 rankings");
                
                // Verify player data in rankings
                var player1Ranking = retrievedMultiplayerRound.Rankings.FirstOrDefault(r => r.User.Id == _user.Id);
                var player2Ranking = retrievedMultiplayerRound.Rankings.FirstOrDefault(r => r.User.Id == secondUser.Id);
                
                Assert.NotNull(player1Ranking, "Player 1 ranking should exist");
                Assert.NotNull(player2Ranking, "Player 2 ranking should exist");
                Assert.AreEqual(1, player1Ranking.Place, "Player 1 should be in place 1");
                Assert.AreEqual(2, player2Ranking.Place, "Player 2 should be in place 2");
                Assert.AreEqual(1200.0, player1Ranking.Score, "Player 1 score should match");
                Assert.AreEqual(1100.0, player2Ranking.Score, "Player 2 score should match");
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
                var initialRound = await _gameRoundsService.CreateGameRoundAsync(_user.Id);
                Assert.NotNull(initialRound);

                // Update the round

                initialRound.Score = 2000;
                initialRound.Place = 2;

                var updatedRound = await _gameRoundsService.UpdateGameRoundAsync(initialRound.Id, initialRound);

                Assert.NotNull(updatedRound);
                Assert.AreEqual(initialRound.Id, updatedRound.Id);
                Assert.AreEqual(2000, updatedRound.Score);
                Assert.AreEqual(2, updatedRound.Place);
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task UpdateMultiplayerGameRound_UpdatesPlayerStats_And_Rankings()
        {
            try
            {
                await SetUpAsync();

                // Create a second player
                var secondUser = await CreateAndSignInUser("secondplayer");
                var secondUserService = new GameRoundsService("https://gamefuse.co/api/v3", secondUser.AuthenticationToken);

                // Create initial multiplayer game round for first player
                var firstPlayerRound = new GameRoundObject
                {
                    GameUserId = _user.Id,
                    StartTime = "2024-01-25T10:00:00Z",
                    EndTime = "2024-01-25T10:30:00Z",
                    Score = 1000.0,
                    Place = 2,
                    GameType = "multiplayer_battle",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Hard" }
                    },
                    Multiplayer = true
                    };

                var firstPlayerCreatedRound = await _gameRoundsService.CreateGameRoundAsync(firstPlayerRound);
                Assert.NotNull(firstPlayerCreatedRound, "First player's round should be created");
                Assert.NotNull(firstPlayerCreatedRound.MultiplayerGameRoundId, "Multiplayer game round ID should be assigned");

                // Add second player to the multiplayer game
                var secondPlayerRound = new GameRoundObject
                {
                    GameUserId = secondUser.Id,
                    StartTime = "2024-01-25T10:00:00Z",
                    EndTime = "2024-01-25T10:30:00Z",
                    Score = 800.0,
                    Place = 3,
                    GameType = "multiplayer_battle",
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "difficulty", "Hard" }
                    },
                    MultiplayerGameRoundId = firstPlayerCreatedRound.MultiplayerGameRoundId
                };

                var secondPlayerCreatedRound = await secondUserService.CreateGameRoundAsync(secondPlayerRound);
                Assert.NotNull(secondPlayerCreatedRound, "Second player's round should be created");

                // Update first player's score and place
                firstPlayerCreatedRound.Score = 1500.0;
                firstPlayerCreatedRound.Place = 1;

                var updatedRound = await _gameRoundsService.UpdateGameRoundAsync(firstPlayerCreatedRound.Id, firstPlayerCreatedRound);
                Assert.NotNull(updatedRound, "Updated round should not be null");
                Assert.AreEqual(1500.0, updatedRound.Score, "Score should be updated");
                Assert.AreEqual(1, updatedRound.Place, "Place should be updated");

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
                var createdRound = await _gameRoundsService.CreateGameRoundAsync(_user.Id);
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
                await _gameRoundsService.CreateGameRoundAsync(_user.Id);
                await _gameRoundsService.CreateGameRoundAsync(_user.Id);

                // Get all rounds
                var response = await _gameRoundsService.GetUserGameRoundsAsync(_user.Id);

                Assert.NotNull(response);
                Assert.NotNull(response.GameRounds);
                Assert.GreaterOrEqual(response.GameRounds.Length, 2);
                Assert.That(response.GameRounds.All(r => r.GameUserId == _user.Id));
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
                var createdRound = await _gameRoundsService.CreateGameRoundAsync(_user.Id);
                Assert.NotNull(createdRound);

                // Delete the round
                var deleteResponse = await _gameRoundsService.DeleteGameRoundAsync(createdRound.Id);

                Assert.NotNull(deleteResponse);
                Assert.NotNull(deleteResponse.Message);
                Assert.That(deleteResponse.Message.Contains("destroyed successfully"));

                // Verify round is deleted
                var response = await _gameRoundsService.GetUserGameRoundsAsync(_user.Id);
                Assert.That(!response.GameRounds.Any(r => r.Id == createdRound.Id));
            }
            finally
            {
                await TearDownAsync();
            }
        }
    }
}