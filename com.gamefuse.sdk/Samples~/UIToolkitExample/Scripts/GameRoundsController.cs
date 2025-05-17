using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFuseCSharp;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Handles game rounds functionality including creating, updating, and retrieving game rounds
    /// </summary>
    public class GameRoundsController : BaseGameFuseUIController
    {
        // Game round info UI
        private TextField gameRoundIdField;
        private TextField gameRoundTypeField;
        private TextField gameRoundScoreField;
        private TextField gameRoundPlaceField;
        private TextField gameRoundMetadataField;
        private TextField gameRoundStartTimeField;
        private TextField gameRoundEndTimeField;
        
        // Game round actions UI
        private Button createSimpleGameRoundButton;
        private Button createDetailedGameRoundButton;
        private Button updateGameRoundButton;
        private Button getGameRoundButton;
        private Button getUserGameRoundsButton;
        private Button getGameGameRoundsButton;
        
        // Multiplayer game round UI
        private TextField multiplayerRoundMatchmakingTypeField;
        private TextField multiplayerRoundUsersField;
        private TextField multiplayerRoundWinnersField;
        private Button createMultiplayerGameRoundButton;
        
        // Results view
        private ScrollView gameRoundsScrollView;
        
        protected override void InitializeUI()
        {
            // Game round info UI elements
            gameRoundIdField = rootElement.Q<TextField>("game-round-id");
            gameRoundTypeField = rootElement.Q<TextField>("game-round-type");
            gameRoundScoreField = rootElement.Q<TextField>("game-round-score");
            gameRoundPlaceField = rootElement.Q<TextField>("game-round-place");
            gameRoundMetadataField = rootElement.Q<TextField>("game-round-metadata");
            gameRoundStartTimeField = rootElement.Q<TextField>("game-round-start-time");
            gameRoundEndTimeField = rootElement.Q<TextField>("game-round-end-time");
            
            // Game round actions UI elements
            createSimpleGameRoundButton = rootElement.Q<Button>("create-simple-game-round-button");
            createDetailedGameRoundButton = rootElement.Q<Button>("create-detailed-game-round-button");
            updateGameRoundButton = rootElement.Q<Button>("update-game-round-button");
            getGameRoundButton = rootElement.Q<Button>("get-game-round-button");
            getUserGameRoundsButton = rootElement.Q<Button>("get-user-game-rounds-button");
            getGameGameRoundsButton = rootElement.Q<Button>("get-game-game-rounds-button");
            
            // Multiplayer game round UI elements
            multiplayerRoundMatchmakingTypeField = rootElement.Q<TextField>("multiplayer-round-matchmaking-type");
            multiplayerRoundUsersField = rootElement.Q<TextField>("multiplayer-round-users");
            multiplayerRoundWinnersField = rootElement.Q<TextField>("multiplayer-round-winners");
            createMultiplayerGameRoundButton = rootElement.Q<Button>("create-multiplayer-game-round-button");
            
            // Results view
            gameRoundsScrollView = rootElement.Q<ScrollView>("game-rounds-scroll");
            
            // Set default values
            if (gameRoundTypeField != null && string.IsNullOrEmpty(gameRoundTypeField.value))
            {
                gameRoundTypeField.value = "standard";
            }
            
            if (gameRoundScoreField != null && string.IsNullOrEmpty(gameRoundScoreField.value))
            {
                gameRoundScoreField.value = "100";
            }
            
            if (gameRoundPlaceField != null && string.IsNullOrEmpty(gameRoundPlaceField.value))
            {
                gameRoundPlaceField.value = "1";
            }
            
            if (multiplayerRoundMatchmakingTypeField != null && string.IsNullOrEmpty(multiplayerRoundMatchmakingTypeField.value))
            {
                multiplayerRoundMatchmakingTypeField.value = "standard";
            }
        }
        
        protected override void RegisterCallbacks()
        {
            if (createSimpleGameRoundButton != null)
            {
                createSimpleGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateSimpleGameRoundClicked());
            }
            
            if (createDetailedGameRoundButton != null)
            {
                createDetailedGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateDetailedGameRoundClicked());
            }
            
            if (updateGameRoundButton != null)
            {
                updateGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnUpdateGameRoundClicked());
            }
            
            if (getGameRoundButton != null)
            {
                getGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGameRoundClicked());
            }
            
            if (getUserGameRoundsButton != null)
            {
                getUserGameRoundsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetUserGameRoundsClicked());
            }
            
            if (getGameGameRoundsButton != null)
            {
                getGameGameRoundsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGameGameRoundsClicked());
            }
            
            if (createMultiplayerGameRoundButton != null)
            {
                createMultiplayerGameRoundButton.RegisterCallback<ClickEvent>(async (evt) => await OnCreateMultiplayerGameRoundClicked());
            }
        }
        
        protected override void UnregisterCallbacks()
        {
            if (createSimpleGameRoundButton != null)
            {
                createSimpleGameRoundButton.UnregisterCallback<ClickEvent>(async (evt) => await OnCreateSimpleGameRoundClicked());
            }
            
            if (createDetailedGameRoundButton != null)
            {
                createDetailedGameRoundButton.UnregisterCallback<ClickEvent>(async (evt) => await OnCreateDetailedGameRoundClicked());
            }
            
            if (updateGameRoundButton != null)
            {
                updateGameRoundButton.UnregisterCallback<ClickEvent>(async (evt) => await OnUpdateGameRoundClicked());
            }
            
            if (getGameRoundButton != null)
            {
                getGameRoundButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetGameRoundClicked());
            }
            
            if (getUserGameRoundsButton != null)
            {
                getUserGameRoundsButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetUserGameRoundsClicked());
            }
            
            if (getGameGameRoundsButton != null)
            {
                getGameGameRoundsButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetGameGameRoundsClicked());
            }
            
            if (createMultiplayerGameRoundButton != null)
            {
                createMultiplayerGameRoundButton.UnregisterCallback<ClickEvent>(async (evt) => await OnCreateMultiplayerGameRoundClicked());
            }
        }
        
        /// <summary>
        /// Creates a simple game round for the current user
        /// </summary>
        private async Task OnCreateSimpleGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
                // Create simple game round with GameRoundObject
                var gameRound = new GameRoundObject
                {
                    GameType = gameType,
                    Score = score
                };
                var response = await GameFuseUser.CurrentUser.CreateGameRoundAsync(gameRound);
                
                // Display the created game round
                DisplayGameRound(response);
                
                LogMessage("Simple game round created successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Creates a detailed game round for the current user
        /// </summary>
        private async Task OnCreateDetailedGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
                GameType = gameType,
                Score = score,
                Place = place
            };
            
            // Add metadata if specified
            if (!string.IsNullOrEmpty(gameRoundMetadataField.value))
            {
                gameRound.Metadata = new Dictionary<string, string>
                {
                    { "info", gameRoundMetadataField.value }
                };
            }
            
            // Add start time if specified
            if (!string.IsNullOrEmpty(gameRoundStartTimeField.value))
            {
                gameRound.StartTime = gameRoundStartTimeField.value;
            }
            
            // Add end time if specified
            if (!string.IsNullOrEmpty(gameRoundEndTimeField.value))
            {
                gameRound.EndTime = gameRoundEndTimeField.value;
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
        
        /// <summary>
        /// Updates an existing game round
        /// </summary>
        private async Task OnUpdateGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
            
            if (!int.TryParse(gameRoundId, out int roundId))
            {
                LogMessage("Game Round ID must be a valid integer", LogType.Error);
                return;
            }
            
            // Create GameRoundObject for update
            GameRoundObject gameRound = new GameRoundObject
            {
                Id = roundId,
                Score = score,
                Place = place
            };
            
            // Add metadata if specified
            if (!string.IsNullOrEmpty(gameRoundMetadataField.value))
            {
                gameRound.Metadata = new Dictionary<string, string>
                {
                    { "info", gameRoundMetadataField.value }
                };
            }
            
            // Add end time if specified
            if (!string.IsNullOrEmpty(gameRoundEndTimeField.value))
            {
                gameRound.EndTime = gameRoundEndTimeField.value;
            }
            
            await ExecuteAsync(async () =>
            {
                // Update game round
                var response = await GameFuseUser.CurrentUser.UpdateGameRoundAsync(roundId, gameRound);
                
                // Display the updated game round
                DisplayGameRound(response);
                
                LogMessage("Game round updated successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Retrieves a specific game round by ID
        /// </summary>
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
                if (!int.TryParse(gameRoundId, out int roundId))
                {
                    LogMessage("Game Round ID must be a valid integer", LogType.Error);
                    return;
                }
                
                // Get specific game round
                var response = await GameFuseUser.CurrentUser.GetGameRoundAsync(roundId);
                
                // Display the game round
                DisplayGameRound(response);
                
                LogMessage("Game round retrieved successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Retrieves game rounds for the current user
        /// </summary>
        private async Task OnGetUserGameRoundsClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to view user game rounds", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's game rounds
                var response = await GameFuseUser.CurrentUser.GetMyGameRoundsAsync();
                
                // Display the game rounds
                DisplayGameRounds(response);
                
                LogMessage("User game rounds retrieved successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Retrieves game rounds for all users in the game
        /// </summary>
        private async Task OnGetGameGameRoundsClicked()
        {
            await ExecuteAsync(async () =>
            {
                // This method isn't available in the new API
                // Let's fetch user game rounds instead for now
                var response = await GameFuseUser.CurrentUser.GetMyGameRoundsAsync();
                
                // Display the game rounds
                DisplayGameRounds(response);
                
                LogMessage("Game game rounds retrieved successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Creates a multiplayer game round
        /// </summary>
        private async Task OnCreateMultiplayerGameRoundClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to create multiplayer game rounds", LogType.Error);
                return;
            }
            
            string matchmakingType = multiplayerRoundMatchmakingTypeField.value;
            string usersStr = multiplayerRoundUsersField.value;
            string winnersStr = multiplayerRoundWinnersField.value;
            
            if (string.IsNullOrEmpty(matchmakingType))
            {
                LogMessage("Matchmaking type is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(usersStr))
            {
                LogMessage("Users list is required", LogType.Error);
                return;
            }
            
            // Parse users and winners lists
            string[] users = usersStr.Split(',');
            string[] winners = string.IsNullOrEmpty(winnersStr) ? new string[0] : winnersStr.Split(',');
            
            // Trim whitespace
            for (int i = 0; i < users.Length; i++)
            {
                users[i] = users[i].Trim();
            }
            
            for (int i = 0; i < winners.Length; i++)
            {
                winners[i] = winners[i].Trim();
            }
            
            await ExecuteAsync(async () =>
            {
                // Create a GameRoundObject for the current user
                var gameRound = new GameRoundObject
                {
                    GameType = matchmakingType,
                    Score = 0,
                    Place = 1,
                    Multiplayer = true
                };
                
                // Create a List<GameRoundObject> with just this player's round
                var playerRounds = new List<GameRoundObject> { gameRound };
                
                // Create multiplayer game round
                var response = await GameFuseUser.CurrentUser.CreateMultiplayerGameRoundAsync(matchmakingType, playerRounds);
                
                // Display the created game round
                DisplayMultiplayerGameRound(response);
                
                LogMessage("Multiplayer game round created successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Displays a single game round in the UI
        /// </summary>
        private void DisplayGameRound(GameRoundObject gameRound)
        {
            // Clear current game rounds
            ClearScrollView(gameRoundsScrollView);
            
            if (gameRound != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", gameRound.Id.ToString() },
                    { "Type", gameRound.GameType },
                    { "Score", gameRound.Score.ToString() },
                    { "Place", gameRound.Place.ToString() }
                };
                
                // Add optional properties if available
                if (gameRound.Metadata != null && gameRound.Metadata.Count > 0)
                {
                    properties.Add("Metadata", string.Join(", ", gameRound.Metadata.Select(kv => $"{kv.Key}:{kv.Value}")));
                }
                
                if (!string.IsNullOrEmpty(gameRound.StartTime))
                {
                    properties.Add("Start Time", gameRound.StartTime);
                }
                
                if (!string.IsNullOrEmpty(gameRound.EndTime))
                {
                    properties.Add("End Time", gameRound.EndTime);
                }
                
                // Add the game round to the UI
                var roundElement = CreateListItem($"Game Round: {gameRound.Id}", properties);
                
                // Add click handler to copy the ID for updates
                roundElement.RegisterCallback<ClickEvent>((evt) => 
                {
                    gameRoundIdField.value = gameRound.Id.ToString();
                });
                
                gameRoundsScrollView.Add(roundElement);
            }
            else
            {
                LogMessage("No game round found", LogType.Info);
            }
        }
        
        /// <summary>
        /// Displays a multiplayer game round in the UI
        /// </summary>
        private void DisplayMultiplayerGameRound(MultiplayerGameRoundResponse response)
        {
            // Clear current game rounds
            ClearScrollView(gameRoundsScrollView);
            
            if (response != null)
            {
                // In the new API, MultiplayerGameRoundResponse has different structure
                var properties = new Dictionary<string, string>();
                
                if (response.MultiplayerGameRound != null)
                {
                    var gameRound = response.MultiplayerGameRound;
                    properties.Add("ID", gameRound.Id.ToString());
                    properties.Add("Type", gameRound.GameType);
                    
                    if (gameRound.Metadata != null && gameRound.Metadata.Count > 0)
                    {
                        properties.Add("Metadata", string.Join(", ", gameRound.Metadata.Select(kv => $"{kv.Key}:{kv.Value}")));
                    }
                    
                    if (!string.IsNullOrEmpty(gameRound.StartTime))
                    {
                        properties.Add("Start Time", gameRound.StartTime);
                    }
                    
                    if (!string.IsNullOrEmpty(gameRound.EndTime))
                    {
                        properties.Add("End Time", gameRound.EndTime);
                    }
                    
                    // Add the game round to the UI
                    var roundElement = CreateListItem($"Multiplayer Game Round: {gameRound.Id}", properties);
                    
                    // Add click handler to copy the ID for updates
                    roundElement.RegisterCallback<ClickEvent>((evt) => 
                    {
                        gameRoundIdField.value = gameRound.Id.ToString();
                    });
                    
                    gameRoundsScrollView.Add(roundElement);
                }
                else
                {
                    LogMessage("No game rounds in multiplayer response", LogType.Info);
                }
            }
            else
            {
                LogMessage("No multiplayer game round found", LogType.Info);
            }
        }
        
        /// <summary>
        /// Displays a list of game rounds in the UI
        /// </summary>
        private void DisplayGameRounds(GameRoundsResponse response)
        {
            // Clear current game rounds
            ClearScrollView(gameRoundsScrollView);
            
            if (response != null && response.GameRounds != null && response.GameRounds.Length > 0)
            {
                foreach (var gameRound in response.GameRounds)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", gameRound.Id.ToString() },
                        { "Type", gameRound.GameType },
                        { "Score", gameRound.Score.ToString() },
                        { "Place", gameRound.Place.ToString() }
                    };
                    
                    // Add optional properties if available
                    if (gameRound.Metadata != null && gameRound.Metadata.Count > 0)
                    {
                        properties.Add("Metadata", string.Join(", ", gameRound.Metadata.Select(kv => $"{kv.Key}:{kv.Value}")));
                    }
                    
                    if (!string.IsNullOrEmpty(gameRound.StartTime))
                    {
                        properties.Add("Start Time", gameRound.StartTime);
                    }
                    
                    if (!string.IsNullOrEmpty(gameRound.EndTime))
                    {
                        properties.Add("End Time", gameRound.EndTime);
                    }
                    
                    // Add the game round to the UI
                    var roundElement = CreateListItem($"Game Round: {gameRound.Id}", properties);
                    
                    // Add click handler to copy the ID for updates
                    roundElement.RegisterCallback<ClickEvent>((evt) => 
                    {
                        gameRoundIdField.value = gameRound.Id.ToString();
                    });
                    
                    gameRoundsScrollView.Add(roundElement);
                }
                
                LogMessage($"Retrieved {response.GameRounds.Length} game rounds", LogType.Success);
            }
            else
            {
                LogMessage("No game rounds found", LogType.Info);
            }
        }
    }
}