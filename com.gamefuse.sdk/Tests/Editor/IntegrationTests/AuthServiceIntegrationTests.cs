using GameFuse.Exceptions;
using GameFuse.Models.Shared;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFuse.Tests.Editor.IntegrationTests
{
    [TestFixture]
    public class AuthServiceIntegrationTests : TestBase
    {
        private AuthService _authService;
        
        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            
            // Skip setup if base setup failed
            if (TestGameId <= 0)
            {
                return;
            }

            // Initialize the service under test
            var transport = new UnityWebRequestTransport(); 
            _authService = new AuthService(transport);
        }
        
        [Test]
        public async Task Test_SignUp_Succeeds()
        {
            // Arrange
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string email = $"tester_{uniqueSuffix}@example.com";
            string username = $"tester_{uniqueSuffix}";
            string password = "Password123!";
            
            // Act
            var user = await _authService.SignUpAsync(
                email,
                password,
                username,
                TestGameId.ToString(),
                TestGameToken
            );
            
            // Assert
            Assert.IsNotNull(user, "User should not be null");
            Assert.Greater(user.Id, 0, "User ID should be positive");
            Assert.AreEqual(username, user.Username, "Username should match");
            Assert.IsTrue(user.Email.Contains(email), "Email should contain the provided email");
            Assert.IsFalse(string.IsNullOrEmpty(user.AuthenticationToken), "Authentication token should be present");
            
            Debug.Log($"Successfully signed up user: ID={user.Id}, Username={user.Username}, Email={user.Email}");
        }

        [Test]
        public async Task Test_SignUp_InvalidGameCredentials_ThrowsException()
        {
            // ---------- Arrange ----------
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string email = $"tester_{uniqueSuffix}@example.com";
            string username = $"tester_{uniqueSuffix}";
            string password = "Password123!";

            const string invalidGameId = "99999";
            const string invalidGameToken = "invalid_token";

            // ---------- Act & Assert ----------
            try
            {
                await _authService.SignUpAsync(
                    email,
                    password,
                    username,
                    invalidGameId,
                    invalidGameToken
                );

                // If the call completes without throwing, the test should fail.
                Assert.Fail("Expected GameFuseApiException was not thrown.");
            }
            catch (GameFuseApiException ex)
            {
                Assert.AreEqual(
                    System.Net.HttpStatusCode.NotFound,
                    ex.StatusCode,
                    "Status code should be NotFound for invalid game credentials."
                );

                Debug.Log($"Successfully caught expected exception: {ex.Message}");
            }
        }


        [Test]
        public async Task Test_SignIn_Succeeds()
        {
            // Arrange - Create a user first
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string email = $"tester_{uniqueSuffix}@example.com";
            string username = $"tester_{uniqueSuffix}";
            string password = "Password123!";
            
            // Create the user
            var createdUser = await _authService.SignUpAsync(
                email,
                password,
                username,
                TestGameId.ToString(),
                TestGameToken
            );
            
            Assert.IsNotNull(createdUser, "User creation should succeed");
            
            // Act - Sign in with the created user
            var signedInUser = await _authService.SignInAsync(
                email,
                password,
                TestGameId.ToString(),
                TestGameToken
            );
            
            // Assert
            Assert.IsNotNull(signedInUser, "Signed in user should not be null");
            Assert.AreEqual(createdUser.Id, signedInUser.Id, "User IDs should match");
            Assert.AreEqual(createdUser.Username, signedInUser.Username, "Usernames should match");
            Assert.IsTrue(signedInUser.Email.Contains(email), "Email should contain the provided email");
            Assert.IsFalse(string.IsNullOrEmpty(signedInUser.AuthenticationToken), "Authentication token should be present");
            
            Debug.Log($"Successfully signed in user: ID={signedInUser.Id}, Username={signedInUser.Username}");
        }

        [Test]
        public async Task Test_SignIn_InvalidCredentials_ThrowsException()
        {
            // ---------- Arrange ----------
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string email = $"tester_{uniqueSuffix}@example.com";
            string username = $"tester_{uniqueSuffix}";
            string password = "Password123!";

            // Create the user first so the sign-in call has an existing account
            var createdUser = await _authService.SignUpAsync(
                email,
                password,
                username,
                TestGameId.ToString(),
                TestGameToken
            );

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {
                var response = await _authService.SignInAsync(
                    email,
                    "WrongPassword123!",          
                    TestGameId.ToString(),
                    TestGameToken,
                    cts.Token                     
                );

                
                Assert.Fail("Expected GameFuseApiException or timeout was not thrown.");
            }
            catch (GameFuseApiException ex)
            {
                
                Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
               
                Assert.Fail("Sign-in call exceeded 3-second timeout.");
            }


           
        }


        [Test]
        public async Task Test_ForgotPassword_Succeeds()
        {
            // Arrange - Create a user first
            string uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            string email = $"tester_{uniqueSuffix}@example.com";
            string username = $"tester_{uniqueSuffix}";
            string password = "Password123!";
            
            // Create the user
            var createdUser = await _authService.SignUpAsync(
                email,
                password,
                username,
                TestGameId.ToString(),
                TestGameToken
            );
            
            Assert.IsNotNull(createdUser, "User creation should succeed");
            
            // Act - Request password reset
            // Note: This will not actually send an email in the test environment
            await _authService.ForgotPasswordAsync(
                email,
                TestGameId.ToString(),
                TestGameToken
            );
            
            // Assert - No exception means success
            Debug.Log($"Successfully requested password reset for user: {email}");
            
            // TODO: In the future, we might want to mock the email service to verify the email was sent
        }

        [Test]
        public async Task Test_ForgotPassword_NonexistentEmail_ThrowsException()
        {
            // Arrange
            string nonExistentEmail = $"nonexistent_{DateTime.UtcNow.Ticks}@example.com";

            try
            {
                await _authService.ForgotPasswordAsync(
                    nonExistentEmail,
                    TestGameId.ToString(),
                    TestGameToken
                );

                // If the call completes without throwing, the expected error did not occur.
                Assert.Fail("Expected GameFuseApiException was not thrown.");
            }
            catch (GameFuseApiException ex)
            {
                // Note: The API may return different responses for a non-existent email
                // (e.g., 404 Not Found or, for security reasons, 200 OK without error).
                // Adjust the assertion to match the behaviour you require.
                Assert.AreEqual(HttpStatusCode.NotFound, ex.StatusCode);

                Debug.Log($"Response for non-existent email: {ex.Message}");
            }
        }

        // Note: We can't fully test ResetPasswordAsync in integration tests 
        // because it requires a token from the email that would be sent
    }
}