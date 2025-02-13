using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;

public class IncomingFriendRequestUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _userNameText, _statusText;

    [SerializeField]
    Button _acceptButton, _declineButton;

    private FriendRequest _friendRequest;

    private void Start()
    {
        _acceptButton.onClick.AddListener(AcceptFriendRequest);
        _declineButton.onClick.AddListener(DeclineFriendRequest);
        _statusText.text = string.Empty;
    }

    public void SetFriendRequest(FriendRequest friendRequest)
    {
        _friendRequest = friendRequest;
        _userNameText.text = _friendRequest.Username;
    }

    private async void AcceptFriendRequest()
    {
        try
        {
            FriendshipStatusResponse friendshipStatusResponse = await GameFuseUser.CurrentUser.AcceptFriendRequestAsync(_friendRequest.FriendshipId);
            UpdateUIStatus("Accepted");
        }
        catch(ApiException ex)
        {
            Debug.Log($"Accept friend request Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private async void DeclineFriendRequest()
    {
        try
        {
            FriendshipStatusResponse friendshipStatusResponse = await GameFuseUser.CurrentUser.DeclineFriendRequestAsync(_friendRequest.FriendshipId);
            UpdateUIStatus("Declined");
        }
        catch(ApiException ex)
        {
            Debug.Log($"Decline friend request Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private void UpdateUIStatus(string status)
    {
        Destroy(_acceptButton.gameObject);
        Destroy(_declineButton.gameObject);
        _statusText.text = status;
    }
}
