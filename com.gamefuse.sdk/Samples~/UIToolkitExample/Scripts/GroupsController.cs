using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using GameFuseCSharp;
using GroupResponse = GameFuseCSharp.GroupResponse;
using GroupConnectionResponse = GameFuseCSharp.GroupConnectionResponse;

namespace GameFuse.UIToolkit
{
    public class GroupsController : BaseGameFuseUIController
    {
        private TextField groupNameField;
        private TextField groupTypeField;
        private TextField groupMaxSizeField;
        private Toggle groupAutoJoinToggle;
        private Toggle groupInviteOnlyToggle;
        private Toggle groupSearchableToggle;
        private TextField groupIdField;
        private TextField groupUserIdField;
        private TextField groupConnectionIdField;
        private TextField groupAttributeGroupIdField;
        private TextField groupAttributeKeyField;
        private TextField groupAttributeValueField;
        private TextField groupMultipleAttributesField;
        private Button createGroupButton;
        private Button getAllGroupsButton;
        private Button getGroupDetailsButton;
        private Button sendGroupConnectionRequestButton;
        private Button acceptGroupConnectionRequestButton;
        private Button declineGroupConnectionRequestButton;
        private Button getGroupAttributesButton;
        private Button addGroupAttributeButton;
        private Button addGroupMultipleAttributesButton;
        private Button modifyGroupAttributeButton;
        private ScrollView groupsResultsScrollView;

        protected override void InitializeUI()
        {
            var content = rootElement.Q<VisualElement>("content-groups");
            
            // Group Creation fields
            groupNameField = content.Q<TextField>("group-name");
            groupTypeField = content.Q<TextField>("group-type");
            groupMaxSizeField = content.Q<TextField>("group-max-size");
            groupAutoJoinToggle = content.Q<Toggle>("group-auto-join-toggle");
            groupInviteOnlyToggle = content.Q<Toggle>("group-invite-only-toggle");
            groupSearchableToggle = content.Q<Toggle>("group-searchable-toggle");
            
            // Group Management fields
            groupIdField = content.Q<TextField>("group-id");
            groupUserIdField = content.Q<TextField>("group-user-id");
            groupConnectionIdField = content.Q<TextField>("group-connection-id");
            
            // Group Attributes fields
            groupAttributeGroupIdField = content.Q<TextField>("group-attribute-group-id");
            groupAttributeKeyField = content.Q<TextField>("group-attribute-key");
            groupAttributeValueField = content.Q<TextField>("group-attribute-value");
            groupMultipleAttributesField = content.Q<TextField>("group-multiple-attributes");
            
            // Buttons
            createGroupButton = content.Q<Button>("create-group-button");
            getAllGroupsButton = content.Q<Button>("get-all-groups-button");
            getGroupDetailsButton = content.Q<Button>("get-group-details-button");
            sendGroupConnectionRequestButton = content.Q<Button>("send-group-connection-request-button");
            acceptGroupConnectionRequestButton = content.Q<Button>("accept-group-connection-request-button");
            declineGroupConnectionRequestButton = content.Q<Button>("decline-group-connection-request-button");
            getGroupAttributesButton = content.Q<Button>("get-group-attributes-button");
            addGroupAttributeButton = content.Q<Button>("add-group-attribute-button");
            addGroupMultipleAttributesButton = content.Q<Button>("add-group-multiple-attributes-button");
            modifyGroupAttributeButton = content.Q<Button>("modify-group-attribute-button");
            
            // Results view
            groupsResultsScrollView = content.Q<ScrollView>("groups-results-scroll");
        }

        protected override void RegisterCallbacks()
        {
            createGroupButton.RegisterCallback<ClickEvent>(async evt => await OnCreateGroupClicked());
            getAllGroupsButton.RegisterCallback<ClickEvent>(async evt => await OnGetAllGroupsClicked());
            getGroupDetailsButton.RegisterCallback<ClickEvent>(async evt => await OnGetGroupDetailsClicked());
            sendGroupConnectionRequestButton.RegisterCallback<ClickEvent>(async evt => await OnSendGroupConnectionRequestClicked());
            acceptGroupConnectionRequestButton.RegisterCallback<ClickEvent>(async evt => await OnAcceptGroupConnectionRequestClicked());
            declineGroupConnectionRequestButton.RegisterCallback<ClickEvent>(async evt => await OnDeclineGroupConnectionRequestClicked());
            getGroupAttributesButton.RegisterCallback<ClickEvent>(async evt => await OnGetGroupAttributesClicked());
            addGroupAttributeButton.RegisterCallback<ClickEvent>(async evt => await OnAddGroupAttributeClicked());
            addGroupMultipleAttributesButton.RegisterCallback<ClickEvent>(async evt => await OnAddGroupMultipleAttributesClicked());
            modifyGroupAttributeButton.RegisterCallback<ClickEvent>(async evt => await OnModifyGroupAttributeClicked());
        }

