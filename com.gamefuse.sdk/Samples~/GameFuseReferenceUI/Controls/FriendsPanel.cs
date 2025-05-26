using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using GameFuse.Models.Shared;

namespace GameFuse.UI.Controls
{
    /// <summary>
    /// A custom control for displaying and managing friends, friend requests, and user search.
    /// Part of Phase 3: Social Features - Friends
    /// </summary>
    [UxmlElement]
    public partial class FriendsPanel : VisualElement
    {
        // Profile popup elements
        private VisualElement profilePopupOverlay;
        private VisualElement profilePopup;
        private Label profileUsername;
        private Label profileFriendsCount;
        private Label profileScore;
        private Label profileCredits;
        private ScrollView profileAttributesList;
        
        // Constructor
        public FriendsPanel()
        {
            // Create the UI structure programmatically
            CreateFriendsPanelContent();
            
            // Create the profile popup
            CreateProfilePopup();
            
            // Add the panel-content class for consistency with other panels
            AddToClassList("panel-content");
            
            // Log creation for debugging
            Debug.Log("FriendsPanel custom control created");
        }
        
        // Method to initialize the friends panel elements after the controller attaches
        public void Initialize()
        {
            Debug.Log("FriendsPanel.Initialize() called");
        }
        
        private void CreateFriendsPanelContent()
        {
            // Header with title and status
            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("friends-header");
            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.justifyContent = Justify.SpaceBetween;
            headerContainer.style.alignItems = Align.Center;
            headerContainer.style.marginBottom = 10;
            
            var titleLabel = new Label("Friends");
            titleLabel.AddToClassList("panel-title");
            headerContainer.Add(titleLabel);
            
            var statusLabel = new Label();
            statusLabel.name = "friends-status";
            statusLabel.AddToClassList("status-label");
            statusLabel.style.display = DisplayStyle.None;
            statusLabel.style.flexGrow = 1;
            statusLabel.style.marginLeft = 20;
            headerContainer.Add(statusLabel);
            
            Add(headerContainer);
            
            // Loading indicator
            var loadingIndicator = new VisualElement();
            loadingIndicator.name = "friends-loading";
            loadingIndicator.AddToClassList("loading-indicator");
            var loadingText = new Label("Processing...");
            loadingText.AddToClassList("loading-text");
            loadingIndicator.Add(loadingText);
            loadingIndicator.style.display = DisplayStyle.None;
            Add(loadingIndicator);
            
            // Main container with tabs for different sections
            var mainContainer = new VisualElement();
            mainContainer.name = "friends-main-container";
            mainContainer.style.flexGrow = 1;
            
            // Tabs container
            var tabsContainer = new VisualElement();
            tabsContainer.name = "friends-tabs";
            tabsContainer.AddToClassList("tabs-container");
            tabsContainer.style.flexDirection = FlexDirection.Row;
            tabsContainer.style.marginBottom = 15;
            
            var friendsTabButton = new Button();
            friendsTabButton.name = "friends-tab-button";
            friendsTabButton.text = "My Friends";
            friendsTabButton.AddToClassList("tab-button");
            friendsTabButton.AddToClassList("active-tab");
            tabsContainer.Add(friendsTabButton);
            
            var requestsTabButton = new Button();
            requestsTabButton.name = "requests-tab-button";
            requestsTabButton.text = "Friend Requests";
            requestsTabButton.AddToClassList("tab-button");
            tabsContainer.Add(requestsTabButton);
            
            // Search tab button removed
            
            mainContainer.Add(tabsContainer);
            
            // Content container for tab panels
            var contentContainer = new VisualElement();
            contentContainer.name = "friends-content-container";
            contentContainer.style.flexGrow = 1;
            
            // Create the different content panels
            CreateFriendsListPanel(contentContainer);
            CreateFriendRequestsPanel(contentContainer);
            
            mainContainer.Add(contentContainer);
            Add(mainContainer);
        }
        
        private void CreateFriendsListPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "friends-list-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.Flex; // Show by default
            
            var infoText = new Label("These are the people you've connected with.");
            infoText.AddToClassList("info-text");
            panel.Add(infoText);
            
            // Friends count
            // Friends controls container (count, filter, refresh)
            var controlsContainer = new VisualElement();
            controlsContainer.name = "friends-controls";
            controlsContainer.AddToClassList("friends-controls");
            controlsContainer.style.flexDirection = FlexDirection.Row;
            controlsContainer.style.justifyContent = Justify.SpaceBetween;
            controlsContainer.style.alignItems = Align.Center;
            controlsContainer.style.marginTop = 10;
            controlsContainer.style.marginBottom = 15;
            
