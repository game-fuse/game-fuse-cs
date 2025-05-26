using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using GameFuse.Models.Shared;

namespace GameFuse.UI.Controls
{
    /// <summary>
    /// A custom control for displaying and managing groups and communities.
    /// Part of Phase 4: Groups & Communities
    /// </summary>
    [UxmlElement]
    public partial class GroupsPanel : VisualElement
    {
        // Group details popup elements
        private VisualElement groupDetailsPopupOverlay;
        private VisualElement groupDetailsPopup;
        private Label groupDetailsName;
        private Label groupDetailsDescription;
        private Label groupDetailsMembersCount;
        private Label groupDetailsCreatedDate;
        private ScrollView groupDetailsAttributesList;
        private ScrollView groupDetailsMembersList;
        private VisualElement groupDetailsAdminControls;
        
        // Constructor
        public GroupsPanel()
        {
            // Create the UI structure programmatically
            CreateGroupsPanelContent();
            
            // Create the group details popup
            CreateGroupDetailsPopup();
            
            // Add the panel-content class for consistency with other panels
            AddToClassList("panel-content");
            
            // Log creation for debugging
            Debug.Log("GroupsPanel custom control created");
        }
        
        // Method to initialize the groups panel elements after the controller attaches
        public void Initialize()
        {
            Debug.Log("GroupsPanel.Initialize() called");
        }
        
        private void CreateGroupsPanelContent()
        {
            // Header with title and status
            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("groups-header");
            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.justifyContent = Justify.SpaceBetween;
            headerContainer.style.alignItems = Align.Center;
            headerContainer.style.marginBottom = 10;
            
            var titleLabel = new Label("Groups & Communities");
            titleLabel.AddToClassList("panel-title");
            headerContainer.Add(titleLabel);
            
            var statusLabel = new Label();
            statusLabel.name = "groups-status";
            statusLabel.AddToClassList("status-label");
            statusLabel.style.display = DisplayStyle.None;
            statusLabel.style.flexGrow = 1;
            statusLabel.style.marginLeft = 20;
            headerContainer.Add(statusLabel);
            
            Add(headerContainer);
            
            // Loading indicator
            var loadingIndicator = new VisualElement();
            loadingIndicator.name = "groups-loading";
            loadingIndicator.AddToClassList("loading-indicator");
            var loadingText = new Label("Processing...");
            loadingText.AddToClassList("loading-text");
            loadingIndicator.Add(loadingText);
            loadingIndicator.style.display = DisplayStyle.None;
            Add(loadingIndicator);
            
            // Main container with tabs for different sections
            var mainContainer = new VisualElement();
            mainContainer.name = "groups-main-container";
            mainContainer.style.flexGrow = 1;
            
            // Tabs container
            var tabsContainer = new VisualElement();
            tabsContainer.name = "groups-tabs";
            tabsContainer.AddToClassList("tabs-container");
            tabsContainer.style.flexDirection = FlexDirection.Row;
            tabsContainer.style.marginBottom = 15;
            
            var myGroupsTabButton = new Button();
            myGroupsTabButton.name = "my-groups-tab-button";
            myGroupsTabButton.text = "My Groups";
            myGroupsTabButton.AddToClassList("tab-button");
            myGroupsTabButton.AddToClassList("active-tab");
            tabsContainer.Add(myGroupsTabButton);
            
            var discoverTabButton = new Button();
            discoverTabButton.name = "discover-tab-button";
            discoverTabButton.text = "Discover Groups";
            discoverTabButton.AddToClassList("tab-button");
            tabsContainer.Add(discoverTabButton);
            
            var createTabButton = new Button();
            createTabButton.name = "create-tab-button";
            createTabButton.text = "Create Group";
            createTabButton.AddToClassList("tab-button");
            tabsContainer.Add(createTabButton);
            
            mainContainer.Add(tabsContainer);
            
            // Content container for tab panels
            var contentContainer = new VisualElement();
            contentContainer.name = "groups-content-container";
            contentContainer.style.flexGrow = 1;
            
            // Create the different content panels
            CreateMyGroupsPanel(contentContainer);
            CreateDiscoverGroupsPanel(contentContainer);
            CreateGroupCreationPanel(contentContainer);
            
            mainContainer.Add(contentContainer);
            Add(mainContainer);
        }
        
