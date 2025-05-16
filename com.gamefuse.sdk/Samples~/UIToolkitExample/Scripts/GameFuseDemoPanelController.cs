using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuseCSharp;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkitDemo
{
    public class GameFuseDemoPanelController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        
        // UI Elements
        private VisualElement rootVisualElement;
        private VisualElement loadingOverlay;
        private ScrollView logScrollView;
        
        // Tab Elements
        private List<Button> tabButtons = new List<Button>();
        private List<VisualElement> tabContents = new List<VisualElement>();
        
        // Current tab state
        private int currentTabIndex = 0;

        private void OnEnable()
        {
            rootVisualElement = uiDocument.rootVisualElement;
            
            InitializeUIElements();
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        #region UI Initialization

        private void InitializeUIElements()
        {
            // Get main UI elements
            loadingOverlay = rootVisualElement.Q<VisualElement>("loading-overlay");
            logScrollView = rootVisualElement.Q<ScrollView>("log-scroll");
            
            // Initialize tabs
            tabButtons.Add(rootVisualElement.Q<Button>("tab-game-setup"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-authentication"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-user-management"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-store"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-leaderboards"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-game-rounds"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-groups"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-chats"));
            tabButtons.Add(rootVisualElement.Q<Button>("tab-friends"));
            
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-game-setup"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-authentication"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-user-management"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-store"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-leaderboards"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-game-rounds"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-groups"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-chats"));
            tabContents.Add(rootVisualElement.Q<VisualElement>("content-friends"));
            
            // Initialize Game Setup UI elements and buttons
            InitializeGameSetupUI();
            
            // Initialize Authentication UI elements and buttons
            InitializeAuthenticationUI();
            
            // Initialize User Management UI elements and buttons
            InitializeUserManagementUI();
            
            // Initialize Store UI elements and buttons
            InitializeStoreUI();
            
            // Initialize Leaderboards UI elements and buttons
            InitializeLeaderboardsUI();
            
            // Initialize Game Rounds UI elements and buttons
            InitializeGameRoundsUI();
            
            // Initialize Groups UI elements and buttons
            InitializeGroupsUI();
            
            // Initialize Chats UI elements and buttons
            InitializeChatsUI();
            
            // Initialize Friends UI elements and buttons
            InitializeFriendsUI();
        }

        private void RegisterCallbacks()
        {
            // Register tab button callbacks
            for (int i = 0; i < tabButtons.Count; i++)
            {
                int index = i; // Capture the index for the lambda
                tabButtons[i].RegisterCallback<ClickEvent>((evt) => SwitchTab(index));
            }
            
            // Register Game Setup button callbacks
            RegisterGameSetupCallbacks();
            
            // Register Authentication button callbacks
            RegisterAuthenticationCallbacks();
            
            // Register User Management button callbacks
            RegisterUserManagementCallbacks();
            
            // Register Store button callbacks
            RegisterStoreCallbacks();
            
            // Register Leaderboards button callbacks
            RegisterLeaderboardsCallbacks();
            
            // Register Game Rounds button callbacks
            RegisterGameRoundsCallbacks();
            
            // Register Groups button callbacks
            RegisterGroupsCallbacks();
            
            // Register Chats button callbacks
            RegisterChatsCallbacks();
            
            // Register Friends button callbacks
            RegisterFriendsCallbacks();
        }

        private void UnregisterCallbacks()
        {
            // Unregister tab button callbacks
            for (int i = 0; i < tabButtons.Count; i++)
            {
                int index = i;
                tabButtons[i].UnregisterCallback<ClickEvent>((evt) => SwitchTab(index));
            }
            
            // Unregister specific button callbacks
            // ... (will be implemented as we add functionality)
        }

        #endregion

        #region Tab Management

        private void SwitchTab(int tabIndex)
        {
            // Deselect current tab
            tabButtons[currentTabIndex].RemoveFromClassList("tab-button-selected");
            tabContents[currentTabIndex].style.display = DisplayStyle.None;
            
            // Select new tab
            currentTabIndex = tabIndex;
            tabButtons[currentTabIndex].AddToClassList("tab-button-selected");
            tabContents[currentTabIndex].style.display = DisplayStyle.Flex;
        }

        #endregion

        #region Loading and Logging

        private void ShowLoading(bool show)
        {
            loadingOverlay.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void LogMessage(string message, LogType type = LogType.Normal)
        {
            Label logEntry = new Label(message);
            logEntry.AddToClassList("log-entry");
            
            switch (type)
            {
                case LogType.Success:
                    logEntry.AddToClassList("success-message");
                    break;
                case LogType.Error:
                    logEntry.AddToClassList("error-message");
                    break;
            }
            
            logScrollView.Add(logEntry);
            logScrollView.scrollOffset = new Vector2(0, float.MaxValue); // Scroll to bottom
        }

        public enum LogType
        {
            Normal,
            Success,
            Error
        }

        #endregion

        #region UI Utility Methods

        private VisualElement CreateListItem(string title, Dictionary<string, string> properties = null)
        {
            // Use VisualTreeAsset to instantiate the template
            var template = Resources.Load<VisualTreeAsset>("UIToolkitExample/UXML/ListItemTemplate");
            VisualElement listItem = template.Instantiate();
            
            // Set the title
            listItem.Q<Label>("item-title").text = title;
            
            // Add properties if provided
            if (properties != null)
            {
                VisualElement propertiesContainer = listItem.Q<VisualElement>("item-properties");
                
                foreach (var property in properties)
                {
                    Label propertyLabel = new Label($"{property.Key}: {property.Value}");
                    propertyLabel.AddToClassList("list-item-property");
                    propertiesContainer.Add(propertyLabel);
                }
            }
            
            return listItem;
        }

        private void ClearScrollView(ScrollView scrollView)
        {
            scrollView.Clear();
        }

        private async Task ExecuteAsync(Func<Task> action, string loadingMessage = "Loading...")
        {
            try
            {
                ShowLoading(true);
                LogMessage(loadingMessage);
                
                await action();
                
                ShowLoading(false);
            }
            catch (ApiException ex)
            {
                ShowLoading(false);
                LogMessage($"API Error: {ex.Message}", LogType.Error);
                Debug.LogException(ex);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                LogMessage($"Error: {ex.Message}", LogType.Error);
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Game Setup Implementation

        private TextField gameIdField;
        private TextField gameTokenField;
        private Label gameNameLabel;
        private Label gameDescriptionLabel;
        private ScrollView gameVariablesScrollView;
        private ScrollView gameStoreItemsScrollView;
        private Button setupGameButton;
        private Button getServerTimeButton;
        private Button fetchGameVariablesButton;
        private Button getStoreItemsButton;

        private void InitializeGameSetupUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-game-setup");
            
            gameIdField = content.Q<TextField>("game-id");
            gameTokenField = content.Q<TextField>("game-token");
            gameNameLabel = content.Q<Label>("game-name-label");
            gameDescriptionLabel = content.Q<Label>("game-description-label");
            gameVariablesScrollView = content.Q<ScrollView>("game-variables-scroll");
            gameStoreItemsScrollView = content.Q<ScrollView>("game-store-items-scroll");
            
            setupGameButton = content.Q<Button>("setup-game-button");
            getServerTimeButton = content.Q<Button>("get-server-time-button");
            fetchGameVariablesButton = content.Q<Button>("fetch-game-variables-button");
            getStoreItemsButton = content.Q<Button>("get-store-items-button");
        }

        private void RegisterGameSetupCallbacks()
        {
            setupGameButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetupGameClicked());
            getServerTimeButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetServerTimeClicked());
            fetchGameVariablesButton.RegisterCallback<ClickEvent>(async (evt) => await OnFetchGameVariablesClicked());
            getStoreItemsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetStoreItemsClicked());
        }

        private async Task OnSetupGameClicked()
        {
            string gameId = gameIdField.value;
            string gameToken = gameTokenField.value;
            
            if (string.IsNullOrEmpty(gameId) || string.IsNullOrEmpty(gameToken))
            {
                LogMessage("Game ID and Game Token are required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set up GameFuse with the provided game ID and token
                GameFuse.Init(gameId, gameToken);
                
                // Update game info display
                gameNameLabel.text = GameFuse.GameName;
                gameDescriptionLabel.text = GameFuse.GameDescription;
                
                LogMessage($"Game '{GameFuse.GameName}' setup successful", LogType.Success);
            });
        }

        private async Task OnGetServerTimeClicked()
        {
            await ExecuteAsync(async () =>
            {
                var timeResponse = await GameFuse.GetServerTimeAsync();
                LogMessage($"Server Time: {timeResponse.server_time}", LogType.Success);
            });
        }

        private async Task OnFetchGameVariablesClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Get game variables
                var variables = await GameFuse.GetGameVariablesAsync();
                
                // Clear current list
                ClearScrollView(gameVariablesScrollView);
                
                // Display variables
                if (variables != null && variables.Count > 0)
                {
                    foreach (var variable in variables)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "Value", variable.Value }
                        };
                        
                        gameVariablesScrollView.Add(CreateListItem(variable.Key, properties));
                    }
                    
                    LogMessage("Game variables fetched successfully", LogType.Success);
                }
                else
                {
                    LogMessage("No game variables found", LogType.Normal);
                }
            });
        }

        private async Task OnGetStoreItemsClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Get store items
                var storeItems = await GameFuse.GetStoreItemsAsync();
                
                // Clear current list
                ClearScrollView(gameStoreItemsScrollView);
                
                // Display store items
                if (storeItems != null && storeItems.Count > 0)
                {
                    foreach (var item in storeItems)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "ID", item.id },
                            { "Name", item.name },
                            { "Description", item.description },
                            { "Cost", item.cost.ToString() }
                        };
                        
                        gameStoreItemsScrollView.Add(CreateListItem(item.name, properties));
                    }
                    
                    LogMessage($"Retrieved {storeItems.Count} store items", LogType.Success);
                }
                else
                {
                    LogMessage("No store items found", LogType.Normal);
                }
            });
        }

        #endregion

        #region Authentication Implementation

        private TextField signupEmailField;
        private TextField signupUsernameField;
        private TextField signupPasswordField;
        private TextField signupConfirmPasswordField;
        private TextField signinEmailField;
        private TextField signinPasswordField;
        private TextField forgotPasswordEmailField;
        private Button signupButton;
        private Button signinButton;
        private Button signoutButton;
        private Button resetPasswordButton;
        private ScrollView currentUserInfoScrollView;

        private void InitializeAuthenticationUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-authentication");
            
            signupEmailField = content.Q<TextField>("signup-email");
            signupUsernameField = content.Q<TextField>("signup-username");
            signupPasswordField = content.Q<TextField>("signup-password");
            signupConfirmPasswordField = content.Q<TextField>("signup-confirm-password");
            
            signinEmailField = content.Q<TextField>("signin-email");
            signinPasswordField = content.Q<TextField>("signin-password");
            
            forgotPasswordEmailField = content.Q<TextField>("forgot-password-email");
            
            signupButton = content.Q<Button>("signup-button");
            signinButton = content.Q<Button>("signin-button");
            signoutButton = content.Q<Button>("signout-button");
            resetPasswordButton = content.Q<Button>("reset-password-button");
            
            currentUserInfoScrollView = content.Q<ScrollView>("current-user-info-scroll");
            
            // Initially disable signout button as user is not signed in
            signoutButton.SetEnabled(false);
        }

        private void RegisterAuthenticationCallbacks()
        {
            signupButton.RegisterCallback<ClickEvent>(async (evt) => await OnSignUpClicked());
            signinButton.RegisterCallback<ClickEvent>(async (evt) => await OnSignInClicked());
            signoutButton.RegisterCallback<ClickEvent>(async (evt) => await OnSignOutClicked());
            resetPasswordButton.RegisterCallback<ClickEvent>(async (evt) => await OnResetPasswordClicked());
        }

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
                // Create signup request
                var signUpRequest = new SignUpRequest
                {
                    email = email,
                    username = username,
                    password = password
                };
                
                // Execute signup
                await GameFuseUser.SignUpAsync(signUpRequest);
                
                LogMessage($"User {username} signed up successfully", LogType.Success);
                
                // Sign in the user automatically
                await SignInUserAsync(email, password);
            });
        }

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

        private async Task SignInUserAsync(string email, string password)
        {
            await ExecuteAsync(async () =>
            {
                // Create signin request
                var signInRequest = new SignInRequest
                {
                    email = email,
                    password = password
                };
                
                // Execute signin
                var response = await GameFuseUser.SignInAsync(signInRequest);
                
                // Update UI state for signed in user
                signoutButton.SetEnabled(true);
                
                // Display current user info
                UpdateCurrentUserInfo();
                
                LogMessage($"User {GameFuseUser.CurrentUser.username} signed in successfully", LogType.Success);
            });
        }

        private async Task OnSignOutClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Sign out the user
                GameFuseUser.SignOut();
                
                // Update UI state
                signoutButton.SetEnabled(false);
                
                // Clear current user info
                ClearScrollView(currentUserInfoScrollView);
                
                LogMessage("User signed out successfully", LogType.Success);
            });
        }

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
                var response = await GameFuseUser.ForgotPasswordAsync(email);
                
                LogMessage("Password reset email sent successfully", LogType.Success);
            });
        }

        private void UpdateCurrentUserInfo()
        {
            // Clear current info
            ClearScrollView(currentUserInfoScrollView);
            
            if (GameFuseUser.CurrentUser != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", GameFuseUser.CurrentUser.id },
                    { "Username", GameFuseUser.CurrentUser.username },
                    { "Email", GameFuseUser.CurrentUser.email },
                    { "Credits", GameFuseUser.CurrentUser.credits.ToString() },
                    { "Score", GameFuseUser.CurrentUser.score.ToString() },
                    { "Created", GameFuseUser.CurrentUser.created }
                };
                
                currentUserInfoScrollView.Add(CreateListItem("Current User", properties));
            }
        }

        #endregion

        #region User Management Implementation

        private TextField creditsAmountField;
        private TextField scoreAmountField;
        private TextField attributeKeyField;
        private TextField attributeValueField;
        private TextField multipleAttributesField;
        private Button addCreditsButton;
        private Button setCreditsButton;
        private Button addScoreButton;
        private Button setScoreButton;
        private Button setAttributeButton;
        private Button removeAttributeButton;
        private Button setMultipleAttributesButton;
        private Button getAttributesButton;
        private ScrollView userAttributesScrollView;

        private void InitializeUserManagementUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-user-management");
            
            creditsAmountField = content.Q<TextField>("credits-amount");
            scoreAmountField = content.Q<TextField>("score-amount");
            attributeKeyField = content.Q<TextField>("attribute-key");
            attributeValueField = content.Q<TextField>("attribute-value");
            multipleAttributesField = content.Q<TextField>("multiple-attributes");
            
            addCreditsButton = content.Q<Button>("add-credits-button");
            setCreditsButton = content.Q<Button>("set-credits-button");
            addScoreButton = content.Q<Button>("add-score-button");
            setScoreButton = content.Q<Button>("set-score-button");
            setAttributeButton = content.Q<Button>("set-attribute-button");
            removeAttributeButton = content.Q<Button>("remove-attribute-button");
            setMultipleAttributesButton = content.Q<Button>("set-multiple-attributes-button");
            getAttributesButton = content.Q<Button>("get-attributes-button");
            
            userAttributesScrollView = content.Q<ScrollView>("user-attributes-scroll");
        }

        private void RegisterUserManagementCallbacks()
        {
            addCreditsButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddCreditsClicked());
            setCreditsButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetCreditsClicked());
            addScoreButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddScoreClicked());
            setScoreButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetScoreClicked());
            setAttributeButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetAttributeClicked());
            removeAttributeButton.RegisterCallback<ClickEvent>(async (evt) => await OnRemoveAttributeClicked());
            setMultipleAttributesButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetMultipleAttributesClicked());
            getAttributesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetAttributesClicked());
        }

        private async Task OnAddCreditsClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage credits", LogType.Error);
                return;
            }
            
            if (!int.TryParse(creditsAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Add credits to the user
                var response = await GameFuseUser.CurrentUser.AddCreditsAsync(amount);
                
                // Update UI
                UpdateCurrentUserInfo();
                
                LogMessage($"Added {amount} credits. New total: {response.credits}", LogType.Success);
            });
        }
        
        private async Task OnSetCreditsClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage credits", LogType.Error);
                return;
            }
            
            if (!int.TryParse(creditsAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set user credits to specific amount
                var response = await GameFuseUser.CurrentUser.SetCreditsAsync(amount);
                
                // Update UI
                UpdateCurrentUserInfo();
                
                LogMessage($"Credits set to {response.credits}", LogType.Success);
            });
        }
        
        private async Task OnAddScoreClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage score", LogType.Error);
                return;
            }
            
            if (!int.TryParse(scoreAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Add score to the user
                var response = await GameFuseUser.CurrentUser.AddScoreAsync(amount);
                
                // Update UI
                UpdateCurrentUserInfo();
                
                LogMessage($"Added {amount} to score. New total: {response.score}", LogType.Success);
            });
        }
        
        private async Task OnSetScoreClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage score", LogType.Error);
                return;
            }
            
            if (!int.TryParse(scoreAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set user score to specific amount
                var response = await GameFuseUser.CurrentUser.SetScoreAsync(amount);
                
                // Update UI
                UpdateCurrentUserInfo();
                
                LogMessage($"Score set to {response.score}", LogType.Success);
            });
        }
        
        private async Task OnSetAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage attributes", LogType.Error);
                return;
            }
            
            string key = attributeKeyField.value;
            string value = attributeValueField.value;
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set a single attribute
                var response = await GameFuseUser.CurrentUser.SetAttributeAsync(key, value);
                
                // Update UI
                await OnGetAttributesClicked();
                
                LogMessage($"Attribute '{key}' set successfully", LogType.Success);
            });
        }
        
        private async Task OnRemoveAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage attributes", LogType.Error);
                return;
            }
            
            string key = attributeKeyField.value;
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Remove the attribute
                var response = await GameFuseUser.CurrentUser.RemoveAttributeAsync(key);
                
                // Update UI
                await OnGetAttributesClicked();
                
                LogMessage($"Attribute '{key}' removed successfully", LogType.Success);
            });
        }
        
        private async Task OnSetMultipleAttributesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to manage attributes", LogType.Error);
                return;
            }
            
            string jsonAttributes = multipleAttributesField.value;
            
            if (string.IsNullOrEmpty(jsonAttributes))
            {
                LogMessage("Attributes JSON is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                try
                {
                    // Parse the JSON into a dictionary
                    var jsonObject = Boomlagoon.JSON.JSONObject.Parse(jsonAttributes);
                    Dictionary<string, string> attributes = new Dictionary<string, string>();
                    
                    foreach (var key in jsonObject.GetKeys())
                    {
                        var value = jsonObject.GetValue(key);
                        if (value.Type == Boomlagoon.JSON.JSONValueType.String)
                        {
                            attributes[key] = value.Str;
                        }
                        else if (value.Type == Boomlagoon.JSON.JSONValueType.Number)
                        {
                            attributes[key] = value.Number.ToString();
                        }
                        else if (value.Type == Boomlagoon.JSON.JSONValueType.Boolean)
                        {
                            attributes[key] = value.Boolean.ToString();
                        }
                        else
                        {
                            attributes[key] = value.ToString();
                        }
                    }
                    
                    // Set multiple attributes
                    var response = await GameFuseUser.CurrentUser.SetAttributesAsync(attributes);
                    
                    // Update UI
                    await OnGetAttributesClicked();
                    
                    LogMessage($"Set {attributes.Count} attributes successfully", LogType.Success);
                }
                catch (Exception ex)
                {
                    LogMessage($"Error parsing JSON: {ex.Message}", LogType.Error);
                }
            });
        }
        
        private async Task OnGetAttributesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view attributes", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user attributes
                var response = await GameFuseUser.CurrentUser.GetAttributesAsync();
                
                // Clear current attributes
                ClearScrollView(userAttributesScrollView);
                
                // Display attributes
                if (response != null && response.Count > 0)
                {
                    foreach (var attribute in response)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "Value", attribute.Value }
                        };
                        
                        userAttributesScrollView.Add(CreateListItem(attribute.Key, properties));
                    }
                    
                    LogMessage($"Retrieved {response.Count} attributes", LogType.Success);
                }
                else
                {
                    LogMessage("No attributes found", LogType.Normal);
                }
            });
        }

        #endregion

        #region Store Implementation

        private TextField storeItemIdField;
        private Toggle reimburseToggle;
        private Button purchaseStoreItemButton;
        private Button removeStoreItemButton;
        private Button getPurchasedItemsButton;
        private ScrollView purchasedItemsScrollView;

        private void InitializeStoreUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-store");
            
            storeItemIdField = content.Q<TextField>("store-item-id");
            reimburseToggle = content.Q<Toggle>("reimburse-toggle");
            purchaseStoreItemButton = content.Q<Button>("purchase-store-item-button");
            removeStoreItemButton = content.Q<Button>("remove-store-item-button");
            getPurchasedItemsButton = content.Q<Button>("get-purchased-items-button");
            purchasedItemsScrollView = content.Q<ScrollView>("purchased-items-scroll");
        }

        private void RegisterStoreCallbacks()
        {
            purchaseStoreItemButton.RegisterCallback<ClickEvent>(async (evt) => await OnPurchaseStoreItemClicked());
            removeStoreItemButton.RegisterCallback<ClickEvent>(async (evt) => await OnRemoveStoreItemClicked());
            getPurchasedItemsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetPurchasedItemsClicked());
        }
        
        private async Task OnPurchaseStoreItemClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to purchase items", LogType.Error);
                return;
            }
            
            string itemId = storeItemIdField.value;
            
            if (string.IsNullOrEmpty(itemId))
            {
                LogMessage("Store Item ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Purchase the store item
                var response = await GameFuseUser.CurrentUser.PurchaseStoreItemAsync(itemId);
                
                // Update UI
                UpdateCurrentUserInfo();
                await OnGetPurchasedItemsClicked();
                
                LogMessage($"Store item purchased successfully. Remaining credits: {response.credits}", LogType.Success);
            });
        }
        
        private async Task OnRemoveStoreItemClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to remove items", LogType.Error);
                return;
            }
            
            string itemId = storeItemIdField.value;
            bool reimburse = reimburseToggle.value;
            
            if (string.IsNullOrEmpty(itemId))
            {
                LogMessage("Store Item ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Remove the store item
                var response = await GameFuseUser.CurrentUser.RemoveStoreItemAsync(itemId, reimburse);
                
                // Update UI
                UpdateCurrentUserInfo();
                await OnGetPurchasedItemsClicked();
                
                LogMessage($"Store item removed successfully. Credits: {response.credits}", LogType.Success);
            });
        }
        
        private async Task OnGetPurchasedItemsClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view purchased items", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's purchased items
                var response = await GameFuseUser.CurrentUser.GetStoreItemsAsync();
                
                // Clear current items
                ClearScrollView(purchasedItemsScrollView);
                
                // Display purchased items
                if (response != null && response.Count > 0)
                {
                    foreach (var item in response)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "ID", item.id },
                            { "Name", item.name },
                            { "Description", item.description },
                            { "Cost", item.cost.ToString() },
                            { "Purchase Date", item.purchased_date }
                        };
                        
                        purchasedItemsScrollView.Add(CreateListItem(item.name, properties));
                    }
                    
                    LogMessage($"Retrieved {response.Count} purchased items", LogType.Success);
                }
                else
                {
                    LogMessage("No purchased items found", LogType.Normal);
                }
            });
        }

        #endregion

        #region Leaderboards Implementation

        private TextField leaderboardNameField;
        private TextField leaderboardScoreField;
        private TextField leaderboardMetadataField;
        private TextField leaderboardUserIdField;
        private TextField leaderboardLimitField;
        private Button addLeaderboardEntryButton;
        private Button addLeaderboardWithMetadataButton;
        private Button clearLeaderboardEntriesButton;
        private Button getMyLeaderboardEntriesButton;
        private Button getUserLeaderboardEntriesButton;
        private Button getGameLeaderboardEntriesButton;
        private ScrollView leaderboardResultsScrollView;

        private void InitializeLeaderboardsUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-leaderboards");
            
            leaderboardNameField = content.Q<TextField>("leaderboard-name");
            leaderboardScoreField = content.Q<TextField>("leaderboard-score");
            leaderboardMetadataField = content.Q<TextField>("leaderboard-metadata");
            leaderboardUserIdField = content.Q<TextField>("leaderboard-user-id");
            leaderboardLimitField = content.Q<TextField>("leaderboard-limit");
            
            addLeaderboardEntryButton = content.Q<Button>("add-leaderboard-entry-button");
            addLeaderboardWithMetadataButton = content.Q<Button>("add-leaderboard-with-metadata-button");
            clearLeaderboardEntriesButton = content.Q<Button>("clear-leaderboard-entries-button");
            getMyLeaderboardEntriesButton = content.Q<Button>("get-my-leaderboard-entries-button");
            getUserLeaderboardEntriesButton = content.Q<Button>("get-user-leaderboard-entries-button");
            getGameLeaderboardEntriesButton = content.Q<Button>("get-game-leaderboard-entries-button");
            
            leaderboardResultsScrollView = content.Q<ScrollView>("leaderboard-results-scroll");
        }

        private void RegisterLeaderboardsCallbacks()
        {
            addLeaderboardEntryButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddLeaderboardEntryClicked());
            addLeaderboardWithMetadataButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddLeaderboardEntryWithMetadataClicked());
            clearLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnClearLeaderboardEntriesClicked());
            getMyLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetMyLeaderboardEntriesClicked());
            getUserLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetUserLeaderboardEntriesClicked());
            getGameLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGameLeaderboardEntriesClicked());
        }
        
        private async Task OnAddLeaderboardEntryClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to add leaderboard entries", LogType.Error);
                return;
            }
            
            string leaderboardName = leaderboardNameField.value;
            
            if (string.IsNullOrEmpty(leaderboardName))
            {
                LogMessage("Leaderboard name is required", LogType.Error);
                return;
            }
            
            if (!int.TryParse(leaderboardScoreField.value, out int score))
            {
                LogMessage("Score must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Add leaderboard entry
                var response = await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync(leaderboardName, score);
                
                LogMessage("Leaderboard entry added successfully", LogType.Success);
            });
        }
        
        private async Task OnAddLeaderboardEntryWithMetadataClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to add leaderboard entries", LogType.Error);
                return;
            }
            
            string leaderboardName = leaderboardNameField.value;
            string metadata = leaderboardMetadataField.value;
            
            if (string.IsNullOrEmpty(leaderboardName))
            {
                LogMessage("Leaderboard name is required", LogType.Error);
                return;
            }
            
            if (!int.TryParse(leaderboardScoreField.value, out int score))
            {
                LogMessage("Score must be a valid integer", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(metadata))
            {
                LogMessage("Metadata is required for this operation", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Add leaderboard entry with metadata
                var response = await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync(leaderboardName, score, metadata);
                
                LogMessage("Leaderboard entry with metadata added successfully", LogType.Success);
            });
        }
        
        private async Task OnClearLeaderboardEntriesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to clear leaderboard entries", LogType.Error);
                return;
            }
            
            string leaderboardName = leaderboardNameField.value;
            
            if (string.IsNullOrEmpty(leaderboardName))
            {
                LogMessage("Leaderboard name is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Clear leaderboard entries
                var response = await GameFuseUser.CurrentUser.ClearLeaderboardEntriesAsync(leaderboardName);
                
                LogMessage("Leaderboard entries cleared successfully", LogType.Success);
            });
        }
        
        private async Task OnGetMyLeaderboardEntriesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view your leaderboard entries", LogType.Error);
                return;
            }
            
            string leaderboardName = leaderboardNameField.value;
            
            if (string.IsNullOrEmpty(leaderboardName))
            {
                LogMessage("Leaderboard name is required", LogType.Error);
                return;
            }
            
            int limit = 10;
            if (!string.IsNullOrEmpty(leaderboardLimitField.value) && 
                !int.TryParse(leaderboardLimitField.value, out limit))
            {
                LogMessage("Limit must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's leaderboard entries
                var response = await GameFuseUser.CurrentUser.GetMyLeaderboardEntriesAsync(leaderboardName, limit);
                
                // Display leaderboard entries
                DisplayLeaderboardEntries(response);
            });
        }
        
        private async Task OnGetUserLeaderboardEntriesClicked()
        {
            string leaderboardName = leaderboardNameField.value;
            string userId = leaderboardUserIdField.value;
            
            if (string.IsNullOrEmpty(leaderboardName))
            {
                LogMessage("Leaderboard name is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(userId))
            {
                LogMessage("User ID is required", LogType.Error);
                return;
            }
            
            int limit = 10;
            if (!string.IsNullOrEmpty(leaderboardLimitField.value) && 
                !int.TryParse(leaderboardLimitField.value, out limit))
            {
                LogMessage("Limit must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get specified user's leaderboard entries
                var response = await GameFuseUser.GetUserLeaderboardEntriesAsync(leaderboardName, userId, limit);
                
                // Display leaderboard entries
                DisplayLeaderboardEntries(response);
            });
        }
        
        private async Task OnGetGameLeaderboardEntriesClicked()
        {
            string leaderboardName = leaderboardNameField.value;
            
            if (string.IsNullOrEmpty(leaderboardName))
            {
                LogMessage("Leaderboard name is required", LogType.Error);
                return;
            }
            
            int limit = 10;
            if (!string.IsNullOrEmpty(leaderboardLimitField.value) && 
                !int.TryParse(leaderboardLimitField.value, out limit))
            {
                LogMessage("Limit must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get game-wide leaderboard entries
                var response = await GameFuse.GetGameLeaderboardEntriesAsync(leaderboardName, limit);
                
                // Display leaderboard entries
                DisplayLeaderboardEntries(response);
            });
        }
        
        private void DisplayLeaderboardEntries(List<LeaderboardEntryObject> entries)
        {
            // Clear current entries
            ClearScrollView(leaderboardResultsScrollView);
            
            if (entries != null && entries.Count > 0)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var displayPosition = i + 1; // 1-based position
                    
                    var properties = new Dictionary<string, string>
                    {
                        { "Position", displayPosition.ToString() },
                        { "User ID", entry.user_id },
                        { "Username", entry.username },
                        { "Score", entry.score.ToString() },
                        { "Created", entry.created },
                    };
                    
                    // Add metadata if present
                    if (!string.IsNullOrEmpty(entry.metadata))
                    {
                        properties.Add("Metadata", entry.metadata);
                    }
                    
                    leaderboardResultsScrollView.Add(CreateListItem($"Leaderboard Entry #{displayPosition}", properties));
                }
                
                LogMessage($"Retrieved {entries.Count} leaderboard entries", LogType.Success);
            }
            else
            {
                LogMessage("No leaderboard entries found", LogType.Normal);
            }
        }

        #endregion

        #region Game Rounds Implementation

        private TextField gameRoundIdField;
        private TextField gameRoundTypeField;
        private TextField gameRoundScoreField;
        private TextField gameRoundPlaceField;
        private TextField gameRoundStartTimeField;
        private TextField gameRoundEndTimeField;
        private TextField gameRoundMetadataField;
        private TextField gameRoundUserIdField;
        private TextField gameRoundMultiplayerUsersField;
        private Button createBasicGameRoundButton;
        private Button createDetailedGameRoundButton;
        private Button createMultiplayerGameRoundButton;
        private Button updateGameRoundButton;
        private Button getGameRoundButton;
        private Button getMyGameRoundsButton;
        private Button getUserGameRoundsButton;
        private Button deleteGameRoundButton;
        private ScrollView gameRoundsResultsScrollView;

        private void InitializeGameRoundsUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-game-rounds");
            
            gameRoundIdField = content.Q<TextField>("game-round-id");
            gameRoundTypeField = content.Q<TextField>("game-round-type");
            gameRoundScoreField = content.Q<TextField>("game-round-score");
            gameRoundPlaceField = content.Q<TextField>("game-round-place");
            gameRoundStartTimeField = content.Q<TextField>("game-round-start-time");
            gameRoundEndTimeField = content.Q<TextField>("game-round-end-time");
            gameRoundMetadataField = content.Q<TextField>("game-round-metadata");
            gameRoundUserIdField = content.Q<TextField>("game-round-user-id");
            gameRoundMultiplayerUsersField = content.Q<TextField>("game-round-multiplayer-users");
            
            createBasicGameRoundButton = content.Q<Button>("create-basic-game-round-button");
            createDetailedGameRoundButton = content.Q<Button>("create-detailed-game-round-button");
            createMultiplayerGameRoundButton = content.Q<Button>("create-multiplayer-game-round-button");
            updateGameRoundButton = content.Q<Button>("update-game-round-button");
            getGameRoundButton = content.Q<Button>("get-game-round-button");
            getMyGameRoundsButton = content.Q<Button>("get-my-game-rounds-button");
            getUserGameRoundsButton = content.Q<Button>("get-user-game-rounds-button");
            deleteGameRoundButton = content.Q<Button>("delete-game-round-button");
            
            gameRoundsResultsScrollView = content.Q<ScrollView>("game-rounds-results-scroll");
        }

        private void RegisterGameRoundsCallbacks()
        {
            createBasicGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateBasicGameRoundClicked());
            createDetailedGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateDetailedGameRoundClicked());
            createMultiplayerGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateMultiplayerGameRoundClicked());
            updateGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnUpdateGameRoundClicked());
            getGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGameRoundClicked());
            getMyGameRoundsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetMyGameRoundsClicked());
            getUserGameRoundsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetUserGameRoundsClicked());
            deleteGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnDeleteGameRoundClicked());
        }
        
        private async Task OnCreateBasicGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create game rounds", LogType.Error);
                return;
            }
            
            string gameType = gameRoundTypeField.value;
            
            if (string.IsNullOrEmpty(gameType))
            {
                LogMessage("Game type is required", LogType.Error);
                return;
            }
            
            if (!int.TryParse(gameRoundScoreField.value, out int score))
            {
                LogMessage("Score must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create basic game round
                var response = await GameFuseUser.CurrentUser.CreateGameRoundAsync(gameType, score);
                
                // Display the created game round
                DisplayGameRound(response);
                
                LogMessage("Game round created successfully", LogType.Success);
            });
        }
        
        private async Task OnCreateDetailedGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create game rounds", LogType.Error);
                return;
            }
            
            string gameType = gameRoundTypeField.value;
            
            if (string.IsNullOrEmpty(gameType))
            {
                LogMessage("Game type is required", LogType.Error);
                return;
            }
            
            if (!int.TryParse(gameRoundScoreField.value, out int score))
            {
                LogMessage("Score must be a valid integer", LogType.Error);
                return;
            }
            
            if (!int.TryParse(gameRoundPlaceField.value, out int place))
            {
                place = 0; // Default place if not specified
            }
            
            // Create GameRoundObject
            GameRoundObject gameRound = new GameRoundObject
            {
                game_type = gameType,
                score = score,
                place = place,
                metadata = gameRoundMetadataField.value
            };
            
            // Add start time if specified
            if (!string.IsNullOrEmpty(gameRoundStartTimeField.value))
            {
                gameRound.start_time = gameRoundStartTimeField.value;
            }
            
            // Add end time if specified
            if (!string.IsNullOrEmpty(gameRoundEndTimeField.value))
            {
                gameRound.end_time = gameRoundEndTimeField.value;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create detailed game round
                var response = await GameFuseUser.CurrentUser.CreateGameRoundAsync(gameRound);
                
                // Display the created game round
                DisplayGameRound(response);
                
                LogMessage("Detailed game round created successfully", LogType.Success);
            });
        }
        
        private async Task OnCreateMultiplayerGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create multiplayer game rounds", LogType.Error);
                return;
            }
            
            string gameType = gameRoundTypeField.value;
            string multiplayerUsers = gameRoundMultiplayerUsersField.value;
            
            if (string.IsNullOrEmpty(gameType))
            {
                LogMessage("Game type is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(multiplayerUsers))
            {
                LogMessage("Multiplayer users data is required", LogType.Error);
                return;
            }
            
            try
            {
                // Parse multiplayer users data (format: userId1,score1,place1;userId2,score2,place2...)
                List<GameRoundObject> gameRounds = new List<GameRoundObject>();
                
                string[] userEntries = multiplayerUsers.Split(';');
                foreach (var entry in userEntries)
                {
                    string[] data = entry.Split(',');
                    if (data.Length >= 3)
                    {
                        string userId = data[0].Trim();
                        int.TryParse(data[1].Trim(), out int score);
                        int.TryParse(data[2].Trim(), out int place);
                        
                        gameRounds.Add(new GameRoundObject
                        {
                            user_id = userId,
                            game_type = gameType,
                            score = score,
                            place = place
                        });
                    }
                }
                
                if (gameRounds.Count == 0)
                {
                    LogMessage("Failed to parse multiplayer users data", LogType.Error);
                    return;
                }
                
                await ExecuteAsync(async () =>
                {
                    // Create multiplayer game round
                    var response = await GameFuseUser.CurrentUser.CreateMultiplayerGameRoundAsync(gameType, gameRounds);
                    
                    // Display the created multiplayer game round
                    DisplayMultiplayerGameRound(response);
                    
                    LogMessage("Multiplayer game round created successfully", LogType.Success);
                });
            }
            catch (Exception ex)
            {
                LogMessage($"Error parsing multiplayer users data: {ex.Message}", LogType.Error);
            }
        }
        
        private async Task OnUpdateGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to update game rounds", LogType.Error);
                return;
            }
            
            string gameRoundId = gameRoundIdField.value;
            
            if (string.IsNullOrEmpty(gameRoundId))
            {
                LogMessage("Game Round ID is required", LogType.Error);
                return;
            }
            
            if (!int.TryParse(gameRoundScoreField.value, out int score))
            {
                LogMessage("Score must be a valid integer", LogType.Error);
                return;
            }
            
            if (!int.TryParse(gameRoundPlaceField.value, out int place))
            {
                place = 0; // Default place if not specified
            }
            
            // Create GameRoundObject for update
            GameRoundObject gameRound = new GameRoundObject
            {
                id = gameRoundId,
                score = score,
                place = place,
                metadata = gameRoundMetadataField.value
            };
            
            // Add end time if specified
            if (!string.IsNullOrEmpty(gameRoundEndTimeField.value))
            {
                gameRound.end_time = gameRoundEndTimeField.value;
            }
            
            await ExecuteAsync(async () =>
            {
                // Update game round
                var response = await GameFuseUser.CurrentUser.UpdateGameRoundAsync(gameRound);
                
                // Display the updated game round
                DisplayGameRound(response);
                
                LogMessage("Game round updated successfully", LogType.Success);
            });
        }
        
        private async Task OnGetGameRoundClicked()
        {
            string gameRoundId = gameRoundIdField.value;
            
            if (string.IsNullOrEmpty(gameRoundId))
            {
                LogMessage("Game Round ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get specific game round
                var response = await GameFuse.GetGameRoundAsync(gameRoundId);
                
                // Display the game round
                DisplayGameRound(response);
                
                LogMessage("Game round retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnGetMyGameRoundsClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view your game rounds", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's game rounds
                var response = await GameFuseUser.CurrentUser.GetMyGameRoundsAsync();
                
                // Display the game rounds
                DisplayGameRounds(response);
                
                LogMessage("Your game rounds retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnGetUserGameRoundsClicked()
        {
            string userId = gameRoundUserIdField.value;
            
            if (string.IsNullOrEmpty(userId))
            {
                LogMessage("User ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get specified user's game rounds
                var response = await GameFuse.GetUserGameRoundsAsync(userId);
                
                // Display the game rounds
                DisplayGameRounds(response);
                
                LogMessage($"Game rounds for user {userId} retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnDeleteGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to delete game rounds", LogType.Error);
                return;
            }
            
            string gameRoundId = gameRoundIdField.value;
            
            if (string.IsNullOrEmpty(gameRoundId))
            {
                LogMessage("Game Round ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Delete game round
                var response = await GameFuseUser.CurrentUser.DeleteGameRoundAsync(gameRoundId);
                
                // Clear the display
                ClearScrollView(gameRoundsResultsScrollView);
                
                LogMessage("Game round deleted successfully", LogType.Success);
            });
        }
        
        private void DisplayGameRound(GameRoundObject gameRound)
        {
            // Clear current display
            ClearScrollView(gameRoundsResultsScrollView);
            
            if (gameRound != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", gameRound.id },
                    { "User ID", gameRound.user_id },
                    { "Game Type", gameRound.game_type },
                    { "Score", gameRound.score.ToString() }
                };
                
                // Add optional properties if they exist
                if (gameRound.place > 0)
                {
                    properties.Add("Place", gameRound.place.ToString());
                }
                
                if (!string.IsNullOrEmpty(gameRound.start_time))
                {
                    properties.Add("Start Time", gameRound.start_time);
                }
                
                if (!string.IsNullOrEmpty(gameRound.end_time))
                {
                    properties.Add("End Time", gameRound.end_time);
                }
                
                if (!string.IsNullOrEmpty(gameRound.created))
                {
                    properties.Add("Created", gameRound.created);
                }
                
                if (!string.IsNullOrEmpty(gameRound.metadata))
                {
                    properties.Add("Metadata", gameRound.metadata);
                }
                
                gameRoundsResultsScrollView.Add(CreateListItem("Game Round", properties));
            }
        }
        
        private void DisplayMultiplayerGameRound(MultiplayerGameRoundResponse response)
        {
            // Clear current display
            ClearScrollView(gameRoundsResultsScrollView);
            
            if (response != null && response.rounds != null && response.rounds.Count > 0)
            {
                gameRoundsResultsScrollView.Add(new Label($"Multiplayer Game Round ID: {response.id}") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                for (int i = 0; i < response.rounds.Count; i++)
                {
                    var round = response.rounds[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", round.id },
                        { "User ID", round.user_id },
                        { "Score", round.score.ToString() },
                        { "Place", round.place.ToString() }
                    };
                    
                    gameRoundsResultsScrollView.Add(CreateListItem($"Player {i + 1}", properties));
                }
            }
        }
        
        private void DisplayGameRounds(List<GameRoundObject> gameRounds)
        {
            // Clear current display
            ClearScrollView(gameRoundsResultsScrollView);
            
            if (gameRounds != null && gameRounds.Count > 0)
            {
                for (int i = 0; i < gameRounds.Count; i++)
                {
                    var round = gameRounds[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", round.id },
                        { "User ID", round.user_id },
                        { "Game Type", round.game_type },
                        { "Score", round.score.ToString() },
                        { "Created", round.created }
                    };
                    
                    // Add optional properties if they exist
                    if (round.place > 0)
                    {
                        properties.Add("Place", round.place.ToString());
                    }
                    
                    if (!string.IsNullOrEmpty(round.start_time))
                    {
                        properties.Add("Start Time", round.start_time);
                    }
                    
                    if (!string.IsNullOrEmpty(round.end_time))
                    {
                        properties.Add("End Time", round.end_time);
                    }
                    
                    if (!string.IsNullOrEmpty(round.metadata))
                    {
                        properties.Add("Metadata", round.metadata);
                    }
                    
                    gameRoundsResultsScrollView.Add(CreateListItem($"Game Round {i + 1}", properties));
                }
            }
            else
            {
                LogMessage("No game rounds found", LogType.Normal);
            }
        }

        #endregion

        #region Groups Implementation

        private TextField groupNameField;
        private TextField groupTypeField;
        private TextField groupMaxSizeField;
        private Toggle groupAutoJoinToggle;
        private Toggle groupInviteOnlyToggle;
        private Toggle groupSearchableToggle;
        private TextField groupIdField;
        private TextField groupUserIdField;
        private TextField groupConnectionIdField;
        private TextField groupAttributeGroupIdField;
        private TextField groupAttributeKeyField;
        private TextField groupAttributeValueField;
        private TextField groupMultipleAttributesField;
        private Button createGroupButton;
        private Button getAllGroupsButton;
        private Button getGroupDetailsButton;
        private Button sendGroupConnectionRequestButton;
        private Button acceptGroupConnectionRequestButton;
        private Button declineGroupConnectionRequestButton;
        private Button getGroupAttributesButton;
        private Button addGroupAttributeButton;
        private Button addGroupMultipleAttributesButton;
        private Button modifyGroupAttributeButton;
        private ScrollView groupsResultsScrollView;

        private void InitializeGroupsUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-groups");
            
            // Group Creation fields
            groupNameField = content.Q<TextField>("group-name");
            groupTypeField = content.Q<TextField>("group-type");
            groupMaxSizeField = content.Q<TextField>("group-max-size");
            groupAutoJoinToggle = content.Q<Toggle>("group-auto-join-toggle");
            groupInviteOnlyToggle = content.Q<Toggle>("group-invite-only-toggle");
            groupSearchableToggle = content.Q<Toggle>("group-searchable-toggle");
            
            // Group Management fields
            groupIdField = content.Q<TextField>("group-id");
            groupUserIdField = content.Q<TextField>("group-user-id");
            groupConnectionIdField = content.Q<TextField>("group-connection-id");
            
            // Group Attributes fields
            groupAttributeGroupIdField = content.Q<TextField>("group-attribute-group-id");
            groupAttributeKeyField = content.Q<TextField>("group-attribute-key");
            groupAttributeValueField = content.Q<TextField>("group-attribute-value");
            groupMultipleAttributesField = content.Q<TextField>("group-multiple-attributes");
            
            // Buttons
            createGroupButton = content.Q<Button>("create-group-button");
            getAllGroupsButton = content.Q<Button>("get-all-groups-button");
            getGroupDetailsButton = content.Q<Button>("get-group-details-button");
            sendGroupConnectionRequestButton = content.Q<Button>("send-group-connection-request-button");
            acceptGroupConnectionRequestButton = content.Q<Button>("accept-group-connection-request-button");
            declineGroupConnectionRequestButton = content.Q<Button>("decline-group-connection-request-button");
            getGroupAttributesButton = content.Q<Button>("get-group-attributes-button");
            addGroupAttributeButton = content.Q<Button>("add-group-attribute-button");
            addGroupMultipleAttributesButton = content.Q<Button>("add-group-multiple-attributes-button");
            modifyGroupAttributeButton = content.Q<Button>("modify-group-attribute-button");
            
            // Results view
            groupsResultsScrollView = content.Q<ScrollView>("groups-results-scroll");
        }

        private void RegisterGroupsCallbacks()
        {
            createGroupButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateGroupClicked());
            getAllGroupsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetAllGroupsClicked());
            getGroupDetailsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGroupDetailsClicked());
            sendGroupConnectionRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnSendGroupConnectionRequestClicked());
            acceptGroupConnectionRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnAcceptGroupConnectionRequestClicked());
            declineGroupConnectionRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnDeclineGroupConnectionRequestClicked());
            getGroupAttributesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGroupAttributesClicked());
            addGroupAttributeButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddGroupAttributeClicked());
            addGroupMultipleAttributesButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddGroupMultipleAttributesClicked());
            modifyGroupAttributeButton.RegisterCallback<ClickEvent>(async (evt) => await OnModifyGroupAttributeClicked());
        }
        
        private async Task OnCreateGroupClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create groups", LogType.Error);
                return;
            }
            
            string name = groupNameField.value;
            string type = groupTypeField.value;
            
            if (string.IsNullOrEmpty(name))
            {
                LogMessage("Group name is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(type))
            {
                LogMessage("Group type is required", LogType.Error);
                return;
            }
            
            if (!int.TryParse(groupMaxSizeField.value, out int maxSize))
            {
                maxSize = 10; // Default max size
            }
            
            bool autoJoin = groupAutoJoinToggle.value;
            bool inviteOnly = groupInviteOnlyToggle.value;
            bool searchable = groupSearchableToggle.value;
            
            await ExecuteAsync(async () =>
            {
                // Create group request
                var createGroupRequest = new CreateGroupRequest
                {
                    name = name,
                    type = type,
                    max_size = maxSize,
                    auto_join = autoJoin,
                    invite_only = inviteOnly,
                    searchable = searchable
                };
                
                // Create the group
                var response = await GameFuseUser.CurrentUser.CreateGroupAsync(createGroupRequest);
                
                // Display the created group
                DisplayGroup(response);
                
                LogMessage($"Group '{name}' created successfully", LogType.Success);
            });
        }
        
        private async Task OnGetAllGroupsClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Get all groups
                var response = await GameFuse.GetGroupsAsync();
                
                // Display all groups
                DisplayGroups(response);
                
                LogMessage($"Retrieved {response.Count} groups", LogType.Success);
            });
        }
        
        private async Task OnGetGroupDetailsClicked()
        {
            string groupId = groupIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get group details
                var response = await GameFuse.GetGroupAsync(groupId);
                
                // Display the group
                DisplayGroup(response);
                
                LogMessage($"Group details retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnSendGroupConnectionRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to send group connection requests", LogType.Error);
                return;
            }
            
            string groupId = groupIdField.value;
            string userId = groupUserIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create group connection request
                var request = new GroupConnectionRequest
                {
                    group_id = groupId
                };
                
                // Add user ID or username if specified
                if (!string.IsNullOrEmpty(userId))
                {
                    // Check if it's a username or user ID
                    if (userId.Length < 36) // Assuming UUIDs are 36 characters
                    {
                        request.username = userId;
                    }
                    else
                    {
                        request.user_id = userId;
                    }
                }
                
                // Send group connection request
                var response = await GameFuseUser.CurrentUser.SendGroupConnectionRequestAsync(request);
                
                // Display the response
                DisplayGroupConnectionResponse(response);
                
                LogMessage("Group connection request sent successfully", LogType.Success);
            });
        }
        
        private async Task OnAcceptGroupConnectionRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to accept group connection requests", LogType.Error);
                return;
            }
            
            string connectionId = groupConnectionIdField.value;
            
            if (string.IsNullOrEmpty(connectionId))
            {
                LogMessage("Connection ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create group connection status request
                var request = new GroupConnectionStatusRequest
                {
                    connection_id = connectionId,
                    status = JoinRequestStatus.Accepted
                };
                
                // Accept group connection request
                var response = await GameFuseUser.CurrentUser.RespondToGroupConnectionRequestAsync(request);
                
                // Display the response
                ClearScrollView(groupsResultsScrollView);
                groupsResultsScrollView.Add(new Label($"Connection Status: {response.status}") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                LogMessage("Group connection request accepted", LogType.Success);
            });
        }
        
        private async Task OnDeclineGroupConnectionRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to decline group connection requests", LogType.Error);
                return;
            }
            
            string connectionId = groupConnectionIdField.value;
            
            if (string.IsNullOrEmpty(connectionId))
            {
                LogMessage("Connection ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create group connection status request
                var request = new GroupConnectionStatusRequest
                {
                    connection_id = connectionId,
                    status = JoinRequestStatus.Declined
                };
                
                // Decline group connection request
                var response = await GameFuseUser.CurrentUser.RespondToGroupConnectionRequestAsync(request);
                
                // Display the response
                ClearScrollView(groupsResultsScrollView);
                groupsResultsScrollView.Add(new Label($"Connection Status: {response.status}") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                LogMessage("Group connection request declined", LogType.Success);
            });
        }
        
        private async Task OnGetGroupAttributesClicked()
        {
            string groupId = groupAttributeGroupIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get group attributes
                var response = await GameFuse.GetGroupAttributesAsync(groupId);
                
                // Display group attributes
                DisplayGroupAttributes(response);
                
                LogMessage($"Retrieved {response.group_attributes.Count} group attributes", LogType.Success);
            });
        }
        
        private async Task OnAddGroupAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to add group attributes", LogType.Error);
                return;
            }
            
            string groupId = groupAttributeGroupIdField.value;
            string key = groupAttributeKeyField.value;
            string value = groupAttributeValueField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create group attribute request
                var request = new GroupAttributeRequest
                {
                    group_id = groupId,
                    key = key,
                    value = value
                };
                
                // Add group attribute
                var response = await GameFuseUser.CurrentUser.AddGroupAttributeAsync(request);
                
                // Get updated attributes
                await OnGetGroupAttributesClicked();
                
                LogMessage($"Group attribute '{key}' added successfully", LogType.Success);
            });
        }
        
        private async Task OnAddGroupMultipleAttributesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to add group attributes", LogType.Error);
                return;
            }
            
            string groupId = groupAttributeGroupIdField.value;
            string attributesJson = groupMultipleAttributesField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(attributesJson))
            {
                LogMessage("Attributes JSON is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                try
                {
                    // Parse JSON to get attributes
                    var jsonObject = Boomlagoon.JSON.JSONObject.Parse(attributesJson);
                    Dictionary<string, string> attributes = new Dictionary<string, string>();
                    
                    foreach (var key in jsonObject.GetKeys())
                    {
                        var jsonValue = jsonObject.GetValue(key);
                        string value = "";
                        
                        if (jsonValue.Type == Boomlagoon.JSON.JSONValueType.String)
                        {
                            value = jsonValue.Str;
                        }
                        else if (jsonValue.Type == Boomlagoon.JSON.JSONValueType.Number)
                        {
                            value = jsonValue.Number.ToString();
                        }
                        else if (jsonValue.Type == Boomlagoon.JSON.JSONValueType.Boolean)
                        {
                            value = jsonValue.Boolean.ToString();
                        }
                        else
                        {
                            value = jsonValue.ToString();
                        }
                        
                        attributes.Add(key, value);
                    }
                    
                    // Create group attributes request
                    var request = new GroupAttributesRequest
                    {
                        group_id = groupId,
                        attributes = attributes
                    };
                    
                    // Add multiple group attributes
                    var response = await GameFuseUser.CurrentUser.AddGroupAttributesAsync(request);
                    
                    // Get updated attributes
                    await OnGetGroupAttributesClicked();
                    
                    LogMessage($"Added {attributes.Count} group attributes successfully", LogType.Success);
                }
                catch (Exception ex)
                {
                    LogMessage($"Error parsing attributes JSON: {ex.Message}", LogType.Error);
                }
            });
        }
        
        private async Task OnModifyGroupAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to modify group attributes", LogType.Error);
                return;
            }
            
            string groupId = groupAttributeGroupIdField.value;
            string key = groupAttributeKeyField.value;
            string value = groupAttributeValueField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create modify group attribute request
                var request = new ModifyGroupAttributeRequest
                {
                    group_id = groupId,
                    key = key,
                    value = value
                };
                
                // Modify group attribute
                var response = await GameFuseUser.CurrentUser.ModifyGroupAttributeAsync(request);
                
                // Get updated attributes
                await OnGetGroupAttributesClicked();
                
                LogMessage($"Group attribute '{key}' modified successfully", LogType.Success);
            });
        }
        
        private void DisplayGroup(GroupResponse group)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (group != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", group.id },
                    { "Name", group.name },
                    { "Type", group.type },
                    { "Owner ID", group.owner_id },
                    { "Max Size", group.max_size.ToString() },
                    { "Current Size", group.current_size.ToString() },
                    { "Auto Join", group.auto_join.ToString() },
                    { "Invite Only", group.invite_only.ToString() },
                    { "Searchable", group.searchable.ToString() },
                    { "Created", group.created }
                };
                
                groupsResultsScrollView.Add(CreateListItem("Group", properties));
                
                // Display members
                if (group.members != null && group.members.Count > 0)
                {
                    groupsResultsScrollView.Add(new Label("Members:") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var member in group.members)
                    {
                        var memberProperties = new Dictionary<string, string>
                        {
                            { "ID", member.id },
                            { "Username", member.username },
                            { "Is Owner", (member.id == group.owner_id).ToString() }
                        };
                        
                        groupsResultsScrollView.Add(CreateListItem(member.username, memberProperties));
                    }
                }
                
                // Display join requests
                if (group.join_requests != null && group.join_requests.Count > 0)
                {
                    groupsResultsScrollView.Add(new Label("Join Requests:") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var request in group.join_requests)
                    {
                        var requestProperties = new Dictionary<string, string>
                        {
                            { "Connection ID", request.id },
                            { "User ID", request.user_id },
                            { "Username", request.username },
                            { "Status", request.status },
                            { "Created", request.created }
                        };
                        
                        groupsResultsScrollView.Add(CreateListItem($"Request from {request.username}", requestProperties));
                    }
                }
            }
        }
        
        private void DisplayGroups(List<GroupResponse> groups)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (groups != null && groups.Count > 0)
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", group.id },
                        { "Type", group.type },
                        { "Current/Max Size", $"{group.current_size}/{group.max_size}" },
                        { "Auto Join", group.auto_join.ToString() },
                        { "Invite Only", group.invite_only.ToString() },
                        { "Searchable", group.searchable.ToString() }
                    };
                    
                    groupsResultsScrollView.Add(CreateListItem(group.name, properties));
                }
            }
            else
            {
                LogMessage("No groups found", LogType.Normal);
            }
        }
        
        private void DisplayGroupAttributes(GroupAttributesResponse response)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (response != null && response.group_attributes != null && response.group_attributes.Count > 0)
            {
                groupsResultsScrollView.Add(new Label($"Group ID: {response.group_id}") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                foreach (var attribute in response.group_attributes)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "Value", attribute.value }
                    };
                    
                    groupsResultsScrollView.Add(CreateListItem(attribute.key, properties));
                }
            }
            else
            {
                LogMessage("No group attributes found", LogType.Normal);
            }
        }
        
        private void DisplayGroupConnectionResponse(GroupConnectionResponse response)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (response != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", response.id },
                    { "Group ID", response.group_id },
                    { "User ID", response.user_id },
                    { "Username", response.username },
                    { "Status", response.status },
                    { "Created", response.created }
                };
                
                groupsResultsScrollView.Add(CreateListItem("Group Connection Request", properties));
            }
        }

        #endregion

        #region Chats Implementation

        private TextField chatUsernamesField;
        private TextField chatGroupIdField;
        private TextField chatIdField;
        private TextField chatPageField;
        private TextField messageChatIdField;
        private TextField messageTextField;
        private TextField markReadMessageIdField;
        private Button getMyChatsButton;
        private Button createDirectChatButton;
        private Button createGroupChatButton;
        private Button getMessagesButton;
        private Button sendMessageButton;
        private Button markMessageReadButton;
        private ScrollView chatResultsScrollView;

        private void InitializeChatsUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-chats");
            
            // Chat operation fields
            chatUsernamesField = content.Q<TextField>("chat-usernames");
            chatGroupIdField = content.Q<TextField>("chat-group-id");
            chatIdField = content.Q<TextField>("chat-id");
            chatPageField = content.Q<TextField>("chat-page");
            
            // Message fields
            messageChatIdField = content.Q<TextField>("message-chat-id");
            messageTextField = content.Q<TextField>("message-text");
            markReadMessageIdField = content.Q<TextField>("mark-read-message-id");
            
            // Buttons
            getMyChatsButton = content.Q<Button>("get-my-chats-button");
            createDirectChatButton = content.Q<Button>("create-direct-chat-button");
            createGroupChatButton = content.Q<Button>("create-group-chat-button");
            getMessagesButton = content.Q<Button>("get-chat-messages-button");
            sendMessageButton = content.Q<Button>("send-message-button");
            markMessageReadButton = content.Q<Button>("mark-message-read-button");
            
            // Results view
            chatResultsScrollView = content.Q<ScrollView>("chat-results-scroll");
        }

        private void RegisterChatsCallbacks()
        {
            getMyChatsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetMyChatsClicked());
            createDirectChatButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateDirectChatClicked());
            createGroupChatButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateGroupChatClicked());
            getMessagesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetMessagesClicked());
            sendMessageButton.RegisterCallback<ClickEvent>(async (evt) => await OnSendMessageClicked());
            markMessageReadButton.RegisterCallback<ClickEvent>(async (evt) => await OnMarkMessageReadClicked());
        }
        
        private async Task OnGetMyChatsClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view your chats", LogType.Error);
                return;
            }
            
            int page = 1;
            if (!string.IsNullOrEmpty(chatPageField.value) && 
                !int.TryParse(chatPageField.value, out page))
            {
                LogMessage("Page must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's chats
                var response = await GameFuseUser.CurrentUser.GetChatsAsync(page);
                
                // Display chats
                DisplayChats(response);
                
                LogMessage("Chats retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnCreateDirectChatClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create chats", LogType.Error);
                return;
            }
            
            string usernames = chatUsernamesField.value;
            
            if (string.IsNullOrEmpty(usernames))
            {
                LogMessage("Usernames are required for direct chat", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Parse comma-separated usernames
                string[] usernameArray = usernames.Split(',').Select(u => u.Trim()).ToArray();
                
                if (usernameArray.Length == 0)
                {
                    LogMessage("At least one username is required", LogType.Error);
                    return;
                }
                
                // Create request
                var request = new CreateDirectChatRequest
                {
                    usernames = usernameArray
                };
                
                // Create direct chat
                var response = await GameFuseUser.CurrentUser.CreateDirectChatAsync(request);
                
                // Display the created chat
                DisplayChat(response);
                
                LogMessage("Direct chat created successfully", LogType.Success);
                
                // Update chat ID field for easier message sending
                messageChatIdField.value = response.id;
            });
        }
        
        private async Task OnCreateGroupChatClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create chats", LogType.Error);
                return;
            }
            
            string groupId = chatGroupIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required for group chat", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create request
                var request = new CreateGroupChatRequest
                {
                    group_id = groupId
                };
                
                // Create group chat
                var response = await GameFuseUser.CurrentUser.CreateGroupChatAsync(request);
                
                // Display the created chat
                DisplayChat(response);
                
                LogMessage("Group chat created successfully", LogType.Success);
                
                // Update chat ID field for easier message sending
                messageChatIdField.value = response.id;
            });
        }
        
        private async Task OnGetMessagesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view messages", LogType.Error);
                return;
            }
            
            string chatId = chatIdField.value;
            
            if (string.IsNullOrEmpty(chatId))
            {
                LogMessage("Chat ID is required", LogType.Error);
                return;
            }
            
            int page = 1;
            if (!string.IsNullOrEmpty(chatPageField.value) && 
                !int.TryParse(chatPageField.value, out page))
            {
                LogMessage("Page must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get messages for chat
                var response = await GameFuseUser.CurrentUser.GetMessagesAsync(chatId, page);
                
                // Display messages
                DisplayMessages(response);
                
                LogMessage($"Retrieved {response.messages.Count} messages", LogType.Success);
                
                // Update chat ID field for easier message sending
                messageChatIdField.value = chatId;
            });
        }
        
        private async Task OnSendMessageClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to send messages", LogType.Error);
                return;
            }
            
            string chatId = messageChatIdField.value;
            string messageText = messageTextField.value;
            
            if (string.IsNullOrEmpty(chatId))
            {
                LogMessage("Chat ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(messageText))
            {
                LogMessage("Message text is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create request
                var request = new SendMessageRequest
                {
                    chat_id = chatId,
                    text = messageText
                };
                
                // Send message
                var response = await GameFuseUser.CurrentUser.SendMessageAsync(request);
                
                // Clear message input
                messageTextField.value = "";
                
                // Refresh messages
                await OnGetMessagesClicked();
                
                LogMessage("Message sent successfully", LogType.Success);
            });
        }
        
        private async Task OnMarkMessageReadClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to mark messages as read", LogType.Error);
                return;
            }
            
            string messageId = markReadMessageIdField.value;
            
            if (string.IsNullOrEmpty(messageId))
            {
                LogMessage("Message ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Mark message as read
                var response = await GameFuseUser.CurrentUser.MarkMessageReadAsync(messageId);
                
                LogMessage("Message marked as read successfully", LogType.Success);
            });
        }
        
        private void DisplayChats(GetChatsResponse response)
        {
            // Clear current display
            ClearScrollView(chatResultsScrollView);
            
            if (response != null)
            {
                // Display direct chats
                if (response.direct_chats != null && response.direct_chats.Count > 0)
                {
                    chatResultsScrollView.Add(new Label("Direct Chats:") 
                    { 
                        style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } 
                    });
                    
                    foreach (var chat in response.direct_chats)
                    {
                        DisplayChatListItem(chat, "Direct");
                    }
                }
                
                // Display group chats
                if (response.group_chats != null && response.group_chats.Count > 0)
                {
                    chatResultsScrollView.Add(new Label("Group Chats:") 
                    { 
                        style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } 
                    });
                    
                    foreach (var chat in response.group_chats)
                    {
                        DisplayChatListItem(chat, "Group");
                    }
                }
            }
        }
        
        private void DisplayChat(Chat chat)
        {
            // Clear current display
            ClearScrollView(chatResultsScrollView);
            
            if (chat != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", chat.id },
                    { "Type", chat.is_group_chat ? "Group" : "Direct" },
                    { "Group ID", chat.group_id ?? "N/A" },
                    { "Created", chat.created },
                    { "Last Message", chat.last_message?.text ?? "No messages" }
                };
                
                chatResultsScrollView.Add(CreateListItem("Chat", properties));
                
                // Display participants
                if (chat.participants != null && chat.participants.Count > 0)
                {
                    chatResultsScrollView.Add(new Label("Participants:") 
                    { 
                        style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } 
                    });
                    
                    foreach (var participant in chat.participants)
                    {
                        var participantProperties = new Dictionary<string, string>
                        {
                            { "ID", participant.id },
                            { "Username", participant.username }
                        };
                        
                        chatResultsScrollView.Add(CreateListItem(participant.username, participantProperties));
                    }
                }
            }
        }
        
        private void DisplayChatListItem(Chat chat, string type)
        {
            string title;
            if (chat.is_group_chat && !string.IsNullOrEmpty(chat.group_id))
            {
                title = $"Group Chat (Group ID: {chat.group_id})";
            }
            else if (chat.participants != null && chat.participants.Count > 0)
            {
                // For direct chats, show the other participants' usernames
                var otherParticipants = chat.participants
                    .Where(p => p.id != GameFuseUser.CurrentUser.id)
                    .Select(p => p.username)
                    .ToList();
                
                title = string.Join(", ", otherParticipants);
            }
            else
            {
                title = $"Chat {chat.id}";
            }
            
            var properties = new Dictionary<string, string>
            {
                { "ID", chat.id },
                { "Type", type }
            };
            
            // Add last message if exists
            if (chat.last_message != null)
            {
                properties.Add("Last Message", $"{chat.last_message.text} (from {chat.last_message.username})");
                properties.Add("Last Message Time", chat.last_message.created);
            }
            
            var chatItem = CreateListItem(title, properties);
            
            // Add a click handler to show messages for this chat
            chatItem.RegisterCallback<ClickEvent>(async (evt) => 
            {
                chatIdField.value = chat.id;
                await OnGetMessagesClicked();
            });
            
            chatResultsScrollView.Add(chatItem);
        }
        
        private void DisplayMessages(GetMessagesResponse response)
        {
            // Clear current display
            ClearScrollView(chatResultsScrollView);
            
            if (response != null && response.messages != null && response.messages.Count > 0)
            {
                chatResultsScrollView.Add(new Label($"Chat ID: {response.chat_id}") 
                { 
                    style = { unityFontStyleAndWeight = FontStyle.Bold } 
                });
                
                // Display messages in reverse order (newest first)
                foreach (var message in response.messages)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", message.id },
                        { "From", message.username },
                        { "Time", message.created },
                        { "Read", message.read ? "Yes" : "No" }
                    };
                    
                    var messageItem = CreateListItem(message.text, properties);
                    
                    // Set background color based on who sent it
                    if (message.user_id == GameFuseUser.CurrentUser.id)
                    {
                        messageItem.style.backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.3f);
                    }
                    
                    // Add click handler to copy message ID for marking as read
                    messageItem.RegisterCallback<ClickEvent>((evt) => 
                    {
                        markReadMessageIdField.value = message.id;
                    });
                    
                    chatResultsScrollView.Add(messageItem);
                }
            }
            else
            {
                LogMessage("No messages found", LogType.Normal);
            }
        }

        #endregion

        #region Friends Implementation

        private TextField friendUsernameField;
        private TextField friendshipIdField;
        private TextField friendUserIdField;
        private Button getAllFriendDataButton;
        private Button sendFriendRequestButton;
        private Button acceptFriendRequestButton;
        private Button declineFriendRequestButton;
        private Button cancelFriendRequestButton;
        private Button unfriendButton;
        private ScrollView friendsScrollView;
        private ScrollView incomingRequestsScrollView;
        private ScrollView outgoingRequestsScrollView;

        private void InitializeFriendsUI()
        {
            var content = rootVisualElement.Q<VisualElement>("content-friends");
            
            // Fields
            friendUsernameField = content.Q<TextField>("friend-username");
            friendshipIdField = content.Q<TextField>("friendship-id");
            friendUserIdField = content.Q<TextField>("friend-user-id");
            
            // Buttons
            getAllFriendDataButton = content.Q<Button>("get-all-friend-data-button");
            sendFriendRequestButton = content.Q<Button>("send-friend-request-button");
            acceptFriendRequestButton = content.Q<Button>("accept-friend-request-button");
            declineFriendRequestButton = content.Q<Button>("decline-friend-request-button");
            cancelFriendRequestButton = content.Q<Button>("cancel-friend-request-button");
            unfriendButton = content.Q<Button>("unfriend-button");
            
            // ScrollViews
            friendsScrollView = content.Q<ScrollView>("friends-scroll");
            incomingRequestsScrollView = content.Q<ScrollView>("incoming-requests-scroll");
            outgoingRequestsScrollView = content.Q<ScrollView>("outgoing-requests-scroll");
        }

        private void RegisterFriendsCallbacks()
        {
            getAllFriendDataButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetAllFriendDataClicked());
            sendFriendRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnSendFriendRequestClicked());
            acceptFriendRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnAcceptFriendRequestClicked());
            declineFriendRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnDeclineFriendRequestClicked());
            cancelFriendRequestButton.RegisterCallback<ClickEvent>(async (evt) => await OnCancelFriendRequestClicked());
            unfriendButton.RegisterCallback<ClickEvent>(async (evt) => await OnUnfriendClicked());
        }
        
        private async Task OnGetAllFriendDataClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view friend data", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get friend data
                var friendsResponse = await GameFuseUser.CurrentUser.GetFriendsAsync();
                var incomingResponse = await GameFuseUser.CurrentUser.GetIncomingFriendRequestsAsync();
                var outgoingResponse = await GameFuseUser.CurrentUser.GetOutgoingFriendRequestsAsync();
                
                // Display friend data
                DisplayFriends(friendsResponse);
                DisplayIncomingFriendRequests(incomingResponse);
                DisplayOutgoingFriendRequests(outgoingResponse);
                
                LogMessage("Friend data retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnSendFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to send friend requests", LogType.Error);
                return;
            }
            
            string username = friendUsernameField.value;
            
            if (string.IsNullOrEmpty(username))
            {
                LogMessage("Username is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create request data
                var requestData = new FriendRequestData
                {
                    username = username
                };
                
                // Send friend request
                var response = await GameFuseUser.CurrentUser.SendFriendRequestAsync(requestData);
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage($"Friend request sent to {username}", LogType.Success);
            });
        }
        
        private async Task OnAcceptFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to accept friend requests", LogType.Error);
                return;
            }
            
            string friendshipId = friendshipIdField.value;
            
            if (string.IsNullOrEmpty(friendshipId))
            {
                LogMessage("Friendship ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create friendship status data
                var statusData = new FriendshipStatusData
                {
                    id = friendshipId,
                    status = FriendRequestStatus.Accepted
                };
                
                // Accept friend request
                var response = await GameFuseUser.CurrentUser.RespondToFriendRequestAsync(statusData);
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Friend request accepted", LogType.Success);
            });
        }
        
        private async Task OnDeclineFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to decline friend requests", LogType.Error);
                return;
            }
            
            string friendshipId = friendshipIdField.value;
            
            if (string.IsNullOrEmpty(friendshipId))
            {
                LogMessage("Friendship ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create friendship status data
                var statusData = new FriendshipStatusData
                {
                    id = friendshipId,
                    status = FriendRequestStatus.Declined
                };
                
                // Decline friend request
                var response = await GameFuseUser.CurrentUser.RespondToFriendRequestAsync(statusData);
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Friend request declined", LogType.Success);
            });
        }
        
        private async Task OnCancelFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to cancel friend requests", LogType.Error);
                return;
            }
            
            string friendshipId = friendshipIdField.value;
            
            if (string.IsNullOrEmpty(friendshipId))
            {
                LogMessage("Friendship ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create friendship status data
                var statusData = new FriendshipStatusData
                {
                    id = friendshipId,
                    status = FriendRequestStatus.Cancelled
                };
                
                // Cancel friend request
                var response = await GameFuseUser.CurrentUser.RespondToFriendRequestAsync(statusData);
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Friend request cancelled", LogType.Success);
            });
        }
        
        private async Task OnUnfriendClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to unfriend users", LogType.Error);
                return;
            }
            
            string userId = friendUserIdField.value;
            
            if (string.IsNullOrEmpty(userId))
            {
                LogMessage("User ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Unfriend user
                var response = await GameFuseUser.CurrentUser.UnfriendAsync(userId);
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("User unfriended successfully", LogType.Success);
            });
        }
        
        private void DisplayFriends(FriendsResponse response)
        {
            // Clear current display
            ClearScrollView(friendsScrollView);
            
            if (response != null && response.friends != null && response.friends.Count > 0)
            {
                foreach (var friend in response.friends)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", friend.id },
                        { "Username", friend.username },
                        { "Created", friend.created }
                    };
                    
                    var friendItem = CreateListItem(friend.username, properties);
                    
                    // Add click handler to copy friend's user ID for unfriending
                    friendItem.RegisterCallback<ClickEvent>((evt) => 
                    {
                        friendUserIdField.value = friend.id;
                    });
                    
                    friendsScrollView.Add(friendItem);
                }
            }
            else
            {
                friendsScrollView.Add(new Label("No friends yet") 
                { 
                    style = { unityTextAlign = TextAnchor.MiddleCenter, color = new Color(0.8f, 0.8f, 0.8f) } 
                });
            }
        }
        
        private void DisplayIncomingFriendRequests(IncomingFriendRequestsResponse response)
        {
            // Clear current display
            ClearScrollView(incomingRequestsScrollView);
            
            if (response != null && response.requests != null && response.requests.Count > 0)
            {
                foreach (var request in response.requests)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "From", request.username },
                        { "ID", request.id },
                        { "Created", request.created }
                    };
                    
                    var requestItem = CreateListItem($"From: {request.username}", properties);
                    
                    // Add click handler to copy request ID for accepting/declining
                    requestItem.RegisterCallback<ClickEvent>((evt) => 
                    {
                        friendshipIdField.value = request.id;
                    });
                    
                    incomingRequestsScrollView.Add(requestItem);
                }
            }
            else
            {
                incomingRequestsScrollView.Add(new Label("No incoming requests") 
                { 
                    style = { unityTextAlign = TextAnchor.MiddleCenter, color = new Color(0.8f, 0.8f, 0.8f) } 
                });
            }
        }
        
        private void DisplayOutgoingFriendRequests(OutgoingFriendRequestsResponse response)
        {
            // Clear current display
            ClearScrollView(outgoingRequestsScrollView);
            
            if (response != null && response.requests != null && response.requests.Count > 0)
            {
                foreach (var request in response.requests)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "To", request.username },
                        { "ID", request.id },
                        { "Created", request.created }
                    };
                    
                    var requestItem = CreateListItem($"To: {request.username}", properties);
                    
                    // Add click handler to copy request ID for cancelling
                    requestItem.RegisterCallback<ClickEvent>((evt) => 
                    {
                        friendshipIdField.value = request.id;
                    });
                    
                    outgoingRequestsScrollView.Add(requestItem);
                }
            }
            else
            {
                outgoingRequestsScrollView.Add(new Label("No outgoing requests") 
                { 
                    style = { unityTextAlign = TextAnchor.MiddleCenter, color = new Color(0.8f, 0.8f, 0.8f) } 
                });
            }
        }

        #endregion
    }
}