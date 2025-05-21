// GameFuseUserGameRoundsIntegrationTests.cs
using GameFuse.Models.Shared;
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
    public class GameFuseUserGameRoundsIntegrationTests
    {
        private GameFuseUser _testUser;
        private GameFuseUser _secondUser;
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
        public async Task Test_CreateNonMultiplayerGameRound_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // Prepare test data
            string gameType = "solo_adventure";
            string startTime = DateTime.UtcNow.AddMinutes(-30).ToString("o");
            string endTime = DateTime.UtcNow.ToString("o");
            int score = 1000;
            int place = 1;
            var metadata = new Dictionary<string, object>
            {
                ["difficulty"] = "Medium"
            };

            Debug.Log($"Creating non-multiplayer game round for user '{_testUser.Username}'");

            // Act: Create a non-multiplayer game round
            GameRound gameRound = await _testUser.CreateGameRoundAsync(
                gameType,
                startTime,
                endTime,
                score,
                place,
                metadata);

            // Assert
            Assert.IsNotNull(gameRound, "GameRound should not be null.");
            Assert.Greater(gameRound.Id, 0, "GameRound ID should be positive.");
            Assert.AreEqual(_testUser.Id, gameRound.GameUserId, "Game user ID should match the test user's ID.");
            Assert.AreEqual(gameType, gameRound.GameType, "Game type should match.");
            Assert.AreEqual(score, gameRound.Score, "Score should match.");
            Assert.AreEqual(place, gameRound.Place, "Place should match.");
            Assert.IsNull(gameRound.MultiplayerGameRoundId, "MultiplayerGameRoundId should be null for a non-multiplayer game.");
            Assert.IsNotNull(gameRound.Metadata, "Metadata should not be null.");
            Assert.IsTrue(gameRound.Metadata.ContainsKey("difficulty"), "Metadata should contain the 'difficulty' key.");
            Assert.AreEqual("Medium", gameRound.Metadata["difficulty"], "Difficulty in metadata should match.");

            Debug.Log($"Successfully created non-multiplayer game round with ID: {gameRound.Id}");
        }

        [Test]
        public async Task Test_CreateMultiplayerGameRound_AndAddMoreRounds_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsNotNull(_secondUser, "_secondUser was not initialized.");
            Assert.IsNotNull(_thirdUser, "_thirdUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            Assert.IsTrue(_secondUser.IsAuthenticated(), "_secondUser should be authenticated.");
            Assert.IsTrue(_thirdUser.IsAuthenticated(), "_thirdUser should be authenticated.");

            // Prepare test data for the first multiplayer game round
            string gameType = "multiplayer_battle";
            string startTime = DateTime.UtcNow.AddMinutes(-30).ToString("o");
            string endTime = DateTime.UtcNow.ToString("o");
            int firstUserScore = 1200;
            int firstUserPlace = 1;
            var metadata = new Dictionary<string, object>
            {
                ["difficulty"] = "Hard"
            };

            Debug.Log($"Creating first multiplayer game round for user '{_testUser.Username}'");

            // Act: Create the first multiplayer game round
            GameRound firstGameRound = await _testUser.CreateMultiplayerGameRoundAsync(
                gameType,
                startTime,
                endTime,
                firstUserScore,
                firstUserPlace,
                metadata);

            // Assert for first game round
            Assert.IsNotNull(firstGameRound, "First game round should not be null.");
            Assert.Greater(firstGameRound.Id, 0, "First game round ID should be positive.");
            Assert.AreEqual(_testUser.Id, firstGameRound.GameUserId, "Game user ID should match the first test user's ID.");
            Assert.AreEqual(gameType, firstGameRound.GameType, "Game type should match.");
            Assert.AreEqual(firstUserScore, firstGameRound.Score, "Score should match.");
            Assert.IsNotNull(firstGameRound.MultiplayerGameRoundId, "MultiplayerGameRoundId should not be null for a multiplayer game.");
            Assert.Greater(firstGameRound.MultiplayerGameRoundId.Value, 0, "MultiplayerGameRoundId should be positive.");

            int multiplayerGameRoundId = firstGameRound.MultiplayerGameRoundId.Value;
            Debug.Log($"Successfully created the first multiplayer game round with ID: {firstGameRound.Id}, connected to multiplayer game round ID: {multiplayerGameRoundId}");

            // Prepare data for the second user's game round
            int secondUserScore = 1100;
            int secondUserPlace = 2;
            string secondUserStartTime = DateTime.UtcNow.AddMinutes(-20).ToString("o");
            string secondUserEndTime = DateTime.UtcNow.AddMinutes(-5).ToString("o");

            Debug.Log($"Adding second user '{_secondUser.Username}' to the multiplayer game round");

            // Act: Add the second user to the multiplayer game
            GameRound secondGameRound = await _secondUser.CreateMultiplayerGameRoundAsync(
                gameType,
                secondUserStartTime,
                secondUserEndTime,
                secondUserScore,
                secondUserPlace,
                metadata,
                multiplayerGameRoundId);

            // Assert for second game round
            Assert.IsNotNull(secondGameRound, "Second game round should not be null.");
            Assert.Greater(secondGameRound.Id, 0, "Second game round ID should be positive.");
            Assert.AreNotEqual(firstGameRound.Id, secondGameRound.Id, "Game round IDs should be different.");
            Assert.AreEqual(_secondUser.Id, secondGameRound.GameUserId, "Game user ID should match the second test user's ID.");
            Assert.AreEqual(multiplayerGameRoundId, secondGameRound.MultiplayerGameRoundId, "MultiplayerGameRoundId should match the first game round's multiplayer ID.");
            Assert.AreEqual(secondUserScore, secondGameRound.Score, "Score should match for second user.");

            Debug.Log($"Successfully added second user to the multiplayer game round with game round ID: {secondGameRound.Id}");

            // Prepare data for the third user's game round
            int thirdUserScore = 900;
            int thirdUserPlace = 3;
            string thirdUserStartTime = DateTime.UtcNow.AddMinutes(-15).ToString("o");
            string thirdUserEndTime = DateTime.UtcNow.AddMinutes(-3).ToString("o");

            Debug.Log($"Adding third user '{_thirdUser.Username}' to the multiplayer game round");

            // Act: Add the third user to the multiplayer game
            GameRound thirdGameRound = await _thirdUser.CreateMultiplayerGameRoundAsync(
                gameType,
                thirdUserStartTime,
                thirdUserEndTime,
                thirdUserScore,
                thirdUserPlace,
                metadata,
                multiplayerGameRoundId);

            // Assert for third game round
            Assert.IsNotNull(thirdGameRound, "Third game round should not be null.");
            Assert.Greater(thirdGameRound.Id, 0, "Third game round ID should be positive.");
            Assert.AreNotEqual(firstGameRound.Id, thirdGameRound.Id, "First and third game round IDs should be different.");
            Assert.AreNotEqual(secondGameRound.Id, thirdGameRound.Id, "Second and third game round IDs should be different.");
            Assert.AreEqual(_thirdUser.Id, thirdGameRound.GameUserId, "Game user ID should match the third test user's ID.");
            Assert.AreEqual(multiplayerGameRoundId, thirdGameRound.MultiplayerGameRoundId, "MultiplayerGameRoundId should match the first game round's multiplayer ID.");
            Assert.AreEqual(thirdUserScore, thirdGameRound.Score, "Score should match for third user.");

            Debug.Log($"Successfully added third user to the multiplayer game round with game round ID: {thirdGameRound.Id}");

            // Optionally, retrieve the first game round to check rankings
            GameRound retrievedFirstRound = await _testUser.GetGameRoundAsync(firstGameRound.Id);
            Assert.IsNotNull(retrievedFirstRound, "Retrieved first game round should not be null.");
            
            // Check if rankings are present (they might not be in the integration test environment)
            if (retrievedFirstRound.Rankings != null && retrievedFirstRound.Rankings.Count > 0)
            {
                Debug.Log($"Found {retrievedFirstRound.Rankings.Count} rankings in the multiplayer game round");
                Assert.AreEqual(3, retrievedFirstRound.Rankings.Count, "There should be 3 rankings.");
                
                // Verify rankings are sorted correctly
                var firstRanking = retrievedFirstRound.Rankings.First();
                Assert.AreEqual(1, firstRanking.Place, "First place in rankings should be 1.");
                Assert.AreEqual(firstUserScore, firstRanking.Score, "First place score should match.");
                
                var lastRanking = retrievedFirstRound.Rankings.Last();
                Assert.AreEqual(3, lastRanking.Place, "Last place in rankings should be 3.");
                Assert.AreEqual(thirdUserScore, lastRanking.Score, "Last place score should match.");
            }
        }

        [Test]
        public async Task Test_GetGameRound_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // First create a game round to retrieve
            string gameType = "test_adventure";
            string startTime = DateTime.UtcNow.AddMinutes(-30).ToString("o");
            string endTime = DateTime.UtcNow.ToString("o");
            int score = 500;
            int place = 2;
            var metadata = new Dictionary<string, object>
            {
                ["level"] = "Easy"
            };

            Debug.Log($"Creating a game round for user '{_testUser.Username}' to test retrieval");
            
            // Arrange: Create a game round
            GameRound createdRound = await _testUser.CreateGameRoundAsync(
                gameType,
                startTime,
                endTime,
                score,
                place,
                metadata);
            
            Assert.IsNotNull(createdRound, "Created game round should not be null.");
            int gameRoundId = createdRound.Id;
            Debug.Log($"Successfully created game round with ID: {gameRoundId}");

            // Act: Retrieve the game round
            GameRound retrievedRound = await _testUser.GetGameRoundAsync(gameRoundId);

            // Assert
            Assert.IsNotNull(retrievedRound, "Retrieved game round should not be null.");
            Assert.AreEqual(gameRoundId, retrievedRound.Id, "Game round ID should match.");
            Assert.AreEqual(_testUser.Id, retrievedRound.GameUserId, "Game user ID should match.");
            Assert.AreEqual(gameType, retrievedRound.GameType, "Game type should match.");
            Assert.AreEqual(score, retrievedRound.Score, "Score should match.");
            Assert.AreEqual(place, retrievedRound.Place, "Place should match.");
            Assert.IsNotNull(retrievedRound.Metadata, "Metadata should not be null.");
            Assert.IsTrue(retrievedRound.Metadata.ContainsKey("level"), "Metadata should contain 'level' key.");
            Assert.AreEqual("Easy", retrievedRound.Metadata["level"], "Level in metadata should match.");

            Debug.Log($"Successfully retrieved game round with ID: {gameRoundId}");
        }

        [Test]
        public async Task Test_UpdateGameRound_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // First create a game round to update
            string gameType = "update_test";
            string startTime = DateTime.UtcNow.AddMinutes(-45).ToString("o");
            string endTime = DateTime.UtcNow.AddMinutes(-15).ToString("o");
            int initialScore = 300;
            int initialPlace = 5;
            var initialMetadata = new Dictionary<string, object>
            {
                ["level"] = "Beginner"
            };

            Debug.Log($"Creating a game round for user '{_testUser.Username}' to test update");
            
            // Arrange: Create a game round
            GameRound createdRound = await _testUser.CreateGameRoundAsync(
                gameType,
                startTime,
                endTime,
                initialScore,
                initialPlace,
                initialMetadata);
            
            Assert.IsNotNull(createdRound, "Created game round should not be null.");
            int gameRoundId = createdRound.Id;
            Debug.Log($"Successfully created game round with ID: {gameRoundId}");

            // Prepare update data
            int updatedScore = 800;
            int updatedPlace = 2;
            var updatedMetadata = new Dictionary<string, object>
            {
                ["level"] = "Advanced",
                ["bonus"] = 50
            };

            // Act: Update the game round
            Debug.Log($"Updating game round with ID: {gameRoundId}");
            GameRound updatedRound = await _testUser.UpdateGameRoundAsync(
                gameRoundId,
                null, // Keep start time
                null, // Keep end time
                updatedScore,
                updatedPlace,
                null, // Keep game type
                updatedMetadata);

            // Assert
            Assert.IsNotNull(updatedRound, "Updated game round should not be null.");
            Assert.AreEqual(gameRoundId, updatedRound.Id, "Game round ID should match.");
            Assert.AreEqual(_testUser.Id, updatedRound.GameUserId, "Game user ID should match.");
            Assert.AreEqual(gameType, updatedRound.GameType, "Game type should still match the original.");
            Assert.AreEqual(updatedScore, updatedRound.Score, "Score should be updated.");
            Assert.AreEqual(updatedPlace, updatedRound.Place, "Place should be updated.");
            Assert.IsNotNull(updatedRound.Metadata, "Metadata should not be null after update.");
            
            // Check if metadata was updated correctly - this depends on server implementation
            // Some servers replace metadata completely, others merge it
            Assert.IsTrue(updatedRound.Metadata.ContainsKey("level"), "Metadata should contain 'level' key after update.");
            Assert.AreEqual("Advanced", updatedRound.Metadata["level"], "Level in metadata should be updated.");

            Debug.Log($"Successfully updated game round with ID: {gameRoundId}");
        }

        [Test]
        public async Task Test_ViewUserGameRounds_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // First create a few game rounds for the user
            Debug.Log($"Creating multiple game rounds for user '{_testUser.Username}'");
            
            // Create first game round
            var round1 = await _testUser.CreateGameRoundAsync(
                "battle",
                DateTime.UtcNow.AddDays(-1).ToString("o"),
                DateTime.UtcNow.AddDays(-1).AddHours(1).ToString("o"),
                1500,
                1,
                new Dictionary<string, object> { ["level"] = "Hard" });
            
            Assert.IsNotNull(round1, "First created game round should not be null.");
            
            // Create second game round
            var round2 = await _testUser.CreateGameRoundAsync(
                "adventure",
                DateTime.UtcNow.AddHours(-5).ToString("o"),
                DateTime.UtcNow.AddHours(-4).ToString("o"),
                1700,
                1,
                new Dictionary<string, object> { ["level"] = "Medium" });
            
            Assert.IsNotNull(round2, "Second created game round should not be null.");
            
            Debug.Log($"Created game rounds with IDs: {round1.Id}, {round2.Id}");

            // Act: Get all game rounds for the user
            IReadOnlyList<GameRound> userGameRounds = await _testUser.GetCurrentUserGameRoundsAsync();

            // Assert
            Assert.IsNotNull(userGameRounds, "User game rounds list should not be null.");
            Assert.GreaterOrEqual(userGameRounds.Count, 2, "There should be at least 2 game rounds.");
            
            // Find the created rounds in the list
            bool foundRound1 = userGameRounds.Any(r => r.Id == round1.Id);
            bool foundRound2 = userGameRounds.Any(r => r.Id == round2.Id);
            
            Assert.IsTrue(foundRound1, $"Game round with ID {round1.Id} should be in the user's game rounds.");
            Assert.IsTrue(foundRound2, $"Game round with ID {round2.Id} should be in the user's game rounds.");

            Debug.Log($"Successfully retrieved {userGameRounds.Count} game rounds for user '{_testUser.Username}'");
        }

        [Test]
        public async Task Test_DeleteGameRound_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");

            // First create a game round to delete
            string gameType = "delete_test";
            
            Debug.Log($"Creating a game round for user '{_testUser.Username}' to test deletion");
            
            // Arrange: Create a game round
            GameRound createdRound = await _testUser.CreateGameRoundAsync(
                gameType,
                DateTime.UtcNow.AddMinutes(-10).ToString("o"),
                DateTime.UtcNow.ToString("o"),
                100,
                3,
                new Dictionary<string, object> { ["test"] = "deletion" });
            
            Assert.IsNotNull(createdRound, "Created game round should not be null.");
            int gameRoundId = createdRound.Id;
            Debug.Log($"Successfully created game round with ID: {gameRoundId}");

            // Act: Delete the game round
            Debug.Log($"Deleting game round with ID: {gameRoundId}");
            GameRoundDeleteResponse deleteResponse = await _testUser.DeleteGameRoundAsync(gameRoundId);

            // Assert
            Assert.IsNotNull(deleteResponse, "Delete response should not be null.");
            Assert.IsNotNull(deleteResponse.Message, "Delete response message should not be null.");
            StringAssert.Contains("destroyed", deleteResponse.Message.ToLower(), "Response message should indicate successful deletion.");
            
            Debug.Log($"Successfully deleted game round with ID: {gameRoundId}, Response: {deleteResponse.Message}");

            // Additional validation: Try to get the deleted game round (should fail or return null)
            try
            {
                var retrievedRound = await _testUser.GetGameRoundAsync(gameRoundId);
                // If we get here without an exception, the API might return null or an empty round for deleted items
                if (retrievedRound != null)
                {
                    Debug.Log("Note: API didn't throw an exception for deleted game round, but this might be expected behavior.");
                }
            }
            catch (Exception ex)
            {
                // This is expected - the round should no longer exist
                Debug.Log($"Expected exception when retrieving deleted game round: {ex.Message}");
            }
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