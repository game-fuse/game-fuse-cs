using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFuseCSharp;
using TMPro;
public class MessagePanelUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField messageText;

    public void SetMessage(ChatMessage message)
    {
        messageText.text = message.Text;
    }

}
