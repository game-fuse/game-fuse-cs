using UnityEngine;
using NUnit.Framework;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameFuseCSharp;

namespace GameFuseCSharp.Tests
{
    [TestFixture]
    public class FriendSerializationTests
    {
        // Helper method to compare JSON content regardless of property order
        private void AssertJsonEqual(string expected, string actual, string message = "JSON content should be equal regardless of property order")
        {
            var expectedJson = JToken.Parse(expected);
            var actualJson = JToken.Parse(actual);
            Assert.IsTrue(JToken.DeepEquals(expectedJson, actualJson), message);
        }

        [Test]
        public void FriendRequest_SerializesCorrectly()
        {
            // Arrange
            var friendRequest = new FriendRequest
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                DisplayEmail = "display@example.com",
                Credits = 100,
                Score = 500,
                FriendshipId = 42,
                RequestedAt = "2024-03-15T10:30:00Z"
            };

            // Act
            string json = JsonConvert.SerializeObject(friendRequest);

            // Expected JSON output
            string expectedJson = "{\"id\":1,\"username\":\"testuser\",\"email\":\"test@example.com\"," +
                "\"display_email\":\"display@example.com\",\"credits\":100,\"score\":500," +
                "\"friendship_id\":42,\"requested_at\":\"2024-03-15T10:30:00Z\"}";

            // Assert
            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void IncomingFriendRequestsResponse_SerializesCorrectly()
        {
            // Arrange
            var response = new IncomingFriendRequestsResponse
            {
                IncomingFriendRequests = new FriendRequest[]
                {
                    new FriendRequest
                    {
                        Id = 1,
                        Username = "requester",
                        Email = "requester@example.com",
                        DisplayEmail = "requesterdisplay@example.com",
                        Credits = 300,
                        Score = 1500,
                        FriendshipId = 456,
                        RequestedAt = "2024-03-15T11:00:00Z"
                    }
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(response);

            // Expected JSON output
            string expectedJson = "{\"incoming_friend_requests\":[{\"id\":1,\"username\":\"requester\"," +
                "\"email\":\"requester@example.com\",\"display_email\":\"requesterdisplay@example.com\"," +
                "\"credits\":300,\"score\":1500,\"friendship_id\":456,\"requested_at\":\"2024-03-15T11:00:00Z\"}]}";

            // Assert
            AssertJsonEqual(expectedJson, json);
        }

        // Update other tests to use AssertJsonEqual
        [Test]
        public void FriendRequestData_SerializesCorrectly()
        {
            var requestData = new FriendRequestData
            {
                Username = "frienduser"
            };

            string json = JsonConvert.SerializeObject(requestData);
            string expectedJson = "{\"username\":\"frienduser\"}";

            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void FriendshipStatusData_SerializesCorrectly()
        {
            var statusData = new FriendshipStatusData
            {
                Status = "accepted"
            };

            string json = JsonConvert.SerializeObject(statusData);
            string expectedJson = "{\"status\":\"accepted\"}";

            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void FriendRequestResponse_SerializesCorrectly()
        {
            var response = new FriendRequestResponse
            {
                Message = "Friend request sent successfully",
                FriendshipId = 123
            };

            string json = JsonConvert.SerializeObject(response);
            string expectedJson = "{\"message\":\"Friend request sent successfully\",\"friendship_id\":123}";

            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void FriendsResponse_SerializesCorrectly()
        {
            var response = new FriendsResponse
            {
                Friends = new UserInfo[]
                {
                    new UserInfo
                    {
                        Id = 1,
                        Username = "friend1",
                        Email = "friend1@example.com",
                        DisplayEmail = "friend1display@example.com",
                        Credits = 200,
                        Score = 1000
                    }
                }
            };

            string json = JsonConvert.SerializeObject(response);
            string expectedJson = "{\"friends\":[{\"id\":1,\"username\":\"friend1\"," +
                "\"email\":\"friend1@example.com\",\"display_email\":\"friend1display@example.com\"," +
                "\"credits\":200,\"score\":1000}]}";

            AssertJsonEqual(expectedJson, json);
        }

        [Test]
        public void OutgoingFriendRequestsResponse_SerializesCorrectly()
        {
            var response = new OutgoingFriendRequestsResponse
            {
                OutgoingFriendRequests = new FriendRequest[]
                {
                    new FriendRequest
                    {
                        Id = 2,
                        Username = "recipient",
                        Email = "recipient@example.com",
                        DisplayEmail = "recipientdisplay@example.com",
                        Credits = 400,
                        Score = 2000,
                        FriendshipId = 789,
                        RequestedAt = "2024-03-15T12:00:00Z"
                    }
                }
            };

            string json = JsonConvert.SerializeObject(response);
            string expectedJson = "{\"outgoing_friend_requests\":[{\"id\":2,\"username\":\"recipient\"," +
                "\"email\":\"recipient@example.com\",\"display_email\":\"recipientdisplay@example.com\"," +
                "\"credits\":400,\"score\":2000,\"friendship_id\":789,\"requested_at\":\"2024-03-15T12:00:00Z\"}]}";

            AssertJsonEqual(expectedJson, json);
        }
    }
}