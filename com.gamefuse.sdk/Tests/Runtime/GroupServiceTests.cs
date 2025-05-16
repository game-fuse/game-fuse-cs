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
            _testGameId = gameResponse.Id;
            _testGameToken = gameResponse.Token;
            Debug.Log($"Test game created. ID: {_testGameId}, Token: {_testGameToken}");

            // Create and sign in a test user
            _user = await CreateAndSignInUser("testuser");

            // Initialize GroupsService with the authenticated user's token
            _groupsService = new GroupsService("https://gamefuse.co/api/v3", _user.AuthenticationToken);
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
                Email = userEmail,
                Password = password,
                PasswordConfirmation = password,
                Username = username,
                GameId = _testGameId,
                GameToken = _testGameToken
            };

            await _userService.SignUpAsync(signUpRequest);

            SignInRequest signInRequest = new SignInRequest
            {
                Email = userEmail,
                Password = password,
                GameId = _testGameId,
                GameToken = _testGameToken
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
                Assert.AreEqual(_user.Id, response.Members[0].Id, "Creator should be a member");
                Assert.AreEqual(_user.Id, response.Admins[0].Id, "Creator should be an admin");
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
                var creatorAdmin = response.Admins.FirstOrDefault(admin => admin.Id == _user.Id);
                Assert.NotNull(creatorAdmin, "Creator should be in the admins list");
                Assert.AreEqual(_user.Username, creatorAdmin.Username, "Admin username should match creator's username");
                Assert.AreEqual(_user.Email, creatorAdmin.Email, "Admin email should match creator's email");

                // Verify admin is also a member
                var creatorMember = response.Members.FirstOrDefault(member => member.Id == _user.Id);
                Assert.NotNull(creatorMember, "Creator should also be in the members list");

                // Additional verification that creator is the only admin initially
                Assert.AreEqual(1, response.Admins.Length, "Initially there should be exactly one admin");
                Assert.AreEqual(_user.Id, response.Admins[0].Id, "The sole admin should be the creator");
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
        public async Task RemoveGroupMember_RemovesUserFromGroup()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group and a second user
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Member Removal Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false
                };

                // Create group
                var group = await _groupsService.CreateGroupAsync(createGroupRequest);
                int groupId = group.Id;

                // Create and sign in a second user
                var secondUser = await CreateAndSignInUser("seconduser");

                // Initialize a service for the second user
                var secondUserGroupsService = new GroupsService("https://gamefuse.co/api/v3", secondUser.AuthenticationToken);

                // Second user joins the group
                var joinRequest = new GroupConnectionRequest
                {
                    GroupId = groupId,
                    UserId = secondUser.Id
                };
                
                await secondUserGroupsService.SendGroupConnectionRequestAsync(joinRequest);
                
                // Verify second user is now in the group
                var groupDetails = await _groupsService.GetGroupDetailsAsync(groupId);
                Assert.IsTrue(groupDetails.Members.Any(m => m.Id == secondUser.Id), "Second user should be a member before removal");
                Assert.AreEqual(2, groupDetails.MemberCount, "Group should have 2 members");

                // Act - Admin removes the second user
                var removeResponse = await _groupsService.RemoveGroupMemberAsync(groupId, secondUser.Id);

                // Assert - Verify the removal was successful
                Assert.NotNull(removeResponse, "Remove response should not be null");
                Assert.NotNull(removeResponse.Group, "Group in response should not be null");
                
                // Verify second user is no longer in the group
                groupDetails = await _groupsService.GetGroupDetailsAsync(groupId);
                Assert.IsFalse(groupDetails.Members.Any(m => m.Id == secondUser.Id), "Second user should not be a member after removal");
                Assert.AreEqual(1, groupDetails.MemberCount, "Group should have 1 member left");
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
        public async Task PromoteToGroupAdmin_MakesMemberAnAdmin()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group and a second user
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Admin Promotion Test Group",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false
                };

                // Create group
                var group = await _groupsService.CreateGroupAsync(createGroupRequest);
                int groupId = group.Id;

                // Create and sign in a second user
                var secondUser = await CreateAndSignInUser("seconduser");

                // Initialize a service for the second user
                var secondUserGroupsService = new GroupsService("https://gamefuse.co/api/v3", secondUser.AuthenticationToken);

                // Second user joins the group
                var joinRequest = new GroupConnectionRequest
                {
                    GroupId = groupId,
                    UserId = secondUser.Id
                };
                
                await secondUserGroupsService.SendGroupConnectionRequestAsync(joinRequest);
                
                // Verify second user is a member but not admin
                var groupDetails = await _groupsService.GetGroupDetailsAsync(groupId);
                Assert.IsTrue(groupDetails.Members.Any(m => m.Id == secondUser.Id), "Second user should be a member");
                Assert.IsFalse(groupDetails.Admins.Any(a => a.Id == secondUser.Id), "Second user should not be an admin yet");

                // Act - Promote second user to admin
                var promoteResponse = await _groupsService.PromoteToGroupAdminAsync(groupId, secondUser.Id);

                // Assert - Verify the promotion was successful
                Assert.NotNull(promoteResponse, "Promote response should not be null");
                Assert.NotNull(promoteResponse.Group, "Group in response should not be null");
                
                // Verify second user is now an admin
                groupDetails = await _groupsService.GetGroupDetailsAsync(groupId);
                Assert.IsTrue(groupDetails.Admins.Any(a => a.Id == secondUser.Id), "Second user should now be an admin");
                Assert.IsTrue(groupDetails.Members.Any(m => m.Id == secondUser.Id), "Second user should still be a member");
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
        public async Task LeaveGroup_RemovesCurrentUserFromGroup()
        {
            try
            {
                await SetUpAsync();

                // Arrange - Create a group with our first user as admin
                var createGroupRequest = new CreateGroupRequest
                {
                    Name = "Leave Group Test",
                    MaxGroupSize = 10,
                    CanAutoJoin = true,
                    IsInviteOnly = false
                };

                // Create group
                var group = await _groupsService.CreateGroupAsync(createGroupRequest);
                int groupId = group.Id;

                // Create and sign in a second user
                var secondUser = await CreateAndSignInUser("seconduser");

                // Initialize a service for the second user
                var secondUserGroupsService = new GroupsService("https://gamefuse.co/api/v3", secondUser.AuthenticationToken);

                // Second user joins the group
                var joinRequest = new GroupConnectionRequest
                {
                    GroupId = groupId,
                    UserId = secondUser.Id
                };
                
                await secondUserGroupsService.SendGroupConnectionRequestAsync(joinRequest);
                
                // Verify both users are members
                var groupDetails = await _groupsService.GetGroupDetailsAsync(groupId);
                Assert.IsTrue(groupDetails.Members.Any(m => m.Id == _user.Id), "First user should be a member");
                Assert.IsTrue(groupDetails.Members.Any(m => m.Id == secondUser.Id), "Second user should be a member");
                Assert.AreEqual(2, groupDetails.MemberCount, "Group should have 2 members");

                // Act - Second user leaves the group
                var leaveResponse = await secondUserGroupsService.LeaveGroupAsync(groupId);

                // Assert - Verify the user left successfully
                Assert.NotNull(leaveResponse, "Leave response should not be null");
                Assert.NotNull(leaveResponse.Group, "Group in response should not be null");
                
                // Verify second user is no longer in the group
                groupDetails = await _groupsService.GetGroupDetailsAsync(groupId);
                Assert.IsFalse(groupDetails.Members.Any(m => m.Id == secondUser.Id), "Second user should not be a member after leaving");
                Assert.AreEqual(1, groupDetails.MemberCount, "Group should have 1 member left");
                Assert.IsTrue(groupDetails.Members.Any(m => m.Id == _user.Id), "First user should still be a member");
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
    }
}