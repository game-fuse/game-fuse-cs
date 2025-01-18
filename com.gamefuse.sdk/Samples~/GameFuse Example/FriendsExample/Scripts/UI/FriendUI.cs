using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;
using System;

public class FriendUI : MonoBehaviour
{
    public static event Action<UserInfo> OnFriendPanelClicked;

    [SerializeField]
    TextMeshProUGUI _userNameText, _statusText;

    [SerializeField]
    Button _removeButton;

    private UserInfo _userInfo;

    private Button _panelButton;


    void Start()
    {
        _panelButton = GetComponent<Button>();
        _panelButton.onClick.AddListener(HandleFriendPanelClicked);

        _removeButton.onClick.AddListener(RemoveFriend);
        _statusText.text = string.Empty;
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        _userInfo = userInfo;
        _userNameText.text = _userInfo.Username;
    }

    private async void RemoveFriend()
    {
        try
        {
            FriendshipStatusResponse friendshipStatusResponse = await GameFuseUser.CurrentUser.UnFriendAsync(_userInfo.Id);
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

    private void HandleFriendPanelClicked()
    {
        OnFriendPanelClicked?.Invoke(_userInfo);
    }

}
