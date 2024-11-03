using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using System;

public class SignInUI : MonoBehaviour
{
    [SerializeField]
    private SignUpUI _signUpUI;

    [SerializeField]
    TMP_InputField _emailInput, _passwordInput, _resultInput;

    [SerializeField]
    Button _signUpButton;

    void Start()
    {
        _signUpButton.onClick.AddListener(SignUp);
        _signUpUI.OnSignUp += HandleSignUp;
    }

    private void OnDestroy()
    {
        _signUpUI.OnSignUp -= HandleSignUp;
    }


    private void SignUp()
    {
        GameFuse.SignIn(_emailInput.text, _passwordInput.text, SignInCallback);
    }

    private void SignInCallback(string message, bool hasError)
    {
        _resultInput.text = $"message: {message}, hasError: {hasError}";
    }
    private void HandleSignUp(string email, string password)
    {
        _emailInput.text = email;
        _passwordInput.text = password;
    }
}

