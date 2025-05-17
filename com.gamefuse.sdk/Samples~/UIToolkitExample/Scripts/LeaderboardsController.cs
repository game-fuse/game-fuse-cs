using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuseCSharp;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Handles leaderboard functionality including adding entries and retrieving leaderboard data
    /// </summary>
    public class LeaderboardsController : BaseGameFuseUIController
    {
        // Leaderboard UI
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
        
        protected override void InitializeUI()
        {
            // Leaderboard UI elements
            leaderboardNameField = rootElement.Q<TextField>("leaderboard-name");
            leaderboardScoreField = rootElement.Q<TextField>("leaderboard-score");
            leaderboardMetadataField = rootElement.Q<TextField>("leaderboard-metadata");
            leaderboardUserIdField = rootElement.Q<TextField>("leaderboard-user-id");
            leaderboardLimitField = rootElement.Q<TextField>("leaderboard-limit");
            
            addLeaderboardEntryButton = rootElement.Q<Button>("add-leaderboard-entry-button");
            addLeaderboardWithMetadataButton = rootElement.Q<Button>("add-leaderboard-with-metadata-button");
            clearLeaderboardEntriesButton = rootElement.Q<Button>("clear-leaderboard-entries-button");
            getMyLeaderboardEntriesButton = rootElement.Q<Button>("get-my-leaderboard-entries-button");
            getUserLeaderboardEntriesButton = rootElement.Q<Button>("get-user-leaderboard-entries-button");
            getGameLeaderboardEntriesButton = rootElement.Q<Button>("get-game-leaderboard-entries-button");
            
            leaderboardResultsScrollView = rootElement.Q<ScrollView>("leaderboard-results-scroll");
            
            // Default limit for leaderboard entries if not specified
            if (leaderboardLimitField != null)
            {
                leaderboardLimitField.value = "10";
            }
        }
        
        protected override void RegisterCallbacks()
        {
            if (addLeaderboardEntryButton != null)
            {
                addLeaderboardEntryButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddLeaderboardEntryClicked());
            }
            
            if (addLeaderboardWithMetadataButton != null)
            {
                addLeaderboardWithMetadataButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddLeaderboardEntryWithMetadataClicked());
            }
            
            if (clearLeaderboardEntriesButton != null)
            {
                clearLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnClearLeaderboardEntriesClicked());
            }
            
            if (getMyLeaderboardEntriesButton != null)
            {
                getMyLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetMyLeaderboardEntriesClicked());
            }
            
            if (getUserLeaderboardEntriesButton != null)
            {
                getUserLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetUserLeaderboardEntriesClicked());
            }
            
            if (getGameLeaderboardEntriesButton != null)
            {
                getGameLeaderboardEntriesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetGameLeaderboardEntriesClicked());
            }
        }
        
        protected override void UnregisterCallbacks()
        {
            if (addLeaderboardEntryButton != null)
            {
                addLeaderboardEntryButton.UnregisterCallback<ClickEvent>(async (evt) => await OnAddLeaderboardEntryClicked());
            }
            
            if (addLeaderboardWithMetadataButton != null)
            {
                addLeaderboardWithMetadataButton.UnregisterCallback<ClickEvent>(async (evt) => await OnAddLeaderboardEntryWithMetadataClicked());
            }
            
            if (clearLeaderboardEntriesButton != null)
            {
                clearLeaderboardEntriesButton.UnregisterCallback<ClickEvent>(async (evt) => await OnClearLeaderboardEntriesClicked());
            }
            
            if (getMyLeaderboardEntriesButton != null)
            {
                getMyLeaderboardEntriesButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetMyLeaderboardEntriesClicked());
            }
            
            if (getUserLeaderboardEntriesButton != null)
            {
                getUserLeaderboardEntriesButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetUserLeaderboardEntriesClicked());
            }
            
            if (getGameLeaderboardEntriesButton != null)
            {
                getGameLeaderboardEntriesButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetGameLeaderboardEntriesClicked());
            }
        }
        
        /// <summary>
        /// Adds a leaderboard entry for the current user
        /// </summary>
        private async Task OnAddLeaderboardEntryClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
                await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync(leaderboardName, score);
                
                LogMessage("Leaderboard entry added successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Adds a leaderboard entry with metadata for the current user
        /// </summary>
        private async Task OnAddLeaderboardEntryWithMetadataClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
                await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync(leaderboardName, score, metadata);
                
                LogMessage("Leaderboard entry with metadata added successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Clears all leaderboard entries for the current user
        /// </summary>
        private async Task OnClearLeaderboardEntriesClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
                await GameFuseUser.CurrentUser.ClearLeaderboardEntriesAsync(leaderboardName);
                
                LogMessage("Leaderboard entries cleared successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Gets and displays leaderboard entries for the current user
        /// </summary>
        private async Task OnGetMyLeaderboardEntriesClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
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
                var entries = await GameFuseUser.CurrentUser.GetMyLeaderboardEntriesAsync(leaderboardName, limit);
                
                // Display leaderboard entries
                DisplayLeaderboardEntries(entries);
            });
        }
        
        /// <summary>
        /// Gets and displays leaderboard entries for a specific user
        /// </summary>
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
                var entries = await GameFuseUser.GetUserLeaderboardEntriesAsync(leaderboardName, userId, limit);
                
                // Display leaderboard entries
                DisplayLeaderboardEntries(entries);
            });
        }
        
        /// <summary>
        /// Gets and displays leaderboard entries for the game
        /// </summary>
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
                // Get game leaderboard entries
                var entries = await GameFuseCSharp.LeaderboardService.GetGameLeaderboardEntriesAsync(leaderboardName, limit);
                
                // Display leaderboard entries
                DisplayLeaderboardEntries(entries);
            });
        }
        
        /// <summary>
        /// Displays leaderboard entries in the UI
        /// </summary>
        private void DisplayLeaderboardEntries(List<GameFuseCSharp.LeaderboardEntryObject> entries)
        {
            // Clear current entries
            ClearScrollView(leaderboardResultsScrollView);
            
            if (entries != null && entries.Count > 0)
            {
                int rank = 1;
                foreach (var entry in entries)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "Rank", rank.ToString() },
                        { "User ID", entry.UserId.ToString() },
                        { "Username", entry.Username },
                        { "Score", entry.Score.ToString() }
                    };
                    
                    // Add metadata if available
                    if (!string.IsNullOrEmpty(entry.Metadata))
                    {
                        properties.Add("Metadata", entry.Metadata);
                    }
                    
                    // Add time if available
                    if (!string.IsNullOrEmpty(entry.Time))
                    {
                        properties.Add("Time", entry.Time);
                    }
                    
                    leaderboardResultsScrollView.Add(CreateListItem($"Entry #{rank}", properties));
                    rank++;
                }
                
                LogMessage($"Retrieved {entries.Count} leaderboard entries", LogType.Success);
            }
            else
            {
                LogMessage("No leaderboard entries found", LogType.Info);
            }
        }
    }
}