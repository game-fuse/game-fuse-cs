using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using GameFuse.Config;
using System;
using System.Threading.Tasks;

namespace GameFuse.UI
{
    /// <summary>
    /// Controls the authentication panel UI - handles sign up and sign in functionality.
    /// </summary>
    public class AuthPanelController
    {
        // Events
        public static event Action<GameFuseUser> OnSignUpSuccess;
        public static event Action<GameFuseUser> OnSignInSuccess;
        public static event Action<string> OnSignUpError;
        public static event Action<string> OnSignInError;

        // UI Elements
        private VisualElement root;
        private VisualElement signInForm;
        private VisualElement signUpForm;

        // Sign In Elements
        private TextField signInEmail;
        private TextField signInPassword;
        private Button signInButton;
        private Button switchToSignUpButton;

        // Sign Up Elements
        private TextField signUpEmail;
        private TextField signUpUsername;
        private TextField signUpPassword;
        private TextField signUpConfirmPassword;
        private Button signUpButton;
        private Button switchToSignInButton;

        // Loading and status
        private VisualElement loadingIndicator;
        private Label statusLabel;

        public AuthPanelController(VisualElement authPanel)
        {
            root = authPanel;

            InitializeElements();
            SetupEventHandlers();

            // Start with sign in form
            ShowSignInForm();
        }

        private void InitializeElements()
        {
            // Get main forms
            signInForm = root.Q<VisualElement>("signin-form");
            signUpForm = root.Q<VisualElement>("signup-form");

            // Sign In elements
            signInEmail = root.Q<TextField>("signin-email");
            signInPassword = root.Q<TextField>("signin-password");
            signInButton = root.Q<Button>("signin-button");
            switchToSignUpButton = root.Q<Button>("switch-to-signup");

            // Sign Up elements
            signUpEmail = root.Q<TextField>("signup-email");
            signUpUsername = root.Q<TextField>("signup-username");
            signUpPassword = root.Q<TextField>("signup-password");
            signUpConfirmPassword = root.Q<TextField>("signup-confirm-password");
            signUpButton = root.Q<Button>("signup-button");
            switchToSignInButton = root.Q<Button>("switch-to-signin");

            // Status elements
            loadingIndicator = root.Q<VisualElement>("loading-indicator");
            statusLabel = root.Q<Label>("status-label");

            // Configure password fields
            signInPassword.isPasswordField = true;
            signUpPassword.isPasswordField = true;
            signUpConfirmPassword.isPasswordField = true;
        }

        private void SetupEventHandlers()
        {
            // Form switching
            switchToSignUpButton.clicked += ShowSignUpForm;
            switchToSignInButton.clicked += ShowSignInForm;

            // Authentication actions
            signInButton.clicked += HandleSignIn;
            signUpButton.clicked += HandleSignUp;

            // Enter key handling
            signInPassword.RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    HandleSignIn();
            });

