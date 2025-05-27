using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuse.Models.Shared;

namespace GameFuse.UI
{
    [UxmlElement]
    public partial class LeaderboardsPanel : VisualElement
    {
        public event Action<string, double, string> OnSubmitEntry;
        public event Action OnRefreshLeaderboard;
        public event Action<string> OnClearEntries;
        public event Action<int> OnViewOtherUserEntries;
        public event Action<string> OnLeaderboardFilterChanged;
        public event Action<int> OnLimitChanged;
        public event Action<bool> OnShowUserEntriesToggled;

        private VisualElement contentArea;
        
        // Submit entry form
        private VisualElement submitEntryForm;
        private TextField leaderboardNameField;
        private FloatField scoreField;
        private TextField metadataField;
        private Button submitButton;
        private Button cancelSubmitButton;
        
        // Filter controls
        private TextField leaderboardFilterField;
        private IntegerField limitField;
        private Toggle showUserEntriesToggle;
        private Button applyFilterButton;
        private Button clearFilterButton;
        
        // Clear entries form
        private VisualElement clearEntriesForm;
        private TextField clearLeaderboardNameField;
        private Button clearEntriesButton;
        private Button cancelClearButton;
        
        // View other user form
        private IntegerField otherUserIdField;
        private Button viewOtherUserButton;
        
        // Entries list
        private ScrollView entriesScrollView;
        private VisualElement entriesList;
        
        private Button refreshButton;
        private Button showSubmitFormButton;
        private Button showClearFormButton;
        
        // Other user entries popup
        private VisualElement otherUserPopup;
        private Label otherUserTitle;
        private ScrollView otherUserScrollView;
        private VisualElement otherUserEntriesList;
        private Button closeOtherUserButton;
        
        // Message display
        private Label messageLabel;
        private VisualElement messageContainer;

        public LeaderboardsPanel()
        {
            // Load the UXML
            var visualTree = Resources.Load<VisualTreeAsset>("LeaderboardsPanel");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
                SetupElements();
            }
            else
            {
                Debug.LogError("Failed to load LeaderboardsPanel.uxml");
            }
        }

        private void SetupElements()
        {
            contentArea = this.Q<VisualElement>("content-area");
            
            // Submit entry form
            submitEntryForm = this.Q<VisualElement>("submit-entry-form");
            leaderboardNameField = this.Q<TextField>("leaderboard-name-field");
            scoreField = this.Q<FloatField>("score-field");
            metadataField = this.Q<TextField>("metadata-field");
            submitButton = this.Q<Button>("submit-button");
            cancelSubmitButton = this.Q<Button>("cancel-submit-button");
            
            // Filter controls
            leaderboardFilterField = this.Q<TextField>("leaderboard-filter-field");
            limitField = this.Q<IntegerField>("limit-field");
            showUserEntriesToggle = this.Q<Toggle>("show-user-entries-toggle");
            applyFilterButton = this.Q<Button>("apply-filter-button");
            clearFilterButton = this.Q<Button>("clear-filter-button");
            
            // Clear entries form
            clearEntriesForm = this.Q<VisualElement>("clear-entries-form");
            clearLeaderboardNameField = this.Q<TextField>("clear-leaderboard-name-field");
            clearEntriesButton = this.Q<Button>("clear-entries-button");
            cancelClearButton = this.Q<Button>("cancel-clear-button");
            
            // View other user
            otherUserIdField = this.Q<IntegerField>("other-user-id-field");
            viewOtherUserButton = this.Q<Button>("view-other-user-button");
            
            // Entries list
            entriesScrollView = this.Q<ScrollView>("entries-scroll-view");
            entriesList = this.Q<VisualElement>("entries-list");
            
            refreshButton = this.Q<Button>("refresh-button");
            showSubmitFormButton = this.Q<Button>("show-submit-form-button");
            showClearFormButton = this.Q<Button>("show-clear-form-button");
            
            // Other user popup
            otherUserPopup = this.Q<VisualElement>("other-user-popup");
            otherUserTitle = this.Q<Label>("other-user-title");
            otherUserScrollView = this.Q<ScrollView>("other-user-scroll-view");
            otherUserEntriesList = this.Q<VisualElement>("other-user-entries-list");
            closeOtherUserButton = this.Q<Button>("close-other-user-button");
            
            SetupEventHandlers();
            
            // Set default values
            limitField.value = 10;
            submitEntryForm.style.display = DisplayStyle.None;
            clearEntriesForm.style.display = DisplayStyle.None;
            otherUserPopup.style.display = DisplayStyle.None;
        }

