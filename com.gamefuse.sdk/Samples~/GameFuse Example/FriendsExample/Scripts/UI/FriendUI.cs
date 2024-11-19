using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;

public class FriendUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _userNameText, _statusText;

    [SerializeField]
    Button _removeButton;

    private UserInfo _userInfo;


    void Start()
    {
        _removeButton.onClick.AddListener(RemoveFriend);
        _statusText.text = string.Empty;
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        _userInfo = userInfo;
        _userNameText.text = _userInfo.username;
    }

    private async void RemoveFriend()
    {
        try
        {
            FriendshipStatusResponse friendshipStatusResponse = await GameFuseUser.CurrentUser.UnFriendAsync(_userInfo.id);
            UpdateUIStatus("Removed");
        }
        catch (ApiException ex)
        {
            Debug.Log($"Remove friend Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private void UpdateUIStatus(string status)
    {
        Destroy(_removeButton.gameObject);
        _statusText.text = status;
    }

}