            signUpConfirmPassword.RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    HandleSignUp();
            });
        }

        private void ShowSignInForm()
        {
            signInForm.style.display = DisplayStyle.Flex;
            signUpForm.style.display = DisplayStyle.None;
            ClearStatus();
        }

        private void ShowSignUpForm()
        {
            signInForm.style.display = DisplayStyle.None;
            signUpForm.style.display = DisplayStyle.Flex;
            ClearStatus();
        }

        private async void HandleSignIn()
        {
            if (!ValidateSignInInputs())
                return;

            SetLoading(true);

            try
            {
                var settings = GameFuseSettings.Settings;
                var user = await GameFuseUser.SignInAsync(
                    signInEmail.value.Trim(),
                    signInPassword.value,
                    settings.GameId,
                    settings.GameApiKey
                );

                if (user != null && user.IsAuthenticated())
                {
                    OnSignInSuccess?.Invoke(user);
                }
                else
                {
                    OnSignInError?.Invoke("Sign in failed - invalid response");
                }
            }
            catch (Exception ex)
            {
                OnSignInError?.Invoke($"Sign in failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleSignUp()
        {
            if (!ValidateSignUpInputs())
                return;

            SetLoading(true);

            try
            {
                var settings = GameFuseSettings.Settings;
                var user = await GameFuseUser.SignUpAsync(
                    signUpEmail.value.Trim(),
                    signUpPassword.value,
                    signUpUsername.value.Trim(),
                    settings.GameId,
                    settings.GameApiKey
                );

                if (user != null && user.IsAuthenticated())
                {
                    OnSignUpSuccess?.Invoke(user);
                }
                else
                {
                    OnSignUpError?.Invoke("Sign up failed - invalid response");
                }
            }
            catch (Exception ex)
            {
                OnSignUpError?.Invoke($"Sign up failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private bool ValidateSignInInputs()
        {
            if (string.IsNullOrWhiteSpace(signInEmail.value))
            {
                ShowStatus("Please enter your email", true);
                signInEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(signInPassword.value))
            {
                ShowStatus("Please enter your password", true);
                signInPassword.Focus();
                return false;
            }

            var settings = GameFuseSettings.Settings;
            if (settings == null || string.IsNullOrWhiteSpace(settings.GameId) || string.IsNullOrWhiteSpace(settings.GameApiKey))
            {
                ShowStatus("GameFuse settings not configured. Please check the GameFuseSettings asset.", true);
                return false;
            }

            return true;
        }

        private bool ValidateSignUpInputs()
        {
            if (string.IsNullOrWhiteSpace(signUpEmail.value))
            {
                ShowStatus("Please enter your email", true);
                signUpEmail.Focus();
                return false;
            }

            if (!IsValidEmail(signUpEmail.value))
            {
                ShowStatus("Please enter a valid email address", true);
                signUpEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(signUpUsername.value))
            {
                ShowStatus("Please enter a username", true);
                signUpUsername.Focus();
                return false;
            }

            if (signUpUsername.value.Length < 3)
            {
                ShowStatus("Username must be at least 3 characters long", true);
                signUpUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(signUpPassword.value))
            {
                ShowStatus("Please enter a password", true);
                signUpPassword.Focus();
                return false;
            }

            if (signUpPassword.value.Length < 6)
            {
                ShowStatus("Password must be at least 6 characters long", true);
                signUpPassword.Focus();
                return false;
            }

            if (signUpPassword.value != signUpConfirmPassword.value)
            {
                ShowStatus("Passwords do not match", true);
                signUpConfirmPassword.Focus();
                return false;
            }

            var settings = GameFuseSettings.Settings;
            if (settings == null || string.IsNullOrWhiteSpace(settings.GameId) || string.IsNullOrWhiteSpace(settings.GameApiKey))
            {
                ShowStatus("GameFuse settings not configured. Please check the GameFuseSettings asset.", true);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void SetLoading(bool loading)
        {
            loadingIndicator.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;

            // Disable buttons during loading
            signInButton.SetEnabled(!loading);
            signUpButton.SetEnabled(!loading);
        }

        private void ShowStatus(string message, bool isError)
        {
            statusLabel.text = message;
            statusLabel.RemoveFromClassList("status-success");
            statusLabel.RemoveFromClassList("status-error");
            statusLabel.AddToClassList(isError ? "status-error" : "status-success");
            statusLabel.style.display = DisplayStyle.Flex;
        }

        private void ClearStatus()
        {
            statusLabel.style.display = DisplayStyle.None;
            statusLabel.text = "";
        }

        public void Reset()
        {
            // Clear all input fields
            signInEmail.value = "";
            signInPassword.value = "";
            signUpEmail.value = "";
            signUpUsername.value = "";
            signUpPassword.value = "";
            signUpConfirmPassword.value = "";

            // Reset to sign in form
            ShowSignInForm();
            ClearStatus();
            SetLoading(false);
        }
    }
}