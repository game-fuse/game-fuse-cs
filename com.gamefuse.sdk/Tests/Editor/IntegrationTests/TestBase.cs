using GameFuse.Models.TestSuite;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFuse.Tests.Editor.IntegrationTests
{
    /// <summary>
    /// Base class for all GameFuse integration tests that use the TestSuiteService.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// The transport used for API communication.
        /// </summary>
        protected ITransport Transport { get; private set; }
        
        /// <summary>
        /// The test suite service used to create test games and users.
        /// </summary>
        protected TestSuiteService TestSuiteService { get; private set; }
        
        /// <summary>
        /// Admin service key token for test suite operations.
        /// </summary>
        protected string AdminToken { get; private set; }
        
        /// <summary>
        /// Admin service key name for test suite operations.
        /// </summary>
        protected string AdminName { get; private set; }
        
        /// <summary>
        /// ID of the test game created for each test.
        /// </summary>
        protected int TestGameId { get; private set; }
        
        /// <summary>
        /// API key for the test game.
        /// </summary>
        protected string TestGameToken { get; private set; }
        
        /// <summary>
        /// Response from creating the test game.
        /// </summary>
        protected CreateGameResponse TestGame { get; private set; }

        /// <summary>
        /// Base URL for API endpoints.
        /// </summary>
        protected const string ApiBaseUrl = "https://gamefuse.co/api/v3";

        [SetUp]
        public virtual async Task Setup()
        {
            // Load admin credentials from configuration
            LoadTestConfig();
            
            // Skip setup if credentials are missing
            if (string.IsNullOrEmpty(AdminToken) || string.IsNullOrEmpty(AdminName))
            {
                Assert.Inconclusive("Admin credentials not available. Skipping integration test.");
                return;
            }
            
            // Initialize transport and service
            Transport = new UnityWebRequestTransport(ApiBaseUrl, 3, 60);
            TestSuiteService = new TestSuiteService(Transport);
            
            // Create a test game for this test
            try
            {
                Debug.Log($"Creating test game with admin credentials. Token: {AdminToken.Substring(0, 5)}... Name: {AdminName}");
                TestGame = await TestSuiteService.CreateGameAsync(AdminToken, AdminName);
                
                Assert.IsNotNull(TestGame, "Failed to create test game");
                Assert.Greater(TestGame.Id, 0, "Test game ID should be positive");
                
                TestGameId = TestGame.Id;
                TestGameToken = TestGame.Token;
                
                Debug.Log($"Created test game: ID={TestGameId}, Name={TestGame.Name}, Token={TestGameToken.Substring(0, 5)}...");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create test game: {ex.Message}");
                Assert.Fail($"Test setup failed: {ex.Message}");
            }
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            // Skip cleanup if no game was created or credentials are missing
            if (TestGameId <= 0 || string.IsNullOrEmpty(AdminToken) || string.IsNullOrEmpty(AdminName))
            {
                return;
            }
            
            try
            {
                Debug.Log($"Cleaning up test game ID={TestGameId}");
                var cleanupResult = await TestSuiteService.CleanUpTestAsync(
                    TestGameId,
                    AdminToken,
                    AdminName
                );
                
                Assert.IsNotNull(cleanupResult, "Cleanup response should not be null");
                Assert.IsTrue(cleanupResult.Message.Contains("everything should have been destroyed!"), 
                    $"Unexpected cleanup message: {cleanupResult.Message}");
                
                Debug.Log($"Test resources cleaned up: {cleanupResult.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clean up test resources: {ex.Message}");
                // Don't fail the test if cleanup fails, just log the error
            }
            finally
            {
                // Reset test data
                TestGameId = 0;
                TestGameToken = null;
                TestGame = null;
            }
        }

        /// <summary>
        /// Creates a test user in the test game.
        /// </summary>
        /// <param name="usernamePrefix">Optional prefix for the username to make it more identifiable.</param>
        /// <returns>The created test user.</returns>
        protected async Task<CreateUserResponse> CreateTestUserAsync(string usernamePrefix = "tester")
        {
            // Generate a unique username and email
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string username = $"{usernamePrefix}_{uniqueSuffix}";
            string email = $"{username}@example.com";
            
            var testUser = await TestSuiteService.CreateUserAsync(
                TestGameId,
                username,
                email,
                AdminToken,
                AdminName
            );
            
            Assert.IsNotNull(testUser, "Created test user should not be null");
            Assert.Greater(testUser.Id, 0, "Test user ID should be positive");
            Assert.AreEqual(username, testUser.Username, "Test user should have the correct username");
            
            Debug.Log($"Created test user: ID={testUser.Id}, Username={testUser.Username}");
            
            return testUser;
        }

        // Model for test configuration
        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
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
    }
}