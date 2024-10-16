using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class SystemAdminTestSuiteServiceTests
    {
        private ISystemAdminTestSuiteService _service;
        private string _testToken;
        private string _testName;

        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Read configuration from JSON file
            string configPath = Path.Combine(Application.dataPath, "TestConfiguration", "testConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                TestConfig config = JsonUtility.FromJson<TestConfig>(json);
                _testToken = config.adminToken;
                _testName = config.adminName;
            }
            else
            {
                Debug.LogWarning($"Test configuration file not found at {configPath}. Using default values.");
                _testToken = "default_token";
                _testName = "default_name";
            }
        }

        [SetUp]
        public void Setup()
        {
            // Use the mock service by default
            //_service = new MockSystemAdminTestSuiteService();

            // To use the real service, comment out the line above and uncomment the line below
            _service = new SystemAdminTestSuiteService();

            // Initialize the service with values from config
            _service.Initialize("https://gamefuse.co/api/v3", _testToken);
            _service.SetServiceKeyName(_testName);
        }

        [Test]
        public async Task CreateGameAsync_ReturnsValidResponse()
        {
            var response = await _service.CreateGameAsync();
            Assert.IsNotNull(response);
            // Clean up
            var cleanupResponse = await _service.CleanUpTestAsync(response.id);
            Assert.AreEqual("everything should have been destroyed!", cleanupResponse.message);            
        }

        [Test]
        public async Task CreateUserAsync_ReturnsValidResponse()
        {
            CreateGameResponse gameResponse = await _service.CreateGameAsync();
            string userName = $"dave{UnityEngine.Random.Range(1, 1001)}";
            string userEmail = $"dave{UnityEngine.Random.Range(1, 1001)}@email.com";
            CreateUserResponse userResponse = await _service.CreateUserAsync(gameResponse.id, userName, userEmail);
            Assert.NotNull(userResponse);
            Assert.AreEqual(userEmail, userResponse.display_email);
            //Clean up
            CleanUpResponse cleanupResponse = await _service.CleanUpTestAsync(gameResponse.id);
            Assert.AreEqual("everything should have been destroyed!", cleanupResponse.message);
        }

    }
}
