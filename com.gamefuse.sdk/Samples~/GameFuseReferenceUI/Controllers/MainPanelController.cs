using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using System;

namespace GameFuse.UI
{
    /// <summary>
    /// Controls the main application panel after authentication.
    /// Manages navigation between different feature panels.
    /// Updated for Phase 3: Added friends functionality and social features.
    /// </summary>
    public class MainPanelController
    {
        // Events
        public static event Action OnSignOutRequested;
        public static event Action<GameFuseUser> OnUserDataUpdated;

        // UI Elements
        private VisualElement root;
        private VisualElement headerPanel;
        private VisualElement navigationPanel;
        private VisualElement contentPanel;

        // Header elements
        private Label userNameLabel;
        private Label userScoreLabel;
        private Label userCreditsLabel;
        private Button signOutButton;

        // Navigation buttons
        private Button profileNavButton;
        private Button friendsNavButton;
        private Button groupsNavButton;
        private Button storeNavButton;
        private Button gameRoundsNavButton;
        private Button leaderboardsNavButton;
        private Button messagesNavButton;

        // Content panels
        private VisualElement profilePanel;
        private VisualElement friendsPanel;
        private VisualElement groupsPanel;
        private VisualElement storePanel;
        private VisualElement gameRoundsPanel;
        private VisualElement leaderboardsPanel;
        private VisualElement messagesPanel;

        // Panel Controllers
        private ProfilePanelController profileController;
        private FriendsPanelController friendsController;
        private GroupsPanelController groupsController;
        private StorePanelController storeController;

        // Current user and state
        private GameFuseUser currentUser;
        private string currentPanel = "profile";

        public MainPanelController(VisualElement mainPanel)
        {
            root = mainPanel;
            InitializeElements();
            SetupEventHandlers();
            ShowPanel("profile"); // Default to profile panel
        }

        private void InitializeElements()
        {
            // Get main sections
            headerPanel = root.Q<VisualElement>("header-panel");
            navigationPanel = root.Q<VisualElement>("navigation-panel");
            contentPanel = root.Q<VisualElement>("content-panel");

            // Header elements
            userNameLabel = root.Q<Label>("user-name");
            userScoreLabel = root.Q<Label>("user-score");
            userCreditsLabel = root.Q<Label>("user-credits");
            signOutButton = root.Q<Button>("sign-out-button");

            // Navigation buttons
            profileNavButton = root.Q<Button>("nav-profile");
            friendsNavButton = root.Q<Button>("nav-friends");
            groupsNavButton = root.Q<Button>("nav-groups");
            storeNavButton = root.Q<Button>("nav-store");
            gameRoundsNavButton = root.Q<Button>("nav-gamerounds");
            leaderboardsNavButton = root.Q<Button>("nav-leaderboards");
            messagesNavButton = root.Q<Button>("nav-messages");

            // Content panels
            profilePanel = root.Q<VisualElement>("profile-panel");
            friendsPanel = root.Q<VisualElement>("friends-panel");
            groupsPanel = root.Q<VisualElement>("groups-panel");
            storePanel = root.Q<VisualElement>("store-panel");
            gameRoundsPanel = root.Q<VisualElement>("gamerounds-panel");
            leaderboardsPanel = root.Q<VisualElement>("leaderboards-panel");
            messagesPanel = root.Q<VisualElement>("messages-panel");

            // Initialize Panel Controllers
            InitializePanelControllers();
        }

        private void InitializePanelControllers()
        {
            // Profile Panel Controller (Phase 2)
            if (profilePanel != null)
            {
                profileController = new ProfilePanelController(profilePanel);
            }

            // Friends Panel Controller (Phase 3)
            if (friendsPanel != null)
            {
                friendsController = new FriendsPanelController(friendsPanel);
            }

            // Groups Panel Controller (Phase 4)
            if (groupsPanel != null)
            {
                groupsController = new GroupsPanelController(groupsPanel);
            }

            // Store Panel Controller (Phase 5)
            if (storePanel != null)
            {
                storeController = new StorePanelController(storePanel);
            }

            // Future panel controllers will be initialized here in subsequent phases
            // Phase 6: gameRoundsController = new GameRoundsPanelController(gameRoundsPanel);
            // etc.
        }

