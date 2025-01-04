using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;

public class GroupScrollViewUI : MonoBehaviour
{
    [SerializeField]
    GroupUI _groupPrefab;

    [SerializeField]
    Button _refreshButton;

    [SerializeField]
    Transform _contentTransform;

    [SerializeField]
    TMP_InputField _resultInput;

    private List<GroupUI> _groupUIs = new List<GroupUI>();

    private void Start()
    {
        _refreshButton.onClick.AddListener(RefreshGUI);
    }

    private void RefreshGUI()
    {
        GetGroups();
    }

    private async void GetGroups()
    {
        try
        {
            GroupResponse[] groups = await GameFuseUser.CurrentUser.GetAllGroupsAsync();
            ClearGroups();
            foreach(var group in groups)
            {
                GroupUI groupUI = Instantiate(_groupPrefab, _contentTransform);
                groupUI.transform.SetParent(_contentTransform, false);
                groupUI.SetGroupInfo(group);
                _groupUIs.Add(groupUI);
            }
        }
        catch (ApiException ex)
        {
            string exceptionMessage = $"Get Groups Api Exception: \n Status Code: {ex.StatusCode} \n {ex.Message}";
            Debug.LogError(exceptionMessage);
            _resultInput.text = exceptionMessage;
        }
    }


    private void ClearGroups()
    {
        foreach(var groupUI in _groupUIs)
        {
            Destroy(groupUI.gameObject);
        }
        _groupUIs.Clear();
    }
}
