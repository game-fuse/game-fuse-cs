using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using System;

public class SetUpGameUI : MonoBehaviour
{
    [SerializeField]
    string _gameTitle, _gameId, _gameToken;

    [SerializeField]
    TMP_InputField _gameTitleInput, _gameIdInput, _gameTokenInput, _resultInput;

    

    [SerializeField]
    Button _setUpGameButton;

    void Start()
    {
        _gameTitleInput.text = _gameTitle;
        _gameIdInput.text = _gameId;
        _gameTokenInput.text = _gameToken;
        _setUpGameButton.onClick.AddListener(SetUpGame);
        
    }

    private void SetUpGame()
    {
        GameFuse.SetUpGame(_gameIdInput.text, _gameTokenInput.text, SetUpGameCallback);
    }

    private void SetUpGameCallback(string message, bool hasError)
    {
        _resultInput.text = $"message: {message}, hasError: {hasError}";
    }
}
