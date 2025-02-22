using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFuseCSharp;
using UnityEngine.UI;

public class CreateLeaderboardUI : MonoBehaviour
{

    [SerializeField]
    Button _createLeaderBoardButton;

    // Start is called before the first frame update
    void Start()
    {
        _createLeaderBoardButton.onClick.AddListener(CreateLeaderBoard);
    }

    // Update is called once per frame
    void CreateLeaderBoard()
    {
        var metadata = new Dictionary<string, string>
                {
                    { "level", "5" },
                    { "difficulty", "hard" }
                };

        // Add entry with metadata
        bool success = false;
        GameFuseUser.CurrentUser.AddLeaderboardEntry("test_leaderboard_metadata", 2000, metadata, LeaderboardEntryAdded2);

        Debug.Log("Created leader board with metada");
    }

    void LeaderboardEntryAdded2(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding leaderboard entry 2: " + message);
        }
        else
        {
            print("Set Leaderboard Entry 2");
            GameFuseUser.CurrentUser.GetLeaderboard(5, true, LeaderboardEntriesRetrieved);
        }
    }

    void LeaderboardEntriesRetrieved(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error loading leaderboard entries: " + message);
        }
        else
        {

            print("Got leaderboard entries for specific user!");
            foreach (GameFuseLeaderboardEntry entry in GameFuse.Instance.leaderboardEntries)
            {
                print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName());
                foreach (KeyValuePair<string, string> kvPair in entry.GetMetadata())
                {
                    Debug.Log(kvPair.Key + ": " + kvPair.Value);
                }

            }
           

        }
    }
}
