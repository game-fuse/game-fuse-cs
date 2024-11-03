using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;

public class OutgoingFriendRequestUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _userNameText, _statusText;

    [SerializeField]
    Button _cancelButton;

    private FriendRequest _friendRequest;

    private void Start()
    {
        _cancelButton.onClick.AddListener(CancelFriendRequest);
        _statusText.text = string.Empty;
    }

    public void SetFriendRequest(FriendRequest friendRequest)
    {
        _friendRequest = friendRequest;
        _userNameText.text = _friendRequest.username;
    }


    private async void CancelFriendRequest()
    {
        try
        {
            FriendshipStatusResponse friendshipStatusResponse = await GameFuseUser.CurrentUser.CancelFriendRequestAsync(_friendRequest.friendship_id);
            UpdateUIStatus("Canceled");
        }
        catch(ApiException ex)
        {
            Debug.Log($"Cancel friend request Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private void UpdateUIStatus(string status)
    {
        Destroy(_cancelButton.gameObject);
        _statusText.text = status;
    }

}
