using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;

public class FriendsScrollViewUI : MonoBehaviour
{
    [SerializeField]
    FriendUI _friendPrefab;

    [SerializeField]
    Button _refreshButton;

    [SerializeField]
    Transform _contentTransform;

    [SerializeField]
    TMP_InputField _resultInput;


    private List<FriendUI> _friendUIs = new List<FriendUI>();

    private void Start()
    {
        _refreshButton.onClick.AddListener(RefreshGUI);
    }

    private void RefreshGUI()
    {
        GetFriends();
    }

    private async void GetFriends()
    {
        try
        {
            UserInfo[] friends = await GameFuseUser.CurrentUser.GetFriendsAsync();
            ClearRequests();
            foreach (var friend in friends)
            {
                FriendUI friendUI = Instantiate(_friendPrefab, _contentTransform);
                friendUI.transform.SetParent(_contentTransform, false);
                friendUI.SetUserInfo(friend);
                _friendUIs.Add(friendUI);
            }
        }
        catch (ApiException ex)
        {
            Debug.Log($"Get Outgoing Friend Requests Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}");
        }
    }

    private void ClearRequests()
    {
        foreach (var friendUI in _friendUIs)
        {
            Destroy(friendUI.gameObject);
        }
        _friendUIs.Clear();
    }
}
