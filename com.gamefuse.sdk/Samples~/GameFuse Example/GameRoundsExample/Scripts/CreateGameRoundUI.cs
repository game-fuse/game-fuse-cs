using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFuseCSharp;
using TMPro;
using Newtonsoft.Json;

public class CreateGameRoundUI : MonoBehaviour
{
    [SerializeField]
    Toggle _multiPlayerToggle;

    [SerializeField]
    TMP_InputField _gameTypeInput, _resultInput;

    [SerializeField]
    TMP_Dropdown _difficultyDropDown;

    [SerializeField]
    Button _createGameRoundButton;

    // Start is called before the first frame update
    void Start()
    {
        _createGameRoundButton.onClick.AddListener(CreateGameRound);

        _difficultyDropDown.ClearOptions();
        _difficultyDropDown.AddOptions(new List<string> { "beginner", "intermediate", "expert" });
    }

    private async void CreateGameRound()
    {
        try
        {
            var gameRound = new GameRoundObject
            {
                GameUserId = GameFuseUser.CurrentUser.GetID(),
                GameType = _gameTypeInput.text,
                Multiplayer = _multiPlayerToggle.isOn,
                Metadata = new Dictionary<string, string>
                    {
                        { "difficulty",  _difficultyDropDown.options[_difficultyDropDown.value].text } 
                    }
            };

            var createdRound = await GameFuseUser.CurrentUser.CreateGameRoundAsync(gameRound);
            string multiplayerStatus = createdRound.Multiplayer ? "Multiplayer" : "Single Player";
            string resultMessage = $"Game Round Created Successfully!\n" +
                                 $"Type: {createdRound.GameType}\n" +
                                 $"Difficulty: {createdRound.Metadata["difficulty"]}\n" +
                                 $"Mode: {multiplayerStatus}\n" +
                                 $"Start Time: {createdRound.StartTime}";

            _resultInput.text = resultMessage;
            string gameRoundJson = JsonConvert.SerializeObject(createdRound, Formatting.Indented);
            Debug.Log(gameRoundJson);
        }
        catch (ApiException ex)
        {
            Debug.Log($"CreateGameRoundUI GameFuse API Exception:\n Status Code: {ex.StatusCode}, \n Message: {ex.Message}");
            _resultInput.text = $"CreateGameRound GameFuse API Exception:\n Status Code: {ex.StatusCode}, \n Message: {ex.Message}";
        }
    }
}
