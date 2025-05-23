using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFuse.UI.Controls;

namespace GameFuse.UI
{
    /// <summary>
    /// Controls the profile panel - manages user information, attributes, and score/credits.
    /// Covers Phase 2 functionality: user data management and customization.
    /// </summary>
    public class ProfilePanelController
    {
        // Events
        public static event Action<GameFuseUser> OnUserDataUpdated;
        public static event Action<string> OnError;
        public static event Action<string> OnSuccess;

        // UI Elements
        private VisualElement root;
        private ProfilePanel profilePanel;
        private GameFuseUser currentUser;

        // Profile Info Section
        private VisualElement profileInfoSection;
        private Label userIdLabel;
        private Label usernameLabel;
        private Label emailLabel;
        private Label scoreLabel;
        private Label creditsLabel;
        private Label lastLoginLabel;

        // Score Management Section
        private VisualElement scoreManagementSection;
        private TextField scoreInput;
        private Button setScoreButton;
        private Button addScoreButton;
        private VisualElement scoreButtons;

        // Credits Management Section
        private VisualElement creditsManagementSection;
        private TextField creditsInput;
        private Button setCreditsButton;
        private Button addCreditsButton;
        private VisualElement creditsButtons;

        // User Attributes Section
        private VisualElement attributesSection;
        private VisualElement attributesList;
        private TextField newAttributeKeyInput;
        private TextField newAttributeValueInput;
        private Button addAttributeButton;
        private Button refreshAttributesButton;

        // Loading and Status
        private VisualElement loadingIndicator;
        private Label statusLabel;

        public ProfilePanelController(VisualElement panelContainer)
        {
            root = panelContainer;
            profilePanel = root as ProfilePanel;
            
            // Handle both custom control and legacy implementation
            if (profilePanel == null)
            {
                Debug.Log("ProfilePanelController: Using standard VisualElement container");
            }
            else
            {
                Debug.Log("ProfilePanelController: Using ProfilePanel custom control");
                profilePanel.Initialize();
            }
            
            InitializeElements();
            SetupEventHandlers();
        }

        private void InitializeElements()
        {
            // Profile Info Section
            profileInfoSection = root.Q<VisualElement>("profile-info-section");
            userIdLabel = root.Q<Label>("user-id");
            usernameLabel = root.Q<Label>("username");
            emailLabel = root.Q<Label>("email");
            scoreLabel = root.Q<Label>("score");
            creditsLabel = root.Q<Label>("credits");
            lastLoginLabel = root.Q<Label>("last-login");

            // Score Management
            scoreManagementSection = root.Q<VisualElement>("score-management-section");
            scoreInput = root.Q<TextField>("score-input");
            setScoreButton = root.Q<Button>("set-score-button");
            addScoreButton = root.Q<Button>("add-score-button");
            scoreButtons = root.Q<VisualElement>("score-buttons");

            // Credits Management
            creditsManagementSection = root.Q<VisualElement>("credits-management-section");
            creditsInput = root.Q<TextField>("credits-input");
            setCreditsButton = root.Q<Button>("set-credits-button");
            addCreditsButton = root.Q<Button>("add-credits-button");
            creditsButtons = root.Q<VisualElement>("credits-buttons");

            // User Attributes
            attributesSection = root.Q<VisualElement>("attributes-section");
            attributesList = root.Q<VisualElement>("attributes-list");
            newAttributeKeyInput = root.Q<TextField>("new-attribute-key");
            newAttributeValueInput = root.Q<TextField>("new-attribute-value");
            addAttributeButton = root.Q<Button>("add-attribute-button");
            refreshAttributesButton = root.Q<Button>("refresh-attributes-button");

            // Status and Loading
            loadingIndicator = root.Q<VisualElement>("profile-loading");
            statusLabel = root.Q<Label>("profile-status");
        }

