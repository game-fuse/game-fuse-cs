using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;


public class MemberUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _userNameText;

    private UserInfo _userInfo;

    public void SetUserInfo(UserInfo userInfo)
    {
        _userInfo = userInfo;
        _userNameText.text = _userInfo.Username;
    }
}
