using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using GameFuseCSharp;

namespace GameFuse.UIToolkit
{
    public class ChatsController : BaseGameFuseUIController
    {
        // Chat UI elements
        private TextField chatUsernamesField;
        private TextField chatGroupIdField;
        private TextField chatIdField;
        private TextField chatPageField;
        private TextField messageChatIdField;
        private TextField messageTextField;
        private TextField markReadMessageIdField;
        private Button getMyChatsButton;
        private Button createDirectChatButton;
        private Button createGroupChatButton;
        private Button getMessagesButton;
        private Button sendMessageButton;
        private Button markMessageReadButton;
        private ScrollView chatResultsScrollView;

        protected override void InitializeUI()
        {
            var content = rootElement.Q<VisualElement>("content-chats");
            
            // Chat operation fields
            chatUsernamesField = content.Q<TextField>("chat-usernames");
            chatGroupIdField = content.Q<TextField>("chat-group-id");
            chatIdField = content.Q<TextField>("chat-id");
            chatPageField = content.Q<TextField>("chat-page");
            
            // Message fields
            messageChatIdField = content.Q<TextField>("message-chat-id");
            messageTextField = content.Q<TextField>("message-text");
            markReadMessageIdField = content.Q<TextField>("mark-read-message-id");
            
            // Buttons
            getMyChatsButton = content.Q<Button>("get-my-chats-button");
            createDirectChatButton = content.Q<Button>("create-direct-chat-button");
            createGroupChatButton = content.Q<Button>("create-group-chat-button");
            getMessagesButton = content.Q<Button>("get-chat-messages-button");
            sendMessageButton = content.Q<Button>("send-message-button");
            markMessageReadButton = content.Q<Button>("mark-message-read-button");
            
            // Results view
            chatResultsScrollView = content.Q<ScrollView>("chat-results-scroll");
        }

        protected override void RegisterCallbacks()
        {
            getMyChatsButton.RegisterCallback<ClickEvent>(async evt => await OnGetMyChatsClicked());
            createDirectChatButton.RegisterCallback<ClickEvent>(async evt => await OnCreateDirectChatClicked());
            createGroupChatButton.RegisterCallback<ClickEvent>(async evt => await OnCreateGroupChatClicked());
            getMessagesButton.RegisterCallback<ClickEvent>(async evt => await OnGetMessagesClicked());
            sendMessageButton.RegisterCallback<ClickEvent>(async evt => await OnSendMessageClicked());
            markMessageReadButton.RegisterCallback<ClickEvent>(async evt => await OnMarkMessageReadClicked());
        }

        protected override void UnregisterCallbacks()
        {
            getMyChatsButton.UnregisterCallback<ClickEvent>(async evt => await OnGetMyChatsClicked());
            createDirectChatButton.UnregisterCallback<ClickEvent>(async evt => await OnCreateDirectChatClicked());
            createGroupChatButton.UnregisterCallback<ClickEvent>(async evt => await OnCreateGroupChatClicked());
            getMessagesButton.UnregisterCallback<ClickEvent>(async evt => await OnGetMessagesClicked());
            sendMessageButton.UnregisterCallback<ClickEvent>(async evt => await OnSendMessageClicked());
            markMessageReadButton.UnregisterCallback<ClickEvent>(async evt => await OnMarkMessageReadClicked());
        }

        private async Task OnGetMyChatsClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view your chats", LogType.Error);
                return;
            }
            
