using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        _createLeaderBoardButton.onClick.AddListener(OnCreateLeaderboardClicked);
    }

    // Handler for button click - launches async method
    private void OnCreateLeaderboardClicked()
    {
        CreateLeaderBoardAsync();
    }

    // Async method to create leaderboard entry and retrieve data
    private async void CreateLeaderBoardAsync()
    {
        try
        {
            var metadata = new Dictionary<string, object>
            {
                { "level", 5 },
                { "difficulty", "hard" }
            };

            // Add entry with metadata using the new async method
            Debug.Log("Creating leaderboard entry with metadata...");
            await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync("test_leaderboard_metadata", 2000, metadata);
            Debug.Log("Leaderboard entry created successfully");

            // Retrieve leaderboard entries using the new async method
            Debug.Log("Retrieving leaderboard entries...");
            var leaderboardResponse = await GameFuseUser.CurrentUser.GetMyLeaderboardEntriesAsync(5, "test_leaderboard_metadata", true);

            // Display leaderboard entries
            if (leaderboardResponse.LeaderboardEntries != null && leaderboardResponse.LeaderboardEntries.Length > 0)
            {
                Debug.Log("Got leaderboard entries for current user!");
                foreach (var entry in leaderboardResponse.LeaderboardEntries)
                {
                    Debug.Log($"{entry.Username}: {entry.Score}: {entry.LeaderboardName}");
                    
                    // Display metadata if available
                    if (entry.Metadata != null && entry.Metadata.Count > 0)
                    {
                        foreach (var kvPair in entry.Metadata)
                        {
                            Debug.Log($"{kvPair.Key}: {kvPair.Value}");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No leaderboard entries found");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error working with leaderboard: {ex.Message}");
        }
    }
}
