using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using GameFuse.Config;
using System.Threading.Tasks;
using System;

namespace GameFuse.UI
{
    /// <summary>
    /// Main UI controller for the GameFuse SDK reference implementation.
    /// Manages all UI panels and navigation between different features.
    /// Updated for Phase 2: Enhanced user data management and global state synchronization.
    /// </summary>
    public class GameFuseUIController : MonoBehaviour
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument uiDocument;

        // Root visual elements
        private VisualElement root;
        private VisualElement authPanel;
        private VisualElement mainPanel;
        private VisualElement loadingOverlay;

        // Panel controllers
        private AuthPanelController authController;
        private MainPanelController mainController;

        // Current user state
        private GameFuseUser currentUser;
        private bool isAuthenticated = false;

        // Events
        public static event Action<GameFuseUser> OnUserAuthenticated;
        public static event Action OnUserSignedOut;

        private void Awake()
        {
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            InitializeUI();
        }

        private void OnEnable()
        {
            // Subscribe to authentication events
            AuthPanelController.OnSignUpSuccess += HandleSignUpSuccess;
            AuthPanelController.OnSignInSuccess += HandleSignInSuccess;
            AuthPanelController.OnSignInError += HandleAuthError;
            AuthPanelController.OnSignUpError += HandleAuthError;

            // Subscribe to main panel events (Phase 2 Addition)
            MainPanelController.OnSignOutRequested += HandleSignOut;
            MainPanelController.OnUserDataUpdated += HandleUserDataUpdated;
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            AuthPanelController.OnSignUpSuccess -= HandleSignUpSuccess;
            AuthPanelController.OnSignInSuccess -= HandleSignInSuccess;
            AuthPanelController.OnSignInError -= HandleAuthError;
            AuthPanelController.OnSignUpError -= HandleAuthError;

            // Unsubscribe from main panel events (Phase 2 Addition)
            MainPanelController.OnSignOutRequested -= HandleSignOut;
            MainPanelController.OnUserDataUpdated -= HandleUserDataUpdated;
        }

        private void InitializeUI()
        {
            root = uiDocument.rootVisualElement;

            // Validate GameFuse settings
            if (!ValidateGameFuseSettings())
            {
                ShowSettingsError();
                return;
            }

            // Get main panels
            authPanel = root.Q<VisualElement>("auth-panel");
            mainPanel = root.Q<VisualElement>("main-panel");
            loadingOverlay = root.Q<VisualElement>("loading-overlay");

            // Initialize panel controllers
            authController = new AuthPanelController(authPanel);
            mainController = new MainPanelController(mainPanel);

            // Start with authentication panel
            ShowAuthPanel();
        }

        private bool ValidateGameFuseSettings()
        {
            var settings = GameFuseSettings.Settings;
            if (settings == null)
            {
                Debug.LogError("GameFuseSettings not found! Please create a GameFuseSettings asset in a Resources folder.");
                return false;
            }

            if (string.IsNullOrEmpty(settings.GameId) || string.IsNullOrEmpty(settings.GameApiKey))
            {
                Debug.LogError("GameFuseSettings is missing GameId or GameApiKey. Please configure the settings asset.");
                return false;
            }

            return true;
        }

        private void ShowSettingsError()
        {
            // Show a clear error message about missing settings
            var errorPanel = new VisualElement();
            errorPanel.AddToClassList("settings-error-panel");

            var titleLabel = new Label("GameFuse Configuration Required");
            titleLabel.AddToClassList("error-title");

            var messageLabel = new Label("Please create and configure a GameFuseSettings asset:\n\n" +
                "1. Right-click in Project window\n" +
                "2. Choose Create > GameFuse > Settings\n" +
                "3. Place it in a Resources folder\n" +
                "4. Set your GameId and GameApiKey");
            messageLabel.AddToClassList("error-message");

            errorPanel.Add(titleLabel);
            errorPanel.Add(messageLabel);

            root.Add(errorPanel);
        }

