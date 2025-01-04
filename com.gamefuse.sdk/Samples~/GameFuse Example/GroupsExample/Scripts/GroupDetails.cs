using UnityEngine;
using UnityEngine.UI;
using TMPro;
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


    public void Awake()
    {
        GroupUI.OnGroupUIClicked += SetValues;
    }

    public void OnDestroy()
    {
        GroupUI.OnGroupUIClicked -= SetValues;
    }

    public void SetValues(GroupResponse groupResponse)
    {
        _idInput.text = groupResponse.Id.ToString();
        _nameInput.text = groupResponse.Name;
        _typeInput.text = groupResponse.GroupType;
        _autoJoinInput.text = groupResponse.CanAutoJoin.ToString();
        _inviteOnlyInput.text = groupResponse.IsInviteOnly.ToString();
        _maxGroupSizeInput.text = groupResponse.MaxGroupSize.ToString();
        _searchableInput.text = groupResponse.Searchable.ToString();
    }
}
