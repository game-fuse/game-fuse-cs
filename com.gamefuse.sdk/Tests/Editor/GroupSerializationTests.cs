using UnityEngine;
using NUnit.Framework;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameFuseCSharp;

namespace GameFuseCSharp.Tests
{
    [TestFixture]
    public class GroupSerializationTests
    {
        // Helper method to compare JSON content regardless of property order
        // Helper method to compare JSON content regardless of property order
        private void AssertJsonEqual(string expected, string actual, string message = "JSON content should be equal regardless of property order")
        {
            var expectedJson = JToken.Parse(expected);
            var actualJson = JToken.Parse(actual);

            if (!JToken.DeepEquals(expectedJson, actualJson))
            {
                Debug.Log($"Expected JSON: {expectedJson.ToString(Formatting.Indented)}");
                Debug.Log($"Actual JSON: {actualJson.ToString(Formatting.Indented)}");
            }

            Assert.IsTrue(JToken.DeepEquals(expectedJson, actualJson), message);
        }

        [Test]
        public void GroupAttribute_SerializesCorrectly()
        {
            // Arrange
            var attribute = new GroupAttribute
            {
                Id = 1,
                Key = "level",
                Value = "expert",
                CreatorId = 100,
                CanEdit = true
            };

            // Act
            string json = JsonConvert.SerializeObject(attribute);

            // Assert
            string expectedJson = "{\"id\":1,\"key\":\"level\",\"value\":\"expert\"," +
                "\"creator_id\":100,\"can_edit\":true}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupAttributeRequest_SerializesCorrectly()
        {
            // Arrange
            var request = new GroupAttributeRequest
            {
                Key = "rank",
                Value = "platinum",
                OnlyCanEditByCreator = true
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            string expectedJson = "{\"key\":\"rank\",\"value\":\"platinum\"," +
                "\"only_can_edit_by_creator\":true}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupConnectionRequest_SerializesCorrectly()
        {
            // Arrange
            var request = new GroupConnectionRequest
            {
                GroupId = 5,
                UserId = 10
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            string expectedJson = "{\"group_id\":5,\"user_id\":10}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupAttributesResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new GroupAttributesResponse
            {
                Attributes = new[]
                {
                    new GroupAttribute
                    {
                        Id = 1,
                        Key = "level",
                        Value = "expert",
                        CreatorId = 100,
                        CanEdit = true
                    }
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(response);

            // Assert
            string expectedJson = "{\"attributes\":[{\"id\":1,\"key\":\"level\",\"value\":\"expert\"," +
                "\"creator_id\":100,\"can_edit\":true}]}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupConnectionResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new GroupConnectionResponse
            {
                Id = 1,
                Status = "pending",
                InviterId = 100,
                User = new UserInfo
                {
                    Id = 200,
                    Username = "testuser",
                    Email = "test@example.com",
                    DisplayEmail = "display@example.com",
                    Credits = 500,
                    Score = 1000
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(response);

            // Assert
            string expectedJson = "{\"id\":1,\"status\":\"pending\",\"inviter_id\":100," +
                "\"user\":{\"id\":200,\"username\":\"testuser\",\"email\":\"test@example.com\"," +
                "\"display_email\":\"display@example.com\",\"credits\":500,\"score\":1000}}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupConnectionStatusResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new GroupConnectionStatusResponse
            {
                Id = 1,
                Status = "accepted",
                Message = "Successfully joined the group"
            };

            // Act
            string json = JsonConvert.SerializeObject(response);

            // Assert
            string expectedJson = "{\"id\":1,\"status\":\"accepted\"," +
                "\"message\":\"Successfully joined the group\"}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new GroupResponse
            {
                Id = 1,
                Name = "Pro Gamers",
                GroupType = "Public",
                CanAutoJoin = true,
                IsInviteOnly = false,
                MaxGroupSize = 50,
                Searchable = true,
                MemberCount = 2,
                Members = new[]
                {
                    new UserInfo
                    {
                        Id = 100,
                        Username = "member1",
                        Email = "member1@example.com",
                        DisplayEmail = "display1@example.com",
                        Credits = 500,
                        Score = 1000
                    }
                },
                Admins = new[]
                {
                    new UserInfo
                    {
                        Id = 200,
                        Username = "admin1",
                        Email = "admin1@example.com",
                        DisplayEmail = "displayadmin1@example.com",
                        Credits = 1000,
                        Score = 2000
                    }
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(response);

            // Assert
            string expectedJson = "{\"id\":1,\"name\":\"Pro Gamers\",\"group_type\":\"Public\"," +
                "\"can_auto_join\":true,\"is_invite_only\":false,\"max_group_size\":50," +
                "\"searchable\":true,\"member_count\":2,\"members\":[{\"id\":100,\"username\":\"member1\"," +
                "\"email\":\"member1@example.com\",\"display_email\":\"display1@example.com\"," +
                "\"credits\":500,\"score\":1000}],\"admins\":[{\"id\":200,\"username\":\"admin1\"," +
                "\"email\":\"admin1@example.com\",\"display_email\":\"displayadmin1@example.com\"," +
                "\"credits\":1000,\"score\":2000}],\"join_requests\":[],\"invites\":[]}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void GroupsResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new GroupsResponse
            {
                Groups = new[]
                {
                    new GroupResponse
                    {
                        Id = 1,
                        Name = "Pro Gamers",
                        GroupType = "Public",
                        CanAutoJoin = true,
                        IsInviteOnly = false,
                        MaxGroupSize = 50,
                        Searchable = true,
                        MemberCount = 1
                    }
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(response);

            // Assert
            string expectedJson = "{\"groups\":[{\"id\":1,\"name\":\"Pro Gamers\",\"group_type\":\"Public\"," +
                "\"can_auto_join\":true,\"is_invite_only\":false,\"max_group_size\":50,\"searchable\":true," +
                "\"member_count\":1,\"members\":[],\"admins\":[],\"join_requests\":[],\"invites\":[]}]}";
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void EmptyCollections_SerializeCorrectly()
        {
            // Arrange
            var emptyAttributesResponse = new GroupAttributesResponse();
            var emptyGroupsResponse = new GroupsResponse();
            var emptyGroupResponse = new GroupResponse
            {
                Id = 1,
                Name = "Empty Group",
                GroupType = null,
                CanAutoJoin = false,
                IsInviteOnly = false,
                MaxGroupSize = 0,
                Searchable = false,
                MemberCount = 0,
                Members = Array.Empty<UserInfo>(),
                Admins = Array.Empty<UserInfo>(),
                JoinRequests = Array.Empty<GroupConnectionResponse>(),
                Invites = Array.Empty<GroupConnectionResponse>()
            };

            // Act
            string attributesJson = JsonConvert.SerializeObject(emptyAttributesResponse);
            string groupsJson = JsonConvert.SerializeObject(emptyGroupsResponse);
            string groupJson = JsonConvert.SerializeObject(emptyGroupResponse);

            // Print the actual JSON for debugging
            Debug.Log($"Actual attributesJson: {attributesJson}");
            Debug.Log($"Actual groupsJson: {groupsJson}");
            Debug.Log($"Actual groupJson: {groupJson}");

            // Assert
            AssertJsonEqual("{\"attributes\":[]}", attributesJson);
            AssertJsonEqual("{\"groups\":[]}", groupsJson);
            AssertJsonEqual(
                "{" +
                    "\"id\":1," +
                    "\"name\":\"Empty Group\"," +
                    "\"can_auto_join\":false," +
                    "\"is_invite_only\":false," +
                    "\"max_group_size\":0," +
                    "\"searchable\":false," +
                    "\"member_count\":0," +
                    "\"members\":[]," +
                    "\"admins\":[]," +
                    "\"join_requests\":[]," +
                    "\"invites\":[]" +
                "}",
                groupJson,
                "Empty GroupResponse should serialize correctly"
            );
        }

        [Test]
        public void GroupAttribute_DeserializesCorrectly()
        {
            // Arrange
            string json = "{\"id\":1,\"key\":\"level\",\"value\":\"expert\"," +
                "\"creator_id\":100,\"can_edit\":true}";

            // Act
            var attribute = JsonConvert.DeserializeObject<GroupAttribute>(json);

            // Assert
            Assert.NotNull(attribute);
            Assert.AreEqual(1, attribute.Id);
            Assert.AreEqual("level", attribute.Key);
            Assert.AreEqual("expert", attribute.Value);
            Assert.AreEqual(100, attribute.CreatorId);
            Assert.IsTrue(attribute.CanEdit);
        }
    }
}