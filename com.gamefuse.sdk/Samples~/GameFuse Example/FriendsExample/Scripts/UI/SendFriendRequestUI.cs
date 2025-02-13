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
            _resultInput.text = $"friend request {friendRequestResponse.FriendshipId}, sent to {_userNameInput.text}";

        }
        catch (ApiException ex)
        {
            Debug.Log($"SendFriendRequestUI GameFuse API Exception:\n Status Code: {ex.StatusCode}, \n Message: {ex.Message}");
        }
    }
}
