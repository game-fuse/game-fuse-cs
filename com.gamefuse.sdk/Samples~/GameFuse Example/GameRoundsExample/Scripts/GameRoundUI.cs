using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameFuseCSharp;
using System;

public class GameRoundUI : MonoBehaviour
{
    public static event Action OnGameRoundDeleted;
    public static event Action<string> OnGameRoundServiceException;


    [SerializeField]
    TextMeshProUGUI _gameTypeText, _multiPlayerText, _difficultyText;

    [SerializeField]
    Button _deleteRoundButton;

    private const string DIFFICULTY_KEY = "difficulty";

    GameRoundObject _gameRound;

    private void Start()
    {
        _deleteRoundButton.onClick.AddListener(DeleteGameRound);
    }

    public void SetRoundInfo(GameRoundObject gameRound)
    {
        _gameRound = gameRound;
        _gameTypeText.text = _gameRound.GameType;
        if (_gameRound.Metadata.ContainsKey(DIFFICULTY_KEY))
        {
            _difficultyText.text = _gameRound.Metadata[DIFFICULTY_KEY];
        }
        else
        {
            _difficultyText.text = "Not Set";
        }
        _multiPlayerText.text = _gameRound.Multiplayer ? "Multiplayer" : "Single Player";
    }

    public async void DeleteGameRound()
    {
        try
        {
            await GameFuseUser.CurrentUser.DeleteGameRoundAsync(_gameRound.Id);
            OnGameRoundDeleted?.Invoke();
        }
        catch(ApiException ex)
        {
            string exceptionMessage = $"Delete Game Round Exception: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}";
            Debug.LogError(exceptionMessage, gameObject);
            OnGameRoundServiceException?.Invoke(exceptionMessage);
        }
    }

    
}