        private void CreateMyGroupsPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "my-groups-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.Flex; // Show by default
            
            var infoText = new Label("Groups you're a member of or administrate.");
            infoText.AddToClassList("info-text");
            panel.Add(infoText);
            
            // Controls container
            var controlsContainer = new VisualElement();
            controlsContainer.name = "my-groups-controls";
            controlsContainer.AddToClassList("groups-controls");
            controlsContainer.style.flexDirection = FlexDirection.Row;
            controlsContainer.style.justifyContent = Justify.SpaceBetween;
            controlsContainer.style.alignItems = Align.Center;
            controlsContainer.style.marginTop = 10;
            controlsContainer.style.marginBottom = 15;
            
            // Left side - count
            var countContainer = new VisualElement();
            countContainer.style.flexDirection = FlexDirection.Row;
            countContainer.style.alignItems = Align.Center;
            
            var countLabel = new Label("Groups: ");
            countLabel.AddToClassList("count-label");
            var countValue = new Label("0");
            countValue.name = "my-groups-count";
            countValue.AddToClassList("count-value");
            
            countContainer.Add(countLabel);
            countContainer.Add(countValue);
            controlsContainer.Add(countContainer);
            
            // Right side - refresh button
            var refreshButton = new Button();
            refreshButton.name = "my-groups-refresh-button";
            refreshButton.text = "Refresh";
            refreshButton.AddToClassList("refresh-button");
            refreshButton.AddToClassList("secondary");
            controlsContainer.Add(refreshButton);
            
            panel.Add(controlsContainer);
            
            // Groups list container
            var listContainer = new ScrollView();
            listContainer.name = "my-groups-list-container";
            listContainer.AddToClassList("groups-list");
            listContainer.style.height = 400;
            listContainer.style.minHeight = 300;
            
            // Empty state
            var emptyState = new VisualElement();
            emptyState.name = "my-groups-empty-state";
            emptyState.AddToClassList("empty-state");
            
            var emptyIcon = new VisualElement();
            emptyIcon.AddToClassList("empty-icon");
            emptyState.Add(emptyIcon);
            
            var emptyText = new Label("You haven't joined any groups yet. Discover groups to join!");
            emptyText.AddToClassList("empty-text");
            emptyState.Add(emptyText);
            
            listContainer.Add(emptyState);
            panel.Add(listContainer);
            
            parent.Add(panel);
        }
        
        private void CreateDiscoverGroupsPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "discover-groups-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.None; // Hidden by default
            
            var infoText = new Label("Browse and join available groups.");
            infoText.AddToClassList("info-text");
            panel.Add(infoText);
            
            // Search and filter controls
            var searchContainer = new VisualElement();
            searchContainer.name = "discover-search-container";
            searchContainer.AddToClassList("search-controls");
            searchContainer.style.flexDirection = FlexDirection.Row;
            searchContainer.style.marginTop = 10;
            searchContainer.style.marginBottom = 15;
            
            var searchInput = new TextField();
            searchInput.name = "discover-search-input";
            searchInput.AddToClassList("search-input");
            searchInput.textEdition.placeholder = "Search groups...";
            searchInput.style.flexGrow = 1;
            searchInput.style.marginRight = 10;
            searchContainer.Add(searchInput);
            
            var searchButton = new Button();
            searchButton.name = "discover-search-button";
            searchButton.text = "Search";
            searchButton.AddToClassList("search-button");
            searchButton.AddToClassList("primary");
            searchContainer.Add(searchButton);
            
            panel.Add(searchContainer);
            
            // Results info
            var resultsInfo = new VisualElement();
            resultsInfo.style.flexDirection = FlexDirection.Row;
            resultsInfo.style.justifyContent = Justify.SpaceBetween;
            resultsInfo.style.alignItems = Align.Center;
            resultsInfo.style.marginBottom = 10;
            
