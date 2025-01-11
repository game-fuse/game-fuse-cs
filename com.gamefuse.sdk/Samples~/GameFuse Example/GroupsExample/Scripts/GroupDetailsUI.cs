using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;
using System.Threading.Tasks;

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
    AttributeUI _attributeUIPrefab;

    [SerializeField]
    Transform _contentTransform, _attributeContentTransform;

    [SerializeField]
    TMP_InputField _resultInput, _inviteUserId, _attributeKey, _attributeValue;

    [SerializeField]
    TextMeshProUGUI _inviteMessage;

    [SerializeField]
    Button memberTabButton, adminTabButton, joinRequestTabButton, invitesTabButton, inviteButton, addAttributesButton;

    private List<MemberUI> _membersUI = new List<MemberUI>();

    private List<JoinRequestUI> _joinRequestsUI = new List<JoinRequestUI>();

    private List<AttributeUI> _attributesUI = new List<AttributeUI>();

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
        inviteButton.onClick.AddListener(InviteToGroup);
        addAttributesButton.onClick.AddListener(AddAttribute);
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
        GetAttributes();
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
        inviteButton.interactable = true;
        addAttributesButton.interactable = true;
    }

    private async void InviteToGroup()
    {
        try
        {
            GroupConnectionRequest connectionRequest = new GroupConnectionRequest
            {
                GroupId = _groupId,
                UserId = int.Parse(_inviteUserId.text)
            };
            await GameFuseUser.CurrentUser.SendGroupConnectionRequestAsync(connectionRequest);
            _inviteMessage.text = $"Invite sent to {_inviteUserId.text}";
            _inviteUserId.text = string.Empty;
            await Task.Delay(3000);
            _inviteMessage.text = string.Empty;

        }
        catch(ApiException ex)
        {
            string exceptionMessage = $"Send Group Connection Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
            Debug.LogError(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
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

    private async void AddAttribute()
    {
        try
        {
            addAttributesButton.interactable = false;
            GroupAttributeRequest attribute = new GroupAttributeRequest
            {
                Key = _attributeKey.text,
                Value = _attributeValue.text
            };
            await GameFuseUser.CurrentUser.AddGroupAttributeAsync(_groupId, attribute);
            _attributeKey.text = string.Empty;
            _attributeValue.text = string.Empty;
            GetAttributes();

        }
        catch(ApiException ex)
        {
            string exceptionMessage = $"Add attribute api exception: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}";
            Debug.Log(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
    }

    private async void GetAttributes()
    {
        try
        {
            GroupAttributesResponse groupAttributesResponse = await GameFuseUser.CurrentUser.GetGroupAttributesAsync(_groupId);
            ClearAttributes();
            foreach(var groupAttribute in groupAttributesResponse.Attributes)
            {
                AttributeUI attributeUI = Instantiate(_attributeUIPrefab, _attributeContentTransform);
                attributeUI.transform.SetParent(_attributeContentTransform, false);
                attributeUI.SetAttribute(groupAttribute);
                _attributesUI.Add(attributeUI);
            }
            addAttributesButton.interactable = true;
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
        _joinRequestsUI.Clear();
    }

    private void ClearAttributes()
    {
        foreach(var attributeUI in _attributesUI)
        {
            Destroy(attributeUI.gameObject);
        }
        _attributesUI.Clear();
    }

}