        private void SetupEventHandlers()
        {
            showSubmitFormButton?.RegisterCallback<ClickEvent>(evt => ShowSubmitForm());
            cancelSubmitButton?.RegisterCallback<ClickEvent>(evt => HideSubmitForm());
            submitButton?.RegisterCallback<ClickEvent>(evt => SubmitEntry());
            
            showClearFormButton?.RegisterCallback<ClickEvent>(evt => ShowClearForm());
            cancelClearButton?.RegisterCallback<ClickEvent>(evt => HideClearForm());
            clearEntriesButton?.RegisterCallback<ClickEvent>(evt => ClearEntries());
            
            viewOtherUserButton?.RegisterCallback<ClickEvent>(evt => ViewOtherUserEntries());
            closeOtherUserButton?.RegisterCallback<ClickEvent>(evt => HideOtherUserPopup());
            
            applyFilterButton?.RegisterCallback<ClickEvent>(evt => ApplyFilter());
            clearFilterButton?.RegisterCallback<ClickEvent>(evt => ClearFilter());
            
            showUserEntriesToggle?.RegisterValueChangedCallback(evt => OnShowUserEntriesToggled?.Invoke(evt.newValue));
            refreshButton?.RegisterCallback<ClickEvent>(evt => OnRefreshLeaderboard?.Invoke());
        }

        private void ShowSubmitForm()
        {
            submitEntryForm.style.display = DisplayStyle.Flex;
            showSubmitFormButton.style.display = DisplayStyle.None;
        }

        private void HideSubmitForm()
        {
            submitEntryForm.style.display = DisplayStyle.None;
            showSubmitFormButton.style.display = DisplayStyle.Flex;
            leaderboardNameField.value = "";
            scoreField.value = 0;
            metadataField.value = "";
        }

        private void SubmitEntry()
        {
            var leaderboardName = leaderboardNameField.value;
            var score = scoreField.value;
            var metadata = metadataField.value;
            
            if (string.IsNullOrWhiteSpace(leaderboardName))
            {
                ShowMessage("Please enter a leaderboard name");
                return;
            }
            
            OnSubmitEntry?.Invoke(leaderboardName, score, metadata);
            HideSubmitForm();
        }

        private void ShowClearForm()
        {
            clearEntriesForm.style.display = DisplayStyle.Flex;
            showClearFormButton.style.display = DisplayStyle.None;
        }

        private void HideClearForm()
        {
            clearEntriesForm.style.display = DisplayStyle.None;
            showClearFormButton.style.display = DisplayStyle.Flex;
            clearLeaderboardNameField.value = "";
        }

        private void ClearEntries()
        {
            var leaderboardName = clearLeaderboardNameField.value;
            
            if (string.IsNullOrWhiteSpace(leaderboardName))
            {
                ShowMessage("Please enter a leaderboard name to clear");
                return;
            }
            
            OnClearEntries?.Invoke(leaderboardName);
            HideClearForm();
        }

        private void ViewOtherUserEntries()
        {
            var userId = otherUserIdField.value;
            
            if (userId <= 0)
            {
                ShowMessage("Please enter a valid user ID");
                return;
            }
            
            OnViewOtherUserEntries?.Invoke(userId);
        }

        private void ApplyFilter()
        {
            var filter = leaderboardFilterField.value;
            var limit = limitField.value;
            
            if (limit <= 0)
            {
                limitField.value = 10;
                limit = 10;
            }
            
            OnLeaderboardFilterChanged?.Invoke(filter);
            OnLimitChanged?.Invoke(limit);
        }

        private void ClearFilter()
        {
            leaderboardFilterField.value = "";
            limitField.value = 10;
            showUserEntriesToggle.value = false;
            
            OnLeaderboardFilterChanged?.Invoke("");
            OnLimitChanged?.Invoke(10);
        }

        private void HideOtherUserPopup()
        {
            otherUserPopup.style.display = DisplayStyle.None;
        }

        public void UpdateEntriesList(List<LeaderboardEntryModel> entries, bool isUserSpecific)
        {
            entriesList.Clear();
            
            if (entries == null || entries.Count == 0)
            {
                var emptyLabel = new Label(isUserSpecific ? "No user entries found" : "No leaderboard entries found");
                emptyLabel.AddToClassList("empty-message");
                entriesList.Add(emptyLabel);
                return;
            }
            
            // Sort entries by score descending
            var sortedEntries = new List<LeaderboardEntryModel>(entries);
            sortedEntries.Sort((a, b) => b.Score.CompareTo(a.Score));
            
            int rank = 1;
            foreach (var entry in sortedEntries)
            {
                var entryElement = CreateLeaderboardEntryElement(entry, rank++, isUserSpecific);
                entriesList.Add(entryElement);
            }
        }

