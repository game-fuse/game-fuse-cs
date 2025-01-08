using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using UnityEngine.TestTools;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;

namespace GameFuseCSharp.Tests.Runtime
{
    [TestFixture]
    public class GroupServiceTests
    {
        private ISystemAdminTestSuiteService _adminService;
        private IUserService _userService;
        private ISessionsService _sessionsService;
        private IGroupsService _groupsService;
        private string _adminToken;
        private string _adminName;
        private int _testGameId;
        private string _testGameToken;
        private SignInResponse _user;
        private UnityWebRequest _request;

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
            _adminService = new SystemAdminTestSuiteService("https://gamefuse.co/api/v3", _adminToken, _adminName);
            _userService = new UserService("https://gamefuse.co/api/v3");
            _sessionsService = new SessionsService("https://gamefuse.co/api/v3");

            // Create a test game
            Debug.Log("Creating test game...");
            var gameResponse = await _adminService.CreateGameAsync();
            _testGameId = gameResponse.id;
            _testGameToken = gameResponse.token;
            Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

            // Create and sign in a test user
            _user = await CreateAndSignInUser("testuser");

            // Initialize GroupsService with the authenticated user's token
            _groupsService = new GroupsService("https://gamefuse.co/api/v3", _user.authentication_token);
        }

        private async Task TearDownAsync()
        {
            if (_testGameId != 0)
            {
                Debug.Log($"Cleaning up test game with ID: {_testGameId}");
                await _adminService.CleanUpTestAsync(_testGameId);
            }
        }

        private async Task<SignInResponse> CreateAndSignInUser(string username)
        {
            string userEmail = $"{username}_{UnityEngine.Random.Range(1, 1001)}@example.com";
            string password = "testpassword123";

            SignUpRequest signUpRequest = new SignUpRequest
            {
                email = userEmail,
                password = password,
                password_confirmation = password,
                username = username,
                game_id = _testGameId,
                game_token = _testGameToken
            };

            await _userService.SignUpAsync(signUpRequest);

            SignInRequest signInRequest = new SignInRequest
            {
                email = userEmail,
                password = password,
                game_id = _testGameId,
                game_token = _testGameToken
            };

            return await _sessionsService.SignInAsync(signInRequest);
        }

