using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using GameFuse.UI.Controls;

namespace GameFuse.UI
{
    public class MessagesPanelController
    {
        private readonly VisualElement _root;
        private readonly MessagesPanel _messagesPanel;
        private GameFuseUser _currentUser;
        
        // Data
        private List<Chat> _allChats = new List<Chat>();
        private Chat _currentChat;
        private List<Message> _currentMessages = new List<Message>();
        
        // Pagination
        private int _currentPage = 1;
        private const int ITEMS_PER_PAGE = 10;
        
        // Events
        public event Action<string> OnError;
        public event Action<string> OnSuccess;
        public event Action OnUserDataUpdated;
        
        public MessagesPanelController(VisualElement root)
        {
            _root = root;
            
            // Try to get the custom control first
            _messagesPanel = root as MessagesPanel;
            if (_messagesPanel == null)
            {
                // Fallback to querying for it
                _messagesPanel = root.Q<MessagesPanel>("messages-panel");
            }
            
            if (_messagesPanel != null)
            {
                SetupEventHandlers();
            }
            else
            {
                Debug.LogError("MessagesPanel control not found!");
            }
        }
        
        private void SetupEventHandlers()
        {
            _messagesPanel.OnRefreshChats += RefreshChats;
            _messagesPanel.OnPreviousPage += () => ChangePage(-1);
            _messagesPanel.OnNextPage += () => ChangePage(1);
            _messagesPanel.OnCreateDirectChat += CreateDirectChat;
            _messagesPanel.OnChatSelected += SelectChat;
            _messagesPanel.OnBackToChats += BackToChats;
            _messagesPanel.OnSendMessage += SendMessage;
            
            // Forward events
            _messagesPanel.OnError += message => OnError?.Invoke(message);
            _messagesPanel.OnSuccess += message => OnSuccess?.Invoke(message);
            _messagesPanel.OnUserDataUpdated += () => OnUserDataUpdated?.Invoke();
        }
        
        public void SetCurrentUser(GameFuseUser user)
        {
            _currentUser = user;
            if (_currentUser != null)
            {
                RefreshChats();
            }
        }
        
        public void Reset()
        {
            _currentUser = null;
            _allChats.Clear();
            _currentChat = null;
            _currentMessages.Clear();
            _currentPage = 1;
            
            if (_messagesPanel != null)
            {
                _messagesPanel.ShowChats();
                _messagesPanel.UpdateChatsDisplay(new List<Chat>(), 1, 1);
            }
        }
        
        private async void RefreshChats()
        {
            if (_currentUser == null) return;
            
            _messagesPanel.SetLoading(true);
            
            try
            {
                var response = await _currentUser.FetchMyPaginatedChatsAsync(_currentPage);
                
                _allChats.Clear();
                _allChats.AddRange(response.DirectChats);
                _allChats.AddRange(response.GroupChats);
                
                // Sort by most recent message
                _allChats = _allChats.OrderByDescending(chat => 
                {
                    if (chat.Messages != null && chat.Messages.Count > 0)
                    {
                        var lastMessage = chat.Messages.Last();
                        if (DateTime.TryParse(lastMessage.CreatedAt, out var date))
                            return date;
                    }
                    return DateTime.MinValue;
                }).ToList();
                
                UpdateChatsDisplay();
                _messagesPanel.ShowStatus("Chats refreshed", false);
            }
            catch (Exception ex)
            {
                _messagesPanel.ShowStatus($"Error loading chats: {ex.Message}", true);
                OnError?.Invoke($"Failed to load chats: {ex.Message}");
            }
            finally
            {
                _messagesPanel.SetLoading(false);
            }
        }
        
