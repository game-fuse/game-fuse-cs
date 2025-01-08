using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;

public class AttributeUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _attributeKeyText;

    [SerializeField]
    TMP_InputField _attributeValueInput;

    [SerializeField]
    Button _updateAttributeButton;

    GroupAttribute _attribute;

    private void Start()
    {
        _updateAttributeButton.onClick.AddListener(UpdateAttribute);
    }

    public void SetAttribute(GroupAttribute attribute)
    {
        _attribute = attribute;
        _attributeKeyText.text = _attribute.Key;
        _attributeValueInput.text = _attribute.Value;
    }

    private async void UpdateAttribute()
    {
        try
        {
            _attribute = await GameFuseUser.CurrentUser.ModifyGroupAttributeAsync(_attribute.GroupId, _attribute.Key, _attributeValueInput.text);
            Debug.Log("Attribute Updated");
        }
        catch(ApiException ex)
        {
            Debug.Log($"ModifyGroupAttributeAsyn API Exception:\n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
        }
    }
}
