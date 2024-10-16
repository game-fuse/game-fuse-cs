using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class UserServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private string _adminToken;
        private string _adminName;
        private int _testGameId;
        private string _testGameToken;

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

        private async Task SetUpAsync()
        {
            _adminService = new SystemAdminTestSuiteService();
            _adminService.Initialize("https://gamefuse.co/api/v3", _adminToken);
            _adminService.SetServiceKeyName(_adminName);

            Debug.Log("Creating test game...");
            var gameResponse = await _adminService.CreateGameAsync();
            _testGameId = gameResponse.id;
            _testGameToken = gameResponse.token;
            Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

            _userService = new UserService();
            _userService.Initialize("https://gamefuse.co/api/v2", _testGameToken);
        }

        private async Task TearDownAsync()
        {
            if (_testGameId != 0)
            {
                Debug.Log($"Cleaning up test game with ID: {_testGameId}");
                await _adminService.CleanUpTestAsync(_testGameId);
            }
        }

        [Test]
        public async Task SignUpAsync_ReturnsValidResponse()
        {
            try
            {
                await SetUpAsync();

                string userName = $"testuser{UnityEngine.Random.Range(1, 1001)}";
                string userEmail = $"testuser{UnityEngine.Random.Range(1, 1001)}@example.com";
                string password = "testpassword123";

                Debug.Log($"Attempting to sign up user: {userName}, Email: {userEmail}");

                SignUpRequest request = new SignUpRequest
                {
                    email = userEmail,
                    password = "123456",
                    password_confirmation = "123456",
                    username = userName,
                    game_id = _testGameId,
                    game_token = _testGameToken
                };

                Debug.Log($"SignUp Request: GameId: {request.game_id}, GameToken: {request.game_token}");

                SignUpResponse response = await _userService.SignUpAsync(request);

                Assert.IsNotNull(response, "SignUp response is null");
                Assert.AreEqual(userName, response.username, "Username mismatch");
                Assert.AreEqual(userEmail, response.display_email, "Email mismatch");
                Assert.IsNotEmpty(response.authentication_token, "Authentication token is empty");
                Assert.Greater(response.id, 0, "User ID is not greater than 0");

                Debug.Log($"User successfully signed up. User ID: {response.id}");
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
                await TearDownAsync();
            }
        }
    }
}