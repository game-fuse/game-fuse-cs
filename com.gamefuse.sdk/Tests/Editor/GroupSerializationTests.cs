using UnityEngine;
using NUnit.Framework;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GameFuseCSharp.Tests
{
    [TestFixture]
    public class GroupSerializationTests
    {
        protected static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        // Helper method to compare JSON content regardless of property order
        private void AssertJsonEqual(string expected, string actual, string message = "JSON content should be equal regardless of property order")
        {
            var expectedJson = JToken.Parse(expected);
            var actualJson = JToken.Parse(actual);
            Assert.IsTrue(JToken.DeepEquals(expectedJson, actualJson), message);
        }

        [Test]
        public void CreateGroupRequest_SerializesCorrectly()
        {
            // Arrange
            var request = new CreateGroupRequest
            {
                Name = "Awesome Gamers",
                GroupType = "Public",
                MaxGroupSize = 50,
                CanAutoJoin = true,
                IsInviteOnly = false,
                Searchable = true,
                AdminsOnlyCanCreateAttributes = true
            };

            // Act
            string actualJson = JsonConvert.SerializeObject(request, JsonSettings);

            // Expected JSON output
            string expectedJson = @"{
                ""name"": ""Awesome Gamers"",
                ""group_type"": ""Public"",
                ""max_group_size"": 50,
                ""can_auto_join"": true,
                ""is_invite_only"": false,
                ""searchable"": true,
                ""admins_only_can_create_attributes"": true
            }";

            // Assert
            AssertJsonEqual(expectedJson, actualJson);
        }

        [Test]
        public void CreateGroupRequest_SerializesCorrectlyWithMinimalData()
        {
            // Arrange
            var request = new CreateGroupRequest
            {
                Name = "Minimal Group",
                MaxGroupSize = 10,
                CanAutoJoin = false,
                IsInviteOnly = false
            };

            // Act
            string actualJson = JsonConvert.SerializeObject(request, JsonSettings);

            // Expected JSON output
            string expectedJson = @"{
                ""name"": ""Minimal Group"",
                ""group_type"": ""default"",
                ""max_group_size"": 10,
                ""can_auto_join"": false,
                ""is_invite_only"": false
            }";

            // Assert
            AssertJsonEqual(expectedJson, actualJson);
        }

        [Test]
        public void CreateGroupRequest_DeserializesCorrectly()
        {
            // Arrange
            string json = @"{
                ""name"": ""Test Group"",
                ""group_type"": ""Private"",
                ""max_group_size"": 10,
                ""can_auto_join"": false,
                ""is_invite_only"": true,
                ""searchable"": false,
                ""admins_only_can_create_attributes"": true
            }";

            // Act
            var request = JsonConvert.DeserializeObject<CreateGroupRequest>(json, JsonSettings);

            // Assert
            Assert.NotNull(request);
            Assert.AreEqual("Test Group", request.Name);
            Assert.AreEqual("Private", request.GroupType);
            Assert.AreEqual(10, request.MaxGroupSize);
            Assert.IsFalse(request.CanAutoJoin);
            Assert.IsTrue(request.IsInviteOnly);
            Assert.IsFalse(request.Searchable);
            Assert.IsTrue(request.AdminsOnlyCanCreateAttributes);
        }

        [Test]
        public void GroupResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new GroupResponse
            {
                Id = 1,
                Name = "Test Group",
                GroupType = "Public",
                CanAutoJoin = true,
                IsInviteOnly = false,
                MaxGroupSize = 50,
                Searchable = true,
                MemberCount = 2,
                Members = new UserInfo[]
                {
                    new UserInfo
                    {
                        Id = 10,
                        Username = "member1",
                        Email = "member1@example.com",
                        DisplayEmail = "member1@example.com",
                        Credits = 100,
                        Score = 500
                    }
                },
                Admins = new UserInfo[]
                {
                    new UserInfo
                    {
                        Id = 20,
                        Username = "admin1",
                        Email = "admin1@example.com",
                        DisplayEmail = "admin1@example.com",
                        Credits = 200,
                        Score = 1000
                    }
                }
            };

            // Act
            string actualJson = JsonConvert.SerializeObject(response, JsonSettings);

            // Expected JSON output
            string expectedJson = @"{
                ""id"": 1,
                ""name"": ""Test Group"",
                ""group_type"": ""Public"",
                ""can_auto_join"": true,
                ""is_invite_only"": false,
                ""max_group_size"": 50,
                ""searchable"": true,
                ""member_count"": 2,
                ""members"": [{
                    ""id"": 10,
                    ""username"": ""member1"",
                    ""email"": ""member1@example.com"",
                    ""display_email"": ""member1@example.com"",
                    ""credits"": 100,
                    ""score"": 500
                }],
                ""admins"": [{
                    ""id"": 20,
                    ""username"": ""admin1"",
                    ""email"": ""admin1@example.com"",
                    ""display_email"": ""admin1@example.com"",
                    ""credits"": 200,
                    ""score"": 1000
                }],
                ""join_requests"": [],
                ""invites"": []
            }";

            // Assert
            AssertJsonEqual(expectedJson, actualJson);
        }

        [Test]
        public void GroupResponse_DeserializesCorrectly()
        {
            // Arrange
            string json = @"{
                ""id"": 1,
                ""name"": ""Test Group"",
                ""group_type"": ""Public"",
                ""can_auto_join"": true,
                ""is_invite_only"": false,
                ""max_group_size"": 50,
                ""searchable"": true,
                ""member_count"": 2,
                ""members"": [{
                    ""id"": 10,
                    ""username"": ""member1"",
                    ""email"": ""member1@example.com"",
                    ""display_email"": ""member1@example.com"",
                    ""credits"": 100,
                    ""score"": 500
                }],
                ""admins"": [{
                    ""id"": 20,
                    ""username"": ""admin1"",
                    ""email"": ""admin1@example.com"",
                    ""display_email"": ""admin1@example.com"",
                    ""credits"": 200,
                    ""score"": 1000
                }],
                ""join_requests"": [],
                ""invites"": []
            }";

            // Act
            var response = JsonConvert.DeserializeObject<GroupResponse>(json, JsonSettings);

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual(1, response.Id);
            Assert.AreEqual("Test Group", response.Name);
            Assert.AreEqual("Public", response.GroupType);
            Assert.IsTrue(response.CanAutoJoin);
            Assert.IsFalse(response.IsInviteOnly);
            Assert.AreEqual(50, response.MaxGroupSize);
            Assert.IsTrue(response.Searchable);
            Assert.AreEqual(2, response.MemberCount);

            // Check members array
            Assert.AreEqual(1, response.Members.Length);
            Assert.AreEqual(10, response.Members[0].Id);
            Assert.AreEqual("member1", response.Members[0].Username);
            Assert.AreEqual("member1@example.com", response.Members[0].Email);
            Assert.AreEqual(100, response.Members[0].Credits);
            Assert.AreEqual(500, response.Members[0].Score);

            // Check admins array
            Assert.AreEqual(1, response.Admins.Length);
            Assert.AreEqual(20, response.Admins[0].Id);
            Assert.AreEqual("admin1", response.Admins[0].Username);
            Assert.AreEqual("admin1@example.com", response.Admins[0].Email);
            Assert.AreEqual(200, response.Admins[0].Credits);
            Assert.AreEqual(1000, response.Admins[0].Score);

            // Check empty arrays
            Assert.IsEmpty(response.JoinRequests);
            Assert.IsEmpty(response.Invites);
        }

        [Test]
        public void GroupResponse_DeserializesCorrectlyWithMinimalData()
        {
            // Arrange
            string json = @"{
                ""id"": 1,
                ""name"": ""Minimal Group""
            }";

            // Act
            var response = JsonConvert.DeserializeObject<GroupResponse>(json, JsonSettings);

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual(1, response.Id);
            Assert.AreEqual("Minimal Group", response.Name);
            Assert.IsEmpty(response.Members);
            Assert.IsEmpty(response.Admins);
            Assert.IsEmpty(response.JoinRequests);
            Assert.IsEmpty(response.Invites);
        }
    }
}