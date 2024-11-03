using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;
public class FriendRequestsScrollViewUI : MonoBehaviour
{
    [SerializeField]
    IncomingFriendRequestUI _incomingFriendRequestPrefab;

    [SerializeField]
    OutgoingFriendRequestUI _outgoingFriendRequestPrefab;

    [SerializeField]
    Toggle _incomingFriendsToggle, _outgoingFriendsToggle;

    [SerializeField]
    Button _refreshButton;

    [SerializeField]
    Transform _contentTransform;

    [SerializeField]
    TMP_InputField _resultInput;


    private List<IncomingFriendRequestUI> _incomingFriendRequestUIs = new List<IncomingFriendRequestUI>();
    private List<OutgoingFriendRequestUI> _outgoingFriendRequestUIs = new List<OutgoingFriendRequestUI>();

    private void Start()
    {
        _incomingFriendsToggle.onValueChanged.AddListener((bool isOn) => RefreshGUI());
        _outgoingFriendsToggle.onValueChanged.AddListener((bool isOn) => RefreshGUI());
        _refreshButton.onClick.AddListener(RefreshGUI);
    }

    private void RefreshGUI()
    {
        if (_incomingFriendsToggle.isOn)
        {
            GetIncomingFriendRequests();
        }
        else
        {
            GetOutgoingFriendRequests();
        }
    }

    private async void GetIncomingFriendRequests()
    {
        try
        {
            FriendRequest[] icomingFriendRequests = await GameFuseUser.CurrentUser.GetIncomingFriendRequestsAsync();
            ClearRequests();
            foreach (var friendRequest in icomingFriendRequests)
            {
                IncomingFriendRequestUI incomingFriendRequestUI = Instantiate(_incomingFriendRequestPrefab, _contentTransform);
                incomingFriendRequestUI.transform.SetParent(_contentTransform, false);
                incomingFriendRequestUI.SetFriendRequest(friendRequest);
                _incomingFriendRequestUIs.Add(incomingFriendRequestUI);
            }
            _resultInput.text = $"{icomingFriendRequests.Length} outgoing frined requests";
        }
        catch(ApiException ex)
        {
            Debug.Log($"Get Incoming Friend Requests Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
            _resultInput.text = $"Get Incoming Friend Requests Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
        }
    }

    private async void GetOutgoingFriendRequests()
    {
        try
        {
            FriendRequest[] outgoingFriendRequests = await GameFuseUser.CurrentUser.GetOutgoingFriendRequests();
            ClearRequests();
            foreach (var friendRequest in outgoingFriendRequests)
            {
                OutgoingFriendRequestUI outgoingFriendRequestUI = Instantiate(_outgoingFriendRequestPrefab, _contentTransform);
                outgoingFriendRequestUI.transform.SetParent(_contentTransform, false);
                outgoingFriendRequestUI.SetFriendRequest(friendRequest);
                _outgoingFriendRequestUIs.Add(outgoingFriendRequestUI);
            }
            _resultInput.text = $"{outgoingFriendRequests.Length} outgoing frined requests";
        }
        catch (ApiException ex)
        {
            Debug.Log($"Get Outgoing Friend Requests Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
            _resultInput.text = $"Get Outgoing Friend Requests Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
        }
    }

    private void ClearRequests()
    {
        foreach (var incomingFriendRequestUI in _incomingFriendRequestUIs)
        {
            Destroy(incomingFriendRequestUI.gameObject);
        }
        _incomingFriendRequestUIs.Clear();
        foreach (var outgoingFriendRequestUI in _outgoingFriendRequestUIs)
        {
            Destroy(outgoingFriendRequestUI.gameObject);
        }
        _outgoingFriendRequestUIs.Clear();
    }   
}
