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
                
                // Create request
                var request = new CreateDirectChatRequest
                {
                    Usernames = usernameArray,
                    Text = string.IsNullOrEmpty(messageTextField.value) ? "Hello!" : messageTextField.value
                };
                
                // Create direct chat
                var response = await GameFuseUser.CurrentUser.CreateDirectChatAsync(request);
                
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
                // Create request
                var request = new CreateGroupChatRequest
                {
                    GroupId = int.Parse(groupId),
                    Text = string.IsNullOrEmpty(messageTextField.value) ? "Hello group!" : messageTextField.value
                };
                
                // Create group chat
                var response = await GameFuseUser.CurrentUser.CreateGroupChatAsync(request);
                
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
                // Create request
                var request = new SendMessageRequest
                {
                    ChatId = int.Parse(chatId),
                    Text = messageText
                };
                
                // Send message
                var response = await GameFuseUser.CurrentUser.SendMessageAsync(request);
                
                // Clear message field
                messageTextField.value = string.Empty;
                
                // Display the message
                ClearScrollView(chatResultsScrollView);
                chatResultsScrollView.Add(new Label("Message sent:") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                var properties = new Dictionary<string, string>
                {
                    { "ID", response.Message.Id.ToString() },
                    { "Text", response.Message.Text },
                    { "Sender", response.Message.SenderId.ToString() },
                    { "Time", response.Message.Created },
                    { "Read", response.Message.Read.ToString() }
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
            
            if (response != null && response.Chats != null && response.Chats.Length > 0)
            {
                chatResultsScrollView.Add(new Label($"Showing page {response.CurrentPage} of {response.TotalPages} ({response.TotalItems} total chats)") 
                { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                foreach (var chat in response.Chats)
                {
                    if (chat.IsDirectChat)
                    {
                        DisplayChatListItem(chat, "Direct");
                    }
                    else
                    {
                        DisplayChatListItem(chat, "Group");
                    }
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
                string chatType = chat.IsDirectChat ? "Direct Chat" : "Group Chat";
                
                var properties = new Dictionary<string, string>
                {
                    { "ID", chat.Id.ToString() },
                    { "Type", chatType },
                    { "Created", chat.Created }
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
                            { "Sender", message.SenderId.ToString() },
                            { "Created", message.Created },
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
                { "Created", chat.Created }
            };
            
            if (chat.Messages != null && chat.Messages.Length > 0)
            {
                var lastMessage = chat.Messages[0];
                properties.Add("Last Message", lastMessage.Text);
                properties.Add("From", lastMessage.SenderId.ToString());
                properties.Add("At", lastMessage.Created);
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
                chatResultsScrollView.Add(new Label($"Chat #{response.ChatId} - Page {response.CurrentPage} of {response.TotalPages} ({response.TotalItems} total messages)") 
                { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                foreach (var message in response.Messages)
                {
                    bool isCurrentUser = message.SenderId == GameFuseUser.CurrentUser.Id;
                    string sender = isCurrentUser ? "You" : $"User {message.SenderId}";
                    
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", message.Id.ToString() },
                        { "From", sender },
                        { "Time", message.Created },
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