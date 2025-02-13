using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;
using System;

public class GroupUI : MonoBehaviour
{
    public static event Action<GroupResponse> OnGroupUIClicked;

    [SerializeField]
    TextMeshProUGUI _groupNameText, _memberCountText;

    [SerializeField]
    Button _panelButton;

    private GroupResponse _groupResponse;

    private void Start()
    {
        _panelButton.onClick.AddListener(PanelClicked);
    }

    public void SetGroupInfo(GroupResponse groupResponse)
    {
        _groupResponse = groupResponse;
        _groupNameText.text = _groupResponse.Name;
        _memberCountText.text = $"Members: {_groupResponse.MemberCount.ToString()} / {_groupResponse.MaxGroupSize} ";
    }

    public void PanelClicked()
    {
        if(_groupResponse != null)
        {
            Debug.Log($"Clicked {_groupResponse.Name}");
            OnGroupUIClicked?.Invoke(_groupResponse);
        }
        else
        {
            Debug.Log("Group not set");
        }
    }
}