        private void SetupEventHandlers()
        {
            // Score Management
            setScoreButton.clicked += HandleSetScore;
            addScoreButton.clicked += HandleAddScore;

            // Credits Management
            setCreditsButton.clicked += HandleSetCredits;
            addCreditsButton.clicked += HandleAddCredits;

            // Attributes Management
            addAttributeButton.clicked += HandleAddAttribute;
            refreshAttributesButton.clicked += HandleRefreshAttributes;

            // Input validation
            scoreInput.RegisterValueChangedCallback(evt => ValidateNumericInput(evt.newValue, scoreInput));
            creditsInput.RegisterValueChangedCallback(evt => ValidateNumericInput(evt.newValue, creditsInput));

            // Enter key handling for quick actions
            newAttributeValueInput.RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    HandleAddAttribute();
            });
        }

        public void SetCurrentUser(GameFuseUser user)
        {
            currentUser = user;
            UpdateUserDisplay();
            RefreshUserAttributes();
        }

        private void UpdateUserDisplay()
        {
            if (currentUser == null) return;

            userIdLabel.text = $"{currentUser.Id}";
            usernameLabel.text = currentUser.Username;
            emailLabel.text = currentUser.Email;
            scoreLabel.text = $"{currentUser.Score:N0}";
            creditsLabel.text = $"{currentUser.Credits:N0}";
            lastLoginLabel.text = currentUser.LastLogin ?? "N/A";
            // Login count removed as requested
        }

        private async void HandleSetScore()
        {
            if (!ValidateScoreInput()) return;

            var newScore = int.Parse(scoreInput.value);
            SetLoading(true);

            try
            {
                var updatedUser = await currentUser.SetScoreAsync(newScore);
                currentUser = new GameFuseUser(updatedUser); // Update the current user with new data
                UpdateUserDisplay();
                ShowStatus($"Score set to {newScore:N0}", false);
                OnUserDataUpdated?.Invoke(currentUser);
                scoreInput.value = "";
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to set score: {ex.Message}", true);
                OnError?.Invoke($"Set score failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleAddScore()
        {
            if (!ValidateScoreInput()) return;

            var scoreToAdd = int.Parse(scoreInput.value);
            SetLoading(true);

            try
            {
                var updatedUser = await currentUser.AddScoreAsync(scoreToAdd);
                currentUser = new GameFuseUser(updatedUser); // Update the current user with new data
                UpdateUserDisplay();
                ShowStatus($"Added {scoreToAdd:N0} to score", false);
                OnUserDataUpdated?.Invoke(currentUser);
                scoreInput.value = "";
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to add score: {ex.Message}", true);
                OnError?.Invoke($"Add score failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleSetCredits()
        {
            if (!ValidateCreditsInput()) return;

            var newCredits = int.Parse(creditsInput.value);
            SetLoading(true);

            try
            {
                var updatedUser = await currentUser.SetCreditsAsync(newCredits);
                currentUser = new GameFuseUser(updatedUser); // Update the current user with new data
                UpdateUserDisplay();
                ShowStatus($"Credits set to {newCredits:N0}", false);
                OnUserDataUpdated?.Invoke(currentUser);
                creditsInput.value = "";
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to set credits: {ex.Message}", true);
                OnError?.Invoke($"Set credits failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleAddCredits()
        {
            if (!ValidateCreditsInput()) return;

            var creditsToAdd = int.Parse(creditsInput.value);
            SetLoading(true);

            try
            {
                var updatedUser = await currentUser.AddCreditsAsync(creditsToAdd);
                currentUser = new GameFuseUser(updatedUser); // Update the current user with new data
                UpdateUserDisplay();
                ShowStatus($"Added {creditsToAdd:N0} credits", false);
                OnUserDataUpdated?.Invoke(currentUser);
                creditsInput.value = "";
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to add credits: {ex.Message}", true);
                OnError?.Invoke($"Add credits failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleAddAttribute()
        {
            if (!ValidateAttributeInput()) return;

            var key = newAttributeKeyInput.value.Trim();
            var value = newAttributeValueInput.value.Trim();

            SetLoading(true);

            try
            {
                await currentUser.SetUserAttributeAsync(key, value);
                ShowStatus($"Attribute '{key}' added successfully", false);
                OnSuccess?.Invoke($"Attribute '{key}' added");

                // Clear inputs and refresh display
                newAttributeKeyInput.value = "";
                newAttributeValueInput.value = "";
                await RefreshUserAttributes();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to add attribute: {ex.Message}", true);
                OnError?.Invoke($"Add attribute failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleRefreshAttributes()
        {
            await RefreshUserAttributes();
        }

        private async Task RefreshUserAttributes()
        {
            if (currentUser == null) return;

            SetLoading(true);

            try
            {
                var userAttributes = await currentUser.GetUserAttributesAsync();
                DisplayUserAttributes(userAttributes.Attributes);
                ShowStatus("Attributes refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to refresh attributes: {ex.Message}", true);
                OnError?.Invoke($"Refresh attributes failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void DisplayUserAttributes(List<UserAttribute> attributes)
        {
            attributesList.Clear();

            if (attributes == null || attributes.Count == 0)
            {
                var noAttributesLabel = new Label("No custom attributes found");
                noAttributesLabel.AddToClassList("no-data-label");
                attributesList.Add(noAttributesLabel);
                return;
            }

            foreach (var attribute in attributes)
            {
                var attributeRow = CreateAttributeRow(attribute);
                attributesList.Add(attributeRow);
            }
        }

        private VisualElement CreateAttributeRow(UserAttribute attribute)
        {
            var row = new VisualElement();
            row.AddToClassList("attribute-row");

            var keyLabel = new Label(attribute.Key);
            keyLabel.AddToClassList("attribute-key");

            var valueLabel = new Label(attribute.Value);
            valueLabel.AddToClassList("attribute-value");

            var deleteButton = new Button(() => HandleDeleteAttribute(attribute.Key));
            deleteButton.text = "Delete";
            deleteButton.AddToClassList("delete-attribute-button");

            row.Add(keyLabel);
            row.Add(valueLabel);
            row.Add(deleteButton);

            return row;
        }

        private async void HandleDeleteAttribute(string key)
        {
            SetLoading(true);

            try
            {
                await currentUser.DeleteUserAttributeAsync(key);
                ShowStatus($"Attribute '{key}' deleted successfully", false);
                OnSuccess?.Invoke($"Attribute '{key}' deleted");
                await RefreshUserAttributes();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to delete attribute: {ex.Message}", true);
                OnError?.Invoke($"Delete attribute failed: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }


        private bool ValidateScoreInput()
        {
            if (string.IsNullOrWhiteSpace(scoreInput.value))
            {
                ShowStatus("Please enter a score value", true);
                scoreInput.Focus();
                return false;
            }

            if (!int.TryParse(scoreInput.value, out int score) || score < 0)
            {
                ShowStatus("Please enter a valid positive number", true);
                scoreInput.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateCreditsInput()
        {
            if (string.IsNullOrWhiteSpace(creditsInput.value))
            {
                ShowStatus("Please enter a credits value", true);
                creditsInput.Focus();
                return false;
            }

            if (!int.TryParse(creditsInput.value, out int credits) || credits < 0)
            {
                ShowStatus("Please enter a valid positive number", true);
                creditsInput.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateAttributeInput()
        {
            if (string.IsNullOrWhiteSpace(newAttributeKeyInput.value))
            {
                ShowStatus("Please enter an attribute key", true);
                newAttributeKeyInput.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(newAttributeValueInput.value))
            {
                ShowStatus("Please enter an attribute value", true);
                newAttributeValueInput.Focus();
                return false;
            }

            return true;
        }

        private void ValidateNumericInput(string value, TextField field)
        {
            if (string.IsNullOrEmpty(value)) return;

            if (!int.TryParse(value, out int result) || result < 0)
            {
                // Remove invalid characters
                var validValue = new string(value.Where(c => char.IsDigit(c)).ToArray());
                field.SetValueWithoutNotify(validValue);
            }
        }

        private void SetLoading(bool loading)
        {
            if (loadingIndicator != null)
                loadingIndicator.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;

            // Disable buttons during loading
            setScoreButton?.SetEnabled(!loading);
            addScoreButton?.SetEnabled(!loading);
            setCreditsButton?.SetEnabled(!loading);
            addCreditsButton?.SetEnabled(!loading);
            addAttributeButton?.SetEnabled(!loading);
            refreshAttributesButton?.SetEnabled(!loading);
        }

        private void ShowStatus(string message, bool isError)
        {
            if (statusLabel == null) return;

            statusLabel.text = message;
            statusLabel.RemoveFromClassList("status-success");
            statusLabel.RemoveFromClassList("status-error");
            statusLabel.AddToClassList(isError ? "status-error" : "status-success");
            statusLabel.style.display = DisplayStyle.Flex;

            // Auto-hide after 3 seconds
            _ = Task.Delay(3000).ContinueWith(_ => {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (statusLabel != null)
                        statusLabel.style.display = DisplayStyle.None;
                });
            });
        }

        public void Reset()
        {
            currentUser = null;

            // Clear all inputs
            scoreInput.value = "";
            creditsInput.value = "";
            newAttributeKeyInput.value = "";
            newAttributeValueInput.value = "";

            // Clear displays
            attributesList.Clear();

            // Hide status
            if (statusLabel != null)
                statusLabel.style.display = DisplayStyle.None;

            SetLoading(false);
        }
    }
}