            // Left side - count and filter
            var leftControls = new VisualElement();
            leftControls.style.flexDirection = FlexDirection.Row;
            leftControls.style.alignItems = Align.Center;
            
            var countContainer = new VisualElement();
            countContainer.style.flexDirection = FlexDirection.Row;
            countContainer.style.alignItems = Align.Center;
            countContainer.style.marginRight = 20;
            
            var countLabel = new Label("Friends: ");
            countLabel.AddToClassList("count-label");
            var countValue = new Label("0");
            countValue.name = "friends-count";
            countValue.AddToClassList("count-value");
            
            countContainer.Add(countLabel);
            countContainer.Add(countValue);
            leftControls.Add(countContainer);
            
            // Filter input
            var filterInput = new TextField();
            filterInput.name = "friends-filter-input";
            filterInput.AddToClassList("filter-input");
            filterInput.textEdition.placeholder = "Filter friends...";
            filterInput.style.width = 200;
            leftControls.Add(filterInput);
            
            controlsContainer.Add(leftControls);
            
            // Right side - refresh button
            var refreshButton = new Button();
            refreshButton.name = "friends-refresh-button";
            refreshButton.text = "Refresh";
            refreshButton.AddToClassList("refresh-button");
            refreshButton.AddToClassList("secondary");
            controlsContainer.Add(refreshButton);
            
            panel.Add(controlsContainer);
            
            // Friends list container
            var listContainer = new ScrollView();
            listContainer.name = "friends-list-container";
            listContainer.AddToClassList("friends-list");
            listContainer.style.height = 400;
            listContainer.style.minHeight = 300;
            
            // Empty state
            var emptyState = new VisualElement();
            emptyState.name = "friends-empty-state";
            emptyState.AddToClassList("empty-state");
            
            var emptyIcon = new VisualElement();
            emptyIcon.AddToClassList("empty-icon");
            emptyState.Add(emptyIcon);
            
            var emptyText = new Label("You don't have any friends yet. Add some friends to see them here.");
            emptyText.AddToClassList("empty-text");
            emptyState.Add(emptyText);
            
            listContainer.Add(emptyState);
            panel.Add(listContainer);
            
            // Pagination controls
            var paginationContainer = new VisualElement();
            paginationContainer.name = "friends-pagination";
            paginationContainer.AddToClassList("pagination-controls");
            paginationContainer.style.flexDirection = FlexDirection.Row;
            paginationContainer.style.justifyContent = Justify.Center;
            paginationContainer.style.alignItems = Align.Center;
            paginationContainer.style.marginTop = 10;
            paginationContainer.style.display = DisplayStyle.None; // Hidden by default
            
            var prevButton = new Button();
            prevButton.name = "friends-prev-page";
            prevButton.text = "Previous";
            prevButton.AddToClassList("pagination-button");
            prevButton.AddToClassList("secondary");
            paginationContainer.Add(prevButton);
            
            var pageInfo = new Label("Page 1 of 1");
            pageInfo.name = "friends-page-info";
            pageInfo.AddToClassList("page-info");
            pageInfo.style.marginLeft = 15;
            pageInfo.style.marginRight = 15;
            paginationContainer.Add(pageInfo);
            
            var nextButton = new Button();
            nextButton.name = "friends-next-page";
            nextButton.text = "Next";
            nextButton.AddToClassList("pagination-button");
            nextButton.AddToClassList("secondary");
            paginationContainer.Add(nextButton);
            
            panel.Add(paginationContainer);
            
            parent.Add(panel);
        }
        
        private void CreateFriendRequestsPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "friend-requests-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.None; // Hidden by default
            
            // Send Friend Request Section
            var sendRequestSection = new VisualElement();
            sendRequestSection.name = "send-request-section";
            sendRequestSection.AddToClassList("requests-section");
            
            var sendRequestTitle = new Label("Send Friend Request");
            sendRequestTitle.AddToClassList("section-title");
            sendRequestSection.Add(sendRequestTitle);
            
            var sendRequestInfo = new Label("Enter the username of the person you want to add as a friend");
            sendRequestInfo.AddToClassList("info-text");
            sendRequestSection.Add(sendRequestInfo);
            
            // Form Controls - Using management-controls style from profile panel
            var sendRequestForm = new VisualElement();
            sendRequestForm.name = "send-request-form";
            sendRequestForm.AddToClassList("management-controls");
            
            var usernameInput = new TextField();
            usernameInput.name = "friend-username-input";
            usernameInput.AddToClassList("management-input"); // Use the same class as profile panel
            usernameInput.textEdition.placeholder = "Enter username";
            sendRequestForm.Add(usernameInput);
            
