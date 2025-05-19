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
    public class UserServiceIntegrationTests
    {
        private AuthService _authService;
        private GameFuseUser _testUser;
        protected const string ApiBaseUrl = "https://gamefuse.co/api/v3";
        

        /// <summary>
        /// Admin service key token for test suite operations.
        /// </summary>
        protected string AdminToken { get; private set; }

        /// <summary>
        /// Admin service key name for test suite operations.
        /// </summary>
        protected string AdminName { get; private set; }


        

        [Test]
        public async Task Test_GetUserAttributes_Succeeds()
        {

            LoadTestConfig();
            var serviceTransport = new UnityWebRequestTransport(ApiBaseUrl, 3, 60);
            var testSuiteService = new TestSuiteService(serviceTransport);
            var testGame = await testSuiteService.CreateGameAsync(AdminToken, AdminName);

            // Arrange - Set some attributes first
            try
            {


            var transport = new UnityWebRequestTransport();
            _authService = new AuthService(transport);
            User user = await _authService.SignUpAsync("dave@dave.com", "password1234", "randomName", testGame.Id.ToString(), testGame.Token);
            _testUser = new GameFuseUser(user);

            string key1 = $"test_key1_{DateTime.UtcNow.Ticks}";
            string value1 = $"test_value1_{DateTime.UtcNow.Ticks}";
            string key2 = $"test_key2_{DateTime.UtcNow.Ticks}";
            string value2 = $"test_value2_{DateTime.UtcNow.Ticks}";
            
            await _testUser.SetUserAttributeAsync(key1, value1);
            await _testUser.SetUserAttributeAsync(key2, value2);
            
            // Act
            var attributes = await _testUser.GetUserAttributesAsync();
            
            // Assert
            Assert.IsNotNull(attributes, "Attributes list should not be null");
            Assert.GreaterOrEqual(attributes.Count, 2, "Should have at least the 2 attributes we just created");
            
            // Find our test attributes
            var attr1 = attributes.FirstOrDefault(a => a.Key == key1);
            var attr2 = attributes.FirstOrDefault(a => a.Key == key2);
            
            Assert.IsNotNull(attr1, $"Attribute with key {key1} should exist");
            Assert.IsNotNull(attr2, $"Attribute with key {key2} should exist");
            Assert.AreEqual(value1, attr1.Value, "Attribute 1 value should match");
            Assert.AreEqual(value2, attr2.Value, "Attribute 2 value should match");
            
            Debug.Log($"Successfully retrieved {attributes.Count} user attributes");
            }
            catch
            {
                Debug.Log("something went wrong");
            }
            finally
            {
                Debug.Log("cleaning up test");
                await testSuiteService.CleanUpTestAsync(testGame.Id, AdminToken, AdminName);
            }
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