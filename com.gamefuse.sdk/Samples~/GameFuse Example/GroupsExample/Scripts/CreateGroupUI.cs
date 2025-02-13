using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using System;

public class CreateGroupUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField _groupNameInput, _groupSizeInput, _resultInput;

    [SerializeField]
    Toggle _autoJoinToggle, _inviteOnlyToggle;

    [SerializeField]
    Button _createGroupButton;

    private void Start()
    {
        _createGroupButton.onClick.AddListener(CreateGroupRequest);
    }

    private async void CreateGroupRequest()
    {
        try
        {
            CreateGroupRequest groupRequest = new CreateGroupRequest();
            groupRequest.Name = _groupNameInput.text;
            groupRequest.MaxGroupSize = int.Parse(_groupSizeInput.text);
            groupRequest.CanAutoJoin = _autoJoinToggle.isOn;
            groupRequest.IsInviteOnly = _inviteOnlyToggle.isOn;

            GroupResponse groupResponse = await GameFuseUser.CurrentUser.CreateGroupAsync(groupRequest);
            _resultInput.text = $"Create Group request Succeeded\n Created Group: {groupResponse.Name} Id: {groupResponse.Id} \n Can Auto Join: {groupResponse.CanAutoJoin}\n Is Invite Only: {groupResponse.IsInviteOnly}";
        }
        catch (ApiException ex)
        {
            Debug.Log($"CreateGroupRequestUI GameFuse API Exception:\n Status Code: {ex.StatusCode}, \n Message: {ex.Message}");
            _resultInput.text = $"CreateGroupRequestUI GameFuse API Exception:\n Status Code: {ex.StatusCode}, \n Message: {ex.Message}";
        }

    }
}
