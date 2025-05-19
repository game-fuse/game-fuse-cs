using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFuse.Tests.Editor.IntegrationTests
{
    [TestFixture]
    public class UserServiceIntegrationTests : TestBase
    {
        private AuthService _authService;
        private GameFuseUser _testUser;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            
            // Skip setup if base setup failed
            if (TestGameId <= 0)
            {
                return;
            }

            // Initialize the services
            var transport = new UnityWebRequestTransport();
            _authService = new AuthService(transport);

            // Create and sign in a test user for each test
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string email = $"tester_{uniqueSuffix}@example.com";
            string username = $"tester_{uniqueSuffix}";
            string password = "Password123!";
            
            var user = await _authService.SignUpAsync(
                email,
                password,
                username,
                TestGameId.ToString(),
                TestGameToken
            );
            Assert.IsNotNull(user, "Test user should be created successfully");

            _testUser = new GameFuseUser(user);
            Assert.Greater(_testUser.Id, 0, "User ID should be positive");
            Debug.Log($"Created test user for UserService tests: ID={_testUser.Id}, Username={_testUser.Username}");
        }
        
        [Test]
        public async Task Test_GetUser_Succeeds()
        {
            // Act
            var user = await _testUser.GetUserAsync();
            
            // Assert
            Assert.IsNotNull(user, "User should not be null");
            Assert.AreEqual(_testUser.Id, user.Id, "User IDs should match");
            Assert.AreEqual(_testUser.Username, user.Username, "Usernames should match");
            
            Debug.Log($"Successfully retrieved user: ID={user.Id}, Username={user.Username}");
        }
        
        [Test]
        public async Task Test_UpdateUser_Succeeds()
        {
            // Arrange
            string newUsername = $"updated_{_testUser.Username}";
            
            // Act
            var updatedUser = await _testUser.UpdateUserAsync(newUsername);
            
            // Assert
            Assert.IsNotNull(updatedUser, "Updated user should not be null");
            Assert.AreEqual(_testUser.Id, updatedUser.Id, "User IDs should match");
            Assert.AreEqual(newUsername, updatedUser.Username, "Username should be updated");
            
            Debug.Log($"Successfully updated user: ID={updatedUser.Id}, Username={updatedUser.Username}");
        }
        
        [Test]
        public async Task Test_UpdatePassword_Succeeds()
        {
            // Arrange
            string currentPassword = "Password123!";
            string newPassword = "NewPassword123!";
            
            // Act
            await _testUser.UpdatePasswordAsync(currentPassword, newPassword);
            
            // Assert - Try to sign in with the new password
            var signedInUser = await _authService.SignInAsync(
                _testUser.Email,
                newPassword,
                TestGameId.ToString(),
                TestGameToken
            );
            
            Assert.IsNotNull(signedInUser, "User should be able to sign in with new password");
            Assert.AreEqual(_testUser.Id, signedInUser.Id, "User IDs should match");
            
            Debug.Log($"Successfully updated password for user: ID={_testUser.Id}");
        }
        
        [Test]
        public async Task Test_SetUserAttribute_Succeeds()
        {
            // Arrange
            string key = $"test_key_{DateTime.UtcNow.Ticks}";
            string value = $"test_value_{DateTime.UtcNow.Ticks}";
            
            // Act
            var attribute = await _testUser.SetUserAttributeAsync(key, value);
            
            // Assert
            Assert.IsNotNull(attribute, "Attribute should not be null");
            Assert.Greater(attribute.Id, 0, "Attribute ID should be positive");
            Assert.AreEqual(key, attribute.Key, "Attribute key should match");
            Assert.AreEqual(value, attribute.Value, "Attribute value should match");
            
            Debug.Log($"Successfully set user attribute: ID={attribute.Id}, Key={attribute.Key}, Value={attribute.Value}");
        }
        
        [Test]
        public async Task Test_GetUserAttributes_Succeeds()
        {
            // Arrange - Set some attributes first
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
        
        [Test]
        public async Task Test_DeleteUserAttribute_Succeeds()
        {
            // Arrange - Set an attribute first
            string key = $"test_key_to_delete_{DateTime.UtcNow.Ticks}";
            string value = $"test_value_{DateTime.UtcNow.Ticks}";
            
            var attribute = await _testUser.SetUserAttributeAsync(key, value);
            Assert.IsNotNull(attribute, "Attribute should be created successfully");
            
            // Get all attributes to confirm our attribute exists
            var attributesBefore = await _testUser.GetUserAttributesAsync();
            var attrBefore = attributesBefore.FirstOrDefault(a => a.Key == key);
            Assert.IsNotNull(attrBefore, $"Attribute with key {key} should exist before deletion");
            
            // Act
            await _testUser.DeleteUserAttributeAsync(attribute.Id);
            
            // Assert - Verify the attribute is gone
            var attributesAfter = await _testUser.GetUserAttributesAsync();
            var attrAfter = attributesAfter.FirstOrDefault(a => a.Key == key);
            
            Assert.IsNull(attrAfter, $"Attribute with key {key} should not exist after deletion");
            Debug.Log($"Successfully deleted user attribute with key: {key}");
        }
        
        // Note: Unfortunately we can't directly test the score-related APIs in integration tests
        // since they require the user's authentication token to be set on the transport layer
        // However, the pattern for testing those would be similar to the attribute tests
    }
}