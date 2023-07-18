﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Networking;

namespace GameConnectCSharp
{
    public class GameConnectUser : MonoBehaviour
    {

        #region instance vars
        private bool signedIn = false;
        private int numberOfLogins;
        private DateTime lastLogin;
        private string authenticationToken;
        private string username;
        private int score;
        private int credits;
        private int id;
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        private List<GameConnectStoreItem> purchasedStoreItems = new List<GameConnectStoreItem>();

        #endregion


        #region instance setters
        internal void SetSignedInInternal()
        {
            this.signedIn = true;

        }
        internal void SetNumberOfLoginsInternal(int numberOfLogins)
        {
            this.numberOfLogins = numberOfLogins;
        }
        internal void SetLastLoginInternal(DateTime lastLogin)
        {
            this.lastLogin = lastLogin;
        }
        internal void SetAuthenticationTokenInternal(string authenticationToken)
        {
            this.authenticationToken = authenticationToken;
        }
        internal void SetUsernameInternal(string username)
        {
            this.username = username;
        }
        internal void SetScoreInternal(int score)
        {
            this.score = score;
        }
        internal void SetCreditsInternal(int credits)
        {
            this.credits = credits;
        }
        internal void SetIDInternal(int id)
        {
            this.id = id;
        }

        #endregion


        #region instance getters
        public bool IsSignedIn()
        {
            return signedIn;
        }
        public int GetNumberOfLogins()
        {
            return numberOfLogins;
        }
        public DateTime GetLastLogin()
        {
            return lastLogin;
        }
        internal string GetAuthenticationToken()
        {
            return authenticationToken;
        }
        public string GetUsername()
        {
            return username;
        }
        public int GetScore()
        {
            return score;
        }
        public int GetCredits()
        {
            return credits;
        }
        internal int GetID()
        {
            return id;
        }
        #endregion


        #region singleton management
        private static GameConnectUser _instance;
        public static GameConnectUser CurrentUser { get { return _instance; } }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion



        #region request: add credits
        public void AddCredits(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(AddCreditsRoutine(credits, callback));
        }

