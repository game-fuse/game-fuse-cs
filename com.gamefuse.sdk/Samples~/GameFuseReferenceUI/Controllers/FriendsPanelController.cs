using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFuse.UI.Controls;

namespace GameFuse.UI
{
    /// <summary>
    /// Controls the friends panel - manages friend lists, requests, and user search.
    /// Covers Phase 3 functionality: Social Features - Friends
    /// </summary>
    public class FriendsPanelController
    {
        // Events
        public static event Action<string> OnError;
        public static event Action<string> OnSuccess;

        // UI Elements
        private VisualElement root;
        private FriendsPanel friendsPanel;
        private GameFuseUser currentUser;

        // Tab elements
        private Button friendsTabButton;
        private Button requestsTabButton;
        private VisualElement friendsListPanel;
        private VisualElement friendRequestsPanel;

        // Friends List Elements
        private Label friendsCount;
        private VisualElement friendsListContainer;
        private VisualElement friendsEmptyState;

        // Friend Requests Elements
        private TextField friendUsernameInput;
        private Button sendRequestButton;
        private VisualElement incomingRequestsList;
        private VisualElement outgoingRequestsList;
        private VisualElement incomingEmptyState;
        private VisualElement outgoingEmptyState;

        // No User Search Elements

        // Loading and Status
        private VisualElement loadingIndicator;
        private Label statusLabel;

        // Data Containers
        private List<Friend> friends = new List<Friend>();
        private List<FriendRequest> incomingRequests = new List<FriendRequest>();
        private List<FriendRequest> outgoingRequests = new List<FriendRequest>();

        public FriendsPanelController(VisualElement panelContainer)
        {
            root = panelContainer;
            friendsPanel = root as FriendsPanel;
            
            // Handle both custom control and legacy implementation
            if (friendsPanel == null)
            {
                Debug.Log("FriendsPanelController: Using standard VisualElement container");
                // Attempt to find the FriendsPanel as a child element
                friendsPanel = root.Q<FriendsPanel>();
                
                if (friendsPanel == null)
                {
                    Debug.LogWarning("FriendsPanelController: Could not find FriendsPanel custom control. Profile popup functionality will be limited.");
                }
            }
            
            if (friendsPanel != null)
            {
                Debug.Log("FriendsPanelController: Using FriendsPanel custom control");
                friendsPanel.Initialize();
            }
            
            InitializeElements();
            SetupEventHandlers();
        }

        private void InitializeElements()
        {
            // Tab elements
            friendsTabButton = root.Q<Button>("friends-tab-button");
            requestsTabButton = root.Q<Button>("requests-tab-button");
            friendsListPanel = root.Q<VisualElement>("friends-list-panel");
            friendRequestsPanel = root.Q<VisualElement>("friend-requests-panel");

            // Friends List Elements
            friendsCount = root.Q<Label>("friends-count");
            friendsListContainer = root.Q<VisualElement>("friends-list-container");
            friendsEmptyState = root.Q<VisualElement>("friends-empty-state");

            // Friend Requests Elements
            friendUsernameInput = root.Q<TextField>("friend-username-input");
            sendRequestButton = root.Q<Button>("send-request-button");
            incomingRequestsList = root.Q<VisualElement>("incoming-requests-list");
            outgoingRequestsList = root.Q<VisualElement>("outgoing-requests-list");
            incomingEmptyState = root.Q<VisualElement>("incoming-empty-state");
            outgoingEmptyState = root.Q<VisualElement>("outgoing-empty-state");

            // Loading and Status
            loadingIndicator = root.Q<VisualElement>("friends-loading");
            statusLabel = root.Q<Label>("friends-status");
        }

