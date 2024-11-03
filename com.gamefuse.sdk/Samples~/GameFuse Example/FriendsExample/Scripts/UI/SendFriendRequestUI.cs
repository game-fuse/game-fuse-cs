using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using System;


public class SendFriendRequestUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField _userNameInput, _resultInput;

    [SerializeField]
    Button _sendFriendRequestButton;

    void Start()
    {
        _sendFriendRequestButton.onClick.AddListener(SendFriendRequest);
        
    }

    private async void SendFriendRequest()
    {
        try
        {
            FriendRequestResponse friendRequestResponse  = await GameFuseUser.CurrentUser.SendFriendRequestAsync(_userNameInput.text);
            _resultInput.text = $"friend request {friendRequestResponse.friendship_id}, sent to {_userNameInput.text}";

        }
        catch
        {
            //UI Does something to indicate the exception
        }
    }
}
