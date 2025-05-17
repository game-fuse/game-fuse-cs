using System;
using System.Threading.Tasks;
using GameFuseCSharp;
using GFuse = GameFuseCSharp.GameFuse; // Alias to avoid namespace conflict
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Handles authentication functionality including sign-up, sign-in, and password reset
    /// </summary>
    public class AuthenticationController : BaseGameFuseUIController
    {
        // Authentication UI elements
        private TextField gameIdField;
        private TextField gameTokenField;
        private Button setupGameButton;
        private Label gameNameLabel;
        private Label gameDescriptionLabel;
        
        // Sign-in UI elements
        private TextField signinEmailField;
        private TextField signinPasswordField;
        private Button signinButton;
        
        // Sign-up UI elements
        private TextField signupEmailField;
        private TextField signupUsernameField;
        private TextField signupPasswordField;
        private TextField signupConfirmPasswordField;
        private Button signupButton;
        
        // Password reset UI elements
        private TextField forgotPasswordEmailField;
        private Button resetPasswordButton;
        
        // User info UI elements
        private Button signoutButton;
        private ScrollView currentUserInfoScrollView;
        
        protected override void InitializeUI()
        {
            // Game setup elements
            gameIdField = rootElement.Q<TextField>("game-id");
            gameTokenField = rootElement.Q<TextField>("game-token");
            setupGameButton = rootElement.Q<Button>("setup-game-button");
            gameNameLabel = rootElement.Q<Label>("game-name");
            gameDescriptionLabel = rootElement.Q<Label>("game-description");
            
            // Sign-in elements
            signinEmailField = rootElement.Q<TextField>("signin-email");
            signinPasswordField = rootElement.Q<TextField>("signin-password");
            signinButton = rootElement.Q<Button>("signin-button");
            
            // Sign-up elements
            signupEmailField = rootElement.Q<TextField>("signup-email");
            signupUsernameField = rootElement.Q<TextField>("signup-username");
            signupPasswordField = rootElement.Q<TextField>("signup-password");
            signupConfirmPasswordField = rootElement.Q<TextField>("signup-confirm-password");
            signupButton = rootElement.Q<Button>("signup-button");
            
            // Password reset elements
            forgotPasswordEmailField = rootElement.Q<TextField>("forgot-password-email");
            resetPasswordButton = rootElement.Q<Button>("reset-password-button");
            
            // User info elements
            signoutButton = rootElement.Q<Button>("signout-button");
            currentUserInfoScrollView = rootElement.Q<ScrollView>("current-user-info-scroll");
            
            // Disable signout button initially
            if (signoutButton != null)
            {
                signoutButton.SetEnabled(false);
            }
            
            // Auto-fill fields from config if available
            if (config != null)
            {
                if (!string.IsNullOrEmpty(config.GameId))
                {
                    gameIdField.value = config.GameId;
                }
                
                if (!string.IsNullOrEmpty(config.GameToken))
                {
                    gameTokenField.value = config.GameToken;
                }
            }
        }
        
        protected override void RegisterCallbacks()
        {
            if (setupGameButton != null)
            {
                setupGameButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetupGameClicked());
            }
            
            if (signinButton != null)
            {
                signinButton.RegisterCallback<ClickEvent>(async (evt) => await OnSignInClicked());
            }
            
            if (signupButton != null)
            {
                signupButton.RegisterCallback<ClickEvent>(async (evt) => await OnSignUpClicked());
            }
            
            if (resetPasswordButton != null)
            {
                resetPasswordButton.RegisterCallback<ClickEvent>(async (evt) => await OnResetPasswordClicked());
            }
            
            if (signoutButton != null)
            {
                signoutButton.RegisterCallback<ClickEvent>(async (evt) => await OnSignOutClicked());
            }
        }
        
        protected override void UnregisterCallbacks()
        {
            if (setupGameButton != null)
            {
                setupGameButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSetupGameClicked());
            }
            
            if (signinButton != null)
            {
                signinButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSignInClicked());
            }
            
            if (signupButton != null)
            {
                signupButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSignUpClicked());
            }
            
            if (resetPasswordButton != null)
            {
                resetPasswordButton.UnregisterCallback<ClickEvent>(async (evt) => await OnResetPasswordClicked());
            }
            
            if (signoutButton != null)
            {
                signoutButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSignOutClicked());
            }
        }
        
        /// <summary>
        /// Auto-initializes the game if config is provided
        /// </summary>
        public async Task AutoInitialize()
        {
            if (config != null && !string.IsNullOrEmpty(config.GameId) && !string.IsNullOrEmpty(config.GameToken))
            {
                await OnSetupGameClicked();
            }
        }
        
        /// <summary>
        /// Sets up the game with the provided ID and token
        /// </summary>
        private async Task OnSetupGameClicked()
        {
            string gameId = config != null ? config.GameId : gameIdField.value;
            string gameToken = config != null ? config.GameToken : gameTokenField.value;
            
            // If config exists, populate the input fields with the config values
            if (config != null)
            {
                gameIdField.value = gameId;
                gameTokenField.value = gameToken;
            }
            
            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(gameToken))
            {
                LogMessage("Game ID and Game Token are required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set up GameFuse with the provided game ID and token
                await GFuse.SetUpGameAsync(gameId, gameToken);
                
                // Update game info display
                gameNameLabel.text = GFuse.GetGameName();
                gameDescriptionLabel.text = GFuse.GetGameDescription();
                
                LogMessage($"Game '{GFuse.GetGameName()}' setup successful", LogType.Success);
            });
        }
        
        /// <summary>
        /// Signs in a user with the provided email and password
        /// </summary>
        private async Task OnSignInClicked()
        {
            string email = signinEmailField.value;
            string password = signinPasswordField.value;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                LogMessage("Email and password are required", LogType.Error);
                return;
            }
            
            await SignInUserAsync(email, password);
        }
        
        /// <summary>
        /// Signs up a new user with the provided information
        /// </summary>
        private async Task OnSignUpClicked()
        {
            string email = signupEmailField.value;
            string username = signupUsernameField.value;
            string password = signupPasswordField.value;
            string confirmPassword = signupConfirmPasswordField.value;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                LogMessage("All fields are required for signup", LogType.Error);
                return;
            }
            
            if (password != confirmPassword)
            {
                LogMessage("Passwords do not match", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Sign up the user
                await GFuse.SignUpAsync(email, password, confirmPassword, username);
                
                LogMessage($"User {username} signed up successfully", LogType.Success);
                
                // Sign in the user automatically
                await SignInUserAsync(email, password);
            });
        }
        
        /// <summary>
        /// Sends a password reset email
        /// </summary>
        private async Task OnResetPasswordClicked()
        {
            string email = forgotPasswordEmailField.value;
            
            if (string.IsNullOrEmpty(email))
            {
                LogMessage("Email is required for password reset", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Send password reset email
                await GFuse.SendPasswordResetEmailAsync(email);
                
                LogMessage($"Password reset email sent to {email}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Signs out the current user
        /// </summary>
        private async Task OnSignOutClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Sign out the user
                GameFuseUser.CurrentUser.SignOut();
                
                // Update UI state
                signoutButton.SetEnabled(false);
                
                // Clear current user info
                ClearScrollView(currentUserInfoScrollView);
                
                LogMessage("User signed out successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Signs in a user and updates the UI
        /// </summary>
        private async Task SignInUserAsync(string email, string password)
        {
            await ExecuteAsync(async () =>
            {
                // Sign in the user
                var response = await GFuse.SignInAsync(email, password);
                
                // Update UI state for signed in user
                signoutButton.SetEnabled(true);
                
                // Display current user info
                UpdateCurrentUserInfo();
                
                LogMessage($"User {GameFuseUser.CurrentUser.GetUsername()} signed in successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Updates the current user info display
        /// </summary>
        private void UpdateCurrentUserInfo()
        {
            if (currentUserInfoScrollView == null) return;
            
            // Clear the current info
            ClearScrollView(currentUserInfoScrollView);
            
            // Make sure we have a current user
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                currentUserInfoScrollView.Add(new Label("No user is currently signed in"));
                return;
            }
            
            // Create the info display
            var properties = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Username", GameFuseUser.CurrentUser.GetUsername() },
                { "Score", GameFuseUser.CurrentUser.GetScore().ToString() },
                { "Credits", GameFuseUser.CurrentUser.GetCredits().ToString() },
                { "ID", GameFuseUser.CurrentUser.GetID().ToString() },
                { "Last Login", GameFuseUser.CurrentUser.GetLastLogin().ToString() },
                { "Number of Logins", GameFuseUser.CurrentUser.GetNumberOfLogins().ToString() }
            };
            
            currentUserInfoScrollView.Add(CreateListItem("Current User", properties));
        }
    }
}