            var buttonsContainer = new VisualElement();
            buttonsContainer.AddToClassList("management-buttons");
            
            var sendButton = new Button();
            sendButton.name = "send-request-button";
            sendButton.text = "Send Request";
            sendButton.AddToClassList("management-button");
            sendButton.AddToClassList("primary");
            buttonsContainer.Add(sendButton);
            
            sendRequestForm.Add(buttonsContainer);
            
            sendRequestSection.Add(sendRequestForm);
            
            panel.Add(sendRequestSection);
            
            // Incoming Requests Section
            var incomingSection = new VisualElement();
            incomingSection.name = "incoming-requests-section";
            incomingSection.AddToClassList("requests-section");
            
            var incomingTitle = new Label("Incoming Requests");
            incomingTitle.AddToClassList("section-title");
            incomingSection.Add(incomingTitle);
            
            var incomingInfo = new Label("People who want to connect with you");
            incomingInfo.AddToClassList("info-text");
            incomingSection.Add(incomingInfo);
            
            var incomingList = new ScrollView();
            incomingList.name = "incoming-requests-list";
            incomingList.AddToClassList("requests-list");
            incomingList.style.height = 180;
            incomingList.style.minHeight = 120;
            
            // Empty state for incoming
            var incomingEmptyState = new VisualElement();
            incomingEmptyState.name = "incoming-empty-state";
            incomingEmptyState.AddToClassList("empty-state");
            
            var incomingEmptyText = new Label("No incoming friend requests");
            incomingEmptyText.AddToClassList("empty-text");
            incomingEmptyState.Add(incomingEmptyText);
            
            incomingList.Add(incomingEmptyState);
            incomingSection.Add(incomingList);
            
            panel.Add(incomingSection);
            
            // Outgoing Requests Section
            var outgoingSection = new VisualElement();
            outgoingSection.name = "outgoing-requests-section";
            outgoingSection.AddToClassList("requests-section");
            outgoingSection.style.marginTop = 20;
            
            var outgoingTitle = new Label("Outgoing Requests");
            outgoingTitle.AddToClassList("section-title");
            outgoingSection.Add(outgoingTitle);
            
            var outgoingInfo = new Label("People you've sent requests to");
            outgoingInfo.AddToClassList("info-text");
            outgoingSection.Add(outgoingInfo);
            
            var outgoingList = new ScrollView();
            outgoingList.name = "outgoing-requests-list";
            outgoingList.AddToClassList("requests-list");
            outgoingList.style.height = 180;
            outgoingList.style.minHeight = 120;
            
            // Empty state for outgoing
            var outgoingEmptyState = new VisualElement();
            outgoingEmptyState.name = "outgoing-empty-state";
            outgoingEmptyState.AddToClassList("empty-state");
            
            var outgoingEmptyText = new Label("No outgoing friend requests");
            outgoingEmptyText.AddToClassList("empty-text");
            outgoingEmptyState.Add(outgoingEmptyText);
            
            outgoingList.Add(outgoingEmptyState);
            outgoingSection.Add(outgoingList);
            
            panel.Add(outgoingSection);
            
