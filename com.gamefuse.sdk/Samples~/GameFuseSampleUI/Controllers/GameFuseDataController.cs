using System;
using System.Collections.Generic;
using GameFuse;
using GameFuse.Exceptions;
using GameFuse.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.Samples
{
    /// <summary>
    /// Controller for the GameFuse data UI.
    /// Handles displaying user info, store items, friends, and leaderboard data.
    /// </summary>
    public class GameFuseDataController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private GameFuseAuthController _authController;
        
        private VisualElement _dataPanel;
        
        private Label _usernameValue;
        private Label _emailValue;
        private Label _creditsValue;
        private Label _scoreValue;
        
        private ListView _storeItemsList;
        private ListView _friendsList;
        private ListView _leaderboardList;
        
        private Button _refreshUserButton;
        private Button _refreshStoreButton;
        private Button _refreshFriendsButton;
        private Button _refreshLeaderboardButton;
        private Button _signOutButton;
        
        private List<StoreItem> _storeItems = new List<StoreItem>();
        private List<Friend> _friends = new List<Friend>();
        private List<LeaderboardEntry> _leaderboardEntries = new List<LeaderboardEntry>();

        private void OnEnable()
        {
            BindUI();
            SetupEventHandlers();
        }

        private void BindUI()
        {
            var root = _document.rootVisualElement;
            
            _dataPanel = root.Q<VisualElement>("data-panel");
            
            _usernameValue = root.Q<Label>("username-value");
            _emailValue = root.Q<Label>("email-value");
            _creditsValue = root.Q<Label>("credits-value");
            _scoreValue = root.Q<Label>("score-value");
            
            _storeItemsList = root.Q<ListView>("store-items-list");
            _friendsList = root.Q<ListView>("friends-list");
            _leaderboardList = root.Q<ListView>("leaderboard-list");
            
            _refreshUserButton = root.Q<Button>("refresh-user-button");
            _refreshStoreButton = root.Q<Button>("refresh-store-button");
            _refreshFriendsButton = root.Q<Button>("refresh-friends-button");
            _refreshLeaderboardButton = root.Q<Button>("refresh-leaderboard-button");
            _signOutButton = root.Q<Button>("sign-out-button");
            
            // Setup list views
            SetupStoreItemsList();
            SetupFriendsList();
            SetupLeaderboardList();
        }

        private void SetupStoreItemsList()
        {
            _storeItemsList.makeItem = () => new VisualElement
            {
                name = "store-item",
                classList = { "list-item" }
            };
            
            _storeItemsList.bindItem = (element, index) =>
            {
                if (index < 0 || index >= _storeItems.Count)
                    return;
                
                var item = _storeItems[index];
                
                if (element.childCount == 0)
                {
                    var nameLabel = new Label { name = "item-name", classList = { "list-item-main" } };
                    var detailsLabel = new Label { name = "item-details", classList = { "list-item-secondary" } };
                    
                    element.Add(nameLabel);
                    element.Add(detailsLabel);
                }
                
                var name = element.Q<Label>("item-name");
                var details = element.Q<Label>("item-details");
                
                name.text = item.Name;
                details.text = $"Cost: {item.Cost} credits | Category: {item.Category}";
            };
            
            _storeItemsList.itemsSource = _storeItems;
        }

        private void SetupFriendsList()
        {
            _friendsList.makeItem = () => new VisualElement
            {
                name = "friend-item",
                classList = { "list-item" }
            };
            
            _friendsList.bindItem = (element, index) =>
            {
                if (index < 0 || index >= _friends.Count)
                    return;
                
                var friend = _friends[index];
                
                if (element.childCount == 0)
                {
                    var nameLabel = new Label { name = "friend-name", classList = { "list-item-main" } };
                    var detailsLabel = new Label { name = "friend-details", classList = { "list-item-secondary" } };
                    
                    element.Add(nameLabel);
                    element.Add(detailsLabel);
                }
                
                var name = element.Q<Label>("friend-name");
                var details = element.Q<Label>("friend-details");
                
                name.text = friend.Username;
                details.text = $"Score: {friend.Score} | Credits: {friend.Credits}";
            };
            
            _friendsList.itemsSource = _friends;
        }

        private void SetupLeaderboardList()
        {
            _leaderboardList.makeItem = () => new VisualElement
            {
                name = "leaderboard-item",
                classList = { "list-item" }
            };
            
            _leaderboardList.bindItem = (element, index) =>
            {
                if (index < 0 || index >= _leaderboardEntries.Count)
                    return;
                
                var entry = _leaderboardEntries[index];
                
                if (element.childCount == 0)
                {
                    var rankLabel = new Label { name = "rank-name", classList = { "list-item-main" } };
                    var detailsLabel = new Label { name = "rank-details", classList = { "list-item-secondary" } };
                    
                    element.Add(rankLabel);
                    element.Add(detailsLabel);
                }
                
                var rank = element.Q<Label>("rank-name");
                var details = element.Q<Label>("rank-details");
                
                rank.text = $"#{entry.Rank} - {entry.Username}";
                details.text = $"Score: {entry.Score}";
            };
            
            _leaderboardList.itemsSource = _leaderboardEntries;
        }

        private void SetupEventHandlers()
        {
            _refreshUserButton.clicked += OnRefreshUserClicked;
            _refreshStoreButton.clicked += OnRefreshStoreClicked;
            _refreshFriendsButton.clicked += OnRefreshFriendsClicked;
            _refreshLeaderboardButton.clicked += OnRefreshLeaderboardClicked;
            _signOutButton.clicked += OnSignOutClicked;
        }

        public void ShowDataPanel()
        {
            _dataPanel.style.display = DisplayStyle.Flex;
        }

        public void HideDataPanel()
        {
            _dataPanel.style.display = DisplayStyle.None;
        }

        public async void UpdateUserInfo()
        {
            if (!GameFuseUser.IsAuthenticated())
            {
                Debug.LogWarning("Cannot update user info: User is not authenticated.");
                return;
            }
            
            try
            {
                var user = await GameFuseUser.GetCurrentUserAsync();
                
                _usernameValue.text = user.Username;
                _emailValue.text = user.DisplayEmail;
                _creditsValue.text = user.Credits.ToString();
                _scoreValue.text = user.Score.ToString();
            }
            catch (GameFuseApiException ex)
            {
                Debug.LogError($"Failed to get user info: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }

        private async void OnRefreshUserClicked()
        {
            UpdateUserInfo();
        }

        private async void OnRefreshStoreClicked()
        {
            if (!GameFuseUser.IsAuthenticated())
            {
                Debug.LogWarning("Cannot get store items: User is not authenticated.");
                return;
            }
            
            try
            {
                var items = await GameFuseUser.CurrentUser.GetStoreItemsAsync();
                
                _storeItems.Clear();
                _storeItems.AddRange(items);
                _storeItemsList.Rebuild();
            }
            catch (GameFuseApiException ex)
            {
                Debug.LogError($"Failed to get store items: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }

        private async void OnRefreshFriendsClicked()
        {
            if (!GameFuseUser.IsAuthenticated())
            {
                Debug.LogWarning("Cannot get friends: User is not authenticated.");
                return;
            }
            
            try
            {
                var friends = await GameFuseUser.CurrentUser.GetFriendsAsync();
                
                _friends.Clear();
                _friends.AddRange(friends);
                _friendsList.Rebuild();
            }
            catch (GameFuseApiException ex)
            {
                Debug.LogError($"Failed to get friends: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }

        private async void OnRefreshLeaderboardClicked()
        {
            if (!GameFuseUser.IsAuthenticated())
            {
                Debug.LogWarning("Cannot get leaderboard: User is not authenticated.");
                return;
            }
            
            try
            {
                var entries = await GameFuseUser.CurrentUser.GetLeaderboardAsync(50);
                
                _leaderboardEntries.Clear();
                _leaderboardEntries.AddRange(entries);
                _leaderboardList.Rebuild();
            }
            catch (GameFuseApiException ex)
            {
                Debug.LogError($"Failed to get leaderboard: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }

        private async void OnSignOutClicked()
        {
            await GameFuseUser.SignOutAsync();
            
            HideDataPanel();
            _authController.ShowAuthPanel();
        }
    }
}