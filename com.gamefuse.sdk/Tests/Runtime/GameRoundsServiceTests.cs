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

                // Step 1: Create a player round for the first user (creator)
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

                // Step 2: Create the multiplayer game round container with the first player
                var playerRounds = new System.Collections.Generic.List<GameRoundObject>
                {
                    player1Round
                };

                Debug.Log("Creating multiplayer game round for first player");
                var multiplayerGameRound = await _gameRoundsService.CreateMultiplayerGameRoundAsync("multiplayer_battle", _user.Id, playerRounds);
                Assert.NotNull(multiplayerGameRound, "Multiplayer game round response should not be null");
                Assert.NotNull(multiplayerGameRound.MultiplayerGameRound, "Multiplayer game round should not be null");
                
                // Extract the multiplayer game round ID
                int multiplayerGameRoundId = multiplayerGameRound.MultiplayerGameRound.Id;
                Debug.Log($"Created multiplayer game round with ID: {multiplayerGameRoundId}");
                
                // Step 3: Create a player round for the second user
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
                
                // Step 4: Have the second user add themselves to the multiplayer game round
                Debug.Log($"Adding second player to multiplayer game round {multiplayerGameRoundId}");
                var secondPlayerRound = await secondUserService.AddPlayerToMultiplayerGameRoundAsync(multiplayerGameRoundId, player2Round);
                Assert.NotNull(secondPlayerRound, "Second player's round should not be null");
                Assert.AreEqual(multiplayerGameRoundId, secondPlayerRound.MultiplayerGameRoundId, "Second player's round should be linked to the multiplayer game round");
                
                // Allow time for the server to process
                await Task.Delay(1000);
                
                // Step 5: Get the updated multiplayer game round details
                var retrievedMultiplayerRound = await _gameRoundsService.GetMultiplayerGameRoundAsync(multiplayerGameRoundId);
                Assert.NotNull(retrievedMultiplayerRound, "Retrieved multiplayer round should not be null");
                Assert.NotNull(retrievedMultiplayerRound.Rankings, "Rankings should not be null");
                
                Debug.Log($"Retrieved multiplayer game round has {retrievedMultiplayerRound.Rankings?.Length ?? 0} rankings");
                
                // Verify that we have rankings for both players
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

                // Create multiple rounds with explicit game type and wait between creations
                var round1 = new GameRoundObject
                {
                    GameUserId = _user.Id,
                    GameType = "test_round_1",
                    Score = 100,
                    Place = 1,
                    // Add more specific data to ensure unique rounds
                    StartTime = "2024-01-25T10:00:00Z",
                    EndTime = "2024-01-25T10:30:00Z",
                    Metadata = new System.Collections.Generic.Dictionary<string, string> { { "test", "round1" } }
                };
                
                var round2 = new GameRoundObject
                {
                    GameUserId = _user.Id,
                    GameType = "test_round_2",
                    Score = 200,
                    Place = 2,
                    // Add more specific data to ensure unique rounds
                    StartTime = "2024-01-25T11:00:00Z",
                    EndTime = "2024-01-25T11:30:00Z",
                    Metadata = new System.Collections.Generic.Dictionary<string, string> { { "test", "round2" } }
                };
                
                Debug.Log($"Creating first game round for user {_user.Id}");
                // Create first round
                var createdRound1 = await _gameRoundsService.CreateGameRoundAsync(round1);
                Assert.NotNull(createdRound1, "First round should be created successfully");
                Debug.Log($"Created first round with ID {createdRound1.Id}");
                
                // Wait to ensure first round is properly saved - longer delay to ensure stability
                await Task.Delay(2000);
                
                Debug.Log($"Creating second game round for user {_user.Id}");
                // Create second round
                var createdRound2 = await _gameRoundsService.CreateGameRoundAsync(round2);
                Assert.NotNull(createdRound2, "Second round should be created successfully");
                Debug.Log($"Created second round with ID {createdRound2.Id}");
                
                // Wait to ensure second round is properly saved - longer delay to ensure stability
                await Task.Delay(2000);

                // Get all rounds
                Debug.Log($"Getting all game rounds for user {_user.Id}");
                var response = await _gameRoundsService.GetUserGameRoundsAsync(_user.Id);

                Assert.NotNull(response, "Response should not be null");
                Assert.NotNull(response.GameRounds, "GameRounds array should not be null");
                
                Debug.Log($"Found {response.GameRounds.Length} game rounds for user {_user.Id}");
                foreach (var round in response.GameRounds)
                {
                    Debug.Log($"  Round {round.Id}: Type={round.GameType}, Score={round.Score}");
                }
                
                // Verify we have both rounds
                Assert.GreaterOrEqual(response.GameRounds.Length, 2, "Should have at least 2 game rounds");
                
                // Check for specific rounds by ID
                var foundRound1 = response.GameRounds.Any(r => r.Id == createdRound1.Id);
                var foundRound2 = response.GameRounds.Any(r => r.Id == createdRound2.Id);
                
                Debug.Log($"Round 1 found: {foundRound1}, Round 2 found: {foundRound2}");
                
                Assert.That(foundRound1, "First created round should be in results");
                Assert.That(foundRound2, "Second created round should be in results");
                Assert.That(response.GameRounds.All(r => r.GameUserId == _user.Id), "All rounds should belong to test user");
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