        private void ShowAuthPanel()
        {
            authPanel.style.display = DisplayStyle.Flex;
            mainPanel.style.display = DisplayStyle.None;
            loadingOverlay.style.display = DisplayStyle.None;
        }

        private void ShowMainPanel()
        {
            authPanel.style.display = DisplayStyle.None;
            mainPanel.style.display = DisplayStyle.Flex;
            loadingOverlay.style.display = DisplayStyle.None;
        }

        private void ShowLoading(bool show)
        {
            loadingOverlay.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private async void HandleSignUpSuccess(GameFuseUser user)
        {
            currentUser = user;
            isAuthenticated = true;

            Debug.Log($"User signed up successfully: {user.Username}");

            // Update main panel with user data
            mainController.SetCurrentUser(user);

            ShowMainPanel();
            OnUserAuthenticated?.Invoke(user);
        }

        private async void HandleSignInSuccess(GameFuseUser user)
        {
            currentUser = user;
            isAuthenticated = true;

            Debug.Log($"User signed in successfully: {user.Username}");

            // Update main panel with user data
            mainController.SetCurrentUser(user);

            ShowMainPanel();
            OnUserAuthenticated?.Invoke(user);
        }

        private void HandleAuthError(string errorMessage)
        {
            Debug.LogError($"Authentication error: {errorMessage}");
            ShowErrorMessage(errorMessage);
        }

        // Phase 2 Addition: Handle user data updates from profile panel
        private void HandleUserDataUpdated(GameFuseUser updatedUser)
        {
            // Update our current user reference
            currentUser = updatedUser;

            // Update main panel with new user data
            mainController.UpdateUserData(updatedUser);

            // Trigger global user data updated event
            OnUserAuthenticated?.Invoke(updatedUser);

            Debug.Log($"User data updated globally: {updatedUser.Username} - Score: {updatedUser.Score}, Credits: {updatedUser.Credits}");
        }

        private void HandleSignOut()
        {
            if (currentUser != null)
            {
                currentUser.SignOut();
                currentUser = null;
            }

            isAuthenticated = false;

            // Reset panels
            mainController.Reset();
            authController.Reset();

            ShowAuthPanel();
            OnUserSignedOut?.Invoke();

            Debug.Log("User signed out successfully");
        }

        private void ShowErrorMessage(string message)
        {
            // Create a simple error popup
            var errorPopup = new VisualElement();
            errorPopup.AddToClassList("error-popup");

            var errorLabel = new Label(message);
            errorLabel.AddToClassList("error-text");

            var closeButton = new Button(() => {
                root.Remove(errorPopup);
            });
            closeButton.text = "OK";
            closeButton.AddToClassList("error-button");

            errorPopup.Add(errorLabel);
            errorPopup.Add(closeButton);

            root.Add(errorPopup);

            // Auto-remove after 5 seconds
            _ = Task.Delay(5000).ContinueWith(_ => {
                if (errorPopup.parent != null)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => {
                        root.Remove(errorPopup);
                    });
                }
            });
        }

        // Public API for other components
        public GameFuseUser GetCurrentUser() => currentUser;
        public bool IsAuthenticated() => isAuthenticated;

        public void ShowLoadingState(bool show) => ShowLoading(show);

        // Phase 2 Addition: Additional methods for enhanced user data management
        public void RefreshUserData()
        {
            if (currentUser != null && isAuthenticated)
            {
                mainController.UpdateUserData(currentUser);
            }
        }

        public void NotifyUserDataChanged(GameFuseUser updatedUser)
        {
            HandleUserDataUpdated(updatedUser);
        }
    }

    /// <summary>
    /// Simple utility class to execute actions on the main thread.
    /// </summary>
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private readonly System.Collections.Generic.Queue<Action> _actionQueue = new System.Collections.Generic.Queue<Action>();

        public static UnityMainThreadDispatcher Instance()
        {
            if (_instance == null)
            {
                var go = new GameObject("MainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }

        public void Enqueue(Action action)
        {
            lock (_actionQueue)
            {
                _actionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (_actionQueue)
            {
                while (_actionQueue.Count > 0)
                {
                    _actionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}