// GameFuseUserGroupIntegrationTests.cs
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
    public class GameFuseUserGroupIntegrationTests
    {
        private GameFuseUser _testUser;
        private GameFuseUser _groupAdminUser;
        private TestSuiteService _testSuiteService;
        private CreateGameResponse _testGame;

        protected const string ApiBaseUrl = "https://gamefuse.co/api/v3";
        protected string AdminToken { get; private set; }
        protected string AdminName { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LoadTestConfig();
            var serviceTransport = new UnityWebRequestTransport(ApiBaseUrl, 3, 60);
            _testSuiteService = new TestSuiteService(serviceTransport);
        }

        [SetUp]
        public async Task SetupForEachTest()
        {
            _testGame = await _testSuiteService.CreateGameAsync(AdminToken, AdminName);
            Assert.IsNotNull(_testGame, "Test game creation failed.");

            string userEmail = $"groupuser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string userName = $"GroupUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _testUser = await GameFuseUser.SignUpAsync(userEmail, "password1234", userName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_testUser, "Test user sign up failed for group tests.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "User should be authenticated.");

            // User to create groups (admin)
            string adminEmail = $"groupadmin_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string adminName = $"GroupAdmin_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _groupAdminUser = await GameFuseUser.SignUpAsync(adminEmail, "password1234", adminName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_groupAdminUser, "Group admin user sign up failed.");
        }

        [TearDown]
        public async Task TearDownForEachTest()
        {
            // Clean up groups created by _testUser? API might not support group deletion by user yet.
            // TestSuiteService cleanup should remove the game and associated data.
            _testUser?.SignOut();
            if (_testGame != null && _testGame.Id > 0)
            {
                await _testSuiteService.CleanUpTestAsync(_testGame.Id, AdminToken, AdminName);
            }
            GameFuseUser.SignOut();
        }

        [Test]
        public async Task Test_CreateGroup_Succeeds_WithRequiredName_And_MaxGroupSize()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            string groupName = $"TestGroup_{Guid.NewGuid().ToString("N").Substring(0, 10)}";

            Debug.Log($"'{_testUser.Username}' attempting to create group: '{groupName}'");

            // Act
            var payload = new CreateGroupPayload { 
                Name = groupName,
                MaxGroupSize = 10,
            };
            Group createdGroup = await _testUser.CreateGroupAsync(payload);

            // Assert
            Assert.IsNotNull(createdGroup, "CreateGroup response (Group object) should not be null.");
            Assert.IsTrue(createdGroup.Id > 0, "Created group ID should be positive.");
            Assert.AreEqual(groupName, createdGroup.Name, "Group name mismatch.");

            // Verify creator is an admin and member
            Assert.IsNotNull(createdGroup.Admins, "Admins list should not be null.");
            Assert.IsTrue(createdGroup.Admins.Any(a => a.Id == _testUser.Id && a.Username == _testUser.Username), "Creator not found in admins list.");

            Assert.IsNotNull(createdGroup.Members, "Members list should not be null.");
            Assert.IsTrue(createdGroup.Members.Any(m => m.Id == _testUser.Id && m.Username == _testUser.Username), "Creator not found in members list.");
            Assert.AreEqual(1, createdGroup.MemberCount, "Member count should be 1 after creation.");

            Debug.Log($"Group '{createdGroup.Name}' (ID: {createdGroup.Id}) created successfully by '{_testUser.Username}'.");
        }

        [Test]
        public async Task Test_CreateGroup_Succeeds_WithAllOptionalParams()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            string groupName = $"FullParamsGroup_{Guid.NewGuid().ToString("N").Substring(0, 10)}";

            var payload = new CreateGroupPayload
            {
                Name = groupName,
                GroupType = "Private_Test",
                MaxGroupSize = 10,
                CanAutoJoin = false,
                IsInviteOnly = true,
                Searchable = false,
                AdminsOnlyCanCreateAttributes = true
            };

            Debug.Log($"'{_testUser.Username}' attempting to create group with all params: '{groupName}'");
            Group createdGroup = await _testUser.CreateGroupAsync(payload);

            Assert.IsNotNull(createdGroup, "CreateGroup response should not be null.");
            Assert.IsTrue(createdGroup.Id > 0, "Group ID invalid.");
            Assert.AreEqual(groupName, createdGroup.Name);
            Assert.AreEqual(payload.GroupType, createdGroup.GroupType);
            Assert.AreEqual(payload.MaxGroupSize, createdGroup.MaxGroupSize);
            Assert.AreEqual(payload.CanAutoJoin, createdGroup.CanAutoJoin);
            Assert.AreEqual(payload.IsInviteOnly, createdGroup.IsInviteOnly);
            //Assert.AreEqual(payload.Searchable, createdGroup.Searchable);
            Assert.AreEqual(payload.AdminsOnlyCanCreateAttributes, createdGroup.AdminsOnlyCanCreateAttributes);

            Assert.IsTrue(createdGroup.Admins.Any(a => a.Id == _testUser.Id), "Creator not admin.");
            Assert.IsTrue(createdGroup.Members.Any(m => m.Id == _testUser.Id), "Creator not member.");
            Assert.AreEqual(1, createdGroup.MemberCount);

            Debug.Log($"Group '{createdGroup.Name}' (ID: {createdGroup.Id}) created successfully with all params.");
        }

        [Test]
        public async Task Test_FetchAllGroups_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "User should be authenticated to fetch groups.");

            // Arrange: Create a couple of groups to ensure the list isn't empty
            // (unless the test environment guarantees existing groups).
            string groupName1 = $"FetchedGroup1_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            string groupName2 = $"FetchedGroup2_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            Group createdGroup1 = await _testUser.CreateGroupAsync(new CreateGroupPayload { Name = groupName1, MaxGroupSize = 10, Searchable = true });
            Assert.IsNotNull(createdGroup1, "Failed to create first group for fetching.");
            Group createdGroup2 = await _testUser.CreateGroupAsync(new CreateGroupPayload { Name = groupName2, MaxGroupSize = 10, Searchable = true });
            Assert.IsNotNull(createdGroup2, "Failed to create second group for fetching.");
            Debug.Log($"Created groups: '{groupName1}' (ID: {createdGroup1.Id}), '{groupName2}' (ID: {createdGroup2.Id})");

            // Act
            Debug.Log($"'{_testUser.Username}' attempting to fetch all groups.");
            IReadOnlyList<GroupSummary> allGroups = await _testUser.FetchAllGroupsAsync();

            // Assert
            Assert.IsNotNull(allGroups, "FetchAllGroups response (list of GroupSummary) should not be null.");
            Assert.IsTrue(allGroups.Count >= 2, $"Expected at least 2 groups, but found {allGroups.Count}. Check if other tests interfere or if creation failed silently.");

            var fetchedGroup1 = allGroups.FirstOrDefault(g => g.Id == createdGroup1.Id);
            Assert.IsNotNull(fetchedGroup1, $"Created group '{groupName1}' not found in fetched list.");
            Assert.AreEqual(groupName1, fetchedGroup1.Name, "Name mismatch for fetchedGroup1.");
            Assert.AreEqual(createdGroup1.MemberCount, fetchedGroup1.MemberCount, "Member count mismatch for fetchedGroup1.");
            // Assert other summary properties if needed

            var fetchedGroup2 = allGroups.FirstOrDefault(g => g.Id == createdGroup2.Id);
            Assert.IsNotNull(fetchedGroup2, $"Created group '{groupName2}' not found in fetched list.");
            Assert.AreEqual(groupName2, fetchedGroup2.Name, "Name mismatch for fetchedGroup2.");

            Debug.Log($"Successfully fetched {allGroups.Count} groups. Verified created groups are present.");
        }

        [Test]
        public async Task Test_FetchGroupDetails_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "User should be authenticated to fetch group details.");

            // Arrange: Create a group first to get its ID and details
            string groupName = $"DetailedGroup_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
            var createPayload = new CreateGroupPayload
            {
                Name = groupName,
                GroupType = "Test_Detail_Type",
                MaxGroupSize = 25, // Providing MaxGroupSize as you noted it's required
                Searchable = true,
                CanAutoJoin = false,
                IsInviteOnly = true,
                AdminsOnlyCanCreateAttributes = false
            };
            Group createdGroup = await _testUser.CreateGroupAsync(createPayload);
            Assert.IsNotNull(createdGroup, "Failed to create group for detail fetching.");
            Assert.IsTrue(createdGroup.Id > 0, "Created group has an invalid ID.");
            int groupIdToFetch = createdGroup.Id;
            Debug.Log($"Created group '{groupName}' (ID: {groupIdToFetch}) for detail fetching by '{_testUser.Username}'.");

            // Act
            Group fetchedGroupDetails = await _testUser.FetchGroupDetailsAsync(groupIdToFetch);

            // Assert
            Assert.IsNotNull(fetchedGroupDetails, "Fetched group details (Group object) should not be null.");
            Assert.AreEqual(createdGroup.Id, fetchedGroupDetails.Id, "ID mismatch.");
            Assert.AreEqual(createdGroup.Name, fetchedGroupDetails.Name, "Name mismatch.");
            Assert.AreEqual(createdGroup.GroupType, fetchedGroupDetails.GroupType, "GroupType mismatch.");
            Assert.AreEqual(createdGroup.MaxGroupSize, fetchedGroupDetails.MaxGroupSize, "MaxGroupSize mismatch.");
            Assert.AreEqual(createdGroup.Searchable, fetchedGroupDetails.Searchable, "Searchable mismatch.");
            Assert.AreEqual(createdGroup.CanAutoJoin, fetchedGroupDetails.CanAutoJoin, "CanAutoJoin mismatch.");
            Assert.AreEqual(createdGroup.IsInviteOnly, fetchedGroupDetails.IsInviteOnly, "IsInviteOnly mismatch.");
            Assert.AreEqual(createdGroup.AdminsOnlyCanCreateAttributes, fetchedGroupDetails.AdminsOnlyCanCreateAttributes, "AdminsOnlyCanCreateAttributes mismatch.");


            Assert.IsNotNull(fetchedGroupDetails.Members, "Fetched Members list should not be null.");
            Assert.IsTrue(fetchedGroupDetails.Members.Any(m => m.Id == _testUser.Id && m.Username == _testUser.Username), "Creator not found in fetched members list.");

            Assert.IsNotNull(fetchedGroupDetails.Admins, "Fetched Admins list should not be null.");
            Assert.IsTrue(fetchedGroupDetails.Admins.Any(a => a.Id == _testUser.Id && a.Username == _testUser.Username), "Creator not found in fetched admins list.");

            Assert.AreEqual(createdGroup.MemberCount, fetchedGroupDetails.MemberCount, "MemberCount mismatch.");
            Assert.AreEqual(1, fetchedGroupDetails.MemberCount, "Fetched group should have 1 member (the creator).");

            // The API doc says join_requests and invites are "if available".
            // For a newly created group by a single user, these should likely be empty.
            Assert.IsNotNull(fetchedGroupDetails.JoinRequests, "Fetched JoinRequests list should not be null (can be empty).");
            Assert.IsEmpty(fetchedGroupDetails.JoinRequests, "JoinRequests should be empty for a new group.");

            Assert.IsNotNull(fetchedGroupDetails.Invites, "Fetched Invites list should not be null (can be empty).");
            Assert.IsEmpty(fetchedGroupDetails.Invites, "Invites should be empty for a new group.");

            Debug.Log($"Successfully fetched and verified details for group ID: {fetchedGroupDetails.Id}, Name: '{fetchedGroupDetails.Name}'.");
        }

        [Test]
        public async Task Test_SendGroupConnectionRequest_AutoJoinGroup_SucceedsAndJoins()
        {
            Assert.IsNotNull(_groupAdminUser, "_groupAdminUser was not initialized.");
            Assert.IsNotNull(_testUser, "_testUser (joiner) was not initialized.");

            // Arrange: Admin creates an auto-join group
            string groupName = $"AutoJoinGroup_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
            var createPayload = new CreateGroupPayload
            {
                Name = groupName,
                MaxGroupSize = 10, // Required
                CanAutoJoin = true,
                IsInviteOnly = false
            };
            Group autoJoinGroup = await _groupAdminUser.CreateGroupAsync(createPayload); // Admin creates
            Assert.IsNotNull(autoJoinGroup, "Failed to create auto-join group.");
            Assert.IsTrue(autoJoinGroup.CanAutoJoin, "Group should be auto-join.");
            Debug.Log($"Admin '{_groupAdminUser.Username}' created auto-join group: '{groupName}' (ID: {autoJoinGroup.Id}).");

            // Act: _testUser sends a connection request (attempts to join)
            Debug.Log($"'{_testUser.Username}' attempting to join auto-join group ID: {autoJoinGroup.Id}.");
            GroupConnectionResponse connectionResponse = await _testUser.SendGroupConnectionRequestAsync(autoJoinGroup.Id);

            // Assert
            Assert.IsNotNull(connectionResponse, "GroupConnectionResponse should not be null.");
            Assert.IsTrue(connectionResponse.Id > 0, "GroupConnection ID should be positive.");
            Assert.AreEqual("accepted", connectionResponse.Status.ToLower(), "Status should be 'accepted' for auto-join.");
            Assert.AreEqual(_testUser.Id, connectionResponse.User.Id, "User ID in response mismatch.");
            // InviterId might be the user themselves or null in auto-join case. API doc example has inviter_id for pending.

            Debug.Log($"'{_testUser.Username}' successfully joined group '{groupName}'. Connection ID: {connectionResponse.Id}, Status: {connectionResponse.Status}.");

            // Verify _testUser is now a member
            Group groupDetailsAfterJoin = await _groupAdminUser.FetchGroupDetailsAsync(autoJoinGroup.Id); // Admin fetches details
            Assert.IsTrue(groupDetailsAfterJoin.Members.Any(m => m.Id == _testUser.Id), $"{_testUser.Username} not found in members list after auto-join.");
            Assert.AreEqual(2, groupDetailsAfterJoin.MemberCount, "Member count should be 2 after auto-join (admin + joiner).");
        }

        [Test]
        public async Task Test_SendGroupConnectionRequest_RequestToJoin_SucceedsAndIsPending()
        {
            Assert.IsNotNull(_groupAdminUser, "_groupAdminUser was not initialized.");
            Assert.IsNotNull(_testUser, "_testUser (joiner) was not initialized.");

            // Arrange: Admin creates a group that requires approval (not auto-join, not invite-only)
            string groupName = $"RequestToJoinGroup_{Guid.NewGuid().ToString("N").Substring(0, 10)}";
            var createPayload = new CreateGroupPayload
            {
                Name = groupName,
                MaxGroupSize = 10, // Required
                CanAutoJoin = false,
                IsInviteOnly = false // Explicitly not invite-only, allowing requests
            };
            Group requestGroup = await _groupAdminUser.CreateGroupAsync(createPayload);
            Assert.IsNotNull(requestGroup, "Failed to create request-to-join group.");
            Assert.IsFalse(requestGroup.CanAutoJoin, "Group should not be auto-join.");
            Debug.Log($"Admin '{_groupAdminUser.Username}' created request-to-join group: '{groupName}' (ID: {requestGroup.Id}).");

            // Act: _testUser sends a connection request (requests to join)
            Debug.Log($"'{_testUser.Username}' requesting to join group ID: {requestGroup.Id}.");
            GroupConnectionResponse connectionResponse = await _testUser.SendGroupConnectionRequestAsync(requestGroup.Id);

            // Assert
            Assert.IsNotNull(connectionResponse, "GroupConnectionResponse should not be null.");
            Assert.IsTrue(connectionResponse.Id > 0, "GroupConnection ID should be positive.");
            Assert.AreEqual("pending", connectionResponse.Status.ToLower(), "Status should be 'pending' for a join request.");
            Assert.AreEqual(_testUser.Id, connectionResponse.User.Id, "User ID in response mismatch.");
            // InviterId might be the user ID (_testUser.Id) if they are initiating the request.
            // Assert.AreEqual(_testUser.Id, connectionResponse.InviterId, "InviterId should be the requesting user's ID.");

            Debug.Log($"'{_testUser.Username}' successfully sent join request to group '{groupName}'. Connection ID: {connectionResponse.Id}, Status: {connectionResponse.Status}.");

            // Verify the join request exists (admin perspective)
            Group groupDetailsAfterRequest = await _groupAdminUser.FetchGroupDetailsAsync(requestGroup.Id);
            Assert.IsNotNull(groupDetailsAfterRequest.JoinRequests, "JoinRequests list should not be null.");
            // The structure of JoinRequests in Group model needs to be defined to assert this properly.
            // For now, we assume FetchGroupDetails populates it.
            // Assert.IsTrue(groupDetailsAfterRequest.JoinRequests.Any(jr => jr.UserId == _testUser.Id && jr.GroupConnectionId == connectionResponse.Id));
            // For now, just check the member count hasn't changed.
            Assert.AreEqual(1, groupDetailsAfterRequest.MemberCount, "Member count should still be 1 (admin only).");
        }

        private async Task<(GroupConnectionResponse, int)> CreatePendingJoinRequest(GameFuseUser requester, GameFuseUser admin)
        {
            // Helper to create a group and a pending join request
            string groupName = $"ManageReqGroup_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            Group groupToJoin = await admin.CreateGroupAsync(new CreateGroupPayload
            {
                Name = groupName,
                MaxGroupSize = 5,
                CanAutoJoin = false, // Requires approval
                IsInviteOnly = false
            });
            Assert.IsNotNull(groupToJoin, "Helper: Failed to create group.");

            Debug.Log($"Helper: Requester '{requester.Username}' sending join request to group '{groupToJoin.Name}' (ID: {groupToJoin.Id}).");
            GroupConnectionResponse joinRequest = await requester.SendGroupConnectionRequestAsync(groupToJoin.Id);
            Assert.IsNotNull(joinRequest, "Helper: Failed to send join request.");
            Assert.AreEqual("pending", joinRequest.Status.ToLower(), "Helper: Join request should be pending.");
            Assert.IsTrue(joinRequest.Id > 0, "Helper: Join request ID (GroupConnectionId) is invalid.");
            return (joinRequest, groupToJoin.Id); // Contains the GroupConnectionId needed for management
        }

        [Test]
        public async Task Test_AcceptGroupMembershipRequest_Succeeds()
        {
            Assert.IsNotNull(_groupAdminUser, "Admin user not initialized.");
            Assert.IsNotNull(_testUser, "Requesting user not initialized.");

            // Arrange: _testUser requests to join a group created by _groupAdminUser
            (GroupConnectionResponse pendingRequest, int groupId) = await CreatePendingJoinRequest(_testUser, _groupAdminUser);
            int groupConnectionId = pendingRequest.Id;
            Debug.Log($"Admin '{_groupAdminUser.Username}' will accept join request (ConnectionID: {groupConnectionId})  from '{_testUser.Username}'.");


            // Act: _groupAdminUser accepts the request
            GroupConnectionStatusUpdateResponse acceptResponse = await _groupAdminUser.AcceptGroupMembershipRequestAsync(groupConnectionId);

            // Assert
            Assert.IsNotNull(acceptResponse, "AcceptGroupMembershipRequest response should not be null.");
            Assert.AreEqual(groupConnectionId, acceptResponse.Id, "GroupConnection ID mismatch in accept response.");
            Assert.AreEqual("accepted", acceptResponse.Status.ToLower(), "Status in accept response should be 'accepted'.");
            Debug.Log($"Join request (ConnectionID: {groupConnectionId}) accepted. New status: {acceptResponse.Status}.");

            // Verify _testUser is now a member
            Group groupDetailsAfterAccept = await _groupAdminUser.FetchGroupDetailsAsync(groupId);
            Assert.IsTrue(groupDetailsAfterAccept.Members.Any(m => m.Id == _testUser.Id), $"{_testUser.Username} not found in members list after being accepted.");
            Assert.AreEqual(2, groupDetailsAfterAccept.MemberCount, "Member count should be 2 after acceptance (admin + accepted user).");
        }

        [Test]
        public async Task Test_DeclineGroupMembershipRequest_Succeeds()
        {
            Assert.IsNotNull(_groupAdminUser, "Admin user not initialized.");
            Assert.IsNotNull(_testUser, "Requesting user not initialized.");

            // Arrange: _testUser requests to join a group created by _groupAdminUser
            (GroupConnectionResponse pendingRequest, int groupId) = await CreatePendingJoinRequest(_testUser, _groupAdminUser);
            int groupConnectionId = pendingRequest.Id;
           
            Debug.Log($"Admin '{_groupAdminUser.Username}' will decline join request (ConnectionID: {groupConnectionId}) for group ID {groupId} from '{_testUser.Username}'.");


            // Act: _groupAdminUser declines the request
            GroupConnectionStatusUpdateResponse declineResponse = await _groupAdminUser.DeclineGroupMembershipRequestAsync(groupConnectionId);

            // Assert
            Assert.IsNotNull(declineResponse, "DeclineGroupMembershipRequest response should not be null.");
            Assert.AreEqual(groupConnectionId, declineResponse.Id, "GroupConnection ID mismatch in decline response.");
            Assert.AreEqual("declined", declineResponse.Status.ToLower(), "Status in decline response should be 'declined'.");
            Debug.Log($"Join request (ConnectionID: {groupConnectionId}) declined. New status: {declineResponse.Status}.");

            // Verify _testUser is NOT a member and request is gone (or marked declined)
            Group groupDetailsAfterDecline = await _groupAdminUser.FetchGroupDetailsAsync(groupId);
            Assert.IsFalse(groupDetailsAfterDecline.Members.Any(m => m.Id == _testUser.Id), $"{_testUser.Username} found in members list after being declined.");
            Assert.AreEqual(1, groupDetailsAfterDecline.MemberCount, "Member count should be 1 (admin only) after decline.");
            // Further check: the join request should ideally be removed or its status updated in the group's details
            // Assert.IsFalse(groupDetailsAfterDecline.JoinRequests.Any(jr => jr.GroupConnectionId == groupConnectionId && jr.Status == "pending"));
        }

        private void LoadTestConfig()
        {
            // ... (same as before)
            string configPath = Path.Combine(Application.dataPath, "TestConfiguration", "testConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                TestConfig config = JsonUtility.FromJson<TestConfig>(json);
                AdminToken = config.adminToken;
                AdminName = config.adminName;
                if (string.IsNullOrEmpty(AdminToken) || string.IsNullOrEmpty(AdminName))
                {
                    Assert.Inconclusive("AdminToken or AdminName is empty in testConfig.json.");
                }
            }
            else
            {
                Assert.Inconclusive("Test configuration file (testConfig.json) not found in Assets/TestConfiguration/.");
            }
        }

        [Serializable]
        private class TestConfig { public string adminToken; public string adminName; }
    }
}