        [Test]
        public async Task GetAllGroups_WithNoGroups_ReturnsEmptyList()
        {
            try
            {
                await SetUpAsync();

                // Act
                Debug.Log("Fetching all groups...");
                var response = await _groupsService.GetAllGroupsAsync();

                // Assert
                Assert.NotNull(response, "Response should not be null");
                Assert.NotNull(response.Groups, "Groups array should not be null");
                Assert.AreEqual(0, response.Groups.Length, "Groups array should be empty");

                Debug.Log("Successfully verified empty groups response");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }


        
        [Test]
        public async Task GroupService_CreateAGroup()
        {
            try
            {
                await SetUpAsync();
                //Create Group

                //Arrange
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Test Group",
                    MaxGroupSize = 20,
                    CanAutoJoin = true,
                    IsInviteOnly = false,
                    GroupType = "default",
                    Searchable = false,
                    AdminsOnlyCanCreateAttributes = true  // Let's include this as the JS version does
                };

                //Act
                var response = await _groupsService.CreateGroupAsync(createGroupRequest);

                //Assert
                Assert.NotNull(response, "Response should not be null");
                Assert.Greater(response.Id, 0, "Group ID should be greater than 0");
                Assert.AreEqual("Test Group", response.Name, "Group name should match");
                Assert.AreEqual(20, response.MaxGroupSize, "Max group size should match");
                Assert.AreEqual(true, response.CanAutoJoin, "CanAutoJoin should match");
                Assert.AreEqual(false, response.IsInviteOnly, "IsInviteOnly should match");
                Assert.AreEqual("default", response.GroupType, "GroupType should match");
                Assert.AreEqual(true, response.Searchable, "Searchable should match");
                Assert.AreEqual(1, response.MemberCount, "New group should have 1 member (creator)");

                // Verify creator is in members and admins
                Assert.NotNull(response.Members, "Members array should not be null");
                Assert.NotNull(response.Admins, "Admins array should not be null");
                Assert.AreEqual(1, response.Members.Length, "Should have one member");
                Assert.AreEqual(1, response.Admins.Length, "Should have one admin");
                Assert.AreEqual(_user.id, response.Members[0].Id, "Creator should be a member");
                Assert.AreEqual(_user.id, response.Admins[0].Id, "Creator should be an admin");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task CreateGroup_CreatorIsAutomaticallyAdmin()
        {
            try
            {
                await SetUpAsync();

                // Arrange
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Admin Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "test_group"
                };

                // Act
                var response = await _groupsService.CreateGroupAsync(createGroupRequest);

                // Assert
                Assert.NotNull(response, "Response should not be null");
                Assert.NotNull(response.Admins, "Admins array should not be null");
                Assert.Greater(response.Admins.Length, 0, "Group should have at least one admin");

                // Verify the creator is an admin
                var creatorAdmin = response.Admins.FirstOrDefault(admin => admin.Id == _user.id);
                Assert.NotNull(creatorAdmin, "Creator should be in the admins list");
                Assert.AreEqual(_user.username, creatorAdmin.Username, "Admin username should match creator's username");
                Assert.AreEqual(_user.email, creatorAdmin.Email, "Admin email should match creator's email");

                // Verify admin is also a member
                var creatorMember = response.Members.FirstOrDefault(member => member.Id == _user.id);
                Assert.NotNull(creatorMember, "Creator should also be in the members list");

                // Additional verification that creator is the only admin initially
                Assert.AreEqual(1, response.Admins.Length, "Initially there should be exactly one admin");
                Assert.AreEqual(_user.id, response.Admins[0].Id, "The sole admin should be the creator");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }


        [Test]
        public async Task GetAllGroups_WithMultipleGroups_ReturnsAllCreatedGroups()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create two groups with different properties
                var group1Request = new CreateGroupRequest
                {
                    Name = "Test Group 1",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false,
                    GroupType = "public"
                };

                var group2Request = new CreateGroupRequest
                {
                    Name = "Test Group 2",
                    MaxGroupSize = 20,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "private"
                };

                // Act - Create both groups
                var group1Response = await _groupsService.CreateGroupAsync(group1Request);
                var group2Response = await _groupsService.CreateGroupAsync(group2Request);

                // Get all groups
                var allGroupsResponse = await _groupsService.GetAllGroupsAsync();

                // Assert
                Assert.NotNull(allGroupsResponse, "Response should not be null");
                Assert.NotNull(allGroupsResponse.Groups, "Groups array should not be null");
                Assert.AreEqual(2, allGroupsResponse.Groups.Length, "Should have exactly two groups");

                // Find both created groups in the response
                var foundGroup1 = allGroupsResponse.Groups.FirstOrDefault(g => g.Id == group1Response.Id);
                var foundGroup2 = allGroupsResponse.Groups.FirstOrDefault(g => g.Id == group2Response.Id);

                // Verify first group
                Assert.NotNull(foundGroup1, "First group should be in the response");
                Assert.AreEqual("Test Group 1", foundGroup1.Name);
                Assert.AreEqual("public", foundGroup1.GroupType);
                Assert.IsTrue(foundGroup1.CanAutoJoin);
                Assert.IsFalse(foundGroup1.IsInviteOnly);
                Assert.AreEqual(1, foundGroup1.MemberCount);
                Assert.AreEqual(10, foundGroup1.MaxGroupSize);

                // Verify second group
                Assert.NotNull(foundGroup2, "Second group should be in the response");
                Assert.AreEqual("Test Group 2", foundGroup2.Name);
                Assert.AreEqual("private", foundGroup2.GroupType);
                Assert.IsFalse(foundGroup2.CanAutoJoin);
                Assert.IsTrue(foundGroup2.IsInviteOnly);
                Assert.AreEqual(1, foundGroup2.MemberCount);
                Assert.AreEqual(20, foundGroup2.MaxGroupSize);
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetAllGroups_WithFullData_ReturnsCompleteGroupInformation()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create two groups with different configurations
                var group1Request = new CreateGroupRequest
                {
                    Name = "Test Group 1",
                    MaxGroupSize = 20,
                    CanAutoJoin = true,
                    IsInviteOnly = false,
                    GroupType = "public",
                    Searchable = true,
                    AdminsOnlyCanCreateAttributes = true
                };

                var group2Request = new CreateGroupRequest
                {
                    Name = "Test Group 2",
                    MaxGroupSize = 10,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "private",
                    Searchable = false,
                    AdminsOnlyCanCreateAttributes = false
                };

                // Create both groups
                var group1Response = await _groupsService.CreateGroupAsync(group1Request);
                var group2Response = await _groupsService.CreateGroupAsync(group2Request);

                // Act - Get all groups with full data
                var allGroupsResponse = await _groupsService.GetAllGroupsAsync(withFullData: true);

                // Assert
                Assert.NotNull(allGroupsResponse, "Groups response should not be null");
                Assert.NotNull(allGroupsResponse.Groups, "Groups array should not be null");
                Assert.AreEqual(2, allGroupsResponse.Groups.Length, "Should have exactly two groups");

                // Helper function to find a group by name in the response
                GroupResponse FindGroup(string name) => allGroupsResponse.Groups.FirstOrDefault(g => g.Name == name);

                // Verify Group 1
                var group1 = FindGroup("Test Group 1");
                Assert.NotNull(group1, "Group 1 should be found in response");
                Debug.Log($"Group 1 Memebers Length {group1.Members.Length}");
                VerifyGroupResponse(group1, group1Request, _user);

                // Verify Group 2
                var group2 = FindGroup("Test Group 2");
                Assert.NotNull(group2, "Group 2 should be found in response");
                VerifyGroupResponse(group2, group2Request, _user);
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task GetGroupDetails_ReturnsCompleteGroupInformation()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group with specific configuration
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Detailed Test Group",
                    MaxGroupSize = 25,
                    CanAutoJoin = true,
                    IsInviteOnly = false,
                    GroupType = "public",
                    Searchable = true,
                    AdminsOnlyCanCreateAttributes = true
                };

                // Create the group first
                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");
                Assert.Greater(createdGroup.Id, 0, "Created group should have valid ID");

                // Act - Get the group details using GetGroupDetailsAsync
                var groupDetails = await _groupsService.GetGroupDetailsAsync(createdGroup.Id);

                // Assert - Use existing helper method to verify all group properties
                Assert.NotNull(groupDetails, "Group details should not be null");
                VerifyGroupResponse(groupDetails, createGroupRequest, _user);
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task SendGroupConnectionRequest_CreatesValidConnection_And_Accepts()
        {
            try
            {
                await SetUpAsync();

                // Arrange - First create a group and a second user
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Connection Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "private"
                };

                // Create the group
                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");
                Assert.Greater(createdGroup.Id, 0, "Created group should have valid ID");

                // Create a second user who will request to join
                var secondUser = await CreateAndSignInUser("joineruser");
                Assert.NotNull(secondUser, "Second user should be created successfully");

                // Create a new GroupsService instance with second user's token
                var secondUserGroupService = new GroupsService("https://gamefuse.co/api/v3", secondUser.authentication_token);

                // Create the connection request
                var connectionRequest = new GroupConnectionRequest
                {
                    GroupId = createdGroup.Id,
                    UserId = secondUser.id
                };

                // Act - Send the group connection request
                var connectionResponse = await secondUserGroupService.SendGroupConnectionRequestAsync(connectionRequest);

                // Assert
                Assert.NotNull(connectionResponse, "Connection response should not be null");
                Assert.Greater(connectionResponse.Id, 0, "Connection ID should be greater than 0");
                Assert.AreEqual("pending", connectionResponse.Status.ToLower(), "Initial connection status should be pending");
                Assert.AreEqual(secondUser.id, connectionResponse.User.Id, "User ID in response should match requesting user");

                // Verify the connection appears in the group's join requests
                var updatedGroup = await _groupsService.GetGroupDetailsAsync(createdGroup.Id);
                Assert.NotNull(updatedGroup.JoinRequests, "Group should have join requests array");
                Assert.Greater(updatedGroup.JoinRequests.Length, 0, "Group should have at least one join request");

                var joinRequest = updatedGroup.JoinRequests.FirstOrDefault(jr => jr.User.Id == secondUser.id);
                Assert.NotNull(joinRequest, "Join request from second user should exist");
                Assert.AreEqual("pending", joinRequest.Status.ToLower(), "Join request status should be pending");
                Assert.AreEqual(secondUser.username, joinRequest.User.Username, "Username in join request should match second user");

                // Accept the join request
                var acceptResponse = await _groupsService.AcceptGroupConnectionRequestAsync(joinRequest.Id);
                Assert.NotNull(acceptResponse, "Accept response should not be null");
                Assert.AreEqual(joinRequest.Id, acceptResponse.Id, "Connection ID should match the original request");
                Assert.AreEqual("accepted", acceptResponse.Status.ToLower(), "Status should be updated to accepted");

                // Verify the user is now a member of the group
                var groupAfterAcceptance = await _groupsService.GetGroupDetailsAsync(createdGroup.Id);
                Assert.NotNull(groupAfterAcceptance.Members, "Group should have members array");
                Assert.AreEqual(2, groupAfterAcceptance.Members.Length, "Group should now have two members");

                var newMember = groupAfterAcceptance.Members.FirstOrDefault(m => m.Id == secondUser.id);
                Assert.NotNull(newMember, "Second user should now be a member");
                Assert.AreEqual(secondUser.username, newMember.Username, "Username should match second user");

                // Verify join request is no longer in pending requests
                Assert.IsEmpty(groupAfterAcceptance.JoinRequests, "Join requests should be empty after acceptance");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task SendGroupConnectionRequest_DeclineRequest_VerifyDenied()
        {
            try
            {
                await SetUpAsync();

                // Arrange - First create a group and a second user
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Decline Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "private"
                };

                // Create the group
                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");
                Assert.Greater(createdGroup.Id, 0, "Created group should have valid ID");

                // Create a second user who will request to join
                var secondUser = await CreateAndSignInUser("declineduser");
                Assert.NotNull(secondUser, "Second user should be created successfully");

                // Create a new GroupsService instance with second user's token
                var secondUserGroupService = new GroupsService("https://gamefuse.co/api/v3", secondUser.authentication_token);

                // Create the connection request
                var connectionRequest = new GroupConnectionRequest
                {
                    GroupId = createdGroup.Id,
                    UserId = secondUser.id
                };

                // Act - Send the group connection request
                var connectionResponse = await secondUserGroupService.SendGroupConnectionRequestAsync(connectionRequest);

                // Assert initial request state
                Assert.NotNull(connectionResponse, "Connection response should not be null");
                Assert.Greater(connectionResponse.Id, 0, "Connection ID should be greater than 0");
                Assert.AreEqual("pending", connectionResponse.Status.ToLower(), "Initial connection status should be pending");

                // Verify the request appears in pending join requests
                var groupWithPendingRequest = await _groupsService.GetGroupDetailsAsync(createdGroup.Id);
                var joinRequest = groupWithPendingRequest.JoinRequests.FirstOrDefault(jr => jr.User.Id == secondUser.id);
                Assert.NotNull(joinRequest, "Join request from second user should exist");
                Assert.AreEqual("pending", joinRequest.Status.ToLower(), "Join request status should be pending");

                // Decline the join request
                var declineResponse = await _groupsService.DeclineGroupConnectionRequestAsync(joinRequest.Id);
                Assert.NotNull(declineResponse, "Decline response should not be null");
                Assert.AreEqual(joinRequest.Id, declineResponse.Id, "Connection ID should match the original request");
                Assert.AreEqual("declined", declineResponse.Status.ToLower(), "Status should be updated to declined");

                // Verify the user is NOT a member of the group
                var groupAfterDecline = await _groupsService.GetGroupDetailsAsync(createdGroup.Id);
                Assert.NotNull(groupAfterDecline.Members, "Group should have members array");
                Assert.AreEqual(1, groupAfterDecline.Members.Length, "Group should still have only one member (the creator)");

                var declinedUser = groupAfterDecline.Members.FirstOrDefault(m => m.Id == secondUser.id);
                Assert.IsNull(declinedUser, "Declined user should not be a member");

                // Verify join request is no longer in pending requests
                Assert.IsEmpty(groupAfterDecline.JoinRequests, "Join requests should be empty after decline");

                // Verify the only member is still the original creator
                Assert.AreEqual(_user.id, groupAfterDecline.Members[0].Id, "Only member should be the group creator");
                Assert.AreEqual(_user.username, groupAfterDecline.Members[0].Username, "Only member should be the group creator");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task AddGroupAttribute_AsAdmin_SuccessfullyAddsAttribute()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Admin Attribute Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "private",
                    AdminsOnlyCanCreateAttributes = true
                };

                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");

                // Create attribute request
                var attributeRequest = new GroupAttributeRequest
                {
                    Key = "test_key",
                    Value = "test_value",
                    OnlyCanEditByCreator = true
                };

                var attributesRequest = new GroupAttributesRequest
                {
                    Attributes = new GroupAttributeRequest[] { attributeRequest }
                };
                // Act - Add attribute as admin
                var attributeResponse = await _groupsService.AddGroupAttributesAsync(createdGroup.Id, attributesRequest);

                // Assert
                Assert.NotNull(attributeResponse, "Attribute response should not be null");
                Assert.NotNull(attributeResponse.Attributes, "Attributes array should not be null");
                Assert.AreEqual(1, attributeResponse.Attributes.Length, "Should have exactly one attribute");

                var addedAttribute = attributeResponse.Attributes[0];
                Assert.AreEqual("test_key", addedAttribute.Key, "Attribute key should match");
                Assert.AreEqual("test_value", addedAttribute.Value, "Attribute value should match");
                Assert.AreEqual(_user.id, addedAttribute.UserId, "Creator ID should match admin user");
                Assert.IsFalse(addedAttribute.OthersCanEdit, "Admin should be able to edit the attribute");

                // Verify attribute exists in group details
                var groupAttributes = await _groupsService.GetGroupAttributesAsync(createdGroup.Id);
                Assert.NotNull(groupAttributes.Attributes, "Group should have attributes");
                Assert.AreEqual(1, groupAttributes.Attributes.Length, "Group should have exactly one attribute");
                Assert.AreEqual("test_key", groupAttributes.Attributes[0].Key, "Attribute key should persist");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task AddGroupAttribute_AsNonAdmin_SucceedsWhenAllowed()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group where any user can create attributes
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "User Attribute Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false,
                    GroupType = "public",
                    AdminsOnlyCanCreateAttributes = false  // Allow all users to create attributes
                };

                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");

                // Create and sign in a second user
                var secondUser = await CreateAndSignInUser("attributetestuser");
                var secondUserGroupService = new GroupsService("https://gamefuse.co/api/v3", secondUser.authentication_token);

                // Send join request as second user
                var connectionRequest = new GroupConnectionRequest
                {
                    GroupId = createdGroup.Id,
                    UserId = secondUser.id
                };
                var connectionResponse = await secondUserGroupService.SendGroupConnectionRequestAsync(connectionRequest);

                // Admin accepts the join request
                await _groupsService.AcceptGroupConnectionRequestAsync(connectionResponse.Id);

                // Create attribute request
                var attributeRequest = new GroupAttributeRequest
                {
                    Key = "user_key",
                    Value = "user_value",
                    OnlyCanEditByCreator = true,
                };

                var attributesRequest = new GroupAttributesRequest
                {
                    Attributes = new GroupAttributeRequest[] { attributeRequest }
                };

                // Act - Add attribute as non-admin user
                var attributeResponse = await secondUserGroupService.AddGroupAttributesAsync(createdGroup.Id, attributesRequest);

                // Assert
                Assert.NotNull(attributeResponse, "Attribute response should not be null");
                Assert.NotNull(attributeResponse.Attributes, "Attributes array should not be null");
                Assert.AreEqual(1, attributeResponse.Attributes.Length, "Should have exactly one attribute");

                var addedAttribute = attributeResponse.Attributes[0];
                Assert.AreEqual("user_key", addedAttribute.Key, "Attribute key should match");
                Assert.AreEqual("user_value", addedAttribute.Value, "Attribute value should match");
                Assert.AreEqual(secondUser.id, addedAttribute.UserId, "Creator ID should match second user");
                Assert.IsFalse(addedAttribute.OthersCanEdit, "Creator should be able to edit their attribute");
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task AddGroupAttribute_AsNonAdmin_FailsWhenNotAllowed()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(".*401 Unauthorized.*"));
            try
            {
                await SetUpAsync();

                // Arrange - Create a group where only admins can create attributes
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Restricted Attribute Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false,
                    GroupType = "public",
                    AdminsOnlyCanCreateAttributes = true  // Only admins can create attributes
                };

                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");

