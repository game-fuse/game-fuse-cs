using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using GameFuse.Models.Shared;

namespace GameFuse.UI.Controls
{
    [UxmlElement]
    public partial class MessagesPanel : VisualElement
    {
        // UI References
        private Label _statusLabel;
        private VisualElement _loadingIndicator;
        
        // Tab containers
        private VisualElement _chatsContainer;
        private VisualElement _newMessageContainer;
        
        // Chat view containers
        private ScrollView _chatsList;
        private Label _totalChatsLabel;
        private VisualElement _chatsContent;
        private VisualElement _emptyChatsState;
        
        // Message view containers
        private VisualElement _messageViewContainer;
        private Label _chatTitleLabel;
        private ScrollView _messagesList;
        private TextField _messageInput;
        private Button _sendButton;
        private Button _backToChatsButton;
        
        // New message containers
        private TextField _usernameInput;
        private TextField _initialMessageInput;
        private Button _createChatButton;
        
        // Pagination
        private Button _previousButton;
        private Button _nextButton;
        private Label _pageInfoLabel;
        
        // Events
        public event Action<string> OnError;
        public event Action<string> OnSuccess;
        public event Action OnUserDataUpdated;
        
        public MessagesPanel()
        {
            AddToClassList("panel-content");
            
            // Create header
            var header = new VisualElement { name = "messages-header" };
            header.AddToClassList("panel-header");
            
            var title = new Label("Messages");
            title.AddToClassList("panel-title");
            header.Add(title);
            
            _statusLabel = new Label();
            _statusLabel.AddToClassList("status-label");
            _statusLabel.style.display = DisplayStyle.None;
            header.Add(_statusLabel);
            
            Add(header);
            
            // Create loading indicator
            _loadingIndicator = new VisualElement();
            _loadingIndicator.AddToClassList("loading-indicator");
            _loadingIndicator.style.display = DisplayStyle.None;
            
            var loadingText = new Label("Loading...");
            loadingText.AddToClassList("loading-text");
            _loadingIndicator.Add(loadingText);
            Add(_loadingIndicator);
            
            // Create tabs
            var tabsContainer = new VisualElement();
            tabsContainer.AddToClassList("tabs-container");
            
            var chatsTab = new Button(() => ShowChats()) { text = "Chats" };
            chatsTab.AddToClassList("tab-button");
            chatsTab.AddToClassList("active-tab");
            tabsContainer.Add(chatsTab);
            
            var newMessageTab = new Button(() => ShowNewMessage()) { text = "New Message" };
            newMessageTab.AddToClassList("tab-button");
            tabsContainer.Add(newMessageTab);
            
            Add(tabsContainer);
            
            // Create chats container
            _chatsContainer = CreateChatsContainer();
            Add(_chatsContainer);
            
            // Create new message container
            _newMessageContainer = CreateNewMessageContainer();
            _newMessageContainer.style.display = DisplayStyle.None;
            Add(_newMessageContainer);
            
            // Create message view container
            _messageViewContainer = CreateMessageViewContainer();
            _messageViewContainer.style.display = DisplayStyle.None;
            Add(_messageViewContainer);
        }
        
        private VisualElement CreateChatsContainer()
        {
            var container = new VisualElement { name = "chats-container" };
            container.AddToClassList("tab-content");
            
            // Header with count and refresh
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 10;
            
            _totalChatsLabel = new Label("Total Chats: 0");
            _totalChatsLabel.AddToClassList("count-label");
            header.Add(_totalChatsLabel);
            
            var refreshButton = new Button(() => OnRefreshChats?.Invoke()) { text = "Refresh" };
            refreshButton.AddToClassList("secondary");
            header.Add(refreshButton);
            
            container.Add(header);
            
            // Chats list
            _chatsList = new ScrollView();
            _chatsList.AddToClassList("chats-list");
            _chatsList.style.flexGrow = 1;
            _chatsList.style.height = 400;
            
            _chatsContent = new VisualElement();
            _chatsList.Add(_chatsContent);
            
            _emptyChatsState = new Label("No chats yet. Start a new conversation!");
            _emptyChatsState.AddToClassList("empty-state");
            _emptyChatsState.style.display = DisplayStyle.None;
            _chatsList.Add(_emptyChatsState);
            
            container.Add(_chatsList);
            
            // Pagination controls
            var paginationContainer = new VisualElement();
            paginationContainer.AddToClassList("pagination-container");
            paginationContainer.style.flexDirection = FlexDirection.Row;
            paginationContainer.style.justifyContent = Justify.Center;
            paginationContainer.style.alignItems = Align.Center;
            paginationContainer.style.marginTop = 10;
            
            _previousButton = new Button(() => OnPreviousPage?.Invoke()) { text = "Previous" };
            _previousButton.AddToClassList("secondary");
            _previousButton.SetEnabled(false);
            paginationContainer.Add(_previousButton);
            
            _pageInfoLabel = new Label("Page 1");
            _pageInfoLabel.style.marginLeft = 10;
            _pageInfoLabel.style.marginRight = 10;
            paginationContainer.Add(_pageInfoLabel);
            
            _nextButton = new Button(() => OnNextPage?.Invoke()) { text = "Next" };
            _nextButton.AddToClassList("secondary");
            _nextButton.SetEnabled(false);
            paginationContainer.Add(_nextButton);
            
            container.Add(paginationContainer);
            
            return container;
        }
        
