using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using GameFuseCSharp;

namespace GameFuse.UIToolkit
{
    public class GameSetupController : BaseGameFuseUIController
    {
        #region UI Elements
        
        private TextField gameIdField;
        private TextField gameTokenField;
        private Label gameNameLabel;
        private Label gameDescriptionLabel;
        private Button setupGameButton;
        private Button getServerTimeButton;
        private Button fetchGameVariablesButton;
        private Button getStoreItemsButton;
        private ScrollView gameVariablesScrollView;
        private ScrollView gameStoreItemsScrollView;
        
        #endregion
        
        public override void Initialize(UIDocument document, GameFuseConfig config, ScrollView logScrollView, VisualElement loadingOverlay)
        {
            base.Initialize(document, config, logScrollView, loadingOverlay);
            
            // Set the root element for this controller
            var content = document.rootVisualElement.Q<VisualElement>("content-game-setup");
            SetRootElement(content);
        }
        
        #region BaseGameFuseUIController Implementation
        
        protected override void InitializeUI()
        {
            if (rootElement == null) return;
            
            // Fields
            gameIdField = rootElement.Q<TextField>("game-id");
            gameTokenField = rootElement.Q<TextField>("game-token");
            
            // Labels
            gameNameLabel = rootElement.Q<Label>("game-name-label");
            gameDescriptionLabel = rootElement.Q<Label>("game-description-label");
            
            // Buttons
            setupGameButton = rootElement.Q<Button>("setup-game-button");
            getServerTimeButton = rootElement.Q<Button>("get-server-time-button");
            fetchGameVariablesButton = rootElement.Q<Button>("fetch-game-variables-button");
            getStoreItemsButton = rootElement.Q<Button>("get-store-items-button");
            
            // ScrollViews
            gameVariablesScrollView = rootElement.Q<ScrollView>("game-variables-scroll");
            gameStoreItemsScrollView = rootElement.Q<ScrollView>("game-store-items-scroll");
            
            // Apply configuration if available
            if (config != null)
            {
                gameIdField.value = config.GameId;
                gameTokenField.value = config.GameToken;
            }
        }
        
        protected override void RegisterCallbacks()
        {
            if (rootElement == null) return;
            
            setupGameButton?.RegisterCallback<ClickEvent>(async (evt) => await OnSetupGameClicked());
            getServerTimeButton?.RegisterCallback<ClickEvent>(async (evt) => await OnGetServerTimeClicked());
            fetchGameVariablesButton?.RegisterCallback<ClickEvent>(async (evt) => await OnFetchGameVariablesClicked());
            getStoreItemsButton?.RegisterCallback<ClickEvent>(async (evt) => await OnGetStoreItemsClicked());
        }
        
        protected override void UnregisterCallbacks()
        {
            if (rootElement == null) return;
            
            setupGameButton?.UnregisterCallback<ClickEvent>(async (evt) => await OnSetupGameClicked());
            getServerTimeButton?.UnregisterCallback<ClickEvent>(async (evt) => await OnGetServerTimeClicked());
            fetchGameVariablesButton?.UnregisterCallback<ClickEvent>(async (evt) => await OnFetchGameVariablesClicked());
            getStoreItemsButton?.UnregisterCallback<ClickEvent>(async (evt) => await OnGetStoreItemsClicked());
        }
        
        #endregion
        
        #region Event Handlers
        
        private async Task OnSetupGameClicked()
        {
            string gameId = gameIdField.value;
            string gameToken = gameTokenField.value;
            
            if (string.IsNullOrEmpty(gameId))
            {
                LogMessage("Game ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(gameToken))
            {
                LogMessage("Game Token is required", LogType.Error);
                return;
            }
            
            // Save configuration
            if (config != null)
            {
                config.GameId = gameId;
                config.GameToken = gameToken;
            }
            
            await ExecuteAsync(async () =>
            {
                // Initialize GameFuse
                await GameFuseCSharp.GameFuse.SetUpGameAsync(gameId, gameToken);
                
                // Update game information display
                UpdateGameInformation();
                
                LogMessage("GameFuse initialized successfully", LogType.Success);
            });
        }
        
        private async Task OnGetServerTimeClicked()
        {
            await ExecuteAsync(async () =>
            {
                // GameFuse doesn't have a GetServerTimeAsync method in the current API
                // As a workaround, we'll use DateTime.UtcNow
                var serverTime = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                LogMessage($"Current UTC time: {serverTime}", LogType.Info);
            });
        }
        
        private async Task OnFetchGameVariablesClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Access the game variables directly from GameFuse instance
                var variables = GameFuseCSharp.GameFuse.Instance.gameVariables;
                
                // Clear current variables display
                ClearScrollView(gameVariablesScrollView);
                
                // Display variables
                if (variables != null && variables.Count > 0)
                {
                    foreach (var variable in variables)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "Value", variable.Value.ToString() }
                        };
                        
                        var variableItem = CreateListItem(variable.Key, properties);
                        gameVariablesScrollView.Add(variableItem);
                    }
                }
                else
                {
                    gameVariablesScrollView.Add(new Label("No game variables available"));
                }
                
                LogMessage("Game variables fetched successfully", LogType.Success);
            });
        }
        
        private async Task OnGetStoreItemsClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Get store items - in the current API, these are accessed through GetStoreItems()
                var storeItems = GameFuseCSharp.GameFuse.GetStoreItems();
                
                // Clear current store items display
                ClearScrollView(gameStoreItemsScrollView);
                
                // Display store items
                if (storeItems != null && storeItems.Count > 0)
                {
                    for (int i = 0; i < storeItems.Count; i++)
                    {
                        var item = storeItems[i];
                        var properties = new Dictionary<string, string>
                        {
                            { "ID", item.GetId().ToString() },
                            { "Type", item.GetCategory() },
                            { "Cost", item.GetCost().ToString() },
                            { "Description", item.GetDescription() }
                        };
                        
                        var itemElement = CreateListItem(item.GetName(), properties);
                        gameStoreItemsScrollView.Add(itemElement);
                    }
                }
                else
                {
                    gameStoreItemsScrollView.Add(new Label("No store items available"));
                }
                
                LogMessage($"Retrieved {storeItems.Count} store items", LogType.Success);
            });
        }
        
        #endregion
        
        #region Helper Methods
        
        private void UpdateGameInformation()
        {
            // Get information directly from GameFuse instance
            gameNameLabel.text = GameFuseCSharp.GameFuse.GetGameName();
            gameDescriptionLabel.text = GameFuseCSharp.GameFuse.GetGameDescription();
        }
        
        #endregion
    }
}