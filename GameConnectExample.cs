using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConnectCSharp;
using System.Linq;
using UnityEditor;
using Random = UnityEngine.Random;

public class GameConnectExample : MonoBehaviour {

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
            Debug.Log("GameConnect start");
            GameConnect.SetVerboseLogging(true);
            GameConnect.SetUpGame(gameID, gameToken, ApplicationSetUp, true);
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
        
            print("<color=yellow>GAME CONNECTED!!" + GameConnect.GetGameId() + "</color>");
            print("Store Items:");
            foreach (GameConnectStoreItem storeItem in GameConnect.GetStoreItems())
            {
                print("      " + storeItem.GetName() + ": " + storeItem.GetCost());
            }
            print("*****************************************");
            print("*****************************************");

            print("Signing Up");
            GameConnect.SignUp(userEmail, "password", "password", username, SignedUp);
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

            print("Before Credits: " + GameConnectUser.CurrentUser.GetCredits());

            GameConnectUser.CurrentUser.AddCredits(50, AddCreditsCallback);

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
            print("After Credits: " + GameConnectUser.CurrentUser.GetCredits());
            print("currently attribute color is null?"+ (GameConnectUser.CurrentUser.GetAttributeValue("Color") == null).ToString());
            print("<color=yellow>Setting attribute color = blue</color>");

            GameConnectUser.CurrentUser.SetAttribute("Color","Blue", SetAttributeCallback);

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
            print("currently attribute color is null?" + (GameConnectUser.CurrentUser.GetAttributeValue("Color") == null).ToString());
            print("currently attribute color " + GameConnectUser.CurrentUser.GetAttributeValue("Color"));
            print("<color=yellow>Setting attribute color = red</color>");
            GameConnectUser.CurrentUser.SetAttribute("Color","Red", updateAttributeCallback);

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
            print("currently attribute color is null?" + (GameConnectUser.CurrentUser.GetAttributeValue("Color") == "").ToString());
            print("currently attribute color " + GameConnectUser.CurrentUser.GetAttributeValue("Color"));
            print("deleting attribute color");
            GameConnectUser.CurrentUser.RemoveAttribute("Color",removeAttributeCallback);

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
            print("currently attribute color is null?" + (GameConnectUser.CurrentUser.GetAttributeValue("Color") == "").ToString());
            print("currently attribute color " + GameConnectUser.CurrentUser.GetAttributeValue("Color"));
            print("ALL STORE ITEMS:");
            foreach (var pair in GameConnectUser.CurrentUser.GetAttributes())
            {

                Console.WriteLine("Key: " + pair.Key + ", Value: " + pair.Value);
            }
            var item = GameConnect.GetStoreItems().First();
            print("Purchase Store Item: " + item.GetName() + ": " + item.GetCost());
            GameConnectUser.CurrentUser.PurchaseStoreItem(GameConnect.GetStoreItems().First(), PurchasedItem);

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
            print("Current Credits: " + GameConnectUser.CurrentUser.GetCredits());
        }

        var extraAttributes = new Dictionary<string, string>();
        extraAttributes.Add("deaths", "15");
        extraAttributes.Add("Jewels", "12");

        GameConnectUser.CurrentUser.AddLeaderboardEntry("TimeRound",10, extraAttributes, LeaderboardEntryAdded);
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

            GameConnectUser.CurrentUser.AddLeaderboardEntry("TimeRound", 7, extraAttributes, LeaderboardEntryAdded2);

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
            GameConnectUser.CurrentUser.GetLeaderboard(5, true, LeaderboardEntriesRetrieved);
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
            foreach( GameConnectLeaderboardEntry entry in GameConnect.Instance.leaderboardEntries)
            {
                print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName() );
                foreach (KeyValuePair<string,string> kvPair in entry.GetExtraAttributes())
                {
                    print(kvPair.Key + ": " + kvPair.Value);
                }
                
            }
            GameConnect.Instance.GetLeaderboard(5, true, "TimeRound", LeaderboardEntriesRetrievedAll);

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
            foreach (GameConnectLeaderboardEntry entry in GameConnect.Instance.leaderboardEntries)
            {
                print(entry.GetUsername() + ": " + entry.GetScore().ToString() + ": " + entry.GetLeaderboardName());
                foreach (KeyValuePair<string, string> kvPair in entry.GetExtraAttributes())
                {
                    print(kvPair.Key + ": " + kvPair.Value);
                }

            }

        }
        print("GameConnect Test Complete");
    }




}
