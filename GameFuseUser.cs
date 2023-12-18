using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    public class GameFuseUser : MonoBehaviour
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
        private Dictionary<string, string> dirtyAttributes = new Dictionary<string, string>();
        private List<GameFuseStoreItem> purchasedStoreItems = new List<GameFuseStoreItem>();

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
        private static GameFuseUser _instance;
        public static GameFuseUser CurrentUser { get { return _instance; } }
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

            GameFuse.Log("GameFuseUser Add Credits: " + credits.ToString());
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("credits", credits);
            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/add_credits", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Add Credits Success: " + credits.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));

            }
            else
            {
                GameFuse.Log("GameFuseUser Add Credits Failure: " + credits.ToString());

            }
            GameFuseUtilities.HandleCallback(request, "Credits have been added to user", callback);
            request.Dispose();



        }
        #endregion

        #region request: set credits
        public void SetCredits(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(SetCreditsRoutine(credits, callback));
        }

        private IEnumerator SetCreditsRoutine(int credits, Action<string, bool> callback = null)
        {

            GameFuse.Log("GameFuseUser Set Credits: " + credits.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("credits", credits);
            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/set_credits", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Set Credits Success: " + credits.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
            }
            else
            {
                GameFuse.Log("GameFuseUser Set Credits Failure: " + credits.ToString());

            }

            GameFuseUtilities.HandleCallback(request, "Credits have been added to user", callback);
            request.Dispose();

        }
        #endregion

        #region request: add score
        public void AddScore(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(AddScoreRoutine(credits, callback));
        }

        private IEnumerator AddScoreRoutine(int score, Action<string, bool> callback = null)
        {

            GameFuse.Log("GameFuseUser Add Score: " + score.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("score", score);

            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/add_score", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Add Score Succcess: " + score.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
            }

            GameFuseUtilities.HandleCallback(request, "Score have been added to user", callback);
            request.Dispose();

        }
        #endregion

        #region request: set score
        public void SetScore(int score, Action<string, bool> callback = null)
        {
            StartCoroutine(SetScoreRoutine(score, callback));
        }

        private IEnumerator SetScoreRoutine(int score, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Set Score: " + score.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("score", score);
            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/set_score", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Set Score Success: " + score.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
            }

            GameFuseUtilities.HandleCallback(request, "Score have been added to user", callback);
            request.Dispose();

        }
        #endregion


        #region attributes


        internal void DownloadAttributes(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadAttributesRoutine(chainedFromLogin, callback));

        }

        private IEnumerator DownloadAttributesRoutine(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Get Attributes");

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken();


            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/users/" + this.id + "/game_user_attributes"+ parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Get Attributes Success");

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
                GameFuseUtilities.HandleCallback(request, chainedFromLogin ? "Users has been signed in successfully" : "Users attributes have been downloaded", callback);
            request.Dispose();


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

        public void SetAttributeLocal(string key, string  val){
            if (attributes.ContainsKey(key))
            {
                attributes.Remove(key);
            }
            if (dirtyAttributes.ContainsKey(key))
            {
                dirtyAttributes.Remove(key);
            }
            attributes.Add(key, val);
            dirtyAttributes.Add(key,val);
        }

        public void SyncLocalAttributes(Action<string, bool> callback = null)
        {
            SetAttributes(attributes, callback, true);
        }

        public Dictionary<string,string> GetDirtyAttributes(){
            return dirtyAttributes;
        }

        public void SetAttribute(string key, string value, Action<string, bool> callback = null)
        {
            StartCoroutine(SetAttributeRoutine(key,value, callback));
        }

        private IEnumerator SetAttributeRoutine(string key, string value, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Set Attributes: "+key);

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("key", key);
            form.AddField("value", value);

            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/add_game_user_attribute", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Set Attributes Success: " + key);

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

            GameFuseUtilities.HandleCallback(request, "Attribute has been added to user", callback);
            request.Dispose();

        }

        public void SetAttributes(Dictionary<string, string> newAttributes, Action<string, bool> callback = null, bool isFromSync = false)
        {
            string token = GameFuseUser.CurrentUser.GetAuthenticationToken();

            // Create a list to hold your attributes
            List<AttributeItem> attributesList = new List<AttributeItem>();
            foreach (var attribute in newAttributes)
            {
                attributesList.Add(new AttributeItem { key = attribute.Key, value = attribute.Value });
            }

            // Create an object to hold the entire payload
            var payload = new AttributePayload
            {
                authentication_token = token,
                attributes = attributesList
            };

            // Serialize the object to JSON
            string jsonData = JsonUtility.ToJson(payload);

            StartCoroutine(SetAttributesRoutine(jsonData, newAttributes, callback, isFromSync));
        }

        private IEnumerator SetAttributesRoutine(string jsonData, Dictionary<string, string> newAttributes, Action<string, bool> callback = null, bool isFromSync = false)
        {
            GameFuse.Log("GameFuseUser Set Attributes: "+jsonData);

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");


            byte[] postData = Encoding.UTF8.GetBytes(jsonData);
            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/add_game_user_attribute", "POST");
            request.SetRequestHeader("Content-Type", "application/json"); 
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Set Attributes Success: " + jsonData);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var newAttributesLooper = new Dictionary<string, string>(newAttributes);
                foreach (var new_attribute in newAttributesLooper)
                {
                    if (attributes.ContainsKey(new_attribute.Key))
                    {
                        attributes.Remove(new_attribute.Key);
                    }
                    attributes.Add(new_attribute.Key, new_attribute.Value);
                }                
                
                foreach (var attribute in attributes)
                {
                    print(attribute.Key + "," + attribute.Value);
                }

                if (isFromSync){
                    dirtyAttributes = new Dictionary<string, string>(); 
                }
            }

            GameFuseUtilities.HandleCallback(request, "Attribute has been added to user", callback);
            request.Dispose();

        }

        [System.Serializable]
        public class AttributeItem
        {
            public string key;
            public string value;
        }

        [System.Serializable]
        public class AttributePayload
        {
            public string authentication_token;
            public List<AttributeItem> attributes;
        }


        public void RemoveAttribute(string key, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveAttributeRoutine(key,callback));
        }

        private IEnumerator RemoveAttributeRoutine(string key, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Remove Attributes: " + key);

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&game_user_attribute_key=" + key;
            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/remove_game_user_attributes" + parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Remove Attributes Success: " + key);

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

            GameFuseUtilities.HandleCallback(request, "Attribute has been removed", callback);
            request.Dispose();

        }


        #endregion

        #region store items
        internal void DownloadStoreItems(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadStoreItemsRoutine(chainedFromLogin, callback));

        }

        private IEnumerator DownloadStoreItemsRoutine(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Download Store Items: ");

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken();


            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/game_user_store_items" + parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Download Store Items Success: ");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_store_items = json.GetArray("game_user_store_items");
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new GameFuseStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id")),
                        item.Obj.GetString("icon_url")
                        )
                    );
                }


            }

            GameFuseUtilities.HandleCallback(request, chainedFromLogin ? "Users has been signed in successfully" : "Users store items have been downloaded", callback);
            request.Dispose();


        }

        public List<GameFuseStoreItem> GetPurchasedStoreItems()
        {
            return purchasedStoreItems;
        }

        public void PurchaseStoreItem(GameFuseStoreItem storeItem, Action<string, bool> callback = null)
        {
            StartCoroutine(PurchaseStoreItemRoutine(storeItem.GetId(), callback));
        }

        public void PurchaseStoreItem(int storeItemId, Action<string, bool> callback = null)
        {
            StartCoroutine(PurchaseStoreItemRoutine(storeItemId, callback));
        }

        private IEnumerator PurchaseStoreItemRoutine(int storeItemId, Action<string, bool> callback = null)
        {

            GameFuse.Log("GameFuseUser Purchase Store Items: ");

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("store_item_id", storeItemId.ToString());

            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/purchase_game_user_store_item", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Purchase Store Items Success: ");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_store_items = json.GetArray("game_user_store_items");
                CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new GameFuseStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id")),
                        item.Obj.GetString("icon_url")
                        )
                    );
                }
            }

            GameFuseUtilities.HandleCallback(request, "Store Item has been purchased by user", callback);
            request.Dispose();

        }

        public void RemoveStoreItem(int storeItemID, bool reimburseUser, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveStoreItemRoutine(storeItemID, reimburseUser, callback));
        }
        public void RemoveStoreItem(GameFuseStoreItem storeItem, bool reimburseUser, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveStoreItemRoutine(storeItem.GetId(), reimburseUser, callback));
        }

        private IEnumerator RemoveStoreItemRoutine(int storeItemID, bool reimburseUser, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Remove Store Item: "+ storeItemID);

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&store_item_id=" + storeItemID+ "&reimburse=" + reimburseUser.ToString();
            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/remove_game_user_store_item" + parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Remove Store Item Success: " + storeItemID);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                var game_user_store_items = json.GetArray("game_user_store_items");
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new GameFuseStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id")),
                        item.Obj.GetString("icon_url")
                        )
                    );
                }
            }

            GameFuseUtilities.HandleCallback(request, "Store Item has been removed", callback);
            request.Dispose();

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
            GameFuse.Log("GameFuseUser Adding Leaderboard Entry: " + leaderboardName + ": "+score.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

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

            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/add_leaderboard_entry", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Add Leaderboard Entry: " + leaderboardName+ ": "+score);

                var data = request.downloadHandler.text;
            }

            GameFuseUtilities.HandleCallback(request, "Leaderboard Entry Has Been Added", callback);
            request.Dispose();

        }

        public void ClearLeaderboardEntries(string leaderboardName, Action<string, bool> callback = null)
        {
            StartCoroutine(ClearLeaderboardEntriesRoutine(leaderboardName, callback));
        }


        private IEnumerator ClearLeaderboardEntriesRoutine(string leaderboardName, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Clearing Leaderboard Entry: " + leaderboardName);

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("leaderboard_name", leaderboardName);

            var request = UnityWebRequest.Post(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/clear_my_leaderboard_entries", form);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Clear Leaderboard Entry: " + leaderboardName);

                var data = request.downloadHandler.text;
            }

            GameFuseUtilities.HandleCallback(request, "Leaderboard Entries Have Been Cleared", callback);
            request.Dispose();

        }

        public void GetLeaderboard(int limit, bool onePerUser, Action<string, bool> callback = null)
        {
            StartCoroutine(GetLeaderboardRoutine(limit, onePerUser, callback));
        }

        private IEnumerator GetLeaderboardRoutine(int limit, bool onePerUser, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuseUser Get Leaderboard: " +limit.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&limit=" + limit.ToString()+ "&one_per_user="+ onePerUser.ToString();
            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/users/" + CurrentUser.id + "/leaderboard_entries" + parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuseUser Get Leaderboard Success: : " + limit.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Debug.Log("got " + json);
                var storeItems = json.GetArray("leaderboard_entries");
                GameFuse.Instance.leaderboardEntries.Clear();
                foreach (var storeItem in storeItems)
                {
                    GameFuse.Instance.leaderboardEntries.Add(new GameFuseLeaderboardEntry(
                        storeItem.Obj.GetString("username"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("score")),
                        storeItem.Obj.GetString("leaderboard_name"),
                        storeItem.Obj.GetString("extra_attributes"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("game_user_id")),
                        storeItem.Obj.GetString("created_at")
                        )
                   );
                }

            }

            GameFuseUtilities.HandleCallback(request, "Store Item has been removed", callback);
            request.Dispose();

        }

        #endregion Leaderboard 



    }



}
