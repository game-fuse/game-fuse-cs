using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFuseCSharp;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChatPanel : MonoBehaviour
{
    [SerializeField]
    MessagePanelUI _fromMessagePrefab, _toMessagePrefab;

    [SerializeField]
    TMP_InputField _messageInput;

    [SerializeField]
    Button _sendButton;

    [SerializeField]
    Transform _contentTransform;

    [SerializeField]
    TextMeshProUGUI _friendName;

    private UserInfo _userInfo;
    private int _chatId = -1;

    private List<MessagePanelUI> _messageUIs = new List<MessagePanelUI>();

    private void Start()
    {
        FriendUI.OnFriendPanelClicked += HandleFriendPanelClicked;
        _sendButton.onClick.AddListener(SendMessage);
    }

    private void OnDestroy()
    {
        FriendUI.OnFriendPanelClicked -= HandleFriendPanelClicked;
    }


    private void HandleFriendPanelClicked(UserInfo userInfo)
    {
        _sendButton.interactable = false;
        _userInfo = userInfo;
        _friendName.text = _userInfo.Username;
        GetChats();
    }

    private async void GetChats()
    {
        GetChatsResponse chatsResponse = await GameFuseUser.CurrentUser.GetChatsAsync();
        Chat chat = FindChatWithFriendId(chatsResponse.DirectChats);
        if(chat != null)
        {
            InstantiateMessages(chat.Messages);
            _chatId = chat.Id;
        }
        else
        {
            ClearMessages();
            _chatId = -1;
        }
        _sendButton.interactable = true;

    }

    private Chat FindChatWithFriendId(Chat[] chats)
    {
        int friendId = _userInfo.Id;
        foreach (var chat in chats)
        {
            if (chat.Participants.Length == 2 &&
                chat.Participants.Any(p => p.Id == friendId))
            {
                return chat;
            }
        }
        return null;
    }


    private void InstantiateMessages(ChatMessage[] messages)
    {
        ClearMessages();
        foreach (ChatMessage message in messages)
        {
            MessagePanelUI messageUI = null;
            if (message.UserId == GameFuseUser.CurrentUser.GetID())
            {
                messageUI = Instantiate(_toMessagePrefab, _contentTransform);
            }
            else if(message.UserId == _userInfo.Id)
            {
                messageUI = Instantiate(_fromMessagePrefab, _contentTransform);
            }
            if(messageUI != null)
            {
                messageUI.transform.SetParent(_contentTransform, false);
                messageUI.transform.SetAsFirstSibling();
                messageUI.SetMessage(message);
                _messageUIs.Add(messageUI);
            }
        }
    }

    private async void SendMessage()
    {
        if(_chatId != -1)
        {
            _sendButton.interactable = false;
            await GameFuseUser.CurrentUser.SendMessageAsync(_chatId, _messageInput.text);
            _messageInput.text = string.Empty;
            GetMessagesResponse messagesResponse = await GameFuseUser.CurrentUser.GetMessagesAsync(_chatId);
            InstantiateMessages(messagesResponse.Messages);
            _sendButton.interactable = true;
        }
        else
        {
            _sendButton.interactable = false;
            Chat chat = await GameFuseUser.CurrentUser.CreateDirectChatAsync(new string[] { _userInfo.Username }, _messageInput.text);
            _chatId = chat.Id;
            _messageInput.text = string.Empty;
            GetMessagesResponse messagesResponse = await GameFuseUser.CurrentUser.GetMessagesAsync(_chatId);
            InstantiateMessages(messagesResponse.Messages);
            _sendButton.interactable = false;
        }
    }



    private void ClearMessages()
    {
        foreach(var messageUI in _messageUIs)
        {
            Destroy(messageUI.gameObject);
        }
        _messageUIs.Clear();
    }
}
