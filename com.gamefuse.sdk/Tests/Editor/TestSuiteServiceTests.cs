using GameFuse.Exceptions;
using GameFuse.Models.TestSuite;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFuse.Tests.Editor
{
    [TestFixture]
    public class TestSuiteServiceTests
    {
        private ITransport _transport;
        private TestSuiteService _testSuiteService;
        private string _adminToken;
        private string _adminName;
        private const string BASE_PATH = "test_suite";

        [SetUp]
        public void Setup()
        {
            // Load admin credentials from configuration
            LoadTestConfig();
            
            // For integration tests, we'll use the real transport with explicit URL and timeout
            string apiBaseUrl = "https://gamefuse.co/api/v3";
            Debug.Log($"Using API base URL: {apiBaseUrl}");
            
            // Initialize transport and service
            _transport = new UnityWebRequestTransport(apiBaseUrl, 3, 60);
            _testSuiteService = new TestSuiteService(_transport);
            
            // For unit tests, we'd use the MockTransport instead:
            // _transport = new MockTransport();
            // _testSuiteService = new TestSuiteService(_transport);
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
                _adminToken = config.adminToken;
                _adminName = config.adminName;
                Debug.Log($"admin name: {_adminName}, admin Toke: {_adminToken}");
            }
            else
            {
                Assert.Fail("Test configuration file not found. Create a testConfig.json file with adminToken and adminName properties.");
            }
        }

        [Test]
        public async Task TestSuite_Integration_FullLifecycle()
        {
            // Skip test if no admin credentials are available
            if (string.IsNullOrEmpty(_adminToken) || string.IsNullOrEmpty(_adminName))
            {
                Assert.Inconclusive("Admin credentials not available. Skipping integration test.");
                return;
            }
            
            Debug.Log($"Starting integration test with admin credentials. Token: {_adminToken.Substring(0, 5)}... Name: {_adminName}");
            Debug.Log($"Connecting to endpoint: {BASE_PATH}/create_game with Content-Type: application/json");

            // Variable to store the created game's ID
            int testGameId = 0;

            try
            {
                // 1. Create a test game
                var testGame = await _testSuiteService.CreateGameAsync(_adminToken, _adminName);
                
                Assert.IsNotNull(testGame);
                Assert.Greater(testGame.Id, 0);
                Assert.IsFalse(string.IsNullOrEmpty(testGame.Name));
                Assert.IsFalse(string.IsNullOrEmpty(testGame.Token));
                
                testGameId = testGame.Id;
                Debug.Log($"Created test game: ID={testGame.Id}, Name={testGame.Name}");

                // 2. Create a test user in the game
                string username = $"tester_{DateTime.UtcNow.Ticks}";
                string email = $"{username}@example.com";
                
                var testUser = await _testSuiteService.CreateUserAsync(
                    testGameId,
                    username,
                    email,
                    _adminToken,
                    _adminName
                );

                
                Assert.IsNotNull(testUser);
                Assert.Greater(testUser.Id, 0);
                Assert.AreEqual(username, testUser.Username);
               
                
                

                // 3. Verify the user was created successfully
                // This would typically involve signing in with the user or other operations
                // For this test, we're just checking that the user was created with the correct properties
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
                throw;
            }
            finally
            {
                // 4. Clean up test resources if a game was created
                if (testGameId > 0)
                {
                    try
                    {
                        var cleanupResult = await _testSuiteService.CleanUpTestAsync(
                            testGameId,
                            _adminToken,
                            _adminName
                        );
                        
                        Assert.IsNotNull(cleanupResult);
                        Assert.IsTrue(cleanupResult.Message.Contains("everything should have been destroyed!"));
                        
                        Debug.Log($"Test resources cleaned up: {cleanupResult.Message}");
                    }
                    catch (Exception cleanupEx)
                    {
                        Debug.LogError($"Failed to clean up test resources: {cleanupEx.Message}");
                    }
                }
            }
        }

        [Test]
        public async Task TestSuite_InvalidAdminCredentials_ReturnsUnauthorized()
        {
            // Arrange
            string invalidToken = "invalid_token";
            string invalidName = "invalid_name";

            try
            {
                await _testSuiteService.CreateGameAsync(invalidToken, invalidName);
                Assert.Fail("Expected GameFuseApiException was not thrown.");
            }
            catch (GameFuseApiException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
            }
        }
    }
}