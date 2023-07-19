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

        if (gameToken == "" || gameID == "")
        {
            EditorUtility.DisplayDialog("Add ID and Token", "Please add your token and ID, if you do not have one, you can create a free account from cloudlogin.dev", "OK");
            throw new Exception("Token and ID Invalid");
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

        var extraAttributes = new Dictionary<string, string>();
        extraAttributes.Add("deaths", "15");
        extraAttributes.Add("Jewels", "12");

        GameFuseUser.CurrentUser.AddLeaderboardEntry("TimeRound",10, extraAttributes, LeaderboardEntryAdded);
    }


    void LeaderboardEntryAdded(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding leaderboard entry: " + message);
        }
        else
        {

            print("Set Leaderboard Entry 2");
            var extraAttributes = new Dictionary<string, string>();
            extraAttributes.Add("deaths", "25");
            extraAttributes.Add("Jewels", "15");

            GameFuseUser.CurrentUser.AddLeaderboardEntry("TimeRound", 7, extraAttributes, LeaderboardEntryAdded2);

        }
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
            foreach( GameFuseLeaderboardEntry entry in GameFuse.Instance.leaderboardEntries)
            {
                print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName() );
                foreach (KeyValuePair<string,string> kvPair in entry.GetExtraAttributes())
                {
                    print(kvPair.Key + ": " + kvPair.Value);
                }
                
            }
            GameFuse.Instance.GetLeaderboard(5, true, "TimeRound", LeaderboardEntriesRetrievedAll);

        }
    }

    void LeaderboardEntriesRetrievedAll(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error loading leaderboard entries: " + message);
        }
        else
        {
            print("Got leaderboard entries for whole game!");
            foreach (GameFuseLeaderboardEntry entry in GameFuse.Instance.leaderboardEntries)
            {
                print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName());
                foreach (KeyValuePair<string, string> kvPair in entry.GetExtraAttributes())
                {
                    print(kvPair.Key + ": " + kvPair.Value);
                }

            }

        }
        print("GameFuse Test Complete");
    }




}
