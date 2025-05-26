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
    /// Controls the groups panel - manages group discovery, creation, and membership.
    /// Covers Phase 4 functionality: Groups & Communities
    /// </summary>
    public class GroupsPanelController
    {
        // Events
        public static event Action<string> OnError;
        public static event Action<string> OnSuccess;

        // UI Elements
        private VisualElement root;
        private GroupsPanel groupsPanel;
        private GameFuseUser currentUser;

        // Tab elements
        private Button myGroupsTabButton;
        private Button discoverTabButton;
        private Button createTabButton;
        private VisualElement myGroupsPanel;
        private VisualElement discoverGroupsPanel;
        private VisualElement createGroupPanel;

        // My Groups Elements
        private Label myGroupsCount;
        private Button myGroupsRefreshButton;
        private VisualElement myGroupsListContainer;
        private VisualElement myGroupsEmptyState;

        // Discover Groups Elements
        private TextField discoverSearchInput;
        private Button discoverSearchButton;
        private Label discoverResultsLabel;
        private Label discoverResultsCount;
        private VisualElement discoverGroupsListContainer;
        private VisualElement discoverEmptyState;
        private Label discoverPageInfo;
        private Button discoverPrevPageButton;
        private Button discoverNextPageButton;

        // Create Group Elements
        private TextField createGroupNameField;
        private TextField createGroupDescriptionField;
        private Toggle createGroupPublicToggle;
        private Toggle createGroupPrivateToggle;
        private VisualElement createGroupAttributesContainer;
        private Button addGroupAttributeButton;
        private Button createGroupCancelButton;
        private Button createGroupSubmitButton;

        // Loading and Status
        private VisualElement loadingIndicator;
        private Label statusLabel;

        // Data Containers
        private List<Group> myGroups = new List<Group>();
        private List<GroupSummary> allGroups = new List<GroupSummary>();
        private List<GroupSummary> filteredGroups = new List<GroupSummary>();
        private Dictionary<int, List<GroupAttributeResponseItem>> groupAttributesCache = new Dictionary<int, List<GroupAttributeResponseItem>>();
        private Dictionary<int, List<Friend>> groupMembersCache = new Dictionary<int, List<Friend>>();
        private List<GroupAttributePayloadItem> pendingGroupAttributes = new List<GroupAttributePayloadItem>();

        // Pagination
        private int groupsPerPage = 10;
        private int currentPage = 0;
        private int totalPages = 0;

        public GroupsPanelController(VisualElement panelContainer)
        {
            root = panelContainer;
            groupsPanel = root as GroupsPanel;
            
            // Handle both custom control and legacy implementation
            if (groupsPanel == null)
            {
                Debug.Log("GroupsPanelController: Using standard VisualElement container");
                // Attempt to find the GroupsPanel as a child element
                groupsPanel = root.Q<GroupsPanel>();
                
                if (groupsPanel == null)
                {
                    Debug.LogWarning("GroupsPanelController: Could not find GroupsPanel custom control. Group details functionality will be limited.");
                }
            }
            
            if (groupsPanel != null)
            {
                Debug.Log("GroupsPanelController: Using GroupsPanel custom control");
                groupsPanel.Initialize();
            }
            
            InitializeElements();
            SetupEventHandlers();
        }

        private void InitializeElements()
        {
            // Tab elements
            myGroupsTabButton = root.Q<Button>("my-groups-tab-button");
            discoverTabButton = root.Q<Button>("discover-tab-button");
            createTabButton = root.Q<Button>("create-tab-button");
            myGroupsPanel = root.Q<VisualElement>("my-groups-panel");
            discoverGroupsPanel = root.Q<VisualElement>("discover-groups-panel");
            createGroupPanel = root.Q<VisualElement>("create-group-panel");

            // My Groups Elements
            myGroupsCount = root.Q<Label>("my-groups-count");
            myGroupsRefreshButton = root.Q<Button>("my-groups-refresh-button");
            myGroupsListContainer = root.Q<VisualElement>("my-groups-list-container");
            myGroupsEmptyState = root.Q<VisualElement>("my-groups-empty-state");

            // Discover Groups Elements
            discoverSearchInput = root.Q<TextField>("discover-search-input");
            discoverSearchButton = root.Q<Button>("discover-search-button");
            discoverResultsLabel = root.Q<Label>("discover-results-label");
            discoverResultsCount = root.Q<Label>("discover-results-count");
            discoverGroupsListContainer = root.Q<VisualElement>("discover-groups-list-container");
            discoverEmptyState = root.Q<VisualElement>("discover-empty-state");
            discoverPageInfo = root.Q<Label>("discover-page-info");
            discoverPrevPageButton = root.Q<Button>("discover-prev-page");
            discoverNextPageButton = root.Q<Button>("discover-next-page");

            // Create Group Elements
            createGroupNameField = root.Q<TextField>("create-group-name");
            createGroupDescriptionField = root.Q<TextField>("create-group-description");
            createGroupPublicToggle = root.Q<Toggle>("create-group-public");
            createGroupPrivateToggle = root.Q<Toggle>("create-group-private");
            createGroupAttributesContainer = root.Q<VisualElement>("create-group-attributes");
            addGroupAttributeButton = root.Q<Button>("add-group-attribute-button");
            createGroupCancelButton = root.Q<Button>("create-group-cancel");
            createGroupSubmitButton = root.Q<Button>("create-group-submit");

            // Loading and Status
            loadingIndicator = root.Q<VisualElement>("groups-loading");
            statusLabel = root.Q<Label>("groups-status");
        }

        private void SetupEventHandlers()
        {
            // Tab navigation
            myGroupsTabButton.clicked += () => SwitchTab("my-groups");
            discoverTabButton.clicked += () => SwitchTab("discover");
            createTabButton.clicked += () => SwitchTab("create");
            
            // My Groups functionality
            if (myGroupsRefreshButton != null)
            {
                myGroupsRefreshButton.clicked += RefreshMyGroups;
            }

            // Discover functionality
            if (discoverSearchButton != null)
            {
                discoverSearchButton.clicked += HandleSearchGroups;
            }
            
            if (discoverSearchInput != null)
            {
                discoverSearchInput.RegisterCallback<KeyDownEvent>(evt => {
                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                        HandleSearchGroups();
                });
            }

            // Pagination
            if (discoverPrevPageButton != null)
            {
                discoverPrevPageButton.clicked += () => ChangePage(-1);
            }
            
            if (discoverNextPageButton != null)
            {
                discoverNextPageButton.clicked += () => ChangePage(1);
            }

            // Create Group functionality
            if (createGroupPublicToggle != null && createGroupPrivateToggle != null)
            {
                createGroupPublicToggle.RegisterValueChangedCallback(evt => {
                    if (evt.newValue) createGroupPrivateToggle.value = false;
                });
                
                createGroupPrivateToggle.RegisterValueChangedCallback(evt => {
                    if (evt.newValue) createGroupPublicToggle.value = false;
                });
            }

            if (addGroupAttributeButton != null)
            {
                addGroupAttributeButton.clicked += AddAttributeField;
            }

            if (createGroupCancelButton != null)
            {
                createGroupCancelButton.clicked += ClearCreateGroupForm;
            }

            if (createGroupSubmitButton != null)
            {
                createGroupSubmitButton.clicked += HandleCreateGroup;
            }
        }

        private void SwitchTab(string tabName)
        {
            // Reset all tabs
            myGroupsTabButton.RemoveFromClassList("active-tab");
            discoverTabButton.RemoveFromClassList("active-tab");
            createTabButton.RemoveFromClassList("active-tab");
            
            myGroupsPanel.style.display = DisplayStyle.None;
            discoverGroupsPanel.style.display = DisplayStyle.None;
            createGroupPanel.style.display = DisplayStyle.None;
            
            // Set active tab
            switch (tabName)
            {
                case "my-groups":
                    myGroupsTabButton.AddToClassList("active-tab");
                    myGroupsPanel.style.display = DisplayStyle.Flex;
                    RefreshMyGroups();
                    break;
                case "discover":
                    discoverTabButton.AddToClassList("active-tab");
                    discoverGroupsPanel.style.display = DisplayStyle.Flex;
                    if (allGroups.Count == 0) // Only load if not already loaded
                    {
                        RefreshAllGroups();
                    }
                    break;
                case "create":
                    createTabButton.AddToClassList("active-tab");
                    createGroupPanel.style.display = DisplayStyle.Flex;
                    break;
            }
        }

        public void SetCurrentUser(GameFuseUser user)
        {
            currentUser = user;
            if (currentUser != null)
            {
                RefreshMyGroups();
            }
        }

        #region My Groups

        private async void RefreshMyGroups()
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                // First get all groups
                var allGroupSummaries = await currentUser.FetchAllGroupsAsync();
                
                // Then filter to get detailed info for groups the user is a member of
                myGroups.Clear();
                foreach (var groupSummary in allGroupSummaries)
                {
                    try
                    {
                        var groupDetails = await currentUser.FetchGroupDetailsAsync(groupSummary.Id);
                        // Check if current user is a member or admin
                        bool isMember = groupDetails.Members?.Any(m => m.Id == currentUser.Id) ?? false;
                        bool isAdmin = groupDetails.Admins?.Any(a => a.Id == currentUser.Id) ?? false;
                        
                        if (isMember || isAdmin)
                        {
                            myGroups.Add(groupDetails);
                        }
                    }
                    catch
                    {
                        // Skip groups we can't access details for
                    }
                }
                
                DisplayMyGroups();
                ShowStatus("My groups refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load groups: {ex.Message}", true);
                OnError?.Invoke($"Failed to load groups: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void DisplayMyGroups()
        {
            // Clear existing items except empty state
            var children = myGroupsListContainer.Children().ToList();
            foreach (var child in children)
            {
                if (child != myGroupsEmptyState)
                {
                    myGroupsListContainer.Remove(child);
                }
            }
            
            // Update counter
            myGroupsCount.text = myGroups.Count.ToString();
            
            // Show/hide empty state
            if (myGroups.Count == 0)
            {
                myGroupsEmptyState.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                myGroupsEmptyState.style.display = DisplayStyle.None;
            }
            
            // Add group items
            foreach (var group in myGroups)
            {
                var groupItem = CreateGroupItem(group, true);
                myGroupsListContainer.Add(groupItem);
            }
        }

        #endregion

        #region Discover Groups

        private async void RefreshAllGroups()
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                var groupsList = await currentUser.FetchAllGroupsAsync();
                allGroups = groupsList.ToList();
                ApplySearchFilter(discoverSearchInput?.value ?? "");
                ShowStatus("Groups list refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load groups: {ex.Message}", true);
                OnError?.Invoke($"Failed to load groups: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void HandleSearchGroups()
        {
            ApplySearchFilter(discoverSearchInput?.value ?? "");
            DisplayDiscoverGroups();
        }

        private void ApplySearchFilter(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredGroups = new List<GroupSummary>(allGroups);
            }
            else
            {
                var lowerSearch = searchText.ToLower();
                filteredGroups = allGroups.Where(g => 
                    g.Name.ToLower().Contains(lowerSearch)
                ).ToList();
            }
            
            // Reset to first page when filter changes
            currentPage = 0;
            
            // Update results label
            if (discoverResultsLabel != null)
            {
                discoverResultsLabel.text = string.IsNullOrWhiteSpace(searchText) 
                    ? "Available Groups" 
                    : $"Search Results for \"{searchText}\"";
            }
        }

        private void DisplayDiscoverGroups()
        {
            // Clear existing items except empty state
            var children = discoverGroupsListContainer.Children().ToList();
            foreach (var child in children)
            {
                if (child != discoverEmptyState)
                {
                    discoverGroupsListContainer.Remove(child);
                }
            }
            
            // Update counter
            discoverResultsCount.text = $"({filteredGroups.Count})";
            
            // Show/hide empty state
            if (filteredGroups.Count == 0)
            {
                discoverEmptyState.style.display = DisplayStyle.Flex;
                HidePagination();
                return;
            }
            else
            {
                discoverEmptyState.style.display = DisplayStyle.None;
            }
            
            // Calculate pagination
            totalPages = (int)Math.Ceiling((double)filteredGroups.Count / groupsPerPage);
            currentPage = Math.Min(currentPage, totalPages - 1);
            currentPage = Math.Max(currentPage, 0);
            
            // Get groups for current page
            int startIndex = currentPage * groupsPerPage;
            int endIndex = Math.Min(startIndex + groupsPerPage, filteredGroups.Count);
            
            // Add group items for current page
            for (int i = startIndex; i < endIndex; i++)
            {
                var groupItem = CreateGroupSummaryItem(filteredGroups[i], false);
                discoverGroupsListContainer.Add(groupItem);
            }
            
            // Update pagination controls
            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            var paginationContainer = root.Q<VisualElement>("discover-pagination");
            
            if (totalPages <= 1)
            {
                HidePagination();
                return;
            }
            
            // Show pagination
            if (paginationContainer != null)
            {
                paginationContainer.style.display = DisplayStyle.Flex;
            }
            
            // Update page info
            if (discoverPageInfo != null)
            {
                discoverPageInfo.text = $"Page {currentPage + 1} of {totalPages}";
            }
            
            // Update button states
            if (discoverPrevPageButton != null)
            {
                discoverPrevPageButton.SetEnabled(currentPage > 0);
            }
            
            if (discoverNextPageButton != null)
            {
                discoverNextPageButton.SetEnabled(currentPage < totalPages - 1);
            }
        }

        private void HidePagination()
        {
            var paginationContainer = root.Q<VisualElement>("discover-pagination");
            if (paginationContainer != null)
            {
                paginationContainer.style.display = DisplayStyle.None;
            }
        }

        private void ChangePage(int direction)
        {
            currentPage += direction;
            currentPage = Math.Max(0, Math.Min(currentPage, totalPages - 1));
            DisplayDiscoverGroups();
        }

        #endregion

        #region Create Group

        private void AddAttributeField()
        {
            var attributeRow = new VisualElement();
            attributeRow.AddToClassList("attribute-row");
            attributeRow.style.flexDirection = FlexDirection.Row;
            attributeRow.style.marginBottom = 5;
            
            var keyField = new TextField();
            keyField.AddToClassList("attribute-key-field");
            keyField.textEdition.placeholder = "Key";
            keyField.style.flexGrow = 1;
            keyField.style.marginRight = 5;
            attributeRow.Add(keyField);
            
            var valueField = new TextField();
            valueField.AddToClassList("attribute-value-field");
            valueField.textEdition.placeholder = "Value";
            valueField.style.flexGrow = 1;
            valueField.style.marginRight = 5;
            attributeRow.Add(valueField);
            
            var removeButton = new Button(() => {
                createGroupAttributesContainer.Remove(attributeRow);
            });
            removeButton.text = "Remove";
            removeButton.AddToClassList("remove-attribute-button");
            removeButton.AddToClassList("danger");
            attributeRow.Add(removeButton);
            
            // Insert before the add button
            var addButton = createGroupAttributesContainer.Q<Button>("add-group-attribute-button");
            createGroupAttributesContainer.Insert(createGroupAttributesContainer.IndexOf(addButton), attributeRow);
        }

        private async void HandleCreateGroup()
        {
            if (currentUser == null) return;
            
            var groupName = createGroupNameField.value?.Trim();
            var groupType = createGroupDescriptionField.value?.Trim(); // Using description field for group type
            var canAutoJoin = createGroupPublicToggle.value;
            var isInviteOnly = createGroupPrivateToggle.value;
            
            if (string.IsNullOrEmpty(groupName))
            {
                ShowStatus("Please enter a group name", true);
                return;
            }
            
            SetLoading(true);
            
            try
            {
                // Collect attributes from the form
                pendingGroupAttributes.Clear();
                var attributeRows = createGroupAttributesContainer.Query<VisualElement>(className: "attribute-row").ToList();
                foreach (var row in attributeRows)
                {
                    var keyField = row.Q<TextField>(className: "attribute-key-field");
                    var valueField = row.Q<TextField>(className: "attribute-value-field");
                    
                    if (!string.IsNullOrEmpty(keyField?.value) && !string.IsNullOrEmpty(valueField?.value))
                    {
                        pendingGroupAttributes.Add(new GroupAttributePayloadItem
                        {
                            Key = keyField.value.Trim(),
                            Value = valueField.value.Trim(),
                            OthersCanEdit = false
                        });
                    }
                }
                
                // Create the group payload
                var payload = new CreateGroupPayload
                {
                    Name = groupName,
                    GroupType = string.IsNullOrEmpty(groupType) ? "general" : groupType,
                    MaxGroupSize = 100, // Default max size
                    CanAutoJoin = canAutoJoin,
                    IsInviteOnly = isInviteOnly,
                    Searchable = true,
                    AdminsOnlyCanCreateAttributes = false
                };
                
                // Create the group
                var newGroup = await currentUser.CreateGroupAsync(payload);
                
                // Add attributes if any
                if (pendingGroupAttributes.Count > 0)
                {
                    try
                    {
                        await currentUser.CreateGroupAttributesAsync(newGroup.Id, pendingGroupAttributes);
                    }
                    catch (Exception attrEx)
                    {
                        Debug.LogWarning($"Failed to add group attributes: {attrEx.Message}");
                        // Continue anyway - group was created successfully
                    }
                }
                
                ShowStatus($"Group '{groupName}' created successfully!", false);
                OnSuccess?.Invoke($"Group '{groupName}' created successfully!");
                
                // Clear the form and switch to My Groups tab
                ClearCreateGroupForm();
                SwitchTab("my-groups");
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to create group: {ex.Message}", true);
                OnError?.Invoke($"Failed to create group: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ClearCreateGroupForm()
        {
            createGroupNameField.value = "";
            createGroupDescriptionField.value = "";
            createGroupPublicToggle.value = true;
            createGroupPrivateToggle.value = false;
            
            // Clear all attribute rows
            var attributeRows = createGroupAttributesContainer.Query<VisualElement>(className: "attribute-row").ToList();
            foreach (var row in attributeRows)
            {
                createGroupAttributesContainer.Remove(row);
            }
            
            pendingGroupAttributes.Clear();
        }

        #endregion

        #region Group Item Creation

        private VisualElement CreateGroupItem(Group group, bool isMyGroup)
        {
            var item = new VisualElement();
            item.AddToClassList("group-item");
            
            // Group info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("group-info");
            
            var icon = new VisualElement();
            icon.AddToClassList("group-icon");
            infoSection.Add(icon);
            
            var details = new VisualElement();
            details.AddToClassList("group-details");
            
            var nameLabel = new Label(group.Name);
            nameLabel.AddToClassList("group-name");
            details.Add(nameLabel);
            
            var metaInfo = new VisualElement();
            metaInfo.style.flexDirection = FlexDirection.Row;
            
            var privacyLabel = new Label(group.CanAutoJoin ? "Public" : "Private");
            privacyLabel.AddToClassList("group-privacy");
            metaInfo.Add(privacyLabel);
            
            var membersLabel = new Label($"Members: {group.MemberCount}");
            membersLabel.AddToClassList("group-members");
            membersLabel.style.marginLeft = 15;
            metaInfo.Add(membersLabel);
            
            details.Add(metaInfo);
            
            infoSection.Add(details);
            item.Add(infoSection);
            
            // Actions section
            var actionsSection = new VisualElement();
            actionsSection.AddToClassList("group-actions");
            actionsSection.style.flexDirection = FlexDirection.Row;
            
            var viewDetailsButton = new Button(() => HandleViewGroupDetails(group.Id, isMyGroup));
            viewDetailsButton.text = "View Details";
            viewDetailsButton.AddToClassList("group-action-button");
            viewDetailsButton.AddToClassList("primary");
            actionsSection.Add(viewDetailsButton);
            
            if (isMyGroup)
            {
                var leaveButton = new Button(() => HandleLeaveGroup(group.Id));
                leaveButton.text = "Leave Group";
                leaveButton.AddToClassList("group-action-button");
                leaveButton.AddToClassList("danger");
                leaveButton.style.marginLeft = 5;
                actionsSection.Add(leaveButton);
            }
            else
            {
                var joinButton = new Button(() => HandleJoinGroup(group.Id, group.CanAutoJoin));
                joinButton.text = group.CanAutoJoin ? "Join Group" : "Request to Join";
                joinButton.AddToClassList("group-action-button");
                joinButton.AddToClassList("success");
                joinButton.style.marginLeft = 5;
                actionsSection.Add(joinButton);
            }
            
            item.Add(actionsSection);
            
            return item;
        }
        
        private VisualElement CreateGroupSummaryItem(GroupSummary group, bool isMyGroup)
        {
            var item = new VisualElement();
            item.AddToClassList("group-item");
            
            // Group info section
            var infoSection = new VisualElement();
            infoSection.AddToClassList("group-info");
            
            var icon = new VisualElement();
            icon.AddToClassList("group-icon");
            infoSection.Add(icon);
            
            var details = new VisualElement();
            details.AddToClassList("group-details");
            
            var nameLabel = new Label(group.Name);
            nameLabel.AddToClassList("group-name");
            details.Add(nameLabel);
            
            var metaInfo = new VisualElement();
            metaInfo.style.flexDirection = FlexDirection.Row;
            
            var privacyLabel = new Label(group.CanAutoJoin ? "Public" : "Private");
            privacyLabel.AddToClassList("group-privacy");
            metaInfo.Add(privacyLabel);
            
            var membersLabel = new Label($"Members: {group.MemberCount}");
            membersLabel.AddToClassList("group-members");
            membersLabel.style.marginLeft = 15;
            metaInfo.Add(membersLabel);
            
            details.Add(metaInfo);
            
            infoSection.Add(details);
            item.Add(infoSection);
            
            // Actions section
            var actionsSection = new VisualElement();
            actionsSection.AddToClassList("group-actions");
            actionsSection.style.flexDirection = FlexDirection.Row;
            
            var viewDetailsButton = new Button(() => HandleViewGroupDetails(group.Id, isMyGroup));
            viewDetailsButton.text = "View Details";
            viewDetailsButton.AddToClassList("group-action-button");
            viewDetailsButton.AddToClassList("primary");
            actionsSection.Add(viewDetailsButton);
            
            if (!isMyGroup)
            {
                var joinButton = new Button(() => HandleJoinGroup(group.Id, group.CanAutoJoin));
                joinButton.text = group.CanAutoJoin ? "Join Group" : "Request to Join";
                joinButton.AddToClassList("group-action-button");
                joinButton.AddToClassList("success");
                joinButton.style.marginLeft = 5;
                actionsSection.Add(joinButton);
            }
            
            item.Add(actionsSection);
            
            return item;
        }

        #endregion

        #region Group Actions

        private async void HandleViewGroupDetails(int groupId, bool isMyGroup)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            ShowStatus($"Loading group details...", false);
            
            try
            {
                // Get group details
                var groupDetails = await currentUser.FetchGroupDetailsAsync(groupId);
                
                // Get attributes
                var attributes = await GetGroupAttributesAsync(groupId);
                
                // The members are already in the group details
                var members = groupDetails.Members ?? new List<Friend>();
                
                // Check if current user is admin
                bool isAdmin = groupDetails.Admins?.Any(a => a.Id == currentUser.Id) ?? false;
                
                // Show the details popup
                if (groupsPanel != null)
                {
                    groupsPanel.ShowGroupDetailsPopup(groupDetails, attributes, members, isAdmin);
                    ShowStatus($"Viewing details for {groupDetails.Name}", false);
                }
                else
                {
                    ShowStatus($"Cannot display group details", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load group details: {ex.Message}", true);
                OnError?.Invoke($"Failed to load group details: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async Task<List<GroupAttributeResponseItem>> GetGroupAttributesAsync(int groupId)
        {
            if (groupAttributesCache.ContainsKey(groupId))
            {
                return groupAttributesCache[groupId];
            }
            
            try
            {
                var attributes = await currentUser.FetchGroupAttributesAsync(groupId);
                var attributesList = attributes.ToList();
                groupAttributesCache[groupId] = attributesList;
                return attributesList;
            }
            catch
            {
                return new List<GroupAttributeResponseItem>();
            }
        }


        private async void HandleJoinGroup(int groupId, bool canAutoJoin)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                // Send connection request (for both auto-join and request-to-join groups)
                var response = await currentUser.SendGroupConnectionRequestAsync(groupId);
                
                if (canAutoJoin)
                {
                    ShowStatus($"Successfully joined the group!", false);
                    OnSuccess?.Invoke($"Successfully joined the group!");
                }
                else
                {
                    ShowStatus($"Join request sent to the group", false);
                    OnSuccess?.Invoke($"Join request sent to the group");
                }
                
                // Refresh lists
                await Task.Delay(500); // Small delay to ensure server state is updated
                RefreshMyGroups();
                RefreshAllGroups();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to join group: {ex.Message}", true);
                OnError?.Invoke($"Failed to join group: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleLeaveGroup(int groupId)
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                // TODO: There's no direct LeaveGroup method in the API
                // The user would need to be removed by an admin or remove themselves
                // For now, we'll show a message that this needs to be implemented
                ShowStatus("Leave group functionality needs to be implemented through group admin", true);
                
                // In a real implementation, you might need to:
                // 1. Check if user is admin and can remove themselves
                // 2. Or request an admin to remove them
                // 3. Or use a custom API endpoint if available
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to leave group: {ex.Message}", true);
                OnError?.Invoke($"Failed to leave group: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        #endregion

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
            if (discoverSearchInput != null)
                discoverSearchInput.value = "";
            
            ClearCreateGroupForm();

            // Clear data collections
            myGroups.Clear();
            allGroups.Clear();
            filteredGroups.Clear();
            groupAttributesCache.Clear();
            groupMembersCache.Clear();
            pendingGroupAttributes.Clear();

            // Reset pagination
            currentPage = 0;
            totalPages = 0;

            // Reset UI displays
            DisplayMyGroups();
            DisplayDiscoverGroups();

            // Reset to my groups tab
            SwitchTab("my-groups");

            // Hide status
            if (statusLabel != null)
                statusLabel.style.display = DisplayStyle.None;

            SetLoading(false);
        }
    }
}