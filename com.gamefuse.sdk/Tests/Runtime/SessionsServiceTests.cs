using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using GameFuseCSharp;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class SessionsServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private ISessionsService _sessionsService;
        private string _adminToken;
        private string _adminName;
        private int _testGameId;
        private string _testGameToken;
        private string _testUserEmail;
        private string _testUserPassword;

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
                _adminToken = config.adminToken;
                _adminName = config.adminName;
            }
            else
            {
                Debug.LogError($"Test configuration file not found at {configPath}. Please ensure the file exists and contains valid admin credentials.");
                Assert.Fail("Test configuration file not found.");
            }

            if (string.IsNullOrEmpty(_adminToken) || string.IsNullOrEmpty(_adminName))
            {
                Debug.LogError("Admin token or name is null or empty. Please check your testConfig.json file.");
                Assert.Fail("Admin credentials are invalid.");
            }

            Debug.Log($"Admin Name: {_adminName}, Admin Token: {_adminToken.Substring(0, 5)}...");
        }

        [SetUp]
        public void SetUp()
        {
            _adminService = new SystemAdminTestSuiteService("https://gamefuse.co/api/v3", _adminToken, _adminName);
            _userService = new UserService("https://gamefuse.co/api/v3");
            _sessionsService = new SessionsService("https://gamefuse.co/api/v3");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up will be done in the test method
        }

        [Test]
        public async Task SignInAsync_ReturnsValidResponse()
        {
            try
            {
                // Create a test game
                Debug.Log("Creating test game...");
                var gameResponse = await _adminService.CreateGameAsync();
                _testGameId = gameResponse.id;
                _testGameToken = gameResponse.token;
                Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

                // Sign up a test user
                string userName = $"testuser{UnityEngine.Random.Range(1, 1001)}";
                _testUserEmail = $"testuser{UnityEngine.Random.Range(1, 1001)}@example.com";
                _testUserPassword = "testpassword123";

                Debug.Log($"Signing up test user: {userName}, Email: {_testUserEmail}");

                SignUpRequest signUpRequest = new SignUpRequest
                {
                    email = _testUserEmail,
                    password = _testUserPassword,
                    password_confirmation = _testUserPassword,
                    username = userName,
                    game_id = _testGameId,
                    game_token = _testGameToken
                };

                await _userService.SignUpAsync(signUpRequest);

                // Test sign in
                Debug.Log($"Attempting to sign in user: {_testUserEmail}");

                SignInRequest signInRequest = new SignInRequest
                {
                    email = _testUserEmail,
                    password = _testUserPassword,
                    game_id = _testGameId,
                    game_token = _testGameToken
                };

                SignInResponse signInResponse = await _sessionsService.SignInAsync(signInRequest);

                Assert.IsNotNull(signInResponse, "SignIn response is null");
                Assert.AreEqual(_testUserEmail, signInResponse.display_email, "Email mismatch");
                Assert.IsNotEmpty(signInResponse.authentication_token, "Authentication token is empty");
                Assert.Greater(signInResponse.id, 0, "User ID is not greater than 0");

                Debug.Log($"User successfully signed in. User ID: {signInResponse.id}");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Test failed with exception: {ex}");
                Assert.Fail($"Test failed: {ex.Message}");
            }
            finally
            {
                // Clean up
                if (_testGameId != 0)
                {
                    Debug.Log($"Cleaning up test game with ID: {_testGameId}");
                    await _adminService.CleanUpTestAsync(_testGameId);
                }
            }
        }
    }
}