        private void SetupEventHandlers()
        {
            // Tab navigation
            friendsTabButton.clicked += () => SwitchTab("friends");
            requestsTabButton.clicked += () => SwitchTab("requests");
            
            // Friend request functionality
            sendRequestButton.clicked += HandleSendFriendRequest;
            friendUsernameInput.RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    HandleSendFriendRequest();
            });
        }

        private void SwitchTab(string tabName)
        {
            // Reset all tabs
            friendsTabButton.RemoveFromClassList("active-tab");
            requestsTabButton.RemoveFromClassList("active-tab");
            
            friendsListPanel.style.display = DisplayStyle.None;
            friendRequestsPanel.style.display = DisplayStyle.None;
            
            // Set active tab
            switch (tabName)
            {
                case "friends":
                    friendsTabButton.AddToClassList("active-tab");
                    friendsListPanel.style.display = DisplayStyle.Flex;
                    break;
                case "requests":
                    requestsTabButton.AddToClassList("active-tab");
                    friendRequestsPanel.style.display = DisplayStyle.Flex;
                    RefreshFriendRequests();
                    break;
            }
        }

        public void SetCurrentUser(GameFuseUser user)
        {
            currentUser = user;
            if (currentUser != null)
            {
                RefreshFriendsList();
                RefreshFriendRequests();
            }
        }

        #region Friends List

        private async void RefreshFriendsList()
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                var friendsList = await currentUser.GetFriendsListAsync();
                friends = friendsList.ToList();
                DisplayFriendsList();
                ShowStatus("Friends list refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load friends: {ex.Message}", true);
                OnError?.Invoke($"Failed to load friends: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void DisplayFriendsList()
        {
            // Clear existing items except empty state
            var children = friendsListContainer.Children().ToList();
            foreach (var child in children)
            {
                if (child != friendsEmptyState)
                {
                    friendsListContainer.Remove(child);
                }
            }
            
            // Update counter
            friendsCount.text = friends.Count.ToString();
            
            // Show/hide empty state
            if (friends.Count == 0)
            {
                friendsEmptyState.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                friendsEmptyState.style.display = DisplayStyle.None;
            }
            
            // Add friend items
            foreach (var friend in friends)
            {
                var friendItem = CreateFriendItem(friend);
                friendsListContainer.Add(friendItem);
            }
        }

        private VisualElement CreateFriendItem(Friend friend)
        {
            var item = new VisualElement();
            item.AddToClassList("friend-item");
            // Force horizontal layout with inline styles
            item.style.flexDirection = FlexDirection.Row;
            item.style.justifyContent = Justify.SpaceBetween;
            item.style.alignItems = Align.Center;
            
            // Friend info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("friend-info");
            
            var avatar = new VisualElement();
            avatar.AddToClassList("friend-avatar");
            infoSection.Add(avatar);
            
            var details = new VisualElement();
            details.AddToClassList("friend-details");
            
            var detailsRow = new VisualElement(); 
            detailsRow.style.flexDirection = FlexDirection.Row;
            detailsRow.style.alignItems = Align.Center;
            
            var nameLabel = new Label(friend.Username);
            nameLabel.AddToClassList("friend-name");
            detailsRow.Add(nameLabel);
            
            var statsLabel = new Label($"Score: {friend.Score} | Credits: {friend.Credits}");
            statsLabel.AddToClassList("friend-status");
            detailsRow.Add(statsLabel);
            
            details.Add(detailsRow);
            
            infoSection.Add(details);
            item.Add(infoSection);
            
            // Actions section
            var actionsSection = new VisualElement();
            actionsSection.AddToClassList("friend-actions");
            actionsSection.style.flexDirection = FlexDirection.Row; // Force horizontal layout
            
            // Directly style the buttons with inline styles to ensure they get the right colors
            var viewProfileButton = new Button(() => HandleViewProfile(friend));
            viewProfileButton.text = "View Profile";
            viewProfileButton.AddToClassList("friend-action-button");
            viewProfileButton.AddToClassList("primary");
            viewProfileButton.style.backgroundColor = new StyleColor(new Color(0.39f, 0.7f, 1f)); // RGB 100, 180, 255
            viewProfileButton.style.color = Color.white;
            viewProfileButton.style.borderLeftWidth = 0;
            viewProfileButton.style.borderRightWidth = 0;
            viewProfileButton.style.borderTopWidth = 0;
            viewProfileButton.style.borderBottomWidth = 0;
            viewProfileButton.style.marginLeft = 5;
            actionsSection.Add(viewProfileButton);
            
            var unfriendButton = new Button(() => HandleUnfriend(friend));
            unfriendButton.text = "Unfriend";
            unfriendButton.AddToClassList("friend-action-button");
            unfriendButton.AddToClassList("danger");
            unfriendButton.style.backgroundColor = new StyleColor(new Color(0.7f, 0.24f, 0.24f)); // RGB 180, 60, 60
            unfriendButton.style.color = Color.white;
            unfriendButton.style.borderLeftWidth = 0;
            unfriendButton.style.borderRightWidth = 0;
            unfriendButton.style.borderTopWidth = 0;
            unfriendButton.style.borderBottomWidth = 0;
            unfriendButton.style.marginLeft = 5;
            actionsSection.Add(unfriendButton);
            
            item.Add(actionsSection);
            
            return item;
        }

        private async void HandleViewProfile(Friend friend)
        {
            if (currentUser == null || friend == null) return;
            
            SetLoading(true);
            ShowStatus($"Loading profile for {friend.Username}...", false);
            
            try
            {
                // Get the friend details including attributes
                var friendDetailsTask = currentUser.GetUserDetailsAsync(friend.Id);
                var friendsCountTask = GetFriendCountAsync(friend.Id);
                
                // Execute both tasks concurrently
                await Task.WhenAll(friendDetailsTask, friendsCountTask);
                
                var friendDetails = await friendDetailsTask;
                int friendCount = await friendsCountTask;
                
                // Get the attributes of the friend
                var attributes = friendDetails.GameUserAttributes;
                
                // Show the profile popup
                if (friendsPanel != null)
                {
                    friendsPanel.ShowProfilePopup(friend, friendCount, attributes.ToList());
                    ShowStatus($"Viewing profile for {friend.Username}", false);
                }
                else
                {
                    ShowStatus($"Cannot display profile for {friend.Username}", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load profile: {ex.Message}", true);
                OnError?.Invoke($"Failed to load profile: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }
        
        private async Task<int> GetFriendCountAsync(int userId)
        {
            try
            {
                var friendDetails = await currentUser.GetUserDetailsAsync(userId);
                return friendDetails.Friends?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private async void HandleUnfriend(Friend friend)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                // UnfriendPlayerAsync takes the friend's user ID
                FriendshipResponse response = await currentUser.UnfriendPlayerAsync(friend.Id);
                
                // Remove from local list
                friends.RemoveAll(f => f.Id == friend.Id);
                DisplayFriendsList();
                
                ShowStatus($"Unfriended {friend.Username}", false);
                OnSuccess?.Invoke($"Unfriended {friend.Username}");
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to unfriend: {ex.Message}", true);
                OnError?.Invoke($"Failed to unfriend: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        #endregion

        #region Friend Requests

        private async void RefreshFriendRequests()
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                var incomingData = await currentUser.GetIncomingFriendRequestsAsync();
                incomingRequests = incomingData.ToList();
                
                var outgoingData = await currentUser.GetOutgoingFriendRequestsAsync();
                outgoingRequests = outgoingData.ToList();
                
                DisplayFriendRequests();
                ShowStatus("Friend requests refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load friend requests: {ex.Message}", true);
                OnError?.Invoke($"Failed to load friend requests: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void DisplayFriendRequests()
        {
            DisplayIncomingRequests();
            DisplayOutgoingRequests();
        }

        private void DisplayIncomingRequests()
        {
            // Clear existing items except empty state
            var children = incomingRequestsList.Children().ToList();
            foreach (var child in children)
            {
                if (child != incomingEmptyState)
                {
                    incomingRequestsList.Remove(child);
                }
            }
            
            // Show/hide empty state
            if (incomingRequests.Count == 0)
            {
                incomingEmptyState.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                incomingEmptyState.style.display = DisplayStyle.None;
            }
            
            // Add request items
            foreach (var request in incomingRequests)
            {
                var requestItem = CreateIncomingRequestItem(request);
                incomingRequestsList.Add(requestItem);
            }
        }

        private void DisplayOutgoingRequests()
        {
            // Clear existing items except empty state
            var children = outgoingRequestsList.Children().ToList();
            foreach (var child in children)
            {
                if (child != outgoingEmptyState)
                {
                    outgoingRequestsList.Remove(child);
                }
            }
            
            // Show/hide empty state
            if (outgoingRequests.Count == 0)
            {
                outgoingEmptyState.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                outgoingEmptyState.style.display = DisplayStyle.None;
            }
            
            // Add request items
            foreach (var request in outgoingRequests)
            {
                var requestItem = CreateOutgoingRequestItem(request);
                outgoingRequestsList.Add(requestItem);
            }
        }

        private VisualElement CreateIncomingRequestItem(FriendRequest request)
        {
            var item = new VisualElement();
            item.AddToClassList("request-item");
            
            // User info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("user-info");
            
            var avatar = new VisualElement();
            avatar.AddToClassList("user-avatar");
            infoSection.Add(avatar);
            
            var details = new VisualElement();
            details.AddToClassList("user-details");
            
            var nameLabel = new Label(request.Username);
            nameLabel.AddToClassList("user-name");
            details.Add(nameLabel);
            
            var statsLabel = new Label($"Score: {request.Score} | Credits: {request.Credits}");
            statsLabel.AddToClassList("user-status");
            details.Add(statsLabel);
            
            var dateLabel = new Label($"Requested: {request.RequestedAt}");
            dateLabel.AddToClassList("request-date");
            details.Add(dateLabel);
            
            infoSection.Add(details);
            item.Add(infoSection);
            
            // Actions section
            var actionsSection = new VisualElement();
            actionsSection.AddToClassList("request-actions");
            
            var acceptButton = new Button(() => HandleAcceptRequest(request));
            acceptButton.text = "Accept";
            acceptButton.AddToClassList("accept-button");
            actionsSection.Add(acceptButton);
            
            var declineButton = new Button(() => HandleDeclineRequest(request));
            declineButton.text = "Decline";
            declineButton.AddToClassList("decline-button");
            actionsSection.Add(declineButton);
            
            item.Add(actionsSection);
            
            return item;
        }

        private VisualElement CreateOutgoingRequestItem(FriendRequest request)
        {
            var item = new VisualElement();
            item.AddToClassList("request-item");
            
            // User info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("user-info");
            
            var avatar = new VisualElement();
            avatar.AddToClassList("user-avatar");
            infoSection.Add(avatar);
            
            var details = new VisualElement();
            details.AddToClassList("user-details");
            
            var nameLabel = new Label(request.Username);
            nameLabel.AddToClassList("user-name");
            details.Add(nameLabel);
            
            var statsLabel = new Label($"Score: {request.Score} | Credits: {request.Credits}");
            statsLabel.AddToClassList("user-status");
            details.Add(statsLabel);
            
            var statusLabel = new Label("Pending");
            statusLabel.AddToClassList("request-status");
            details.Add(statusLabel);
            
            var dateLabel = new Label($"Requested: {request.RequestedAt}");
            dateLabel.AddToClassList("request-date");
            details.Add(dateLabel);
            
            infoSection.Add(details);
            item.Add(infoSection);
            
            // Actions section
            var actionsSection = new VisualElement();
            actionsSection.AddToClassList("request-actions");
            
            var cancelButton = new Button(() => HandleCancelRequest(request));
            cancelButton.text = "Cancel";
            cancelButton.AddToClassList("cancel-button");
            actionsSection.Add(cancelButton);
            
            item.Add(actionsSection);
            
            return item;
        }

        private async void HandleAcceptRequest(FriendRequest request)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                await currentUser.AcceptFriendRequestAsync(request.FriendshipId);
                
                // Remove from incoming requests
                incomingRequests.RemoveAll(r => r.FriendshipId == request.FriendshipId);
                DisplayIncomingRequests();
                
                // Refresh friends list
                await Task.Delay(500); // Small delay to ensure server state is updated
                RefreshFriendsList();
                
                ShowStatus($"Accepted friend request from {request.Username}", false);
                OnSuccess?.Invoke($"Accepted friend request from {request.Username}");
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to accept request: {ex.Message}", true);
                OnError?.Invoke($"Failed to accept request: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleDeclineRequest(FriendRequest request)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                await currentUser.DeclineFriendRequestAsync(request.FriendshipId);
                
                // Remove from incoming requests
                incomingRequests.RemoveAll(r => r.FriendshipId == request.FriendshipId);
                DisplayIncomingRequests();
                
                ShowStatus($"Declined friend request from {request.Username}", false);
                OnSuccess?.Invoke($"Declined friend request from {request.Username}");
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to decline request: {ex.Message}", true);
                OnError?.Invoke($"Failed to decline request: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleCancelRequest(FriendRequest request)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                await currentUser.CancelFriendRequestAsync(request.FriendshipId);
                
                // Remove from outgoing requests
                outgoingRequests.RemoveAll(r => r.FriendshipId == request.FriendshipId);
                DisplayOutgoingRequests();
                
                ShowStatus($"Cancelled friend request to {request.Username}", false);
                OnSuccess?.Invoke($"Cancelled friend request to {request.Username}");
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to cancel request: {ex.Message}", true);
                OnError?.Invoke($"Failed to cancel request: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        #endregion

        // Friend Request methods
        private async void HandleSendFriendRequest()
        {
            if (currentUser == null) return;
            
            var username = friendUsernameInput.value?.Trim();
            
            if (string.IsNullOrEmpty(username))
            {
                ShowStatus("Please enter a username", true);
                return;
            }
            
            SetLoading(true);
            
            try
            {
                FriendshipResponse response = await currentUser.SendFriendRequestAsync(username);
                
                // Clear the input field
                friendUsernameInput.value = "";
                
                // Refresh outgoing requests list
                await Task.Delay(500); // Small delay to ensure server state is updated
                await RefreshOutgoingRequestsOnly();
                DisplayOutgoingRequests();
                
                ShowStatus($"Friend request sent to {username}", false);
                OnSuccess?.Invoke($"Friend request sent to {username}");
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to send friend request: {ex.Message}", true);
                OnError?.Invoke($"Failed to send friend request: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }
        
        // RefreshOutgoingRequestsOnly utility method
        private async Task RefreshOutgoingRequestsOnly()
        {
            if (currentUser == null) return;
            
            try
            {
                var outgoingData = await currentUser.GetOutgoingFriendRequestsAsync();
                outgoingRequests = outgoingData.ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to refresh outgoing requests: {ex.Message}");
            }
        }

        private void SetLoading(bool loading)
        {
            if (loadingIndicator != null)
                loadingIndicator.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ShowStatus(string message, bool isError)
        {
            if (statusLabel == null) return;

            statusLabel.text = message;
            statusLabel.RemoveFromClassList("status-success");
            statusLabel.RemoveFromClassList("status-error");
            statusLabel.AddToClassList(isError ? "status-error" : "status-success");
            statusLabel.style.display = DisplayStyle.Flex;

            // Auto-hide after 3 seconds
            _ = Task.Delay(3000).ContinueWith(_ => {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (statusLabel != null)
                        statusLabel.style.display = DisplayStyle.None;
                });
            });
        }

        public void Reset()
        {
            currentUser = null;

            // Clear inputs
            if (friendUsernameInput != null)
                friendUsernameInput.value = "";

            // Clear data collections
            friends.Clear();
            incomingRequests.Clear();
            outgoingRequests.Clear();

            // Reset UI displays
            DisplayFriendsList();
            DisplayFriendRequests();

            // Reset to friends tab
            SwitchTab("friends");

            // Hide status
            if (statusLabel != null)
                statusLabel.style.display = DisplayStyle.None;

            SetLoading(false);
        }
    }
}