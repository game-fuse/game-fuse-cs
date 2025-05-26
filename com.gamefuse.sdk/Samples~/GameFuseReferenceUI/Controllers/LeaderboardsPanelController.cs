using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using GameFuse;
using GameFuse.Models.Shared;
using GameFuse.Config;

namespace GameFuse.UI
{
    public class LeaderboardsPanelController
    {
        private LeaderboardsPanel panel;
        private GameFuseUser currentUser;
        private List<LeaderboardEntryModel> gameLeaderboardEntries = new List<LeaderboardEntryModel>();
        private List<LeaderboardEntryModel> userLeaderboardEntries = new List<LeaderboardEntryModel>();
        private string currentLeaderboardFilter = "";

        public LeaderboardsPanelController(LeaderboardsPanel panel)
        {
            this.panel = panel;
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            panel.OnSubmitEntry += HandleSubmitEntry;
            panel.OnRefreshLeaderboard += RefreshLeaderboard;
            panel.OnClearEntries += HandleClearEntries;
            panel.OnViewOtherUserEntries += HandleViewOtherUserEntries;
            panel.OnLeaderboardFilterChanged += HandleLeaderboardFilterChanged;
            panel.OnLimitChanged += HandleLimitChanged;
            panel.OnShowUserEntriesToggled += HandleShowUserEntriesToggled;
        }

        public void Initialize(GameFuseUser user)
        {
            currentUser = user;
            // Start with user entries view which doesn't require a filter
            panel.SetShowUserEntries(true);
            RefreshLeaderboard();
        }