                // Create and sign in a second user
                var secondUser = await CreateAndSignInUser("restricteduser");
                var secondUserGroupService = new GroupsService("https://gamefuse.co/api/v3", secondUser.authentication_token);

                // Send join request as second user
                var connectionRequest = new GroupConnectionRequest
                {
                    GroupId = createdGroup.Id,
                    UserId = secondUser.id
                };
                var connectionResponse = await secondUserGroupService.SendGroupConnectionRequestAsync(connectionRequest);

                // Admin accepts the join request
                await _groupsService.AcceptGroupConnectionRequestAsync(connectionResponse.Id);

                // Create attribute request
                var attributeRequest = new GroupAttributeRequest
                {
                    Key = "restricted_key",
                    Value = "restricted_value",
                    OnlyCanEditByCreator = true
                };

                var attributesRequest = new GroupAttributesRequest
                {
                    Attributes = new GroupAttributeRequest[] { attributeRequest }
                };

                // Act & Assert - Attempt to add attribute as non-admin user
                ApiException ex = null;
                try
                {
                    await secondUserGroupService.AddGroupAttributesAsync(createdGroup.Id, attributesRequest);
                    Assert.Fail("Expected ApiException was not thrown");
                }
                catch (ApiException apiEx)
                {
                    ex = apiEx;
                }