        private VisualElement CreateNewMessageContainer()
        {
            var container = new VisualElement { name = "new-message-container" };
            container.AddToClassList("tab-content");
            
            var form = new VisualElement();
            form.AddToClassList("form-container");
            
            // Username input
            var usernameField = new VisualElement();
            usernameField.AddToClassList("form-field");
            
            var usernameLabel = new Label("Username");
            usernameLabel.AddToClassList("form-label");
            usernameField.Add(usernameLabel);
            
            _usernameInput = new TextField();
            _usernameInput.AddToClassList("form-input");
            usernameField.Add(_usernameInput);
            
            form.Add(usernameField);
            
            // Initial message input
            var messageField = new VisualElement();
            messageField.AddToClassList("form-field");
            
            var messageLabel = new Label("Message");
            messageLabel.AddToClassList("form-label");
            messageField.Add(messageLabel);
            
            _initialMessageInput = new TextField();
            _initialMessageInput.multiline = true;
            _initialMessageInput.style.height = 100;
            _initialMessageInput.AddToClassList("form-input");
            messageField.Add(_initialMessageInput);
            
            form.Add(messageField);
            
            // Create button
            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("form-buttons");
            
            _createChatButton = new Button(() => OnCreateDirectChat?.Invoke()) { text = "Start Chat" };
            _createChatButton.AddToClassList("primary");
            buttonContainer.Add(_createChatButton);
            
            form.Add(buttonContainer);
            
            container.Add(form);
            
            return container;
        }
        
