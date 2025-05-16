using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;

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
        private SignInResponse _testUser;

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
            _adminService = new SystemAdminTestSuiteService("https://gamefuse.co/api/v3", _adminToken,_adminName);
           

            Debug.Log("Creating test game...");
            var gameResponse = await _adminService.CreateGameAsync();
            _testGameId = gameResponse.Id;
            _testGameToken = gameResponse.Token;
            Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

            _userService = new UserService("https://gamefuse.co/api/v3");
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
                    Email = userEmail,
                    Password = password,
                    PasswordConfirmation = password,
                    Username = userName,
                    GameId = _testGameId,
                    GameToken = _testGameToken
                };

                Debug.Log($"SignUp Request: GameId: {request.GameId}, GameToken: {request.GameToken}");

                SignInResponse response = await _userService.SignUpAsync(request);
                _testUser = response;

                string responseJsonString = JsonUtility.ToJson(response, true);
                Debug.Log(responseJsonString);

                Assert.IsNotNull(response, "SignUp response is null");
                Assert.AreEqual(userName, response.Username, "Username mismatch");
                Assert.AreEqual(userEmail, response.Email, "Email mismatch");
                Assert.Greater(response.Id, 0, "User ID is not greater than 0");

                Debug.Log($"User successfully signed up. User ID: {response.Id}");
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
        
        [Test]
        public void UserService_HasAllRequiredUserEndpoints()
        {
            // This test verifies that all necessary methods are defined in the interface
            
            // Get the interface type
            var interfaceType = typeof(IUserService);
            
            // Get all methods from the interface
            var methods = interfaceType.GetMethods();
            var methodNames = methods.Select(m => m.Name).ToList();
            
            // Essential methods
            Assert.Contains("SignUpAsync", methodNames, "SignUpAsync method should exist");
            
            // Credits and Score methods
            Assert.Contains("AddCreditsAsync", methodNames, "AddCreditsAsync method should exist");
            Assert.Contains("SetCreditsAsync", methodNames, "SetCreditsAsync method should exist");
            Assert.Contains("AddScoreAsync", methodNames, "AddScoreAsync method should exist");
            Assert.Contains("SetScoreAsync", methodNames, "SetScoreAsync method should exist");
            
            // Attributes methods
            Assert.Contains("GetAttributesAsync", methodNames, "GetAttributesAsync method should exist");
            Assert.Contains("SetAttributeAsync", methodNames, "SetAttributeAsync method should exist");
            Assert.Contains("SetAttributesAsync", methodNames, "SetAttributesAsync method should exist");
            Assert.Contains("RemoveAttributeAsync", methodNames, "RemoveAttributeAsync method should exist");
            
            // Store items methods
            Assert.Contains("GetStoreItemsAsync", methodNames, "GetStoreItemsAsync method should exist");
            Assert.Contains("PurchaseStoreItemAsync", methodNames, "PurchaseStoreItemAsync method should exist");
            Assert.Contains("RemoveStoreItemAsync", methodNames, "RemoveStoreItemAsync method should exist");
        }
        
        [Test]
        public void GameFuseUserExtensions_HasAllRequiredExtensionMethods()
        {
            try
            {
                // Check for extension methods
                var extensionType = typeof(GameFuseUserExtensions);
                
                // Find methods with the GameFuseUser parameter (extension methods)
                var methods = extensionType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                var extensionMethods = new List<string>();
                
                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length > 0 && parameters[0].ParameterType == typeof(GameFuseUser))
                    {
                        extensionMethods.Add(method.Name);
                    }
                }
                
                Debug.Log($"Found extension methods: {string.Join(", ", extensionMethods)}");
                
                // Credits and Score methods
                Assert.Contains("AddCreditsAsync", extensionMethods, "AddCreditsAsync extension method should exist");
                Assert.Contains("SetCreditsAsync", extensionMethods, "SetCreditsAsync extension method should exist");
                Assert.Contains("AddScoreAsync", extensionMethods, "AddScoreAsync extension method should exist");
                Assert.Contains("SetScoreAsync", extensionMethods, "SetScoreAsync extension method should exist");
                
                // Attributes methods
                Assert.Contains("GetAttributesAsync", extensionMethods, "GetAttributesAsync extension method should exist");
                Assert.Contains("SetAttributeAsync", extensionMethods, "SetAttributeAsync extension method should exist");
                Assert.Contains("SetAttributesAsync", extensionMethods, "SetAttributesAsync extension method should exist");
                Assert.Contains("RemoveAttributeAsync", extensionMethods, "RemoveAttributeAsync extension method should exist");
                
                // Store items methods
                Assert.Contains("GetStoreItemsAsync", extensionMethods, "GetStoreItemsAsync extension method should exist");
                Assert.Contains("PurchaseStoreItemAsync", extensionMethods, "PurchaseStoreItemAsync extension method should exist");
                Assert.Contains("RemoveStoreItemAsync", extensionMethods, "RemoveStoreItemAsync extension method should exist");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Test failed with exception: {ex}");
                Assert.Fail($"Test failed: {ex.Message}");
            }
        }
    }
}