        private void SetupEventHandlers()
        {
            // Header actions
            signOutButton.clicked += () => OnSignOutRequested?.Invoke();

            // Navigation
            profileNavButton.clicked += () => ShowPanel("profile");
            friendsNavButton.clicked += () => ShowPanel("friends");
            groupsNavButton.clicked += () => ShowPanel("groups");
            storeNavButton.clicked += () => ShowPanel("store");
            gameRoundsNavButton.clicked += () => ShowPanel("gamerounds");
            leaderboardsNavButton.clicked += () => ShowPanel("leaderboards");
            messagesNavButton.clicked += () => ShowPanel("messages");

            // Profile Panel Events (Phase 2)
            ProfilePanelController.OnUserDataUpdated += HandleUserDataUpdated;
            ProfilePanelController.OnError += HandlePanelError;
            ProfilePanelController.OnSuccess += HandlePanelSuccess;
            
            // Friends Panel Events (Phase 3)
            FriendsPanelController.OnError += HandlePanelError;
            FriendsPanelController.OnSuccess += HandlePanelSuccess;
            
            // Groups Panel Events (Phase 4)
            GroupsPanelController.OnError += HandlePanelError;
            GroupsPanelController.OnSuccess += HandlePanelSuccess;
            
            // Store Panel Events (Phase 5)
            StorePanelController.OnError += HandlePanelError;
            StorePanelController.OnSuccess += HandlePanelSuccess;
            StorePanelController.OnUserDataUpdated += HandleUserDataUpdated;
        }

        private void HandleUserDataUpdated(GameFuseUser updatedUser)
        {
            // Update our reference to the current user
            currentUser = updatedUser;

            // Update the header display
            UpdateUserDisplay();

            // Notify other components that user data changed
            OnUserDataUpdated?.Invoke(updatedUser);

            Debug.Log($"User data updated: Score={updatedUser.Score}, Credits={updatedUser.Credits}");
        }

        private void HandlePanelError(string errorMessage)
        {
            Debug.LogError($"Panel Error: {errorMessage}");
            // Could show a global error notification here if needed
        }

        private void HandlePanelSuccess(string successMessage)
        {
            Debug.Log($"Panel Success: {successMessage}");
            // Could show a global success notification here if needed
        }

        public void SetCurrentUser(GameFuseUser user)
        {
            currentUser = user;
            UpdateUserDisplay();

            // Pass user to active panel controllers
            profileController?.SetCurrentUser(user);
            friendsController?.SetCurrentUser(user);
            groupsController?.SetCurrentUser(user);
            storeController?.SetCurrentUser(user);

            // Future panel controllers will also receive the user here
        }

        private void UpdateUserDisplay()
        {
            if (currentUser == null) return;

            userNameLabel.text = currentUser.Username;
            userScoreLabel.text = $"Score: {currentUser.Score:N0}";
            userCreditsLabel.text = $"Credits: {currentUser.Credits:N0}";
        }

        private void ShowPanel(string panelName)
        {
            // Hide all panels
            profilePanel.style.display = DisplayStyle.None;
            friendsPanel.style.display = DisplayStyle.None;
            groupsPanel.style.display = DisplayStyle.None;
            storePanel.style.display = DisplayStyle.None;
            gameRoundsPanel.style.display = DisplayStyle.None;
            leaderboardsPanel.style.display = DisplayStyle.None;
            messagesPanel.style.display = DisplayStyle.None;

            // Remove active class from all nav buttons
            profileNavButton.RemoveFromClassList("nav-active");
            friendsNavButton.RemoveFromClassList("nav-active");
            groupsNavButton.RemoveFromClassList("nav-active");
            storeNavButton.RemoveFromClassList("nav-active");
            gameRoundsNavButton.RemoveFromClassList("nav-active");
            leaderboardsNavButton.RemoveFromClassList("nav-active");
            messagesNavButton.RemoveFromClassList("nav-active");

            // Show selected panel and mark nav button as active
            switch (panelName)
            {
                case "profile":
                    profilePanel.style.display = DisplayStyle.Flex;
                    profileNavButton.AddToClassList("nav-active");
                    SetupProfilePanel();
                    break;
                case "friends":
                    friendsPanel.style.display = DisplayStyle.Flex;
                    friendsNavButton.AddToClassList("nav-active");
                    SetupFriendsPanel();
                    break;
                case "groups":
                    groupsPanel.style.display = DisplayStyle.Flex;
                    groupsNavButton.AddToClassList("nav-active");
                    SetupGroupsPanel();
                    break;
                case "store":
                    storePanel.style.display = DisplayStyle.Flex;
                    storeNavButton.AddToClassList("nav-active");
                    SetupStorePanel();
                    break;
                case "gamerounds":
                    gameRoundsPanel.style.display = DisplayStyle.Flex;
                    gameRoundsNavButton.AddToClassList("nav-active");
                    SetupGameRoundsPanel();
                    break;
                case "leaderboards":
                    leaderboardsPanel.style.display = DisplayStyle.Flex;
                    leaderboardsNavButton.AddToClassList("nav-active");
                    SetupLeaderboardsPanel();
                    break;
                case "messages":
                    messagesPanel.style.display = DisplayStyle.Flex;
                    messagesNavButton.AddToClassList("nav-active");
                    SetupMessagesPanel();
                    break;
            }

            currentPanel = panelName;
        }

