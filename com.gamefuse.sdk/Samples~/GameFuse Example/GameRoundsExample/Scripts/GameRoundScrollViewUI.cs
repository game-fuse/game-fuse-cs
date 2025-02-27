using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFuseCSharp;

public class GameRoundScrollViewUI : MonoBehaviour
{
    [SerializeField]
    GameRoundUI _gameRoundPrefab;

    [SerializeField]
    Button _refreshButton;

    [SerializeField]
    Transform _contentTransform;

    [SerializeField]
    TMP_InputField _resultInput;

    private List<GameRoundUI> _gameRounds = new List<GameRoundUI>();

    private void Start()
    {
        _refreshButton.onClick.AddListener(RefreshGUI);
        GameRoundUI.OnGameRoundDeleted += HandleGameRoundDeleted;
        GameRoundUI.OnGameRoundServiceException += HandleGameRoundServiceException;
    }

    private void RefreshGUI()
    {
        GetGameRounds();
    }

    private async void GetGameRounds()
    {
        try
        {
            GameRoundsResponse rounds = await GameFuseUser.CurrentUser.GetMyGameRoundsAsync();
            ClearRounds();
            foreach(var round in rounds.GameRounds)
            {
                GameRoundUI roundUI = Instantiate(_gameRoundPrefab, _contentTransform);
                roundUI.transform.SetParent(_contentTransform, false);
                roundUI.SetRoundInfo(round);
                _gameRounds.Add(roundUI);
            }
            _resultInput.text = $"Got {rounds.GameRounds.Length} rounds";
        }catch(ApiException ex)
        {
            string exceptionMessage = $"Get Game Rounds Api Exception: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}";
            Debug.LogError(exceptionMessage, gameObject);
            _resultInput.text = exceptionMessage;
        }
    } 

    private void ClearRounds()
    {
        foreach(var roundUI in _gameRounds)
        {
            Destroy(roundUI.gameObject);
        }
        _gameRounds.Clear();
    }

    private void HandleGameRoundDeleted()
    {
        RefreshGUI();
    }

    private void HandleGameRoundServiceException(string message)
    {
        _resultInput.text = message;
    }
}
