using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using System;

public class SignUpUI : MonoBehaviour
{
    public event Action<string, string> OnSignUp;

    string _userName, _email, _password;

    [SerializeField]
    TMP_InputField _userNameInput, _emailInput, _passwordInput, _resultInput;

    [SerializeField]
    Button _signUpButton;

    void Start()
    {
        int randomNumber = UnityEngine.Random.Range(0, 1000);
        _userName = $"dave{randomNumber}";
        _email = $"dave{randomNumber}@dave.com";
        _password = "password1234";
        _userNameInput.text = _userName;
        _emailInput.text = _email;
        _passwordInput.text = _password;
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