        private IEnumerator AddCreditsRoutine(int credits, Action<string, bool> callback = null)
        {

            GameConnect.Log("GameConnectUser Add Credits: " + credits.ToString());
            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("credits", credits);
            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/add_credits", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Add Credits Success: " + credits.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));

            }
            else
            {
                GameConnect.Log("GameConnectUser Add Credits Failure: " + credits.ToString());

            }
            GameConnectUtilities.HandleCallback(request, "Credits have been added to user", callback);


        }
        #endregion

        #region request: set credits
        public void SetCredits(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(SetCreditsRoutine(credits, callback));
        }

        private IEnumerator SetCreditsRoutine(int credits, Action<string, bool> callback = null)
        {

            GameConnect.Log("GameConnectUser Set Credits: " + credits.ToString());

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("credits", credits);
            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/set_credits", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Set Credits Success: " + credits.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
            }
            else
            {
                GameConnect.Log("GameConnectUser Set Credits Failure: " + credits.ToString());

            }

            GameConnectUtilities.HandleCallback(request, "Credits have been added to user", callback);
        }
        #endregion

        #region request: add score
        public void AddScore(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(AddScoreRoutine(credits, callback));
        }

        private IEnumerator AddScoreRoutine(int score, Action<string, bool> callback = null)
        {

            GameConnect.Log("GameConnectUser Add Score: " + score.ToString());

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("score", score);

            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/add_score", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Add Score Succcess: " + score.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
            }

            GameConnectUtilities.HandleCallback(request, "Score have been added to user", callback);
        }
        #endregion

        #region request: set score
        public void SetScore(int score, Action<string, bool> callback = null)
        {
            StartCoroutine(SetScoreRoutine(score, callback));
        }

        private IEnumerator SetScoreRoutine(int score, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Set Score: " + score.ToString());

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("score", score);
            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/set_score", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Set Score Success: " + score.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
            }

            GameConnectUtilities.HandleCallback(request, "Score have been added to user", callback);
        }
        #endregion


        #region attributes


        internal void DownloadAttributes(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadAttributesRoutine(chainedFromLogin, callback));

        }

        private IEnumerator DownloadAttributesRoutine(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Get Attributes");

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken();


            var request = UnityWebRequest.Get(GameConnect.GetBaseURL() + "/users/" + this.id + "/game_user_attributes"+ parameters);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Get Attributes Success");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_attributes = json.GetArray("game_user_attributes");

                attributes.Clear();
                foreach (var attribute in game_user_attributes)
                {
                    attributes.Add(attribute.Obj.GetString("key"), attribute.Obj.GetString("value"));
                }
                DownloadStoreItems(chainedFromLogin, callback);
            } else 
                GameConnectUtilities.HandleCallback(request, chainedFromLogin ? "Users has been signed in successfully" : "Users attributes have been downloaded", callback);

        }

        public Dictionary<string,string> GetAttributes()
        {
            return attributes;
        }

        public Dictionary<string, string>.KeyCollection GetAttributesKeys()
        {
            return attributes.Keys;
        }

        public string GetAttributeValue(string key)
        {
            if (attributes.ContainsKey(key)){
                return attributes[key];
            }
            else
                return "";
        }

        public void SetAttribute(string key, string value, Action<string, bool> callback = null)
        {
            StartCoroutine(SetAttributeRoutine(key,value, callback));
        }

        private IEnumerator SetAttributeRoutine(string key, string value, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Set Attributes: "+key);

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("key", key);
            form.AddField("value", value);

            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/add_game_user_attribute", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Set Attributes Success: " + key);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                if (attributes.ContainsKey(key))
                {
                    attributes.Remove(key);
                }
                attributes.Add(key, value);
                foreach (var attribute in attributes)
                {
                    print(attribute.Key + "," + attribute.Value);
                }
            }

            GameConnectUtilities.HandleCallback(request, "Attribute has been added to user", callback);
        }

        public void RemoveAttribute(string key, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveAttributeRoutine(key,callback));
        }

        private IEnumerator RemoveAttributeRoutine(string key, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Remove Attributes: " + key);

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&game_user_attribute_key=" + key;
            var request = UnityWebRequest.Get(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/remove_game_user_attributes" + parameters);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Remove Attributes Success: " + key);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_attributes = json.GetArray("game_user_attributes");

                print("ATTRIBUTES CLEARED DUE TO KEY REMOVAL:");
                attributes.Clear();
                foreach (var attribute in game_user_attributes)
                {
                    print("adding: "+attribute.Obj.GetString("key"));
                    attributes.Add(attribute.Obj.GetString("key"), attribute.Obj.GetString("value"));
                }
            }

            GameConnectUtilities.HandleCallback(request, "Attribute has been removed", callback);
        }


        #endregion

        #region store items
        internal void DownloadStoreItems(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadStoreItemsRoutine(chainedFromLogin, callback));

        }

        private IEnumerator DownloadStoreItemsRoutine(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Download Store Items: ");

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken();


            var request = UnityWebRequest.Get(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/game_user_store_items" + parameters);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Download Store Items Success: ");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_store_items = json.GetArray("game_user_store_items");
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new GameConnectStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id"))));
                }


            }

            GameConnectUtilities.HandleCallback(request, chainedFromLogin ? "Users has been signed in successfully" : "Users store items have been downloaded", callback);

        }

        public List<GameConnectStoreItem> GetPurchasedStoreItems()
        {
            return purchasedStoreItems;
        }

        public void PurchaseStoreItem(GameConnectStoreItem storeItem, Action<string, bool> callback = null)
        {
            StartCoroutine(PurchaseStoreItemRoutine(storeItem.GetId(), callback));
        }

        public void PurchaseStoreItem(int storeItemId, Action<string, bool> callback = null)
        {
            StartCoroutine(PurchaseStoreItemRoutine(storeItemId, callback));
        }

        private IEnumerator PurchaseStoreItemRoutine(int storeItemId, Action<string, bool> callback = null)
        {

            GameConnect.Log("GameConnectUser Purchase Store Items: ");

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("store_item_id", storeItemId.ToString());

            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/purchase_game_user_store_item", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Purchase Store Items Success: ");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_store_items = json.GetArray("game_user_store_items");
                CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new GameConnectStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id"))));
                }
            }

            GameConnectUtilities.HandleCallback(request, "Store Item has been purchased by user", callback);
        }

        public void RemoveStoreItem(int storeItemID, bool reimburseUser, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveStoreItemRoutine(storeItemID, reimburseUser, callback));
        }
        public void RemoveStoreItem(GameConnectStoreItem storeItem, bool reimburseUser, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveStoreItemRoutine(storeItem.GetId(), reimburseUser, callback));
        }

        private IEnumerator RemoveStoreItemRoutine(int storeItemID, bool reimburseUser, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Remove Store Item: "+ storeItemID);

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&store_item_id=" + storeItemID+ "&reimburse=" + reimburseUser.ToString();
            var request = UnityWebRequest.Get(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/remove_game_user_store_item" + parameters);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Remove Store Item Success: " + storeItemID);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                var game_user_store_items = json.GetArray("game_user_store_items");
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new GameConnectStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id"))));
                }
            }

            GameConnectUtilities.HandleCallback(request, "Store Item has been removed", callback);
        }

        #endregion

        #region Leaderboard

        public void AddLeaderboardEntry(string leaderboardName, int score, Dictionary<string, string> extraAttributes = null, Action<string, bool> callback = null)
        {
            StartCoroutine(AddLeaderboardEntryRoutine(leaderboardName, score, extraAttributes, callback));
        }

        public void AddLeaderboardEntry(string leaderboardName, int score, Action<string, bool> callback = null)
        {
            StartCoroutine(AddLeaderboardEntryRoutine(leaderboardName, score, new Dictionary<string, string>(), callback));
        }


        private IEnumerator AddLeaderboardEntryRoutine(string leaderboardName, int score, Dictionary<string, string> extraAttributes, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Adding Leaderboard Entry: " + leaderboardName + ": "+score.ToString());

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            List<string> extraAttributesList = new List<string>();
            foreach (KeyValuePair<string, string> entry in extraAttributes)
            {
                extraAttributesList.Add("\""+entry.Key.ToString() + "\": " + entry.Value.ToString());
            }
            
            string extraAttributesJson = "{" + String.Join(", ", extraAttributesList.ToArray())+ "}";
            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("leaderboard_name", leaderboardName);
            form.AddField("extra_attributes", extraAttributesJson);
            form.AddField("score", score);

            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/add_leaderboard_entry", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Add Leaderboard Entry: " + leaderboardName+ ": "+score);

                var data = request.downloadHandler.text;
            }

            GameConnectUtilities.HandleCallback(request, "Leaderboard Entry Has Been Added", callback);
        }

        public void ClearLeaderboardEntries(string leaderboardName, Action<string, bool> callback = null)
        {
            StartCoroutine(ClearLeaderboardEntriesRoutine(leaderboardName, callback));
        }


        private IEnumerator ClearLeaderboardEntriesRoutine(string leaderboardName, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Clearing Leaderboard Entry: " + leaderboardName);

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("leaderboard_name", leaderboardName);

            var request = UnityWebRequest.Post(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/clear_my_leaderboard_entries", form);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Clear Leaderboard Entry: " + leaderboardName);

                var data = request.downloadHandler.text;
            }

            GameConnectUtilities.HandleCallback(request, "Leaderboard Entries Have Been Cleared", callback);
        }

        public void GetLeaderboard(int limit, bool onePerUser, Action<string, bool> callback = null)
        {
            StartCoroutine(GetLeaderboardRoutine(limit, onePerUser, callback));
        }

        private IEnumerator GetLeaderboardRoutine(int limit, bool onePerUser, Action<string, bool> callback = null)
        {
            GameConnect.Log("GameConnectUser Get Leaderboard: " +limit.ToString());

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&limit=" + limit.ToString()+ "&one_per_user="+ onePerUser.ToString();
            var request = UnityWebRequest.Get(GameConnect.GetBaseURL() + "/users/" + CurrentUser.id + "/leaderboard_entries" + parameters);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnectUser Get Leaderboard Success: : " + limit.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Debug.Log("got " + json);
                var storeItems = json.GetArray("leaderboard_entries");
                GameConnect.Instance.leaderboardEntries.Clear();
                foreach (var storeItem in storeItems)
                {
                    GameConnect.Instance.leaderboardEntries.Add(new GameConnectLeaderboardEntry(
                        storeItem.Obj.GetString("username"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("score")),
                        storeItem.Obj.GetString("leaderboard_name"),
                        storeItem.Obj.GetString("extra_attributes"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("game_user_id"))
                        )
                   );
                }

            }

            GameConnectUtilities.HandleCallback(request, "Store Item has been removed", callback);
        }

        #endregion Leaderboard 



    }



}