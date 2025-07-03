using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace GameFuse.UI
{
    [UxmlElement]
    public partial class GameRoundsPanel : VisualElement
    {
        public event Action<string> OnCreateSinglePlayerRound;
        public event Action<string, int?> OnCreateMultiplayerRound;
        public event Action<int> OnViewRoundDetails;
        public event Action OnRefreshRounds;
        public event Action<int> OnPageChanged;
        public event Action<string> OnGameTypeFilterChanged;

        private VisualElement contentArea;
        private VisualElement createRoundForm;
        private TextField gameTypeField;
        private IntegerField scoreField;
        private IntegerField placeField;
        private TextField metadataField;
        private Toggle multiplayerToggle;
        private IntegerField multiplayerIdField;
        private Button createRoundButton;
        private Button cancelButton;
        
        private VisualElement roundsList;
        private ScrollView roundsScrollView;
        
        // Filter controls
        private TextField gameTypeFilter;
        private Button filterButton;
        private Button clearFilterButton;
        
        // Pagination
        private Label pageLabel;
        private Button prevPageButton;
        private Button nextPageButton;
        
        private Button refreshButton;
        private Button showCreateFormButton;
        
        private VisualElement detailsPanel;
        private Label detailsContent;
        private Button closeDetailsButton;
        
        private int currentPage = 1;
        private int totalPages = 1;

        public GameRoundsPanel()
        {
            // Load the UXML
            var visualTree = Resources.Load<VisualTreeAsset>("GameRoundsPanel");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
                SetupElements();
            }
            else
            {
                Debug.LogError("Failed to load GameRoundsPanel.uxml");
            }
        }

        private void SetupElements()
        {
            contentArea = this.Q<VisualElement>("content-area");
            
            // Create form elements
            createRoundForm = this.Q<VisualElement>("create-round-form");
            gameTypeField = this.Q<TextField>("game-type-field");
            scoreField = this.Q<IntegerField>("score-field");
            placeField = this.Q<IntegerField>("place-field");
            metadataField = this.Q<TextField>("metadata-field");
            multiplayerToggle = this.Q<Toggle>("multiplayer-toggle");
            multiplayerIdField = this.Q<IntegerField>("multiplayer-id-field");
            createRoundButton = this.Q<Button>("create-round-button");
            cancelButton = this.Q<Button>("cancel-button");
            
            // List elements
            roundsList = this.Q<VisualElement>("rounds-list");
            roundsScrollView = this.Q<ScrollView>("rounds-scroll-view");
            
            // Filter elements
            gameTypeFilter = this.Q<TextField>("game-type-filter");
            filterButton = this.Q<Button>("filter-button");
            clearFilterButton = this.Q<Button>("clear-filter-button");
            
            // Pagination elements
            pageLabel = this.Q<Label>("page-label");
            prevPageButton = this.Q<Button>("prev-page-button");
            nextPageButton = this.Q<Button>("next-page-button");
            
            refreshButton = this.Q<Button>("refresh-button");
            showCreateFormButton = this.Q<Button>("show-create-form-button");
            
            // Details panel
            detailsPanel = this.Q<VisualElement>("details-panel");
            detailsContent = this.Q<Label>("details-content");
            closeDetailsButton = this.Q<Button>("close-details-button");
            
            // Setup event handlers
            SetupEventHandlers();
            
            // Initially hide forms
            createRoundForm.style.display = DisplayStyle.None;
            detailsPanel.style.display = DisplayStyle.None;
            multiplayerIdField.style.display = DisplayStyle.None;
        }

        private void SetupEventHandlers()
        {
            showCreateFormButton?.RegisterCallback<ClickEvent>(evt => ShowCreateForm());
            cancelButton?.RegisterCallback<ClickEvent>(evt => HideCreateForm());
            createRoundButton?.RegisterCallback<ClickEvent>(evt => HandleCreateRound());
            
            multiplayerToggle?.RegisterCallback<ChangeEvent<bool>>(evt => 
            {
                multiplayerIdField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            
            filterButton?.RegisterCallback<ClickEvent>(evt => 
            {
                OnGameTypeFilterChanged?.Invoke(gameTypeFilter.value);
            });
            
            clearFilterButton?.RegisterCallback<ClickEvent>(evt => 
            {
                gameTypeFilter.value = "";
                OnGameTypeFilterChanged?.Invoke("");
            });
            
            refreshButton?.RegisterCallback<ClickEvent>(evt => OnRefreshRounds?.Invoke());
            
            prevPageButton?.RegisterCallback<ClickEvent>(evt => 
            {
                if (currentPage > 1)
                {
                    OnPageChanged?.Invoke(currentPage - 1);
                }
            });
            
            nextPageButton?.RegisterCallback<ClickEvent>(evt => 
            {
                if (currentPage < totalPages)
                {
                    OnPageChanged?.Invoke(currentPage + 1);
                }
            });
            
            closeDetailsButton?.RegisterCallback<ClickEvent>(evt => HideDetails());
        }

        private void ShowCreateForm()
        {
            createRoundForm.style.display = DisplayStyle.Flex;
            roundsList.style.display = DisplayStyle.None;
            showCreateFormButton.style.display = DisplayStyle.None;
        }

        private void HideCreateForm()
        {
            createRoundForm.style.display = DisplayStyle.None;
            roundsList.style.display = DisplayStyle.Flex;
            showCreateFormButton.style.display = DisplayStyle.Flex;
            
            // Clear form
            gameTypeField.value = "";
            scoreField.value = 0;
            placeField.value = 1;
            metadataField.value = "";
            multiplayerToggle.value = false;
            multiplayerIdField.value = 0;
        }

        private void HandleCreateRound()
        {
            var gameType = gameTypeField.value;
            if (string.IsNullOrWhiteSpace(gameType))
            {
                Debug.LogError("Game type is required");
                return;
            }
            
            if (multiplayerToggle.value)
            {
                int? mpId = multiplayerIdField.value > 0 ? multiplayerIdField.value : null;
                OnCreateMultiplayerRound?.Invoke(gameType, mpId);
            }
            else
            {
                OnCreateSinglePlayerRound?.Invoke(gameType);
            }
        }

        public void UpdateRoundsList(GameFuse.Models.Shared.GameRound[] rounds)
        {
            roundsScrollView.Clear();
            
            foreach (var round in rounds)
            {
                var roundItem = new VisualElement();
                roundItem.AddToClassList("round-item");
                
                var typeLabel = new Label($"{round.GameType} - {(round.RoundType == GameFuse.Models.Shared.GameRoundType.Multiplayer ? "Multiplayer" : "Single Player")}");
                typeLabel.AddToClassList("round-type");
                roundItem.Add(typeLabel);
                
                var scoreLabel = new Label($"Score: {round.Score} | Place: {round.Place}");
                scoreLabel.AddToClassList("round-score");
                roundItem.Add(scoreLabel);
                
                var dateLabel = new Label($"Created: {round.CreatedAt}");
                dateLabel.AddToClassList("round-date");
                roundItem.Add(dateLabel);
                
                var viewButton = new Button(() => OnViewRoundDetails?.Invoke(round.Id))
                {
                    text = "View Details"
                };
                viewButton.AddToClassList("view-button");
                roundItem.Add(viewButton);
                
                roundsScrollView.Add(roundItem);
            }
            
            if (rounds.Length == 0)
            {
                var emptyLabel = new Label("No game rounds found");
                emptyLabel.AddToClassList("empty-label");
                roundsScrollView.Add(emptyLabel);
            }
        }

        public void UpdatePagination(int page, int total)
        {
            currentPage = page;
            totalPages = total;
            pageLabel.text = $"Page {page} of {total}";
            prevPageButton.SetEnabled(page > 1);
            nextPageButton.SetEnabled(page < total);
        }

        public void ShowRoundDetails(GameFuse.Models.Shared.GameRound round)
        {
            detailsPanel.style.display = DisplayStyle.Flex;
            
            var details = $"<b>Game Round #{round.Id}</b>\n\n";
            details += $"<b>Type:</b> {round.GameType}\n";
            details += $"<b>Mode:</b> {(round.RoundType == GameFuse.Models.Shared.GameRoundType.Multiplayer ? "Multiplayer" : "Single Player")}\n";
            details += $"<b>Score:</b> {round.Score}\n";
            details += $"<b>Place:</b> {round.Place}\n";
            details += $"<b>Start Time:</b> {round.StartTime ?? "N/A"}\n";
            details += $"<b>End Time:</b> {round.EndTime ?? "N/A"}\n";
            details += $"<b>Created:</b> {round.CreatedAt}\n\n";
            
            if (round.RoundType == GameFuse.Models.Shared.GameRoundType.Multiplayer && round.Rankings != null && round.Rankings.Count > 0)
            {
                details += "<b>Rankings:</b>\n";
                foreach (var ranking in round.Rankings)
                {
                    details += $"{ranking.Place}. {ranking.User?.Username ?? "Unknown"} - Score: {ranking.Score}\n";
                }
            }
            
            if (round.Metadata != null && round.Metadata.Count > 0)
            {
                details += "\n<b>Metadata:</b>\n";
                foreach (var kvp in round.Metadata)
                {
                    details += $"{kvp.Key}: {kvp.Value}\n";
                }
            }
            
            detailsContent.text = details;
        }

        private void HideDetails()
        {
            detailsPanel.style.display = DisplayStyle.None;
        }

        public void ShowMessage(string message)
        {
            Debug.Log($"GameRounds: {message}");
        }

        public int GetScore() => scoreField.value;
        public int GetPlace() => placeField.value;
        public string GetMetadata() => metadataField.value;
    }
}