        private void UpdateChatsDisplay()
        {
            var totalPages = Math.Max(1, (_allChats.Count + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE);
            _messagesPanel.UpdateChatsDisplay(_allChats, _currentPage, totalPages);
        }
        
        private void ChangePage(int direction)
        {
            var totalPages = Math.Max(1, (_allChats.Count + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE);
            var newPage = _currentPage + direction;
            
            if (newPage >= 1 && newPage <= totalPages)
            {
                _currentPage = newPage;
                RefreshChats();
            }
        }
        
        private async void CreateDirectChat()
        {
            if (_currentUser == null) return;
            
            var username = _messagesPanel.GetUsernameInput();
            var message = _messagesPanel.GetInitialMessageInput();
            
            if (string.IsNullOrEmpty(username))
            {
                _messagesPanel.ShowStatus("Please enter a username", true);
                return;
            }
            
            if (string.IsNullOrEmpty(message))
            {
                _messagesPanel.ShowStatus("Please enter a message", true);
                return;
            }
            
            _messagesPanel.SetLoading(true);
            
            try
            {
                var chat = await _currentUser.CreateDirectChatAsync(
                    new List<string> { username },
                    message
                );
                
                _messagesPanel.ShowStatus("Chat created successfully", false);
                OnSuccess?.Invoke("New chat started");
                
                // Add to chats list and select it
                _allChats.Insert(0, chat);
                UpdateChatsDisplay();
                SelectChat(chat);
            }
            catch (Exception ex)
            {
                _messagesPanel.ShowStatus($"Error creating chat: {ex.Message}", true);
                OnError?.Invoke($"Failed to create chat: {ex.Message}");
            }
            finally
            {
                _messagesPanel.SetLoading(false);
            }
        }
        
        private async void SelectChat(Chat chat)
        {
            _currentChat = chat;
            _messagesPanel.ShowMessageView(chat);
            
            await LoadMessages();
            
            // Mark messages as read
            if (_currentMessages.Count > 0)
            {
                var unreadMessages = _currentMessages.Where(m => !m.Read && m.UserId != _currentUser.Id);
                foreach (var message in unreadMessages)
                {
                    try
                    {
                        await _currentUser.MarkMessageAsReadAsync(message.Id);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to mark message as read: {ex.Message}");
                    }
                }
            }
        }
        
        private async Task LoadMessages()
        {
            if (_currentUser == null || _currentChat == null) return;
            
            _messagesPanel.SetLoading(true);
            
            try
            {
                var response = await _currentUser.FetchPaginatedMessagesForChatAsync(_currentChat.Id);
                _currentMessages = response.Messages;
                
                // Sort by creation date
                _currentMessages = _currentMessages.OrderBy(m => 
                {
                    if (DateTime.TryParse(m.CreatedAt, out var date))
                        return date;
                    return DateTime.MinValue;
                }).ToList();
                
                _messagesPanel.UpdateMessagesDisplay(_currentMessages);
            }
            catch (Exception ex)
            {
                _messagesPanel.ShowStatus($"Error loading messages: {ex.Message}", true);
                OnError?.Invoke($"Failed to load messages: {ex.Message}");
            }
            finally
            {
                _messagesPanel.SetLoading(false);
            }
        }
        
        private async void SendMessage()
        {
            if (_currentUser == null || _currentChat == null) return;
            
            var messageText = _messagesPanel.GetMessageInput();
            if (string.IsNullOrEmpty(messageText)) return;
            
            _messagesPanel.SetLoading(true);
            
            try
            {
                var message = await _currentUser.SendMessageToChatAsync(_currentChat.Id, messageText);
                
                // Add to messages list
                _currentMessages.Add(message);
                _messagesPanel.UpdateMessagesDisplay(_currentMessages);
                _messagesPanel.ClearMessageInput();
                
                // Update chat's last message
                if (_currentChat.Messages == null)
                    _currentChat.Messages = new List<Message>();
                _currentChat.Messages.Add(message);
                
                OnSuccess?.Invoke("Message sent");
            }
            catch (Exception ex)
            {
                _messagesPanel.ShowStatus($"Error sending message: {ex.Message}", true);
                OnError?.Invoke($"Failed to send message: {ex.Message}");
            }
            finally
            {
                _messagesPanel.SetLoading(false);
            }
        }
        
        private void BackToChats()
        {
            _currentChat = null;
            _currentMessages.Clear();
            _messagesPanel.ShowChats();
            RefreshChats();
        }
    }
}