                Assert.NotNull(ex, "Should have thrown an ApiException");
                Assert.AreEqual(401, ex.StatusCode, "Should receive a 401 Unauthorized."); //403 Forbidden response

                // Verify no attributes were added
                var groupAttributes = await _groupsService.GetGroupAttributesAsync(createdGroup.Id);
                Assert.NotNull(groupAttributes, "Group attributes response should not be null");
                Assert.NotNull(groupAttributes.Attributes, "Attributes array should not be null");
                Assert.AreEqual(0, groupAttributes.Attributes.Length, "No attributes should have been added");
            }
            finally
            {
                await TearDownAsync();
            }
        }

        [Test]
        public async Task ModifyGroupAttribute_AsAdmin_SuccessfullyModifiesAttribute()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Attribute Modification Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = false,
                    IsInviteOnly = true,
                    GroupType = "private",
                    AdminsOnlyCanCreateAttributes = true
                };

                var createdGroup = await _groupsService.CreateGroupAsync(createGroupRequest);
                Assert.NotNull(createdGroup, "Created group should not be null");

                // Create initial attribute
                var initialAttributeRequest = new GroupAttributeRequest
                {
                    Key = "test_key",
                    Value = "initial_value",
                    OnlyCanEditByCreator = true
                };

                var attributesRequest = new GroupAttributesRequest
                {
                    Attributes = new GroupAttributeRequest[] { initialAttributeRequest }
                };

                // Add initial attribute as admin
                var initialAttributeResponse = await _groupsService.AddGroupAttributesAsync(createdGroup.Id, attributesRequest);
                Assert.NotNull(initialAttributeResponse, "Initial attribute response should not be null");
                Assert.NotNull(initialAttributeResponse.Attributes, "Initial attributes array should not be null");
                Assert.AreEqual(1, initialAttributeResponse.Attributes.Length, "Should have exactly one attribute");
                Assert.AreEqual("initial_value", initialAttributeResponse.Attributes[0].Value, "Initial value should be set");
                
                // Act - Modify the attribute
                var modifiedAttributeResponse = await _groupsService.ModifyGroupAttributeAsync(
                    createdGroup.Id,
                    "test_key",
                    "modified_value"
                );

                Assert.AreEqual("modified_value", modifiedAttributeResponse.Value, "The value was updated");
                
            }
            catch (ApiException ex)
            {
                Debug.LogError($"API Exception: Status Code: {ex.StatusCode}, Message: {ex.Message}");
                Debug.LogError($"Response Body: {ex.ResponseBody}");
                Assert.Fail($"API Exception: {ex.Message}");
            }
            finally
            {
                await TearDownAsync();
            }
        }


        // Helper method to verify all fields in a GroupResponse
        private void VerifyGroupResponse(GroupResponse group, CreateGroupRequest request, SignInResponse creator)
        {
            Assert.Greater(group.Id, 0, "Group ID should be greater than 0");
            Assert.AreEqual(request.Name, group.Name, "Group name should match");
            Assert.AreEqual(request.GroupType, group.GroupType, "Group type should match");
            Assert.AreEqual(request.CanAutoJoin, group.CanAutoJoin, "CanAutoJoin should match");
            Assert.AreEqual(request.IsInviteOnly, group.IsInviteOnly, "IsInviteOnly should match");
            Assert.AreEqual(request.MaxGroupSize, group.MaxGroupSize, "MaxGroupSize should match");
            Assert.AreEqual(request.Searchable ?? true, group.Searchable, "Searchable should match");
            Assert.AreEqual(1, group.MemberCount, "New group should have 1 member (creator)");

            // Verify Members array
            Assert.NotNull(group.Members, "Members array should not be null");
            Assert.AreEqual(1, group.Members.Length, "Should have exactly one member");
            var member = group.Members[0];
            Assert.AreEqual(creator.id, member.Id, "Member ID should match creator");
            Assert.AreEqual(creator.username, member.Username, "Member username should match creator");
            Assert.AreEqual(creator.email, member.Email, "Member email should match creator");

            // Verify Admins array
            Assert.NotNull(group.Admins, "Admins array should not be null");
            Assert.AreEqual(1, group.Admins.Length, "Should have exactly one admin");
            var admin = group.Admins[0];
            Assert.AreEqual(creator.id, admin.Id, "Admin ID should match creator");
            Assert.AreEqual(creator.username, admin.Username, "Admin username should match creator");
            Assert.AreEqual(creator.email, admin.Email, "Admin email should match creator");

            // Verify Join Requests and Invites arrays exist (they should be empty for new groups)
            Assert.NotNull(group.JoinRequests, "JoinRequests array should not be null");
            Assert.AreEqual(0, group.JoinRequests.Length, "New group should have no join requests");
            Assert.NotNull(group.Invites, "Invites array should not be null");
            Assert.AreEqual(0, group.Invites.Length, "New group should have no invites");
        }

    }
}