            int page = 1;
            if (!string.IsNullOrEmpty(chatPageField.value) && 
                !int.TryParse(chatPageField.value, out page))
            {
                LogMessage("Page must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's chats
                var response = await GameFuseUser.CurrentUser.GetChatsAsync(page);
                
                // Display chats
                DisplayChats(response);
                
                LogMessage("Chats retrieved successfully", LogType.Success);
            });
        }
        
        private async Task OnCreateDirectChatClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create chats", LogType.Error);
                return;
            }
            
            string usernames = chatUsernamesField.value;
            
            if (string.IsNullOrEmpty(usernames))
            {
                LogMessage("Usernames are required for direct chat", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Parse comma-separated usernames
                string[] usernameArray = usernames.Split(',').Select(u => u.Trim()).ToArray();
                
                // Create message text
                string text = string.IsNullOrEmpty(messageTextField.value) ? "Hello!" : messageTextField.value;
                
                // Create direct chat with first username and text
                string username = usernameArray.Length > 0 ? usernameArray[0] : "";
                if (string.IsNullOrEmpty(username))
                {
                    LogMessage("At least one username is required for direct chat", LogType.Error);
                    return;
                }
                
                // Create direct chat
                var response = await GameFuseUser.CurrentUser.CreateDirectChatAsync(new[] { username }, text);
                
                // Display the chat
                DisplayChat(response);
                
                LogMessage("Direct chat created successfully", LogType.Success);
            });
        }

        private async Task OnCreateGroupChatClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to create chats", LogType.Error);
                return;
            }
            
            string groupId = chatGroupIdField.value;
            
            if (string.IsNullOrEmpty(groupId))
            {
                LogMessage("Group ID is required for group chat", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                if (!int.TryParse(groupId, out int groupIdInt))
                {
                    LogMessage("Group ID must be a valid integer", LogType.Error);
                    return;
                }
                
                // Create message text
                string text = string.IsNullOrEmpty(messageTextField.value) ? "Hello group!" : messageTextField.value;
                
                // Create group chat
                var response = await GameFuseUser.CurrentUser.CreateGroupChatAsync(groupIdInt, text);
                
                // Display the chat
                DisplayChat(response);
                
                LogMessage("Group chat created successfully", LogType.Success);
            });
        }

        private async Task OnGetMessagesClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view messages", LogType.Error);
                return;
            }
            
            string chatId = chatIdField.value;
            
            if (string.IsNullOrEmpty(chatId))
            {
                LogMessage("Chat ID is required", LogType.Error);
                return;
            }
            
            int page = 1;
            if (!string.IsNullOrEmpty(chatPageField.value) && 
                !int.TryParse(chatPageField.value, out page))
            {
                LogMessage("Page must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get messages for chat
                var response = await GameFuseUser.CurrentUser.GetMessagesAsync(int.Parse(chatId), page);
                
                // Display messages
                DisplayMessages(response);
                
                LogMessage($"Retrieved {response.Messages.Length} messages", LogType.Success);
            });
        }

        private async Task OnSendMessageClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to send messages", LogType.Error);
                return;
            }
            
            string chatId = messageChatIdField.value;
            string messageText = messageTextField.value;
            
            if (string.IsNullOrEmpty(chatId))
            {
                LogMessage("Chat ID is required", LogType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(messageText))
            {
                LogMessage("Message text is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                if (!int.TryParse(chatId, out int chatIdInt))
                {
                    LogMessage("Chat ID must be a valid integer", LogType.Error);
                    return;
                }
                
                // Send message
                var message = await GameFuseUser.CurrentUser.SendMessageAsync(chatIdInt, messageText);
                
                // Clear message field
                messageTextField.value = string.Empty;
                
                // Display the message
                ClearScrollView(chatResultsScrollView);
                chatResultsScrollView.Add(new Label("Message sent:") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                var properties = new Dictionary<string, string>
                {
                    { "ID", message.Id.ToString() },
                    { "Text", message.Text },
                    { "Sender", message.UserId.ToString() },
                    { "Time", message.CreatedAt },
                    { "Read", message.Read.ToString() }
                };
                
                var messageItem = CreateListItem("Message", properties);
                chatResultsScrollView.Add(messageItem);
                
                LogMessage("Message sent successfully", LogType.Success);
            });
        }

        private async Task OnMarkMessageReadClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to mark messages as read", LogType.Error);
                return;
            }
            
            string messageId = markReadMessageIdField.value;
            
            if (string.IsNullOrEmpty(messageId))
            {
                LogMessage("Message ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Mark message as read
                var response = await GameFuseUser.CurrentUser.MarkMessageAsReadAsync(int.Parse(messageId));
                
                // Display success message
                ClearScrollView(chatResultsScrollView);
                chatResultsScrollView.Add(new Label($"Message {messageId} marked as read") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                LogMessage("Message marked as read successfully", LogType.Success);
            });
        }

        private void DisplayChats(GetChatsResponse response)
        {
            // Clear current display
            ClearScrollView(chatResultsScrollView);
            
            if (response != null)
            {
                int totalChats = 0;
                
                // Display direct chats
                if (response.DirectChats != null && response.DirectChats.Length > 0)
                {
                    chatResultsScrollView.Add(new Label($"Direct Chats ({response.DirectChats.Length})") 
                    { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                    
                    foreach (var chat in response.DirectChats)
                    {
                        DisplayChatListItem(chat, "Direct");
                        totalChats++;
                    }
                }
                
                // Display group chats
                if (response.GroupChats != null && response.GroupChats.Length > 0)
                {
                    chatResultsScrollView.Add(new Label($"Group Chats ({response.GroupChats.Length})") 
                    { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var chat in response.GroupChats)
                    {
                        DisplayChatListItem(chat, "Group");
                        totalChats++;
                    }
                }
                
                if (totalChats == 0)
                {
                    chatResultsScrollView.Add(new Label("No chats available"));
                }
            }
            else
            {
                chatResultsScrollView.Add(new Label("No chats available"));
            }
        }

        private void DisplayChat(Chat chat)
        {
            // Clear current display
            ClearScrollView(chatResultsScrollView);
            
            if (chat != null)
            {
                // Determine chat type from creator type
                string chatType = "Chat";
                if (chat.CreatorType != null)
                {
                    chatType = chat.CreatorType.Contains("Group") ? "Group Chat" : "Direct Chat";
                }
                
                var properties = new Dictionary<string, string>
                {
                    { "ID", chat.Id.ToString() },
                    { "Type", chatType },
                    { "Creator ID", chat.CreatorId.ToString() }
                };
                
                var chatItem = CreateListItem($"{chatType} #{chat.Id}", properties);
                chatResultsScrollView.Add(chatItem);
                
                // Add participants section
                if (chat.Participants != null && chat.Participants.Length > 0)
                {
                    chatResultsScrollView.Add(new Label("Participants:") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var participant in chat.Participants)
                    {
                        var participantProperties = new Dictionary<string, string>
                        {
                            { "ID", participant.Id.ToString() },
                            { "Username", participant.Username }
                        };
                        
                        var participantItem = CreateListItem(participant.Username, participantProperties);
                        chatResultsScrollView.Add(participantItem);
                    }
                }
                
                // Add messages section if there are messages
                if (chat.Messages != null && chat.Messages.Length > 0)
                {
                    chatResultsScrollView.Add(new Label("Recent Messages:") { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 10 } });
                    
                    foreach (var message in chat.Messages)
                    {
                        var messageProperties = new Dictionary<string, string>
                        {
                            { "ID", message.Id.ToString() },
                            { "Sender", message.UserId.ToString() },
                            { "Created", message.CreatedAt },
                            { "Read", message.Read.ToString() }
                        };
                        
                        var messageItem = CreateListItem(message.Text, messageProperties);
                        chatResultsScrollView.Add(messageItem);
                    }
                }
            }
            else
            {
                chatResultsScrollView.Add(new Label("No chat data available"));
            }
        }

        private void DisplayChatListItem(Chat chat, string type)
        {
            string title = $"{type} Chat #{chat.Id}";
            
            if (chat.Participants != null && chat.Participants.Length > 0)
            {
                string participantNames = string.Join(", ", chat.Participants.Select(p => p.Username));
                title += $" ({participantNames})";
            }
            
            var properties = new Dictionary<string, string>
            {
                { "ID", chat.Id.ToString() },
                { "Creator", chat.CreatorId.ToString() }
            };
            
            if (chat.Messages != null && chat.Messages.Length > 0)
            {
                var lastMessage = chat.Messages[0];
                properties.Add("Last Message", lastMessage.Text);
                properties.Add("From", lastMessage.UserId.ToString());
                properties.Add("At", lastMessage.CreatedAt);
                properties.Add("Read", lastMessage.Read.ToString());
            }
            
            var chatItem = CreateListItem(title, properties);
            chatResultsScrollView.Add(chatItem);
        }

        private void DisplayMessages(GetMessagesResponse response)
        {
            // Clear current display
            ClearScrollView(chatResultsScrollView);
            
            if (response != null && response.Messages != null && response.Messages.Length > 0)
            {
                chatResultsScrollView.Add(new Label($"Messages - Total: {response.Messages.Length}") 
                { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                foreach (var message in response.Messages)
                {
                    // Try to determine if this message is from current user
                    bool isCurrentUser = false;
                    if (GameFuseUser.CurrentUser != null) 
                    {
                        isCurrentUser = message.UserId == GameFuseUser.CurrentUser.GetID();
                    }
                    string sender = isCurrentUser ? "You" : $"User {message.UserId}";
                    
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", message.Id.ToString() },
                        { "From", sender },
                        { "Time", message.CreatedAt },
                        { "Read", message.Read.ToString() }
                    };
                    
                    var messageItem = CreateListItem(message.Text, properties);
                    
                    // Add some styling based on sender
                    if (isCurrentUser)
                    {
                        messageItem.AddToClassList("current-user-message");
                    }
                    else
                    {
                        messageItem.AddToClassList("other-user-message");
                    }
                    
                    chatResultsScrollView.Add(messageItem);
                }
            }
            else
            {
                chatResultsScrollView.Add(new Label("No messages available"));
            }
        }
    }
}