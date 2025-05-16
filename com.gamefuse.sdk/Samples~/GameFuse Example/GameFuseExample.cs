using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFuseCSharp;
using System.Linq;
using UnityEditor;
using Random = UnityEngine.Random;

public class GameFuseExample : MonoBehaviour {

    public string gameToken = "";
    public string gameID = "";

    string userEmail = "tom@mundo.com";
    string username = "tommundo";

    // Use this for initialization
    void Start () {
        var random = ((int)(Random.value * 10000)).ToString();
        userEmail = "tom"+ random + "@mundo.com";
        username = "tommundo" + random;

        if (string.IsNullOrEmpty(gameToken) || string.IsNullOrEmpty(gameID))
        {
            Debug.LogWarning("Add ID and Token: Please add your token and ID. If you do not have one, you can create a free account from gamefuse.co");
            throw new System.Exception("Token and ID Invalid");
        }
        else
        {
            Debug.Log("GameFuse start");
            GameFuse.SetVerboseLogging(true);
            GameFuse.SetUpGame(gameID, gameToken, ApplicationSetUp, true);
        }

       
    }

    void ApplicationSetUp(string message, bool hasError)
    {
        if (hasError)
        {
            print("error setting aplication");
            print(message);
        }
        else
        {
        
            print("<color=yellow>GAME CONNECTED!!" + GameFuse.GetGameId() + "</color>");
            print("Store Items:");
            foreach (GameFuseStoreItem storeItem in GameFuse.GetStoreItems())
            {
                print("      " + storeItem.GetName() + ": " + storeItem.GetCost());
            }
            print("*****************************************");
            print("*****************************************");

            print("Signing Up");

            GameFuse.SignUp(userEmail, "password", "password", username, SignedUp);
        }


    }

    void SignedUp(string message, bool hasError)
    {

        if (hasError)
        {
            print("Error signign up: "+message);
        }
        else
        {
            print("signed up!");
            print("<color=yellow>Adding credits!</color>");

            print("Before Credits: " + GameFuseUser.CurrentUser.GetCredits());

            GameFuseUser.CurrentUser.AddCredits(50, AddCreditsCallback);

        }

    }

    void AddCreditsCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding credits: " + message);
        }
        else
        {
            print("After Credits: " + GameFuseUser.CurrentUser.GetCredits());
            print("currently attribute color is null?"+ (GameFuseUser.CurrentUser.GetAttributeValue("Color") == null).ToString());
            print("<color=yellow>Setting attribute color = blue</color>");

            GameFuseUser.CurrentUser.SetAttribute("Color","Blue", SetAttributeCallback);

        }
       

    }

    void SetAttributeCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding attribute: " + message);
        }
        else
        {
            print("currently attribute color is null?" + (GameFuseUser.CurrentUser.GetAttributeValue("Color") == null).ToString());
            print("currently attribute color " + GameFuseUser.CurrentUser.GetAttributeValue("Color"));
            print("<color=yellow>Setting attribute color = red</color>");
            GameFuseUser.CurrentUser.SetAttribute("Color","Red", updateAttributeCallback);

        }
    }

    void updateAttributeCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding attribute: " + message);
        }
        else
        {
            print("currently attribute color is null?" + (GameFuseUser.CurrentUser.GetAttributeValue("Color") == "").ToString());
            print("currently attribute color " + GameFuseUser.CurrentUser.GetAttributeValue("Color"));
            print("deleting attribute color");
            GameFuseUser.CurrentUser.RemoveAttribute("Color",removeAttributeCallback);

        }
    }

    void removeAttributeCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error removing attribute: " + message);
        }
        else
        {
            print("currently attribute color is null?" + (GameFuseUser.CurrentUser.GetAttributeValue("Color") == "").ToString());
            print("currently attribute color " + GameFuseUser.CurrentUser.GetAttributeValue("Color"));
            print("ALL STORE ITEMS:");
            foreach (var pair in GameFuseUser.CurrentUser.GetAttributes())
            {

                Console.WriteLine("Key: " + pair.Key + ", Value: " + pair.Value);
            }
            var item = GameFuse.GetStoreItems().First();
            print("Purchase Store Item: " + item.GetName() + ": " + item.GetCost());
            GameFuseUser.CurrentUser.PurchaseStoreItem(GameFuse.GetStoreItems().First(), PurchasedItem);

        }
    }

    void PurchasedItem(string message, bool hasError)
    {
        if (hasError)
        {
            Debug.Log("Error purchasing item: " + message);
        }
        else
        {
            print("Purchased Item");
            print("Current Credits: " + GameFuseUser.CurrentUser.GetCredits());
        }

        // Add metadata for leaderboard entry
        var extraAttributes = new Dictionary<string, object>
        {
            { "deaths", 15 },
            { "Jewels", 12 }
        };

        // Start asynchronous leaderboard operations
        AddLeaderboardEntriesAsync(extraAttributes);
    }

    // Async method to handle all leaderboard operations
    private async void AddLeaderboardEntriesAsync(Dictionary<string, object> initialAttributes)
    {
        try
        {
            // Add first leaderboard entry
            print("Adding first leaderboard entry...");
            await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync("TimeRound", 10, initialAttributes);
            print("First leaderboard entry added successfully");

            // Add second leaderboard entry with different metadata
            var secondAttributes = new Dictionary<string, object>
            {
                { "deaths", 25 },
                { "Jewels", 15 }
            };
            
            print("Adding second leaderboard entry...");
            await GameFuseUser.CurrentUser.AddLeaderboardEntryAsync("TimeRound", 7, secondAttributes);
            print("Second leaderboard entry added successfully");

            // Get user's own leaderboard entries
            print("Retrieving user's leaderboard entries...");
            var userEntries = await GameFuseUser.CurrentUser.GetMyLeaderboardEntriesAsync(5, "TimeRound", true);
            
            print("Got leaderboard entries for current user!");
            if (userEntries.LeaderboardEntries != null)
            {
                foreach (var entry in userEntries.LeaderboardEntries)
                {
                    print($"{entry.Username}: {entry.Score}: {entry.LeaderboardName}");
                    if (entry.Metadata != null && entry.Metadata.Count > 0)
                    {
                        foreach (var kvPair in entry.Metadata)
                        {
                            print($"{kvPair.Key}: {kvPair.Value}");
                        }
                    }
                }
            }

            // Get game-wide leaderboard entries
            string gameIdStr = GameFuse.GetGameId();
            if (!string.IsNullOrEmpty(gameIdStr))
            {
                print("Retrieving game-wide leaderboard entries...");
                var gameEntries = await GameFuseUser.CurrentUser.GetGameLeaderboardEntriesAsync("TimeRound", 5);
                
                print("Got leaderboard entries for whole game!");
                if (gameEntries.LeaderboardEntries != null)
                {
                    foreach (var entry in gameEntries.LeaderboardEntries)
                    {
                        print($"{entry.Username}: {entry.Score}: {entry.LeaderboardName}");
                        if (entry.Metadata != null && entry.Metadata.Count > 0)
                        {
                            foreach (var kvPair in entry.Metadata)
                            {
                                print($"{kvPair.Key}: {kvPair.Value}");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            print($"Error in leaderboard operations: {ex.Message}");
        }
        
        print("GameFuse Test Complete");
    }




}
