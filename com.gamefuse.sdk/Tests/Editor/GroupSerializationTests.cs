using UnityEngine;
using NUnit.Framework;
using System;

namespace GameFuseCSharp.Tests
{
    [TestFixture]
    public class GroupSerializationTests : MonoBehaviour
    {
        
       



        [Test]
        public void GroupResponse_SerializesCorrectly()
        {
            // Arrange
            GroupResponse groupResponse = new GroupResponse
            {
                id = 1,
                name = "Elite Squad",
                group_type = "Public",
                can_auto_join = true,
                is_invite_only = false,
                max_group_size = 50,
                searchable = true,
                member_count = 2,
                members = new UserInfo[]
                {
                    new UserInfo
                    {
                        id = 10,
                        username = "player1",
                        email = "player1@email.com",
                        display_email = "player1@email.com",
                        credits = 100,
                        score = 500
                    },
                    new UserInfo
                    {
                        id = 11,
                        username = "player2",
                        email = "player2@email.com",
                        display_email = "player2@email.com",
                        credits = 150,
                        score = 750
                    }
                },
                admins = new UserInfo[]
                {
                    new UserInfo
                    {
                        id = 10,
                        username = "player1",
                        email = "player1@email.com",
                        display_email = "player1@email.com",
                        credits = 100,
                        score = 500
                    }
                },
                join_requests = new object[0],
                invites = new object[0]
            };

            // Act
            string json = JsonUtility.ToJson(groupResponse);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"name\":\"Elite Squad\",\"group_type\":\"Public\",\"can_auto_join\":true,\"is_invite_only\":false,\"max_group_size\":50,\"searchable\":true,\"member_count\":2,\"members\":[{\"id\":10,\"username\":\"player1\",\"email\":\"player1@email.com\",\"display_email\":\"player1@email.com\",\"credits\":100,\"score\":500},{\"id\":11,\"username\":\"player2\",\"email\":\"player2@email.com\",\"display_email\":\"player2@email.com\",\"credits\":150,\"score\":750}],\"admins\":[{\"id\":10,\"username\":\"player1\",\"email\":\"player1@email.com\",\"display_email\":\"player1@email.com\",\"credits\":100,\"score\":500}],\"join_requests\":[],\"invites\":[]}";

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void GroupAttributeRequest_SerializesCorrectly()
        {
            // Arrange
            GroupAttributeRequest attributeRequest = new GroupAttributeRequest
            {
                key = "level",
                value = "expert",
                only_can_edit_by_creator = true
            };

            // Act
            string json = JsonUtility.ToJson(attributeRequest);

            // Expected JSON output
            string expectedJson = "{\"key\":\"level\",\"value\":\"expert\",\"only_can_edit_by_creator\":true}";

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void GroupAttributesResponse_SerializesCorrectly()
        {
            // Arrange
            GroupAttributesResponse attributesResponse = new GroupAttributesResponse
            {
                attributes = new GroupAttribute[]
                {
                    new GroupAttribute
                    {
                        id = 1,
                        key = "level",
                        value = "expert",
                        creator_id = 10,
                        can_edit = true
                    },
                    new GroupAttribute
                    {
                        id = 2,
                        key = "region",
                        value = "europe",
                        creator_id = 10,
                        can_edit = false
                    }
                }
            };

            // Act
            string json = JsonUtility.ToJson(attributesResponse);

            // Expected JSON output
            string expectedJson = "{\"attributes\":[{\"id\":1,\"key\":\"level\",\"value\":\"expert\",\"creator_id\":10,\"can_edit\":true},{\"id\":2,\"key\":\"region\",\"value\":\"europe\",\"creator_id\":10,\"can_edit\":false}]}";

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void GroupConnectionRequest_SerializesCorrectly()
        {
            // Arrange
            GroupConnectionRequest connectionRequest = new GroupConnectionRequest
            {
                group_id = 1,
                user_id = 10
            };

            // Act
            string json = JsonUtility.ToJson(connectionRequest);

            // Expected JSON output
            string expectedJson = "{\"group_id\":1,\"user_id\":10}";

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void GroupConnectionResponse_SerializesCorrectly()
        {
            // Arrange
            GroupConnectionResponse connectionResponse = new GroupConnectionResponse
            {
                id = 1,
                status = "pending",
                inviter_id = 10,
                user = new UserInfo
                {
                    id = 11,
                    username = "player2",
                    email = "player2@email.com",
                    display_email = "player2@email.com",
                    credits = 150,
                    score = 750
                }
            };

            // Act
            string json = JsonUtility.ToJson(connectionResponse);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"status\":\"pending\",\"inviter_id\":10,\"user\":{\"id\":11,\"username\":\"player2\",\"email\":\"player2@email.com\",\"display_email\":\"player2@email.com\",\"credits\":150,\"score\":750}}";

            // Assert
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void GroupConnectionStatusResponse_SerializesCorrectly()
        {
            // Arrange
            GroupConnectionStatusResponse statusResponse = new GroupConnectionStatusResponse
            {
                id = 1,
                status = "accepted"
            };

            // Act
            string json = JsonUtility.ToJson(statusResponse);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"status\":\"accepted\"}";

            // Assert
            Assert.AreEqual(expectedJson, json);
        }
    }
}
