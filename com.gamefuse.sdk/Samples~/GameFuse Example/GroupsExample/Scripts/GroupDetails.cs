using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;
public class GroupDetails : MonoBehaviour
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
    Transform _contentTransform;

    [SerializeField]
    TMP_InputField _resultInput;

    [SerializeField]
    Button memberTabButton, adminTabButton, joinRequestTabButton, invitesTabButton;

    private List<MemberUI> _membersUI = new List<MemberUI>();

    private int _groupId;

    public void Awake()
    {
        GroupUI.OnGroupUIClicked += SetValues;
    }

    private void Start()
    {
        memberTabButton.onClick.AddListener(GetGroupMembers);
        adminTabButton.onClick.AddListener(GetGroupAdmins);
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
            ClearMembers();
            foreach(var user in groupResponse.Members)
            {
                MemberUI memberUI = Instantiate(_memberUIPrefab, _contentTransform);
                memberUI.transform.SetParent(_contentTransform, false);
                memberUI.SetUserInfo(user);
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
            ClearMembers();
            foreach (var user in groupResponse.Admins)
            {
                MemberUI memberUI = Instantiate(_memberUIPrefab, _contentTransform);
                memberUI.transform.SetParent(_contentTransform, false);
                memberUI.SetUserInfo(user);
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

    private void ClearMembers()
    {
        foreach(var memberUI in _membersUI)
        {
            Destroy(memberUI.gameObject);
        }
        _membersUI.Clear();
    }

}