        protected override void UnregisterCallbacks()
        {
            createGroupButton.UnregisterCallback<ClickEvent>(async evt => await OnCreateGroupClicked());
            getAllGroupsButton.UnregisterCallback<ClickEvent>(async evt => await OnGetAllGroupsClicked());
            getGroupDetailsButton.UnregisterCallback<ClickEvent>(async evt => await OnGetGroupDetailsClicked());
            sendGroupConnectionRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnSendGroupConnectionRequestClicked());
            acceptGroupConnectionRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnAcceptGroupConnectionRequestClicked());
            declineGroupConnectionRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnDeclineGroupConnectionRequestClicked());
            getGroupAttributesButton.UnregisterCallback<ClickEvent>(async evt => await OnGetGroupAttributesClicked());
            addGroupAttributeButton.UnregisterCallback<ClickEvent>(async evt => await OnAddGroupAttributeClicked());
            addGroupMultipleAttributesButton.UnregisterCallback<ClickEvent>(async evt => await OnAddGroupMultipleAttributesClicked());
            modifyGroupAttributeButton.UnregisterCallback<ClickEvent>(async evt => await OnModifyGroupAttributeClicked());
        }

        private async Task OnCreateGroupClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create groups", LogType.Error);
                return;
            }
            
            string name = groupNameField.value;
            string type = groupTypeField.value;
            
            if (string.IsNullOrEmpty(name))
            {
                LogMessage("Group name is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(type))
            {
                LogMessage("Group type is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create group request
                var request = new CreateGroupRequest
                {
                    Name = name,
                    GroupType = type,
                    MaxGroupSize = int.Parse(string.IsNullOrEmpty(groupMaxSizeField.value) ? "10" : groupMaxSizeField.value),
                    CanAutoJoin = groupAutoJoinToggle.value,
                    IsInviteOnly = groupInviteOnlyToggle.value,
                    Searchable = groupSearchableToggle.value
                };
                
                // Create the group
                var response = await GameFuseUser.CurrentUser.CreateGroupAsync(request);
                
                // Display the created group
                DisplayGroup(response);
                
                LogMessage($"Group '{response.Name}' created successfully", LogType.Success);
            });
        }

        private async Task OnGetAllGroupsClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Get all groups
                var response = await GameFuseUser.CurrentUser.GetAllGroupsAsync();
                
                // Display all groups
                DisplayGroups(response);
                
                LogMessage($"Retrieved {response.Length} groups", LogType.Success);
            });
        }
        
        private async Task OnGetGroupDetailsClicked()
        {
            string groupId = groupIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get group details
                var response = await GameFuseUser.CurrentUser.GetGroupDetailsAsync(int.Parse(groupId));
                
                // Display the group
                DisplayGroup(response);
                
                LogMessage($"Group details retrieved successfully", LogType.Success);
            });
        }

        private async Task OnSendGroupConnectionRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to send group connection requests", LogType.Error);
                return;
            }
            
            string groupId = groupIdField.value;
            string userId = groupUserIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(userId))
            {
                LogMessage("User ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create group connection request
                var request = new GroupConnectionRequest
                {
                    GroupId = int.Parse(groupId),
                    UserId = int.Parse(userId)
                };
                
                // Send the connection request
                var response = await GameFuseUser.CurrentUser.SendGroupConnectionRequestAsync(request);
                
                // Display the connection response
                DisplayGroupConnectionResponse(response);
                
                LogMessage($"Group connection request sent successfully", LogType.Success);
            });
        }

        private async Task OnAcceptGroupConnectionRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to accept group connection requests", LogType.Error);
                return;
            }
            
            string connectionId = groupConnectionIdField.value;
            
            if (string.IsNullOrEmpty(connectionId))
            {
                LogMessage("Connection ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Accept the connection request
                var response = await GameFuseUser.CurrentUser.AcceptGroupConnectionRequestAsync(int.Parse(connectionId));
                
                // Clear the current display and show a message
                ClearScrollView(groupsResultsScrollView);
                groupsResultsScrollView.Add(new Label($"Connection request accepted. Status: {response.Status}"));
                
                LogMessage($"Group connection request accepted successfully", LogType.Success);
            });
        }

        private async Task OnDeclineGroupConnectionRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to decline group connection requests", LogType.Error);
                return;
            }
            
            string connectionId = groupConnectionIdField.value;
            
            if (string.IsNullOrEmpty(connectionId))
            {
                LogMessage("Connection ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Decline the connection request
                var response = await GameFuseUser.CurrentUser.DeclineGroupConnectionRequestAsync(int.Parse(connectionId));
                
                // Clear the current display and show a message
                ClearScrollView(groupsResultsScrollView);
                groupsResultsScrollView.Add(new Label($"Connection request declined. Status: {response.Status}"));
                
                LogMessage($"Group connection request declined successfully", LogType.Success);
            });
        }

        private async Task OnGetGroupAttributesClicked()
        {
            string groupId = groupAttributeGroupIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get group attributes
                var response = await GameFuseUser.CurrentUser.GetGroupAttributesAsync(int.Parse(groupId));
                
                // Display group attributes
                DisplayGroupAttributes(response);
                
                int attrCount = response.Attributes != null ? response.Attributes.Length : 0;
                LogMessage($"Retrieved {attrCount} group attributes", LogType.Success);
            });
        }

        private async Task OnAddGroupAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to add group attributes", LogType.Error);
                return;
            }
            
            string groupId = groupAttributeGroupIdField.value;
            string key = groupAttributeKeyField.value;
            string value = groupAttributeValueField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create attribute request
                var request = new GroupAttributeRequest
                {
                    Key = key,
                    Value = value
                };
                
                // Add the attribute
                await GameFuseUser.CurrentUser.AddGroupAttributeAsync(int.Parse(groupId), request);
                
                // Refresh attributes display
                var response = await GameFuseUser.CurrentUser.GetGroupAttributesAsync(int.Parse(groupId));
                DisplayGroupAttributes(response);
                
                LogMessage($"Group attribute '{key}' added successfully", LogType.Success);
            });
        }

        private async Task OnAddGroupMultipleAttributesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to add group attributes", LogType.Error);
                return;
            }
            
            string groupId = groupAttributeGroupIdField.value;
            string attributesJson = groupMultipleAttributesField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(attributesJson))
            {
                LogMessage("Attributes JSON is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                try
                {
                    // Parse attributes from JSON
                    var attributes = new List<GroupAttributeRequest>();
                    var jsonObj = JObject.Parse(attributesJson);
                    
                    foreach (var property in jsonObj.Properties())
                    {
                        attributes.Add(new GroupAttributeRequest
                        {
                            Key = property.Name,
                            Value = property.Value.ToString()
                        });
                    }
                    
                    // Create request
                    var request = new GroupAttributesRequest
                    {
                        Attributes = attributes.ToArray()
                    };
                    
                    // Add multiple attributes
                    await GameFuseUser.CurrentUser.AddGroupAttributesAsync(int.Parse(groupId), request);
                    
                    // Refresh attributes display
                    var response = await GameFuseUser.CurrentUser.GetGroupAttributesAsync(int.Parse(groupId));
                    DisplayGroupAttributes(response);
                    
                    LogMessage($"Added {attributes.Count} group attributes successfully", LogType.Success);
                }
                catch (System.Exception ex)
                {
                    LogMessage($"Error parsing attributes JSON: {ex.Message}", LogType.Error);
                }
            });
        }

        private async Task OnModifyGroupAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to modify group attributes", LogType.Error);
                return;
            }
            
            string groupId = groupAttributeGroupIdField.value;
            string key = groupAttributeKeyField.value;
            string value = groupAttributeValueField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Modify the attribute
                await GameFuseUser.CurrentUser.ModifyGroupAttributeAsync(int.Parse(groupId), key, value);
                
                // Refresh attributes display
                var response = await GameFuseUser.CurrentUser.GetGroupAttributesAsync(int.Parse(groupId));
                DisplayGroupAttributes(response);
                
                LogMessage($"Group attribute '{key}' modified successfully", LogType.Success);
            });
        }

        private void DisplayGroup(GroupResponse group)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (group != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", group.Id.ToString() },
                    { "Name", group.Name },
                    { "Type", group.GroupType },
                    { "Owner ID", group.OwnerId.ToString() },
                    { "Max Size", group.MaxGroupSize.ToString() },
                    { "Member Count", group.MemberCount.ToString() },
                    { "Auto Join", group.CanAutoJoin.ToString() },
                    { "Invite Only", group.IsInviteOnly.ToString() },
                    { "Searchable", group.Searchable.ToString() },
                    { "Created", group.Created }
                };
                
                var groupItem = CreateListItem($"Group: {group.Name}", properties);
                groupsResultsScrollView.Add(groupItem);
                
                // Add members section if there are members
                if (group.Members != null && group.Members.Length > 0)
                {
                    groupsResultsScrollView.Add(new Label("Members:") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var member in group.Members)
                    {
                        var memberProperties = new Dictionary<string, string>
                        {
                            { "ID", member.Id.ToString() },
                            { "Username", member.Username }
                        };
                        
                        var memberItem = CreateListItem(member.Username, memberProperties);
                        groupsResultsScrollView.Add(memberItem);
                    }
                }
                
                // Add admins section if there are admins
                if (group.Admins != null && group.Admins.Length > 0)
                {
                    groupsResultsScrollView.Add(new Label("Admins:") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var admin in group.Admins)
                    {
                        var adminProperties = new Dictionary<string, string>
                        {
                            { "ID", admin.Id.ToString() },
                            { "Username", admin.Username }
                        };
                        
                        var adminItem = CreateListItem(admin.Username, adminProperties);
                        groupsResultsScrollView.Add(adminItem);
                    }
                }
            }
            else
            {
                groupsResultsScrollView.Add(new Label("No group data available"));
            }
        }

        private void DisplayGroups(GroupResponse[] groups)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (groups != null && groups.Length > 0)
            {
                for (int i = 0; i < groups.Length; i++)
                {
                    var group = groups[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", group.Id.ToString() },
                        { "Type", group.GroupType },
                        { "Current/Max Size", $"{group.MemberCount}/{group.MaxGroupSize}" },
                        { "Auto Join", group.CanAutoJoin.ToString() },
                        { "Invite Only", group.IsInviteOnly.ToString() },
                        { "Searchable", group.Searchable.ToString() },
                        { "Created", group.Created }
                    };
                    
                    var groupItem = CreateListItem($"Group: {group.Name}", properties);
                    groupsResultsScrollView.Add(groupItem);
                }
            }
            else
            {
                groupsResultsScrollView.Add(new Label("No groups available"));
            }
        }

        private void DisplayGroupAttributes(GroupAttributesResponse response)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (response != null && response.Attributes != null && response.Attributes.Length > 0)
            {
                // Use first attribute's GroupId since response doesn't have a direct GroupId property
                int groupId = response.Attributes.Length > 0 ? response.Attributes[0].GroupId : 0;
                groupsResultsScrollView.Add(new Label($"Group ID: {groupId}") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                foreach (var attribute in response.Attributes)
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "Value", attribute.Value }
                    };
                    
                    var attributeItem = CreateListItem(attribute.Key, properties);
                    groupsResultsScrollView.Add(attributeItem);
                }
            }
            else
            {
                groupsResultsScrollView.Add(new Label("No group attributes available"));
            }
        }

        private void DisplayGroupConnectionResponse(GroupConnectionResponse response)
        {
            // Clear current display
            ClearScrollView(groupsResultsScrollView);
            
            if (response != null)
            {
                var properties = new Dictionary<string, string>
                {
                    { "ID", response.Id.ToString() },
                    { "Group ID", response.GroupId.ToString() },
                    { "User ID", response.User.Id.ToString() },
                    { "Username", response.User.Username },
                    { "Status", response.Status },
                    { "Created", response.Created }
                };
                
                var connectionItem = CreateListItem("Group Connection", properties);
                groupsResultsScrollView.Add(connectionItem);
            }
            else
            {
                groupsResultsScrollView.Add(new Label("No connection response available"));
            }
        }
    }
}