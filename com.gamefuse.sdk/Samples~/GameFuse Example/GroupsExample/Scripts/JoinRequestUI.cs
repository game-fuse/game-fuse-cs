using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;

public class JoinRequestUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _userNameText, _statusText;

    [SerializeField]
    Button _acceptButton, _declineButton;

    private GroupConnectionResponse _connectionResponse;

    private void Start()
    {
        _acceptButton.onClick.AddListener(AcceptJoinRequest);
        _declineButton.onClick.AddListener(DeclineJoinRequest);
        _statusText.text = string.Empty;
    }

    public void SetConnectionResponse(GroupConnectionResponse connectionResponse, bool isInvite = false)
    {
        _connectionResponse = connectionResponse;
        _userNameText.text = _connectionResponse.User.Username;
        if (isInvite)
        {
            UpdateUIStatus("invite sent");
        }

    }

    

    private async void AcceptJoinRequest()
    {
        try
        {
            GroupConnectionStatusResponse connectionStatusResponse = await GameFuseUser.CurrentUser.AcceptGroupConnectionRequestAsync(_connectionResponse.Id);
            UpdateUIStatus(connectionStatusResponse.Status);
        }
        catch(ApiException ex)
        {
            Debug.Log($"Accept friend request Api Exception:\n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private async void DeclineJoinRequest()
    {
        try
        {
            GroupConnectionStatusResponse connectionStatusResponse = await GameFuseUser.CurrentUser.DeclineGroupConnectionRequestAsync(_connectionResponse.Id);
            UpdateUIStatus(connectionStatusResponse.Status);
        }
        catch (ApiException ex)
        {
            Debug.Log($"Decline friend request Api Exception:\n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private void UpdateUIStatus(string status)
    {
        Destroy(_acceptButton.gameObject);
        Destroy(_declineButton.gameObject);
        _statusText.text = status;
    }
}
