using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameFuse;
using GameFuse.Models.Shared;

namespace GameFuse.UI
{
    public class GameRoundsPanelController
    {
        private GameRoundsPanel panel;
        private GameFuseUser currentUser;
        private List<GameRound> allRounds = new List<GameRound>();
        private List<GameRound> filteredRounds = new List<GameRound>();
        private int currentPage = 1;
        private int itemsPerPage = 10;
        private string currentFilter = "";

        public GameRoundsPanelController(GameRoundsPanel panel)
        {
            this.panel = panel;
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            panel.OnCreateSinglePlayerRound += HandleCreateSinglePlayerRound;
            panel.OnCreateMultiplayerRound += HandleCreateMultiplayerRound;
            panel.OnViewRoundDetails += HandleViewRoundDetails;
            panel.OnRefreshRounds += RefreshRounds;
            panel.OnPageChanged += HandlePageChanged;
            panel.OnGameTypeFilterChanged += HandleFilterChanged;
        }

        public void Initialize(GameFuseUser user)
        {
            currentUser = user;
            RefreshRounds();
        }

        private async void HandleCreateSinglePlayerRound(string gameType)
        {
            if (currentUser == null) return;

            try
            {
                var score = panel.GetScore();
                var place = panel.GetPlace();
                var metadataJson = panel.GetMetadata();
                
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

                var startTime = DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                var round = await currentUser.CreateGameRoundAsync(
                    gameType: gameType,
                    startTime: startTime,
                    endTime: endTime,
                    score: score,
                    place: place,
                    metadata: metadata
                );

                if (round != null)
                {
                    panel.ShowMessage("Single player round created successfully!");
                    RefreshRounds();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating single player round: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private async void HandleCreateMultiplayerRound(string gameType, int? multiplayerRoundId)
        {
            if (currentUser == null) return;

            try
            {
                var score = panel.GetScore();
                var place = panel.GetPlace();
                var metadataJson = panel.GetMetadata();
                
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

                var startTime = DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                var round = await currentUser.CreateMultiplayerGameRoundAsync(
                    gameType: gameType,
                    startTime: startTime,
                    endTime: endTime,
                    score: score,
                    place: place,
                    metadata: metadata,
                    multiplayerGameRoundId: multiplayerRoundId
                );

                if (round != null)
                {
                    panel.ShowMessage(multiplayerRoundId.HasValue ? 
                        "Joined multiplayer round successfully!" : 
                        "New multiplayer round created successfully!");
                    RefreshRounds();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating multiplayer round: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private async void HandleViewRoundDetails(int roundId)
        {
            if (currentUser == null) return;

            try
            {
                // Get full round details with rankings
                var round = await currentUser.GetGameRoundAsync(roundId);
                if (round != null)
                {
                    panel.ShowRoundDetails(round);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching round details: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private async void RefreshRounds()
        {
            if (currentUser == null) return;

            try
            {
                // Fetch all pages to get complete list
                var rounds = new List<GameRound>();
                int page = 1;
                bool hasMore = true;
                
                while (hasMore)
                {
                    var pageRounds = await currentUser.GetCurrentUserGameRoundsAsync(page: page, perPage: 100);
                    if (pageRounds != null && pageRounds.Count > 0)
                    {
                        rounds.AddRange(pageRounds);
                        hasMore = pageRounds.Count == 100;
                        page++;
                    }
                    else
                    {
                        hasMore = false;
                    }
                }

                allRounds = rounds;
                ApplyFilter(currentFilter);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching game rounds: {ex.Message}");
                panel.ShowMessage($"Error: {ex.Message}");
            }
        }

        private void HandleFilterChanged(string filter)
        {
            currentFilter = filter;
            currentPage = 1;
            ApplyFilter(filter);
        }

        private void ApplyFilter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                filteredRounds = new List<GameRound>(allRounds);
            }
            else
            {
                var lowerFilter = filter.ToLower();
                filteredRounds = allRounds.Where(r => 
                    r.GameType.ToLower().Contains(lowerFilter)
                ).ToList();
            }

            UpdateDisplay();
        }

        private void HandlePageChanged(int newPage)
        {
            currentPage = newPage;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)filteredRounds.Count / itemsPerPage));
            currentPage = Math.Min(currentPage, totalPages);

            var startIndex = (currentPage - 1) * itemsPerPage;
            var pageRounds = filteredRounds.Skip(startIndex).Take(itemsPerPage).ToArray();

            panel.UpdateRoundsList(pageRounds);
            panel.UpdatePagination(currentPage, totalPages);
        }

        public void Cleanup()
        {
            panel.OnCreateSinglePlayerRound -= HandleCreateSinglePlayerRound;
            panel.OnCreateMultiplayerRound -= HandleCreateMultiplayerRound;
            panel.OnViewRoundDetails -= HandleViewRoundDetails;
            panel.OnRefreshRounds -= RefreshRounds;
            panel.OnPageChanged -= HandlePageChanged;
            panel.OnGameTypeFilterChanged -= HandleFilterChanged;
            currentUser = null;
        }
    }
}