        private void SetupProfilePanel()
        {
            // Profile panel is now managed by ProfilePanelController
            // Ensure the controller has the current user
            if (profileController != null && currentUser != null)
            {
                profileController.SetCurrentUser(currentUser);
            }
        }

        private void SetupFriendsPanel()
        {
            // Friends panel is now managed by FriendsPanelController (Phase 3)
            if (friendsController != null && currentUser != null)
            {
                friendsController.SetCurrentUser(currentUser);
            }
        }

        private void SetupGroupsPanel()
        {
            // Groups panel is now managed by GroupsPanelController (Phase 4)
            if (groupsController != null && currentUser != null)
            {
                groupsController.SetCurrentUser(currentUser);
            }
        }

        private void SetupStorePanel()
        {
            // Store panel is now managed by StorePanelController (Phase 5)
            if (storeController != null && currentUser != null)
            {
                storeController.SetCurrentUser(currentUser);
            }
        }

        private void SetupGameRoundsPanel()
        {
            // Placeholder for game rounds functionality (Phase 6)
            var statusLabel = gameRoundsPanel.Q<Label>("gamerounds-status");
            if (statusLabel != null)
            {
                statusLabel.text = "Game Rounds panel - Coming in Phase 6";
            }
        }

        private void SetupLeaderboardsPanel()
        {
            // Placeholder for leaderboards functionality (Phase 7)
            var statusLabel = leaderboardsPanel.Q<Label>("leaderboards-status");
            if (statusLabel != null)
            {
                statusLabel.text = "Leaderboards panel - Coming in Phase 7";
            }
        }

        private void SetupMessagesPanel()
        {
            // Placeholder for messages functionality (Phase 8)
            var statusLabel = messagesPanel.Q<Label>("messages-status");
            if (statusLabel != null)
            {
                statusLabel.text = "Messages panel - Coming in Phase 8";
            }
        }

        public void RefreshCurrentPanel()
        {
            ShowPanel(currentPanel);
        }

        public void UpdateUserData(GameFuseUser user)
        {
            currentUser = user;
            UpdateUserDisplay();

            // Update active panel controllers
            profileController?.SetCurrentUser(user);
            friendsController?.SetCurrentUser(user);
            groupsController?.SetCurrentUser(user);
            storeController?.SetCurrentUser(user);
            // Future panel controllers will also be updated here
        }

        public GameFuseUser GetCurrentUser()
        {
            return currentUser;
        }

        public void Reset()
        {
            currentUser = null;
            currentPanel = "profile";

            // Reset user display
            userNameLabel.text = "";
            userScoreLabel.text = "Score: 0";
            userCreditsLabel.text = "Credits: 0";

            // Reset panel controllers
            profileController?.Reset();
            friendsController?.Reset();
            groupsController?.Reset();
            storeController?.Reset();
            // Future panel controllers will also be reset here

            // Show default panel
            ShowPanel("profile");
        }

        private void OnDestroy()
        {
            // Unsubscribe from panel controller events
            ProfilePanelController.OnUserDataUpdated -= HandleUserDataUpdated;
            ProfilePanelController.OnError -= HandlePanelError;
            ProfilePanelController.OnSuccess -= HandlePanelSuccess;
            
            // Unsubscribe from friends panel events
            FriendsPanelController.OnError -= HandlePanelError;
            FriendsPanelController.OnSuccess -= HandlePanelSuccess;
            
            // Unsubscribe from groups panel events
            GroupsPanelController.OnError -= HandlePanelError;
            GroupsPanelController.OnSuccess -= HandlePanelSuccess;
            
            // Unsubscribe from store panel events
            StorePanelController.OnError -= HandlePanelError;
            StorePanelController.OnSuccess -= HandlePanelSuccess;
            StorePanelController.OnUserDataUpdated -= HandleUserDataUpdated;
        }
    }
}