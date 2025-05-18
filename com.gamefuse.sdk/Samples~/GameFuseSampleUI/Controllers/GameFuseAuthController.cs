using System;
using System.Threading.Tasks;
using GameFuse;
using GameFuse.Config;
using GameFuse.Exceptions;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.Samples
{
    /// <summary>
    /// Controller for the GameFuse authentication UI.
    /// Handles sign-in, sign-up, and forgot password functionality.
    /// </summary>
    public class GameFuseAuthController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private GameFuseDataController _dataController;
        
        private VisualElement _authPanel;
        private VisualElement _signInPanel;
        private VisualElement _signUpPanel;
        private VisualElement _forgotPasswordPanel;
        private VisualElement _messagePanel;
        private Label _messageText;
        
        private TextField _usernameInput;
        private TextField _passwordInput;
        private TextField _signUpEmailInput;
        private TextField _signUpUsernameInput;
        private TextField _signUpPasswordInput;
        private TextField _signUpConfirmPasswordInput;
        private TextField _forgotPasswordEmailInput;
        
        private Button _signInButton;
        private Button _signUpToggleButton;
        private Button _signInToggleButton;
        private Button _signUpButton;
        private Button _forgotPasswordButton;
        private Button _resetPasswordButton;
        private Button _forgotPasswordCancelButton;

        private void OnEnable()
        {
            BindUI();
            SetupEventHandlers();
        }

        private void Start()
        {
            // If user is already authenticated, hide auth panel and show data panel
            if (GameFuseUser.IsAuthenticated())
            {
                HideAuthPanel();
                _dataController.ShowDataPanel();
            }
        }

        private void BindUI()
        {
            var root = _document.rootVisualElement;
            
            _authPanel = root.Q<VisualElement>("auth-panel");
            _signInPanel = root.Q<VisualElement>("sign-in-panel");
            _signUpPanel = root.Q<VisualElement>("sign-up-panel");
            _forgotPasswordPanel = root.Q<VisualElement>("forgot-password-panel");
            _messagePanel = root.Q<VisualElement>("message-panel");
            _messageText = root.Q<Label>("message-text");
            
            _usernameInput = root.Q<TextField>("username-input");
            _passwordInput = root.Q<TextField>("password-input");
            _signUpEmailInput = root.Q<TextField>("sign-up-email-input");
            _signUpUsernameInput = root.Q<TextField>("sign-up-username-input");
            _signUpPasswordInput = root.Q<TextField>("sign-up-password-input");
            _signUpConfirmPasswordInput = root.Q<TextField>("sign-up-confirm-password-input");
            _forgotPasswordEmailInput = root.Q<TextField>("forgot-password-email-input");
            
            _signInButton = root.Q<Button>("sign-in-button");
            _signUpToggleButton = root.Q<Button>("sign-up-toggle-button");
            _signInToggleButton = root.Q<Button>("sign-in-toggle-button");
            _signUpButton = root.Q<Button>("sign-up-button");
            _forgotPasswordButton = root.Q<Button>("forgot-password-button");
            _resetPasswordButton = root.Q<Button>("reset-password-button");
            _forgotPasswordCancelButton = root.Q<Button>("forgot-password-cancel-button");
        }

        private void SetupEventHandlers()
        {
            _signInButton.clicked += OnSignInClicked;
            _signUpToggleButton.clicked += OnSignUpToggleClicked;
            _signInToggleButton.clicked += OnSignInToggleClicked;
            _signUpButton.clicked += OnSignUpClicked;
            _forgotPasswordButton.clicked += OnForgotPasswordClicked;
            _resetPasswordButton.clicked += OnResetPasswordClicked;
            _forgotPasswordCancelButton.clicked += OnForgotPasswordCancelClicked;
        }

        public void ShowAuthPanel()
        {
            _authPanel.style.display = DisplayStyle.Flex;
            ShowPanel(_signInPanel);
            HidePanel(_signUpPanel);
            HidePanel(_forgotPasswordPanel);
            HideMessage();
            ClearInputs();
        }

        public void HideAuthPanel()
        {
            _authPanel.style.display = DisplayStyle.None;
        }

        private async void OnSignInClicked()
        {
            string username = _usernameInput.value;
            string password = _passwordInput.value;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Please enter both username/email and password.", true);
                return;
            }
            
            try
            {
                SetUIInteractable(false);
                ShowMessage("Signing in...", false);
                
                var settings = GameFuseSettings.Settings;
                if (settings == null || string.IsNullOrEmpty(settings.GameId) || string.IsNullOrEmpty(settings.GameApiKey))
                {
                    ShowMessage("GameFuse settings not found. Please set up GameFuseSettings in your project.", true);
                    SetUIInteractable(true);
                    return;
                }
                
                var user = await GameFuseUser.SignInAsync(username, password);
                
                ShowMessage("Sign in successful!", false);
                HideAuthPanel();
                _dataController.ShowDataPanel();
                _dataController.UpdateUserInfo();
            }
            catch (GameFuseApiException ex)
            {
                ShowMessage($"Sign in failed: {ex.Message}", true);
            }
            catch (Exception ex)
            {
                ShowMessage($"An error occurred: {ex.Message}", true);
            }
            finally
            {
                SetUIInteractable(true);
            }
        }

        private async void OnSignUpClicked()
        {
            string email = _signUpEmailInput.value;
            string username = _signUpUsernameInput.value;
            string password = _signUpPasswordInput.value;
            string confirmPassword = _signUpConfirmPasswordInput.value;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("Please fill in all fields.", true);
                return;
            }
            
            if (password != confirmPassword)
            {
                ShowMessage("Passwords do not match.", true);
                return;
            }
            
            try
            {
                SetUIInteractable(false);
                ShowMessage("Creating account...", false);
                
                var settings = GameFuseSettings.Settings;
                if (settings == null || string.IsNullOrEmpty(settings.GameId) || string.IsNullOrEmpty(settings.GameApiKey))
                {
                    ShowMessage("GameFuse settings not found. Please set up GameFuseSettings in your project.", true);
                    SetUIInteractable(true);
                    return;
                }
                
                var user = await GameFuseUser.SignUpAsync(email, password, username);
                
                ShowMessage("Account created successfully!", false);
                HideAuthPanel();
                _dataController.ShowDataPanel();
                _dataController.UpdateUserInfo();
            }
            catch (GameFuseApiException ex)
            {
                ShowMessage($"Sign up failed: {ex.Message}", true);
            }
            catch (Exception ex)
            {
                ShowMessage($"An error occurred: {ex.Message}", true);
            }
            finally
            {
                SetUIInteractable(true);
            }
        }

        private async void OnResetPasswordClicked()
        {
            string email = _forgotPasswordEmailInput.value;
            
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("Please enter your email address.", true);
                return;
            }
            
            try
            {
                SetUIInteractable(false);
                ShowMessage("Sending password reset email...", false);
                
                var settings = GameFuseSettings.Settings;
                if (settings == null || string.IsNullOrEmpty(settings.GameId) || string.IsNullOrEmpty(settings.GameApiKey))
                {
                    ShowMessage("GameFuse settings not found. Please set up GameFuseSettings in your project.", true);
                    SetUIInteractable(true);
                    return;
                }
                
                await GameFuseUser.ForgotPasswordAsync(email);
                
                ShowMessage("Password reset email sent! Please check your inbox.", false);
                
                // Return to sign in panel after a delay
                await Task.Delay(3000);
                OnSignInToggleClicked();
            }
            catch (GameFuseApiException ex)
            {
                ShowMessage($"Password reset failed: {ex.Message}", true);
            }
            catch (Exception ex)
            {
                ShowMessage($"An error occurred: {ex.Message}", true);
            }
            finally
            {
                SetUIInteractable(true);
            }
        }

        private void OnSignUpToggleClicked()
        {
            ShowPanel(_signUpPanel);
            HidePanel(_signInPanel);
            HidePanel(_forgotPasswordPanel);
            HideMessage();
        }

        private void OnSignInToggleClicked()
        {
            ShowPanel(_signInPanel);
            HidePanel(_signUpPanel);
            HidePanel(_forgotPasswordPanel);
            HideMessage();
        }

        private void OnForgotPasswordClicked()
        {
            ShowPanel(_forgotPasswordPanel);
            HidePanel(_signInPanel);
            HidePanel(_signUpPanel);
            HideMessage();
        }

        private void OnForgotPasswordCancelClicked()
        {
            OnSignInToggleClicked();
        }

        private void ShowPanel(VisualElement panel)
        {
            panel.style.display = DisplayStyle.Flex;
        }

        private void HidePanel(VisualElement panel)
        {
            panel.style.display = DisplayStyle.None;
        }

        private void ShowMessage(string message, bool isError)
        {
            _messagePanel.style.display = DisplayStyle.Flex;
            _messageText.text = message;
            
            if (isError)
            {
                _messagePanel.style.backgroundColor = new Color(0.6f, 0.2f, 0.2f);
            }
            else
            {
                _messagePanel.style.backgroundColor = new Color(0.2f, 0.6f, 0.2f);
            }
        }

        private void HideMessage()
        {
            _messagePanel.style.display = DisplayStyle.None;
        }

        private void SetUIInteractable(bool interactable)
        {
            _signInButton.SetEnabled(interactable);
            _signUpButton.SetEnabled(interactable);
            _resetPasswordButton.SetEnabled(interactable);
            _signUpToggleButton.SetEnabled(interactable);
            _signInToggleButton.SetEnabled(interactable);
            _forgotPasswordButton.SetEnabled(interactable);
            _forgotPasswordCancelButton.SetEnabled(interactable);
        }

        private void ClearInputs()
        {
            _usernameInput.value = "";
            _passwordInput.value = "";
            _signUpEmailInput.value = "";
            _signUpUsernameInput.value = "";
            _signUpPasswordInput.value = "";
            _signUpConfirmPasswordInput.value = "";
            _forgotPasswordEmailInput.value = "";
        }
    }
}