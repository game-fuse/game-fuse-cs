using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;
public class GroupDetailsUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField _idInput,
        _nameInput,
        _typeInput,
        _autoJoinInput,
        _inviteOnlyInput,
        _maxGroupSizeInput,
        _searchableInput;

    [SerializeField]
    MemberUI _memberUIPrefab;

    [SerializeField]
    JoinRequestUI _joinRequestUIPrefab;

    [SerializeField]
    Transform _contentTransform;

    [SerializeField]
    TMP_InputField _resultInput;

    [SerializeField]
    Button memberTabButton, adminTabButton, joinRequestTabButton, invitesTabButton;

    private List<MemberUI> _membersUI = new List<MemberUI>();

    private List<JoinRequestUI> _joinRequestsUI = new List<JoinRequestUI>();

    private int _groupId;

    public void Awake()
    {
        GroupUI.OnGroupUIClicked += SetValues;
    }

    private void Start()
    {
        memberTabButton.onClick.AddListener(GetGroupMembers);
        adminTabButton.onClick.AddListener(GetGroupAdmins);
        joinRequestTabButton.onClick.AddListener(GetJoinRequests);
        invitesTabButton.onClick.AddListener(GetGroupInvites);
    }

    public void OnDestroy()
    {
        GroupUI.OnGroupUIClicked -= SetValues;
    }

    public void SetValues(GroupResponse groupResponse)
    {
        _groupId = groupResponse.Id;
        SetGroupDetails(groupResponse);
        ActivateButtons();
    }

    private void SetGroupDetails(GroupResponse groupResponse)
    {
        _idInput.text = groupResponse.Id.ToString();
        _nameInput.text = groupResponse.Name;
        _typeInput.text = groupResponse.GroupType;
        _autoJoinInput.text = groupResponse.CanAutoJoin.ToString();
        _inviteOnlyInput.text = groupResponse.IsInviteOnly.ToString();
        _maxGroupSizeInput.text = groupResponse.MaxGroupSize.ToString();
        _searchableInput.text = groupResponse.Searchable.ToString();
    }

    private void ActivateButtons()
    {
        memberTabButton.interactable = true;
        adminTabButton.interactable = true;
        joinRequestTabButton.interactable = true;
        invitesTabButton.interactable = true;
    }

    
    private async void GetGroupMembers()
    {
        try
        {
            GroupResponse groupResponse = await GameFuseUser.CurrentUser.GetGroupDetailsAsync(_groupId);
            ClearScrollView();
            foreach(var member in groupResponse.Members)
            {
                MemberUI memberUI = Instantiate(_memberUIPrefab, _contentTransform);
                memberUI.transform.SetParent(_contentTransform, false);
                memberUI.SetUserInfo(member);
                _membersUI.Add(memberUI);
            }

        }
        catch (ApiException ex)
        {
            string exceptionMessage = $"Get Group Details Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
            Debug.LogError(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
    }

    private async void GetGroupAdmins()
    {
        try
        {
            GroupResponse groupResponse = await GameFuseUser.CurrentUser.GetGroupDetailsAsync(_groupId);
            ClearScrollView();
            foreach (var admin in groupResponse.Admins)
            {
                MemberUI memberUI = Instantiate(_memberUIPrefab, _contentTransform);
                memberUI.transform.SetParent(_contentTransform, false);
                memberUI.SetUserInfo(admin);
                _membersUI.Add(memberUI);
            }

        }
        catch (ApiException ex)
        {
            string exceptionMessage = $"Get Group Details Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
            Debug.LogError(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
    }

    private async void GetJoinRequests()
    {
        try
        {
            GroupResponse groupResponse = await GameFuseUser.CurrentUser.GetGroupDetailsAsync(_groupId);
            ClearScrollView();
            foreach (var joinRequest in groupResponse.JoinRequests)
            {
                JoinRequestUI joinRequestUI = Instantiate(_joinRequestUIPrefab, _contentTransform);
                joinRequestUI.transform.SetParent(_contentTransform, false);
                joinRequestUI.SetConnectionResponse(joinRequest);
                _joinRequestsUI.Add(joinRequestUI);
            }
        }
        catch (ApiException ex)
        {
            string exceptionMessage = $"Get Group Details Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
            Debug.LogError(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
    }

    private async void GetGroupInvites()
    {
        try
        {
            GroupResponse groupResponse = await GameFuseUser.CurrentUser.GetGroupDetailsAsync(_groupId);
            ClearScrollView();
            foreach (var invite in groupResponse.Invites)
            {
                JoinRequestUI joinRequestUI = Instantiate(_joinRequestUIPrefab, _contentTransform);
                joinRequestUI.transform.SetParent(_contentTransform, false);
                joinRequestUI.SetConnectionResponse(invite, true);
                _joinRequestsUI.Add(joinRequestUI);
            }
        }
        catch (ApiException ex)
        {
            string exceptionMessage = $"Get Group Details Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
            Debug.LogError(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
    }

    private void ClearScrollView()
    {
        ClearMembers();
        ClearJoinRequests();
    }

    private void ClearMembers()
    {
        foreach(var memberUI in _membersUI)
        {
            Destroy(memberUI.gameObject);
        }
        _membersUI.Clear();
    }

    private void ClearJoinRequests()
    {
        foreach(var joinRequestUI in _joinRequestsUI)
        {
            Destroy(joinRequestUI.gameObject);
        }
        _membersUI.Clear();
    }

}