            var resultsLabel = new Label("Available Groups");
            resultsLabel.name = "discover-results-label";
            resultsLabel.AddToClassList("section-title");
            resultsInfo.Add(resultsLabel);
            
            var resultsCount = new Label("(0)");
            resultsCount.name = "discover-results-count";
            resultsCount.AddToClassList("count-value");
            resultsInfo.Add(resultsCount);
            
            panel.Add(resultsInfo);
            
            // Groups list container
            var listContainer = new ScrollView();
            listContainer.name = "discover-groups-list-container";
            listContainer.AddToClassList("groups-list");
            listContainer.style.height = 350;
            listContainer.style.minHeight = 250;
            
            // Empty state
            var emptyState = new VisualElement();
            emptyState.name = "discover-empty-state";
            emptyState.AddToClassList("empty-state");
            
            var emptyText = new Label("No groups found. Try adjusting your search.");
            emptyText.AddToClassList("empty-text");
            emptyState.Add(emptyText);
            
            listContainer.Add(emptyState);
            panel.Add(listContainer);
            
            // Pagination controls
            var paginationContainer = new VisualElement();
            paginationContainer.name = "discover-pagination";
            paginationContainer.AddToClassList("pagination-controls");
            paginationContainer.style.flexDirection = FlexDirection.Row;
            paginationContainer.style.justifyContent = Justify.Center;
            paginationContainer.style.alignItems = Align.Center;
            paginationContainer.style.marginTop = 10;
            paginationContainer.style.display = DisplayStyle.None;
            
            var prevButton = new Button();
            prevButton.name = "discover-prev-page";
            prevButton.text = "Previous";
            prevButton.AddToClassList("pagination-button");
            prevButton.AddToClassList("secondary");
            paginationContainer.Add(prevButton);
            
            var pageInfo = new Label("Page 1 of 1");
            pageInfo.name = "discover-page-info";
            pageInfo.AddToClassList("page-info");
            pageInfo.style.marginLeft = 15;
            pageInfo.style.marginRight = 15;
            paginationContainer.Add(pageInfo);
            
            var nextButton = new Button();
            nextButton.name = "discover-next-page";
            nextButton.text = "Next";
            nextButton.AddToClassList("pagination-button");
            nextButton.AddToClassList("secondary");
            paginationContainer.Add(nextButton);
            
            panel.Add(paginationContainer);
            
