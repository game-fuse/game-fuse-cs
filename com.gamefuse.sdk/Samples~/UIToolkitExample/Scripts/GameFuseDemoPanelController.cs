using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GameFuseCSharp;
using GameFuse.UIToolkit;

namespace GameFuse.UIToolkit
{
    public class GameFuseDemoPanelController : MonoBehaviour
    {
        [SerializeField] 
        private GameFuseConfig config;
        
        #region UI Fields
        
        private UIDocument uiDocument;
        private VisualElement rootVisualElement;
        private List<Button> tabButtons = new List<Button>();
        private List<VisualElement> tabContents = new List<VisualElement>();
        private ScrollView logScrollView;
        private VisualElement loadingOverlay;
        
        #endregion
        
        #region Controllers
        
        private GameSetupController gameSetupController;
        private AuthenticationController authController;
        private UserManagementController userManagementController;
        private StoreController storeController;
        private LeaderboardsController leaderboardsController;
        private GameRoundsController gameRoundsController;
        private GroupsController groupsController;
        private ChatsController chatsController;
        private FriendsController friendsController;
        
        #endregion
        
        #region Lifecycle Methods
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument component not found on GameFuseDemoPanelController");
                return;
            }
            
            rootVisualElement = uiDocument.rootVisualElement;
            
            // Get tab buttons and content containers
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
            
            // Get log and loading overlay
            logScrollView = rootVisualElement.Q<ScrollView>("log-scroll");
            loadingOverlay = rootVisualElement.Q<VisualElement>("loading-overlay");
            
            // Create and initialize specialized controllers
            CreateControllers();
        }
        
        private void OnEnable()
        {
            RegisterCallbacks();
        }
        
        private void OnDisable()
        {
            UnregisterCallbacks();
        }
        
        #endregion
        
        #region Controller Creation
        
        private void CreateControllers()
        {
            // Create all specialized controllers
            gameSetupController = CreateController<GameSetupController>();
            authController = CreateController<AuthenticationController>();
            userManagementController = CreateController<UserManagementController>();
            storeController = CreateController<StoreController>();
            leaderboardsController = CreateController<LeaderboardsController>();
            gameRoundsController = CreateController<GameRoundsController>();
            groupsController = CreateController<GroupsController>();
            chatsController = CreateController<ChatsController>();
            friendsController = CreateController<FriendsController>();
        }
        
        private T CreateController<T>() where T : BaseGameFuseUIController, new()
        {
            // Create a new instance of the controller
            T controller = new T();
            
            // Initialize the controller with shared resources
            controller.Initialize(uiDocument, config, logScrollView, loadingOverlay);
            
            return controller;
        }
        
        #endregion
        
        #region Callbacks
        
        private void RegisterCallbacks()
        {
            // Register tab button callbacks
            for (int i = 0; i < tabButtons.Count; i++)
            {
                int index = i; // Capture the index for the lambda
                tabButtons[i].RegisterCallback<ClickEvent>((evt) => SwitchTab(index));
            }
        }
        
        private void UnregisterCallbacks()
        {
            // Unregister tab button callbacks
            for (int i = 0; i < tabButtons.Count; i++)
            {
                int index = i;
                tabButtons[i].UnregisterCallback<ClickEvent>((evt) => SwitchTab(index));
            }
            
            // Cleanup all controllers
            gameSetupController?.Cleanup();
            authController?.Cleanup();
            userManagementController?.Cleanup();
            storeController?.Cleanup();
            leaderboardsController?.Cleanup();
            gameRoundsController?.Cleanup();
            groupsController?.Cleanup();
            chatsController?.Cleanup();
            friendsController?.Cleanup();
        }
        
        #endregion
        
        #region Tab Management
        
        private void SwitchTab(int tabIndex)
        {
            // Update button styles
            for (int i = 0; i < tabButtons.Count; i++)
            {
                if (i == tabIndex)
                {
                    tabButtons[i].AddToClassList("tab-button-selected");
                }
                else
                {
                    tabButtons[i].RemoveFromClassList("tab-button-selected");
                }
            }
            
            // Update content visibility
            for (int i = 0; i < tabContents.Count; i++)
            {
                if (i == tabIndex)
                {
                    tabContents[i].style.display = DisplayStyle.Flex;
                }
                else
                {
                    tabContents[i].style.display = DisplayStyle.None;
                }
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        private async System.Threading.Tasks.Task ExecuteAsync(System.Func<System.Threading.Tasks.Task> action)
        {
            // Show loading overlay
            loadingOverlay.style.display = DisplayStyle.Flex;
            
            try
            {
                // Execute the action
                await action();
            }
            catch (System.Exception ex)
            {
                // Log any exceptions
                LogMessage($"Error: {ex.Message}", LogType.Error);
                Debug.LogException(ex);
            }
            finally
            {
                // Hide loading overlay
                loadingOverlay.style.display = DisplayStyle.None;
            }
        }
        
        private void LogMessage(string message, LogType type)
        {
            // Create log entry
            var logEntry = new Label(message);
            
            // Add timestamp
            logEntry.text = $"[{System.DateTime.Now:HH:mm:ss}] {logEntry.text}";
            
            // Apply styling based on log type
            switch (type)
            {
                case LogType.Success:
                    logEntry.AddToClassList("log-success");
                    break;
                case LogType.Error:
                    logEntry.AddToClassList("log-error");
                    break;
                case LogType.Warning:
                    logEntry.AddToClassList("log-warning");
                    break;
                default:
                    logEntry.AddToClassList("log-info");
                    break;
            }
            
            // Add to log scroll view
            logScrollView.Add(logEntry);
            
            // Scroll to bottom
            logScrollView.scrollOffset = new Vector2(0, float.MaxValue);
        }
        
        #endregion
    }
    
    public enum LogType
    {
        Info,
        Success,
        Warning,
        Error
    }
}