        private VisualElement CreateLeaderboardEntryElement(LeaderboardEntryModel entry, int rank, bool isUserSpecific)
        {
            var container = new VisualElement();
            container.AddToClassList("leaderboard-entry");
            
            // Add rank-based styling
            if (rank == 1)
                container.AddToClassList("gold-rank");
            else if (rank == 2)
                container.AddToClassList("silver-rank");
            else if (rank == 3)
                container.AddToClassList("bronze-rank");
            
            var rankLabel = new Label($"#{rank}");
            rankLabel.AddToClassList("rank-label");
            container.Add(rankLabel);
            
            var detailsContainer = new VisualElement();
            detailsContainer.AddToClassList("entry-details");
            
            var userLabel = new Label($"{entry.Username} (ID: {entry.GameUserId})");
            userLabel.AddToClassList("entry-user");
            detailsContainer.Add(userLabel);
            
            var scoreLabel = new Label($"Score: {entry.Score:F2}");
            scoreLabel.AddToClassList("entry-score");
            detailsContainer.Add(scoreLabel);
            
            if (isUserSpecific && !string.IsNullOrEmpty(entry.LeaderboardName))
            {
                var leaderboardLabel = new Label($"Leaderboard: {entry.LeaderboardName}");
                leaderboardLabel.AddToClassList("entry-leaderboard");
                detailsContainer.Add(leaderboardLabel);
            }
            
            if (entry.Metadata != null)
            {
                var metadataLabel = new Label($"Metadata: {Newtonsoft.Json.JsonConvert.SerializeObject(entry.Metadata)}");
                metadataLabel.AddToClassList("entry-metadata");
                detailsContainer.Add(metadataLabel);
            }
            
            var dateLabel = new Label($"Submitted: {entry.CreatedAt}");
            dateLabel.AddToClassList("entry-date");
            detailsContainer.Add(dateLabel);
            
            container.Add(detailsContainer);
            
            return container;
        }

        public void ShowOtherUserEntries(int userId, List<LeaderboardEntryModel> entries)
        {
            otherUserTitle.text = $"Leaderboard Entries for User {userId}";
            otherUserEntriesList.Clear();
            
            if (entries == null || entries.Count == 0)
            {
                var emptyLabel = new Label("No entries found for this user");
                emptyLabel.AddToClassList("empty-message");
                otherUserEntriesList.Add(emptyLabel);
            }
            else
            {
                // Sort entries by score descending
                var sortedEntries = new List<LeaderboardEntryModel>(entries);
                sortedEntries.Sort((a, b) => b.Score.CompareTo(a.Score));
                
                int rank = 1;
                foreach (var entry in sortedEntries)
                {
                    var entryElement = CreateLeaderboardEntryElement(entry, rank++, true);
                    otherUserEntriesList.Add(entryElement);
                }
            }
            
            otherUserPopup.style.display = DisplayStyle.Flex;
        }

        public void ShowMessage(string message)
        {
            Debug.Log($"Leaderboards: {message}");
            
            // Create a temporary message if we don't have a dedicated message area
            if (messageLabel == null)
            {
                var tempMessage = new Label(message);
                tempMessage.AddToClassList("temporary-message");
                tempMessage.style.position = Position.Absolute;
                tempMessage.style.top = 10;
                tempMessage.style.left = Length.Percent(50);
                tempMessage.style.translate = new Translate(Length.Percent(-50), 0);
                tempMessage.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
                tempMessage.style.color = Color.white;
                tempMessage.style.paddingTop = 10;
                tempMessage.style.paddingBottom = 10;
                tempMessage.style.paddingLeft = 20;
                tempMessage.style.paddingRight = 20;
                tempMessage.style.borderTopLeftRadius = 5;
                tempMessage.style.borderTopRightRadius = 5;
                tempMessage.style.borderBottomLeftRadius = 5;
                tempMessage.style.borderBottomRightRadius = 5;
                
                contentArea.Add(tempMessage);
                
                // Remove after 3 seconds
                var task = Task.Delay(3000).ContinueWith(_ => 
                {
                    if (tempMessage.parent != null)
                    {
                        tempMessage.RemoveFromHierarchy();
                    }
                });
            }
            else
            {
                messageLabel.text = message;
                messageContainer.style.display = DisplayStyle.Flex;
                
                // Hide after 3 seconds
                var task = Task.Delay(3000).ContinueWith(_ => 
                {
                    if (messageContainer != null)
                    {
                        messageContainer.style.display = DisplayStyle.None;
                    }
                });
            }
        }

        public int GetEntriesLimit()
        {
            return limitField?.value ?? 10;
        }

        public bool IsShowingUserEntries()
        {
            return showUserEntriesToggle?.value ?? false;
        }

        public void SetLeaderboardFilter(string leaderboardName)
        {
            if (leaderboardFilterField != null)
            {
                leaderboardFilterField.value = leaderboardName;
            }
        }

        public void SetShowUserEntries(bool show)
        {
            if (showUserEntriesToggle != null)
            {
                showUserEntriesToggle.value = show;
            }
        }
    }
}