        private async void HandleSubmitEntry(string leaderboardName, double score, string metadataJson)
        {
            if (currentUser == null) return;

            try
            {
                Dictionary<string, object> metadata = null;
                if (!string.IsNullOrWhiteSpace(metadataJson))
                {
                    try
                    {
                        metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(metadataJson);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Invalid metadata JSON: {ex.Message}");
                        panel.ShowMessage("Invalid metadata JSON format");
                        return;
                    }
                }

                Debug.Log($"Submitting leaderboard entry - Name: '{leaderboardName}', Score: {score}, Metadata: {metadataJson}");

                var updatedUser = await currentUser.SubmitLeaderboardEntryAsync(
                    leaderboardName: leaderboardName,
                    score: score,
                    metadata: metadata
                );

                if (updatedUser != null)
                {
                    Debug.Log($"Leaderboard entry submitted successfully! User ID: {updatedUser.Id}, Username: {updatedUser.Username}");
                    panel.ShowMessage($"Leaderboard entry submitted successfully! Score: {score}");
                    
                    // Update the filter to show the leaderboard we just submitted to
                    currentLeaderboardFilter = leaderboardName;
                    panel.SetLeaderboardFilter(leaderboardName);
                    
                    // Switch to user entries to see the submission
                    panel.SetShowUserEntries(true);
                    
                    // Wait a moment for the API to process the entry
                    await Task.Delay(500);
                    
                    RefreshLeaderboard();
                }
                else
                {
                    Debug.LogError("SubmitLeaderboardEntryAsync returned null");
                    panel.ShowMessage("Failed to submit leaderboard entry - no response");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error submitting leaderboard entry: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private async void HandleClearEntries(string leaderboardName)
        {
            if (currentUser == null || string.IsNullOrWhiteSpace(leaderboardName)) return;

            try
            {
                var updatedUser = await currentUser.ClearLeaderboardEntriesAsync(leaderboardName);

                if (updatedUser != null)
                {
                    panel.ShowMessage($"Cleared all entries from '{leaderboardName}' leaderboard");
                    RefreshLeaderboard();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error clearing leaderboard entries: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private async void HandleViewOtherUserEntries(int userId)
        {
            if (currentUser == null || userId <= 0) return;

            try
            {
                var limit = panel.GetEntriesLimit();
                var entries = await currentUser.GetUserLeaderboardEntriesAsync(
                    userId: userId,
                    limit: limit,
                    leaderboardName: string.IsNullOrWhiteSpace(currentLeaderboardFilter) ? null : currentLeaderboardFilter
                );

                if (entries != null && entries.LeaderboardEntries != null)
                {
                    panel.ShowOtherUserEntries(userId, entries.LeaderboardEntries);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching other user's leaderboard entries: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private void HandleLeaderboardFilterChanged(string filter)
        {
            currentLeaderboardFilter = filter;
            RefreshLeaderboard();
        }

        private void HandleLimitChanged(int limit)
        {
            RefreshLeaderboard();
        }

        private void HandleShowUserEntriesToggled(bool showUserEntries)
        {
            if (showUserEntries)
            {
                panel.UpdateEntriesList(userLeaderboardEntries, true);
            }
            else
            {
                panel.UpdateEntriesList(gameLeaderboardEntries, false);
            }
        }

        private async void RefreshLeaderboard()
        {
            if (currentUser == null) return;

            try
            {
                var limit = panel.GetEntriesLimit();
                var showUserEntries = panel.IsShowingUserEntries();

                if (showUserEntries)
                {
                    Debug.Log($"Fetching user-specific entries. Filter: '{currentLeaderboardFilter}', Limit: {limit}");
                    
                    // Fetch user-specific entries
                    var userEntries = await currentUser.GetCurrentUserLeaderboardEntriesAsync(
                        limit: limit,
                        leaderboardName: string.IsNullOrWhiteSpace(currentLeaderboardFilter) ? null : currentLeaderboardFilter,
                        onePerUser: false
                    );

                    if (userEntries != null)
                    {
                        if (userEntries.LeaderboardEntries != null && userEntries.LeaderboardEntries.Count > 0)
                        {
                            userLeaderboardEntries = userEntries.LeaderboardEntries;
                            Debug.Log($"Successfully fetched {userLeaderboardEntries.Count} user leaderboard entries");
                            
                            // Log details of each entry for debugging
                            foreach (var entry in userLeaderboardEntries)
                            {
                                Debug.Log($"  Entry: User={entry.Username}, Score={entry.Score}, Leaderboard={entry.LeaderboardName}, Created={entry.CreatedAt}");
                            }
                            
                            panel.UpdateEntriesList(userLeaderboardEntries, true);
                        }
                        else
                        {
                            Debug.Log("API returned empty leaderboard entries list");
                            userLeaderboardEntries = new List<LeaderboardEntryModel>();
                            panel.UpdateEntriesList(userLeaderboardEntries, true);
                            panel.ShowMessage("No entries found for your user");
                        }
                    }
                    else
                    {
                        Debug.LogError("GetCurrentUserLeaderboardEntriesAsync returned null response");
                        panel.UpdateEntriesList(new List<LeaderboardEntryModel>(), true);
                        panel.ShowMessage("Failed to fetch leaderboard entries");
                    }
                }
                else
                {
                    // Fetch game-wide entries
                    if (!string.IsNullOrWhiteSpace(currentLeaderboardFilter))
                    {
                        var gameSettings = GameFuseSettings.Settings;
                        if (gameSettings != null && !string.IsNullOrEmpty(gameSettings.GameId))
                        {
                            if (int.TryParse(gameSettings.GameId, out int gameId))
                            {
                                var gameEntries = await currentUser.GetLeaderboardEntriesAsync(
                                    gameId: gameId,
                                    leaderboardName: currentLeaderboardFilter,
                                    limit: limit
                                );

                                if (gameEntries != null && gameEntries.LeaderboardEntries != null)
                                {
                                    gameLeaderboardEntries = gameEntries.LeaderboardEntries;
                                    Debug.Log($"Fetched {gameLeaderboardEntries.Count} game-wide entries for leaderboard: '{currentLeaderboardFilter}'");
                                    panel.UpdateEntriesList(gameLeaderboardEntries, false);
                                }
                                else
                                {
                                    Debug.Log($"No game-wide entries found for leaderboard: '{currentLeaderboardFilter}'");
                                    panel.UpdateEntriesList(new List<LeaderboardEntryModel>(), false);
                                }
                            }
                            else
                            {
                                panel.ShowMessage("Game ID must be a valid number");
                            }
                        }
                        else
                        {
                            panel.ShowMessage("Game ID not configured in GameFuseSettings");
                        }
                    }
                    else
                    {
                        panel.ShowMessage("Please enter a leaderboard name to view game-wide entries");
                        panel.UpdateEntriesList(new List<LeaderboardEntryModel>(), false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching leaderboard entries: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            panel.OnSubmitEntry -= HandleSubmitEntry;
            panel.OnRefreshLeaderboard -= RefreshLeaderboard;
            panel.OnClearEntries -= HandleClearEntries;
            panel.OnViewOtherUserEntries -= HandleViewOtherUserEntries;
            panel.OnLeaderboardFilterChanged -= HandleLeaderboardFilterChanged;
            panel.OnLimitChanged -= HandleLimitChanged;
            panel.OnShowUserEntriesToggled -= HandleShowUserEntriesToggled;
            currentUser = null;
        }
    }
}