            parent.Add(panel);
        }
        
        private void CreateGroupCreationPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "create-group-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.None; // Hidden by default
            
            var infoText = new Label("Create a new group for your community.");
            infoText.AddToClassList("info-text");
            panel.Add(infoText);
            
            // Creation form
            var formContainer = new VisualElement();
            formContainer.name = "create-group-form";
            formContainer.AddToClassList("group-form");
            formContainer.style.marginTop = 15;
            
            // Group name
            var nameField = new TextField("Group Name");
            nameField.name = "create-group-name";
            nameField.AddToClassList("form-field");
            nameField.textEdition.placeholder = "Enter group name";
            formContainer.Add(nameField);
            
            // Group type
            var typeLabel = new Label("Group Type");
            typeLabel.AddToClassList("form-label");
            formContainer.Add(typeLabel);
            
            var typeField = new TextField();
            typeField.name = "create-group-description";
            typeField.AddToClassList("form-field");
            typeField.textEdition.placeholder = "e.g., guild, clan, team, general";
            formContainer.Add(typeField);
            
            // Privacy settings
            var privacyLabel = new Label("Privacy");
            privacyLabel.AddToClassList("form-label");
            privacyLabel.style.marginTop = 15;
            formContainer.Add(privacyLabel);
            
            var privacyContainer = new VisualElement();
            privacyContainer.style.flexDirection = FlexDirection.Row;
            privacyContainer.style.marginBottom = 15;
            
            var publicToggle = new Toggle("Public (Anyone can join)");
            publicToggle.name = "create-group-public";
            publicToggle.AddToClassList("form-toggle");
            publicToggle.value = true;
            privacyContainer.Add(publicToggle);
            
            var privateToggle = new Toggle("Private (Requires approval)");
            privateToggle.name = "create-group-private";
            privateToggle.AddToClassList("form-toggle");
            privateToggle.style.marginLeft = 20;
            privacyContainer.Add(privateToggle);
            
            formContainer.Add(privacyContainer);
            
            // Initial attributes section
            var attributesLabel = new Label("Initial Attributes (Optional)");
            attributesLabel.AddToClassList("form-label");
            formContainer.Add(attributesLabel);
            
            var attributesContainer = new VisualElement();
            attributesContainer.name = "create-group-attributes";
            attributesContainer.AddToClassList("attributes-container");
            attributesContainer.style.marginBottom = 15;
            
            var addAttributeButton = new Button();
            addAttributeButton.name = "add-group-attribute-button";
            addAttributeButton.text = "+ Add Attribute";
            addAttributeButton.AddToClassList("add-attribute-button");
            addAttributeButton.AddToClassList("secondary");
            attributesContainer.Add(addAttributeButton);
            
            formContainer.Add(attributesContainer);
            
            // Form buttons
            var buttonsContainer = new VisualElement();
            buttonsContainer.AddToClassList("form-buttons");
            buttonsContainer.style.flexDirection = FlexDirection.Row;
            buttonsContainer.style.justifyContent = Justify.FlexEnd;
            buttonsContainer.style.marginTop = 20;
            
            var cancelButton = new Button();
            cancelButton.name = "create-group-cancel";
            cancelButton.text = "Cancel";
            cancelButton.AddToClassList("form-button");
            cancelButton.AddToClassList("secondary");
            buttonsContainer.Add(cancelButton);
            
            var createButton = new Button();
            createButton.name = "create-group-submit";
            createButton.text = "Create Group";
            createButton.AddToClassList("form-button");
            createButton.AddToClassList("primary");
            createButton.style.marginLeft = 10;
            buttonsContainer.Add(createButton);
            
            formContainer.Add(buttonsContainer);
            
            panel.Add(formContainer);
            parent.Add(panel);
        }
        
        private void CreateGroupDetailsPopup()
        {
            // Create the group details popup overlay
            groupDetailsPopupOverlay = new VisualElement();
            groupDetailsPopupOverlay.name = "group-details-popup-overlay";
            groupDetailsPopupOverlay.AddToClassList("popup-overlay");
            groupDetailsPopupOverlay.pickingMode = PickingMode.Ignore;
            
            // Create the popup container
            groupDetailsPopup = new VisualElement();
            groupDetailsPopup.name = "group-details-popup";
            groupDetailsPopup.AddToClassList("popup-container");
            groupDetailsPopup.pickingMode = PickingMode.Position;
            
            // Popup header
            var header = new VisualElement();
            header.AddToClassList("popup-header");
            
            var title = new Label("Group Details");
            title.AddToClassList("popup-title");
            header.Add(title);
            
            var closeButton = new Button(() => HideGroupDetailsPopup());
            closeButton.text = "×";
            closeButton.AddToClassList("popup-close");
            closeButton.AddToClassList("danger");
            closeButton.style.backgroundColor = new StyleColor(new Color(0.7f, 0.24f, 0.24f));
            closeButton.style.color = Color.white;
            closeButton.style.fontSize = 18;
            closeButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            closeButton.style.borderLeftWidth = 0;
            closeButton.style.borderRightWidth = 0;
            closeButton.style.borderTopWidth = 0;
            closeButton.style.borderBottomWidth = 0;
            header.Add(closeButton);
            
            groupDetailsPopup.Add(header);
            
            // Popup content with scrollview
            var scrollView = new ScrollView();
            scrollView.AddToClassList("popup-content-scroll");
            
            var content = new VisualElement();
            content.AddToClassList("popup-content");
            
            // Group info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("group-info-section");
            
            groupDetailsName = new Label();
            groupDetailsName.AddToClassList("group-name");
            infoSection.Add(groupDetailsName);
            
            groupDetailsDescription = new Label();
            groupDetailsDescription.AddToClassList("group-description");
            infoSection.Add(groupDetailsDescription);
            
            // Stats container
            var statsContainer = new VisualElement();
            statsContainer.AddToClassList("group-stats");
            statsContainer.style.flexDirection = FlexDirection.Row;
            statsContainer.style.marginTop = 10;
            
            var membersContainer = new VisualElement();
            membersContainer.AddToClassList("stat-item");
            var membersLabel = new Label("Members:");
            membersLabel.AddToClassList("stat-label");
            membersContainer.Add(membersLabel);
            groupDetailsMembersCount = new Label();
            groupDetailsMembersCount.AddToClassList("stat-value");
            membersContainer.Add(groupDetailsMembersCount);
            statsContainer.Add(membersContainer);
            
            var createdContainer = new VisualElement();
            createdContainer.AddToClassList("stat-item");
            createdContainer.style.marginLeft = 20;
            var createdLabel = new Label("Created:");
            createdLabel.AddToClassList("stat-label");
            createdContainer.Add(createdLabel);
            groupDetailsCreatedDate = new Label();
            groupDetailsCreatedDate.AddToClassList("stat-value");
            createdContainer.Add(groupDetailsCreatedDate);
            statsContainer.Add(createdContainer);
            
            infoSection.Add(statsContainer);
            content.Add(infoSection);
            
            // Admin controls (hidden by default)
            groupDetailsAdminControls = new VisualElement();
            groupDetailsAdminControls.name = "group-admin-controls";
            groupDetailsAdminControls.AddToClassList("admin-controls-section");
            groupDetailsAdminControls.style.display = DisplayStyle.None;
            
            var adminTitle = new Label("Admin Controls");
            adminTitle.AddToClassList("section-title");
            groupDetailsAdminControls.Add(adminTitle);
            
            var adminButtons = new VisualElement();
            adminButtons.style.flexDirection = FlexDirection.Row;
            adminButtons.style.marginTop = 10;
            
            var manageMembersButton = new Button();
            manageMembersButton.name = "manage-members-button";
            manageMembersButton.text = "Manage Members";
            manageMembersButton.AddToClassList("admin-button");
            manageMembersButton.AddToClassList("secondary");
            adminButtons.Add(manageMembersButton);
            
            var editGroupButton = new Button();
            editGroupButton.name = "edit-group-button";
            editGroupButton.text = "Edit Group";
            editGroupButton.AddToClassList("admin-button");
            editGroupButton.AddToClassList("primary");
            editGroupButton.style.marginLeft = 10;
            adminButtons.Add(editGroupButton);
            
            groupDetailsAdminControls.Add(adminButtons);
            content.Add(groupDetailsAdminControls);
            
            // Attributes section
            var attributesSection = new VisualElement();
            attributesSection.AddToClassList("group-attributes-section");
            
            var attributesTitle = new Label("Group Attributes");
            attributesTitle.AddToClassList("section-title");
            attributesSection.Add(attributesTitle);
            
            groupDetailsAttributesList = new ScrollView();
            groupDetailsAttributesList.AddToClassList("attributes-list");
            groupDetailsAttributesList.style.maxHeight = 150;
            attributesSection.Add(groupDetailsAttributesList);
            
            content.Add(attributesSection);
            
            // Members section
            var membersSection = new VisualElement();
            membersSection.AddToClassList("group-members-section");
            
            var membersTitle = new Label("Members");
            membersTitle.AddToClassList("section-title");
            membersSection.Add(membersTitle);
            
            groupDetailsMembersList = new ScrollView();
            groupDetailsMembersList.AddToClassList("members-list");
            groupDetailsMembersList.style.maxHeight = 200;
            membersSection.Add(groupDetailsMembersList);
            
            content.Add(membersSection);
            
            scrollView.Add(content);
            groupDetailsPopup.Add(scrollView);
            
            // Add the popup to the overlay
            groupDetailsPopupOverlay.Add(groupDetailsPopup);
            
            // Add the overlay to the panel
            Add(groupDetailsPopupOverlay);
        }
        
        public void ShowGroupDetailsPopup(Group group, List<GroupAttributeResponseItem> attributes, List<Friend> members, bool isAdmin)
        {
            // Set group data
            groupDetailsName.text = group.Name;
            groupDetailsDescription.text = group.GroupType ?? "No type specified";
            groupDetailsMembersCount.text = group.MemberCount.ToString();
            groupDetailsCreatedDate.text = "N/A"; // CreatedAt not available in the model
            
            // Show/hide admin controls
            if (groupDetailsAdminControls != null)
            {
                groupDetailsAdminControls.style.display = isAdmin ? DisplayStyle.Flex : DisplayStyle.None;
            }
            
            // Clear and populate attributes
            groupDetailsAttributesList.Clear();
            if (attributes != null && attributes.Count > 0)
            {
                foreach (var attribute in attributes)
                {
                    var attributeItem = new VisualElement();
                    attributeItem.AddToClassList("attribute-item");
                    
                    var keyLabel = new Label(attribute.Key);
                    keyLabel.AddToClassList("attribute-key");
                    attributeItem.Add(keyLabel);
                    
                    var valueLabel = new Label(attribute.Value);
                    valueLabel.AddToClassList("attribute-value");
                    attributeItem.Add(valueLabel);
                    
                    groupDetailsAttributesList.Add(attributeItem);
                }
            }
            else
            {
                var emptyLabel = new Label("No attributes set");
                emptyLabel.AddToClassList("empty-attributes");
                groupDetailsAttributesList.Add(emptyLabel);
            }
            
            // Clear and populate members
            groupDetailsMembersList.Clear();
            if (members != null && members.Count > 0)
            {
                foreach (var member in members)
                {
                    var memberItem = new VisualElement();
                    memberItem.AddToClassList("member-item");
                    
                    var memberInfo = new VisualElement();
                    memberInfo.AddToClassList("member-info");
                    
                    var avatar = new VisualElement();
                    avatar.AddToClassList("member-avatar");
                    memberInfo.Add(avatar);
                    
                    var nameLabel = new Label(member.Username);
                    nameLabel.AddToClassList("member-name");
                    memberInfo.Add(nameLabel);
                    
                    memberItem.Add(memberInfo);
                    
                    if (isAdmin)
                    {
                        var actionsContainer = new VisualElement();
                        actionsContainer.AddToClassList("member-actions");
                        
                        var removeButton = new Button();
                        removeButton.text = "Remove";
                        removeButton.AddToClassList("remove-member-button");
                        removeButton.AddToClassList("danger");
                        actionsContainer.Add(removeButton);
                        
                        memberItem.Add(actionsContainer);
                    }
                    
                    groupDetailsMembersList.Add(memberItem);
                }
            }
            else
            {
                var emptyLabel = new Label("No members yet");
                emptyLabel.AddToClassList("empty-members");
                groupDetailsMembersList.Add(emptyLabel);
            }
            
            // Show the popup
            groupDetailsPopupOverlay.style.display = DisplayStyle.Flex;
            
            // Register a click event on the overlay background to close the popup when clicking outside
            groupDetailsPopupOverlay.RegisterCallback<ClickEvent>(evt => {
                if (evt.target == groupDetailsPopupOverlay)
                {
                    HideGroupDetailsPopup();
                }
            });
        }
        
        public void HideGroupDetailsPopup()
        {
            groupDetailsPopupOverlay.style.display = DisplayStyle.None;
            
            // Unregister the click event to prevent memory leaks
            groupDetailsPopupOverlay.UnregisterCallback<ClickEvent>(evt => {
                if (evt.target == groupDetailsPopupOverlay)
                {
                    HideGroupDetailsPopup();
                }
            });
        }
    }
}