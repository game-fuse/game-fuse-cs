using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using GameFuse.UI.Controls;
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
        private GameRoundsPanelController gameRoundsController;
        private LeaderboardsPanelController leaderboardsController;
        private MessagesPanelController messagesController;

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
            
            if (headerPanel == null) Debug.LogError("header-panel not found");
            if (navigationPanel == null) Debug.LogError("navigation-panel not found");
            if (contentPanel == null) Debug.LogError("content-panel not found");

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

            // Game Rounds Panel Controller (Phase 6)
            if (gameRoundsPanel != null)
            {
                var gameRoundsControl = gameRoundsPanel.Q<GameRoundsPanel>("game-rounds-control");
                if (gameRoundsControl != null)
                {
                    gameRoundsController = new GameRoundsPanelController(gameRoundsControl);
                }
            }

            // Leaderboards Panel Controller (Phase 7)
            if (leaderboardsPanel != null)
            {
                var leaderboardsControl = leaderboardsPanel.Q<LeaderboardsPanel>("leaderboards-control");
                if (leaderboardsControl != null)
                {
                    leaderboardsController = new LeaderboardsPanelController(leaderboardsControl);
                }
            }

            // Messages Panel Controller (Phase 8)
            if (messagesPanel != null)
            {
                messagesController = new MessagesPanelController(messagesPanel);
            }
        }

        private void SetupEventHandlers()
        {
            // Header actions
            if (signOutButton != null) signOutButton.clicked += () => OnSignOutRequested?.Invoke();

            // Navigation
            if (profileNavButton != null) profileNavButton.clicked += () => ShowPanel("profile");
            if (friendsNavButton != null) friendsNavButton.clicked += () => ShowPanel("friends");
            if (groupsNavButton != null) groupsNavButton.clicked += () => ShowPanel("groups");
            if (storeNavButton != null) storeNavButton.clicked += () => ShowPanel("store");
            if (gameRoundsNavButton != null) gameRoundsNavButton.clicked += () => ShowPanel("gamerounds");
            if (leaderboardsNavButton != null) leaderboardsNavButton.clicked += () => ShowPanel("leaderboards");
            if (messagesNavButton != null) messagesNavButton.clicked += () => ShowPanel("messages");

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
            gameRoundsController?.Initialize(user);
            leaderboardsController?.Initialize(user);
            messagesController?.SetCurrentUser(user);
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
            if (profilePanel != null) profilePanel.style.display = DisplayStyle.None;
            if (friendsPanel != null) friendsPanel.style.display = DisplayStyle.None;
            if (groupsPanel != null) groupsPanel.style.display = DisplayStyle.None;
            if (storePanel != null) storePanel.style.display = DisplayStyle.None;
            if (gameRoundsPanel != null) gameRoundsPanel.style.display = DisplayStyle.None;
            if (leaderboardsPanel != null) leaderboardsPanel.style.display = DisplayStyle.None;
            if (messagesPanel != null) messagesPanel.style.display = DisplayStyle.None;

            // Remove active class from all nav buttons
            if (profileNavButton != null) profileNavButton.RemoveFromClassList("nav-active");
            if (friendsNavButton != null) friendsNavButton.RemoveFromClassList("nav-active");
            if (groupsNavButton != null) groupsNavButton.RemoveFromClassList("nav-active");
            if (storeNavButton != null) storeNavButton.RemoveFromClassList("nav-active");
            if (gameRoundsNavButton != null) gameRoundsNavButton.RemoveFromClassList("nav-active");
            if (leaderboardsNavButton != null) leaderboardsNavButton.RemoveFromClassList("nav-active");
            if (messagesNavButton != null) messagesNavButton.RemoveFromClassList("nav-active");

            // Show selected panel and mark nav button as active
            switch (panelName)
            {
                case "profile":
                    if (profilePanel != null) profilePanel.style.display = DisplayStyle.Flex;
                    if (profileNavButton != null) profileNavButton.AddToClassList("nav-active");
                    SetupProfilePanel();
                    break;
                case "friends":
                    if (friendsPanel != null) friendsPanel.style.display = DisplayStyle.Flex;
                    if (friendsNavButton != null) friendsNavButton.AddToClassList("nav-active");
                    SetupFriendsPanel();
                    break;
                case "groups":
                    if (groupsPanel != null) groupsPanel.style.display = DisplayStyle.Flex;
                    if (groupsNavButton != null) groupsNavButton.AddToClassList("nav-active");
                    SetupGroupsPanel();
                    break;
                case "store":
                    if (storePanel != null) storePanel.style.display = DisplayStyle.Flex;
                    if (storeNavButton != null) storeNavButton.AddToClassList("nav-active");
                    SetupStorePanel();
                    break;
                case "gamerounds":
                    if (gameRoundsPanel != null) gameRoundsPanel.style.display = DisplayStyle.Flex;
                    if (gameRoundsNavButton != null) gameRoundsNavButton.AddToClassList("nav-active");
                    SetupGameRoundsPanel();
                    break;
                case "leaderboards":
                    if (leaderboardsPanel != null) leaderboardsPanel.style.display = DisplayStyle.Flex;
                    if (leaderboardsNavButton != null) leaderboardsNavButton.AddToClassList("nav-active");
                    SetupLeaderboardsPanel();
                    break;
                case "messages":
                    if (messagesPanel != null) messagesPanel.style.display = DisplayStyle.Flex;
                    if (messagesNavButton != null) messagesNavButton.AddToClassList("nav-active");
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
            // Game Rounds panel is now managed by GameRoundsPanelController (Phase 6)
            if (gameRoundsController != null && currentUser != null)
            {
                gameRoundsController.Initialize(currentUser);
            }
        }

        private void SetupLeaderboardsPanel()
        {
            // Leaderboards panel is now managed by LeaderboardsPanelController (Phase 7)
            if (leaderboardsController != null && currentUser != null)
            {
                leaderboardsController.Initialize(currentUser);
            }
        }

        private void SetupMessagesPanel()
        {
            // Messages panel is now managed by MessagesPanelController (Phase 8)
            if (messagesController != null && currentUser != null)
            {
                messagesController.SetCurrentUser(currentUser);
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
            gameRoundsController?.Initialize(user);
            leaderboardsController?.Initialize(user);
            messagesController?.SetCurrentUser(user);
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
            gameRoundsController?.Cleanup();
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