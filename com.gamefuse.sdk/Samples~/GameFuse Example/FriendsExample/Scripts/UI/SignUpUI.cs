using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using System;

public class SignUpUI : MonoBehaviour
{
    public event Action<string, string> OnSignUp;

    [SerializeField]
    TMP_InputField _userNameInput, _emailInput, _passwordInput, _resultInput;

    [SerializeField]
    Button _signUpButton;

    void Start()
    {
        _signUpButton.onClick.AddListener(SignUp);
    }

    private void SignUp()
    {
        GameFuse.SignUp(_emailInput.text, _passwordInput.text, _passwordInput.text, _userNameInput.text, SignUpCallback);
    }

    private void SignUpCallback(string message, bool hasError)
    {
        _resultInput.text = $"message: {message}, hasError: {hasError}";
        OnSignUp?.Invoke(_emailInput.text, _passwordInput.text);
    }
}