            parent.Add(panel);
        }
        
        // CreateUserSearchPanel method removed
        
        private void CreateProfilePopup()
        {
            // Create the profile popup overlay
            profilePopupOverlay = new VisualElement();
            profilePopupOverlay.name = "profile-popup-overlay";
            profilePopupOverlay.AddToClassList("profile-popup-overlay");
            profilePopupOverlay.pickingMode = PickingMode.Ignore; // Prevents clicks going through
            
            // Create the popup container
            profilePopup = new VisualElement();
            profilePopup.name = "profile-popup";
            profilePopup.AddToClassList("profile-popup");
            profilePopup.pickingMode = PickingMode.Position; // Allow clicks on the popup
            
            // Popup header
            var header = new VisualElement();
            header.AddToClassList("profile-popup-header");
            
            var title = new Label("Friend Profile");
            title.AddToClassList("profile-popup-title");
            header.Add(title);
            
            var closeButton = new Button(() => HideProfilePopup());
            closeButton.text = "×";
            closeButton.AddToClassList("profile-popup-close");
            closeButton.AddToClassList("danger"); // Add danger class for red styling
            // Apply inline styles to ensure it shows up as red
            closeButton.style.backgroundColor = new StyleColor(new Color(0.7f, 0.24f, 0.24f)); // RGB 180, 60, 60
            closeButton.style.color = Color.white;
            closeButton.style.fontSize = 18;
            closeButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            closeButton.style.borderLeftWidth = 0;
            closeButton.style.borderRightWidth = 0;
            closeButton.style.borderTopWidth = 0;
            closeButton.style.borderBottomWidth = 0;
            header.Add(closeButton);
            
            profilePopup.Add(header);
            
            // Popup content
            var content = new VisualElement();
            content.AddToClassList("profile-popup-content");
            
            // Info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("profile-info-section");
            
            var avatar = new VisualElement();
            avatar.AddToClassList("profile-avatar");
            infoSection.Add(avatar);
            
            var details = new VisualElement();
            details.AddToClassList("profile-details");
            
            profileUsername = new Label();
            profileUsername.AddToClassList("profile-username");
            details.Add(profileUsername);
            
            // Friends count
            var friendsCountContainer = new VisualElement();
            friendsCountContainer.AddToClassList("profile-stat");
            
            var friendsCountLabel = new Label("Friends:");
            friendsCountLabel.AddToClassList("profile-stat-label");
            friendsCountContainer.Add(friendsCountLabel);
            
            profileFriendsCount = new Label();
            profileFriendsCount.AddToClassList("profile-stat-value");
            friendsCountContainer.Add(profileFriendsCount);
            
            details.Add(friendsCountContainer);
            
            // Score
            var scoreContainer = new VisualElement();
            scoreContainer.AddToClassList("profile-stat");
            
            var scoreLabel = new Label("Score:");
            scoreLabel.AddToClassList("profile-stat-label");
            scoreContainer.Add(scoreLabel);
            
            profileScore = new Label();
            profileScore.AddToClassList("profile-stat-value");
            scoreContainer.Add(profileScore);
            
            details.Add(scoreContainer);
            
            // Credits
            var creditsContainer = new VisualElement();
            creditsContainer.AddToClassList("profile-stat");
            
            var creditsLabel = new Label("Credits:");
            creditsLabel.AddToClassList("profile-stat-label");
            creditsContainer.Add(creditsLabel);
            
            profileCredits = new Label();
            profileCredits.AddToClassList("profile-stat-value");
            creditsContainer.Add(profileCredits);
            
            details.Add(creditsContainer);
            
            infoSection.Add(details);
            content.Add(infoSection);
            
            // Attributes section
            var attributesSection = new VisualElement();
            attributesSection.AddToClassList("profile-attributes-section");
            
            var attributesTitle = new Label("Attributes");
            attributesTitle.AddToClassList("profile-attributes-title");
            attributesSection.Add(attributesTitle);
            
            profileAttributesList = new ScrollView();
            profileAttributesList.AddToClassList("profile-attributes-list");
            attributesSection.Add(profileAttributesList);
            
            content.Add(attributesSection);
            profilePopup.Add(content);
            
            // Add the popup to the overlay
            profilePopupOverlay.Add(profilePopup);
            
            // Add the overlay to the panel
            Add(profilePopupOverlay);
        }
    
        public void ShowProfilePopup(Friend friend, int friendCount, List<UserAttribute> attributes)
        {
            // Set profile data
            profileUsername.text = friend.Username;
            profileFriendsCount.text = friendCount.ToString();
            profileScore.text = friend.Score.ToString();
            profileCredits.text = friend.Credits.ToString();
            
            // Clear existing attributes
            profileAttributesList.Clear();
            
            // Add attributes
            if (attributes != null && attributes.Count > 0)
            {
                foreach (var attribute in attributes)
                {
                    var attributeItem = new VisualElement();
                    attributeItem.AddToClassList("profile-attribute-item");
                    
                    var keyLabel = new Label(attribute.Key);
                    keyLabel.AddToClassList("profile-attribute-key");
                    attributeItem.Add(keyLabel);
                    
                    var valueLabel = new Label(attribute.Value);
                    valueLabel.AddToClassList("profile-attribute-value");
                    attributeItem.Add(valueLabel);
                    
                    profileAttributesList.Add(attributeItem);
                }
            }
            else
            {
                var emptyLabel = new Label("No attributes found");
                emptyLabel.AddToClassList("profile-empty-attributes");
                profileAttributesList.Add(emptyLabel);
            }
            
            // Show the popup
            profilePopupOverlay.style.display = DisplayStyle.Flex;
            
            // Register a click event on the overlay background to close the popup when clicking outside
            profilePopupOverlay.RegisterCallback<ClickEvent>(evt => {
                if (evt.target == profilePopupOverlay)
                {
                    HideProfilePopup();
                }
            });
        }
        
        public void HideProfilePopup()
        {
            profilePopupOverlay.style.display = DisplayStyle.None;
            
            // Unregister the click event to prevent memory leaks
            profilePopupOverlay.UnregisterCallback<ClickEvent>(evt => {
                if (evt.target == profilePopupOverlay)
                {
                    HideProfilePopup();
                }
            });
        }
    }
}