        private VisualElement CreateMessageViewContainer()
        {
            var container = new VisualElement { name = "message-view-container" };
            container.AddToClassList("message-view");
            container.style.flexGrow = 1;
            container.style.display = DisplayStyle.Flex;
            container.style.flexDirection = FlexDirection.Column;
            
            // Header
            var header = new VisualElement();
            header.AddToClassList("message-view-header");
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;
            header.style.paddingTop = 10;
            header.style.paddingBottom = 10;
            header.style.paddingLeft = 10;
            header.style.paddingRight = 10;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new StyleColor(new UnityEngine.Color(0.2f, 0.2f, 0.2f));
            
            _backToChatsButton = new Button(() => OnBackToChats?.Invoke()) { text = "← Back" };
            _backToChatsButton.AddToClassList("secondary");
            header.Add(_backToChatsButton);
            
            _chatTitleLabel = new Label();
            _chatTitleLabel.AddToClassList("chat-title");
            _chatTitleLabel.style.marginLeft = 20;
            _chatTitleLabel.style.flexGrow = 1;
            header.Add(_chatTitleLabel);
            
            container.Add(header);
            
            // Messages list
            _messagesList = new ScrollView();
            _messagesList.AddToClassList("messages-list");
            _messagesList.style.flexGrow = 1;
            _messagesList.style.paddingTop = 10;
            _messagesList.style.paddingBottom = 10;
            container.Add(_messagesList);
            
            // Message input area
            var inputArea = new VisualElement();
            inputArea.AddToClassList("message-input-area");
            inputArea.style.flexDirection = FlexDirection.Row;
            inputArea.style.paddingTop = 10;
            inputArea.style.paddingBottom = 10;
            inputArea.style.paddingLeft = 10;
            inputArea.style.paddingRight = 10;
            inputArea.style.borderTopWidth = 1;
            inputArea.style.borderTopColor = new StyleColor(new UnityEngine.Color(0.2f, 0.2f, 0.2f));
            
            _messageInput = new TextField();
            _messageInput.style.flexGrow = 1;
            _messageInput.style.marginRight = 10;
            _messageInput.AddToClassList("message-input");
            _messageInput.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == UnityEngine.KeyCode.Return && !evt.shiftKey)
                {
                    evt.StopPropagation();
                    OnSendMessage?.Invoke();
                }
            });
            inputArea.Add(_messageInput);
            
            _sendButton = new Button(() => OnSendMessage?.Invoke()) { text = "Send" };
            _sendButton.AddToClassList("primary");
            inputArea.Add(_sendButton);
            
            container.Add(inputArea);
            
            return container;
        }
        
        // Public methods for controller interaction
        public event Action OnRefreshChats;
        public event Action OnPreviousPage;
        public event Action OnNextPage;
        public event Action OnCreateDirectChat;
        public event Action OnBackToChats;
        public event Action OnSendMessage;
        public event Action<Chat> OnChatSelected;
        
        public void ShowChats()
        {
            _chatsContainer.style.display = DisplayStyle.Flex;
            _newMessageContainer.style.display = DisplayStyle.None;
            _messageViewContainer.style.display = DisplayStyle.None;
            
            // Update tab styles
            var tabs = this.Query<Button>(className: "tab-button").ToList();
            tabs[0].AddToClassList("active-tab");
            tabs[1].RemoveFromClassList("active-tab");
        }
        
        public void ShowNewMessage()
        {
            _chatsContainer.style.display = DisplayStyle.None;
            _newMessageContainer.style.display = DisplayStyle.Flex;
            _messageViewContainer.style.display = DisplayStyle.None;
            
            // Update tab styles
            var tabs = this.Query<Button>(className: "tab-button").ToList();
            tabs[0].RemoveFromClassList("active-tab");
            tabs[1].AddToClassList("active-tab");
            
            // Clear form
            _usernameInput.value = "";
            _initialMessageInput.value = "";
        }
        
        public void ShowMessageView(Chat chat)
        {
            _chatsContainer.style.display = DisplayStyle.None;
            _newMessageContainer.style.display = DisplayStyle.None;
            _messageViewContainer.style.display = DisplayStyle.Flex;
            
            // Update chat title
            if (chat.ChatType == ChatType.Direct)
            {
                var otherUser = chat.Participants.Find(p => p.Id != GameFuseUser.CurrentUser.Id);
                _chatTitleLabel.text = otherUser != null ? otherUser.Username : "Direct Chat";
            }
            else
            {
                _chatTitleLabel.text = $"Group Chat (ID: {chat.GroupId})";
            }
        }
        
        public void SetLoading(bool isLoading)
        {
            _loadingIndicator.style.display = isLoading ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        public void ShowStatus(string message, bool isError = false)
        {
            _statusLabel.text = message;
            _statusLabel.RemoveFromClassList("status-success");
            _statusLabel.RemoveFromClassList("status-error");
            _statusLabel.AddToClassList(isError ? "status-error" : "status-success");
            _statusLabel.style.display = DisplayStyle.Flex;
            
            // Auto-hide after 3 seconds
            var hideTask = System.Threading.Tasks.Task.Delay(3000).ContinueWith(_ =>
            {
                _statusLabel.style.display = DisplayStyle.None;
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        public void UpdateChatsDisplay(List<Chat> chats, int currentPage, int totalPages)
        {
            _chatsContent.Clear();
            
            if (chats.Count == 0)
            {
                _emptyChatsState.style.display = DisplayStyle.Flex;
                _totalChatsLabel.text = "Total Chats: 0";
            }
            else
            {
                _emptyChatsState.style.display = DisplayStyle.None;
                _totalChatsLabel.text = $"Total Chats: {chats.Count}";
                
                foreach (var chat in chats)
                {
                    _chatsContent.Add(CreateChatItem(chat));
                }
            }
            
            // Update pagination
            _pageInfoLabel.text = $"Page {currentPage} of {totalPages}";
            _previousButton.SetEnabled(currentPage > 1);
            _nextButton.SetEnabled(currentPage < totalPages);
        }
        
        public void UpdateMessagesDisplay(List<Message> messages)
        {
            _messagesList.Clear();
            
            foreach (var message in messages)
            {
                _messagesList.Add(CreateMessageItem(message));
            }
            
            // Scroll to bottom
            _messagesList.ScrollTo(_messagesList.contentContainer[_messagesList.contentContainer.childCount - 1]);
        }
        
        private VisualElement CreateChatItem(Chat chat)
        {
            var item = new VisualElement();
            item.AddToClassList("chat-item");
            item.style.paddingTop = 10;
            item.style.paddingBottom = 10;
            item.style.paddingLeft = 10;
            item.style.paddingRight = 10;
            item.style.marginBottom = 5;
            item.style.borderTopLeftRadius = 5;
            item.style.borderTopRightRadius = 5;
            item.style.borderBottomLeftRadius = 5;
            item.style.borderBottomRightRadius = 5;
            item.style.backgroundColor = new StyleColor(new UnityEngine.Color(0.15f, 0.15f, 0.15f));
            item.style.cursor = StyleKeyword.Auto;
            item.RegisterCallback<ClickEvent>(evt => OnChatSelected?.Invoke(chat));
            
            // Chat info
            var infoSection = new VisualElement();
            infoSection.style.flexGrow = 1;
            
            var titleLabel = new Label();
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            
            if (chat.ChatType == ChatType.Direct)
            {
                var otherUser = chat.Participants.Find(p => p.Id != GameFuseUser.CurrentUser.Id);
                titleLabel.text = otherUser != null ? otherUser.Username : "Direct Chat";
            }
            else
            {
                titleLabel.text = $"Group Chat (ID: {chat.GroupId})";
            }
            infoSection.Add(titleLabel);
            
            // Last message preview
            if (chat.Messages != null && chat.Messages.Count > 0)
            {
                var lastMessage = chat.Messages[chat.Messages.Count - 1];
                var previewLabel = new Label(lastMessage.Text);
                previewLabel.style.fontSize = 12;
                previewLabel.style.color = new StyleColor(new UnityEngine.Color(0.7f, 0.7f, 0.7f));
                previewLabel.style.whiteSpace = WhiteSpace.NoWrap;
                previewLabel.style.overflow = Overflow.Hidden;
                previewLabel.style.textOverflow = TextOverflow.Ellipsis;
                infoSection.Add(previewLabel);
                
                // Unread indicator
                if (!lastMessage.Read)
                {
                    var unreadBadge = new Label("●");
                    unreadBadge.style.color = new StyleColor(new UnityEngine.Color(0.3f, 0.7f, 1f));
                    unreadBadge.style.position = Position.Absolute;
                    unreadBadge.style.right = 10;
                    unreadBadge.style.top = 10;
                    item.Add(unreadBadge);
                }
            }
            
            item.Add(infoSection);
            
            return item;
        }
        
        private VisualElement CreateMessageItem(Message message)
        {
            var item = new VisualElement();
            item.AddToClassList("message-item");
            item.style.paddingTop = 10;
            item.style.paddingBottom = 10;
            item.style.paddingLeft = 10;
            item.style.paddingRight = 10;
            item.style.marginBottom = 10;
            item.style.borderTopLeftRadius = 10;
            item.style.borderTopRightRadius = 10;
            item.style.borderBottomLeftRadius = 10;
            item.style.borderBottomRightRadius = 10;
            
            var isCurrentUser = message.UserId == GameFuseUser.CurrentUser.Id;
            
            if (isCurrentUser)
            {
                item.style.marginLeft = Length.Percent(20);
                item.style.backgroundColor = new StyleColor(new UnityEngine.Color(0.2f, 0.4f, 0.6f));
                item.style.alignSelf = Align.FlexEnd;
            }
            else
            {
                item.style.marginRight = Length.Percent(20);
                item.style.backgroundColor = new StyleColor(new UnityEngine.Color(0.2f, 0.2f, 0.2f));
                item.style.alignSelf = Align.FlexStart;
            }
            
            // Message text
            var textLabel = new Label(message.Text);
            textLabel.style.whiteSpace = WhiteSpace.Normal;
            textLabel.style.fontSize = 14;
            item.Add(textLabel);
            
            // Timestamp
            var timeLabel = new Label(message.CreatedAt);
            timeLabel.style.fontSize = 10;
            timeLabel.style.color = new StyleColor(new UnityEngine.Color(0.6f, 0.6f, 0.6f));
            timeLabel.style.marginTop = 5;
            item.Add(timeLabel);
            
            return item;
        }
        
        public void ClearMessageInput()
        {
            _messageInput.value = "";
        }
        
        public string GetMessageInput() => _messageInput.value;
        public string GetUsernameInput() => _usernameInput.value;
        public string GetInitialMessageInput() => _initialMessageInput.value;
        
        public void Initialize()
        {
            // Initialization logic if needed
        }
    }
}