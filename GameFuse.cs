using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Boomlagoon.JSON;
//using UnityEditor;

namespace GameFuseCSharp
{
    /// <summary>Class <c>GameFuse</c> is your connection with the GameFuse
    /// API.  Through this class you can connect to your apps, login users,
    /// create users.  When a user is signed in you can use GameFuseUser to 
    /// access your account, attributes and purchased store items.
    /// </summary>
    public class GameFuse : MonoBehaviour
    {

        static UnityWebRequestAsyncOperation request;

        #region instance vars
        private string id;
        private string token;
        private string name;
        private string description;
        private bool verboseLogging = true;
        private List<GameFuseStoreItem> store = new List<GameFuseStoreItem>();
        public List<GameFuseLeaderboardEntry> leaderboardEntries = new List<GameFuseLeaderboardEntry>();
        public Dictionary<string, string> gameVariables = new Dictionary<string, string>();
        #endregion

        #region singleton management
        private static GameFuse _instance;
        public static GameFuse Instance { get { return _instance; } }
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

        #region globals
        // private static string baseURL = "https://gamefuse.co/api/v1";
        private static string baseURL = "http://localhost/api/v2";

        public static string GetBaseURL()
        {
            return baseURL;
        }
        #endregion


        #region instance getters
        public static string GetGameId()
        {
            return Instance.id;
        }
        public static string GetGameName()
        {
            return Instance.name;
        }
        public static string GetGameDescription()
        {
            return Instance.description;
        }
        internal static string GetGameToken()
        {
            return Instance.token;
        }

        internal static bool GetVerboseLogging()
        {
            return Instance.verboseLogging;
        }
        #endregion


        #region instance setters
        internal static void SetVerboseLogging(bool _verboseLogging)
        {
            Instance.verboseLogging = _verboseLogging;
        }
        #endregion

        #region logger
        internal static void Log(string log)
        {
            if (GetVerboseLogging())
                Debug.Log("<color=green> " + log + " </color>");
        }
        #endregion


        #region request: set up applicaton
        public static void SetUpGame(string gameId, string token, Action<string, bool> callback = null, bool seedStore = false)
        {
            Log("GameFuse Setting Up Game: "+ gameId+": "+ token);
            Instance.SetUpGamePrivate(gameId, token, callback, seedStore);
        }

        private void SetUpGamePrivate(string gameId, string token, Action<string, bool> callback = null, bool seedStore = false)
        {
            StartCoroutine(SetUpGameRoutine(gameId, token, callback,seedStore));
        }

        private IEnumerator SetUpGameRoutine(string gameId, string token, Action<string, bool> callback = null, bool seedStore = false)
        {
            var body = "game_id=" + gameId + "&game_token=" + token;
            if (seedStore) body = body + "&seed_store=true";
            Log("GameFuse Setting Up Game Sending Request: " + baseURL + "/games/verify?client_from_library=cs&" + body);
            var request = UnityWebRequest.Get(baseURL + "/games/verify?client_from_library=cs&" + body);
            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                Log("GameFuse Setting Up Game Recieved Request Success: " + gameId + ": " + token);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Instance.id = json.GetNumber("id").ToString();
                Instance.name = json.GetString("name");
                Instance.description = json.GetString("description");
                Instance.token = json.GetString("token");

                Dictionary<string, string> gameVariables = new Dictionary<string, string>();
                JSONArray gameVariablesArray = json.GetArray("game_variables");
                for (int i = 0; i < gameVariablesArray.Length; i++) 
                {
                    JSONObject iterjson = JSONObject.Parse(gameVariablesArray[i].ToString());
                    string key = iterjson.GetString("key");
                    string value = iterjson.GetString("value");
                    gameVariables[key] = value;
                }
                Instance.gameVariables = gameVariables;
                DownloadStoreItemsPrivate(callback);

            }
            else
            {
                Log("GameFuse Setting Up Game Recieved Request Failure: " + gameId + ": " + token);
                GameFuseUtilities.HandleCallback(request, "Game has failed to set up!", callback);
            }

        }

        public static void FetchGameVariables(string gameId, string token, Action<string, bool> callback = null)
        {
            Log("GameFuse Fetch Game Variables: "+ gameId+": "+ token);
            Instance.FetchGameVariablesPrivate(gameId, token, callback);
        }

        private void FetchGameVariablesPrivate(string gameId, string token, Action<string, bool> callback = null)
        {
            StartCoroutine(FetchGameVariablesRoutine(gameId, token, callback));
        }

        private IEnumerator FetchGameVariablesRoutine(string gameId, string token, Action<string, bool> callback = null)
        {
            var body = "game_id=" + gameId + "&game_token=" + token;
            Log("GameFuse Fetch Game Variables Sending Request: " + baseURL + "/games/fetch_game_variables?" + body);
            var request = UnityWebRequest.Get(baseURL + "/games/fetch_game_variables?" + body);
            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                Log("GameFuse Fetch Game Variables Recieved Request Success: " + gameId + ": " + token);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Instance.id = json.GetNumber("id").ToString();
                Instance.name = json.GetString("name");
                Instance.description = json.GetString("description");
                Instance.token = json.GetString("token");

                Dictionary<string, string> gameVariables = new Dictionary<string, string>();
                JSONArray gameVariablesArray = json.GetArray("game_variables");
                for (int i = 0; i < gameVariablesArray.Length; i++) 
                {
                    JSONObject iterjson = JSONObject.Parse(gameVariablesArray[i].ToString());
                    string key = iterjson.GetString("key");
                    string value = iterjson.GetString("value");
                    gameVariables[key] = value;
                }
                Instance.gameVariables = gameVariables;
                DownloadStoreItemsPrivate(callback);

            }
            else
            {
                Log("GameFuse Fetch Game Variables Recieved Request Failure: " + gameId + ": " + token);
                GameFuseUtilities.HandleCallback(request, "Game has failed to set up!", callback);
            }

        }

        public static string GetGameVariable(string key)
        {
            return Instance.gameVariables[key];
        }

        private void DownloadStoreItemsPrivate(Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadStoreItemsRoutine(callback));
        }

        private IEnumerator DownloadStoreItemsRoutine(Action<string, bool> callback = null)
        {
            Log("GameFuse Downloading Store Items");
            var body = "game_id=" + id + "&game_token=" + token;
            var request = UnityWebRequest.Get(baseURL + "/games/store_items?" + body);
            if (GameFuseUser.CurrentUser.GetAuthenticationToken() != null)
                request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                Log("GameFuse Downloading Store Items Success");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var storeItems = json.GetArray("store_items");
                store.Clear();
                foreach (var storeItem in storeItems)
                {
                    store.Add(new GameFuseStoreItem(
                        storeItem.Obj.GetString("name"),
                        storeItem.Obj.GetString("category"),
                        storeItem.Obj.GetString("description"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("cost")),
                        Convert.ToInt32(storeItem.Obj.GetNumber("id")),
                        storeItem.Obj.GetString("icon_url")
                        )
                    );
                }


            }
            else
            {
                GameFuseUtilities.HandleCallback(request, "Game has failed to set up!", callback);
                Log("GameFuse Downloading Store Items FAiled");

            }

            GameFuseUtilities.HandleCallback(request, "Game has been set up!", callback);

        }

        public static List<GameFuseStoreItem> GetStoreItems()
        {
            return Instance.store;
        }
        #endregion



        #region request: sign in
        public static void SignIn(string email, string password, Action<string, bool> callback = null)
        {
            Instance.SignInPrivate(email, password, callback);
        }

        private void SignInPrivate(string email, string password, Action<string, bool> callback = null)
        {
            StartCoroutine(SignInRoutine(email, password, callback));
        }

        private IEnumerator SignInRoutine(string email, string password, Action<string, bool> callback = null)
        {

            Log("GameFuse Sign In: " + email );

            if (GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before signing in users");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            form.AddField("game_id", GetGameId());

            var request = UnityWebRequest.Post(baseURL + "/sessions", form);

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                Log("GameFuse Sign In Success: " + email);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                GameFuseUser.CurrentUser.SetSignedInInternal();
                GameFuseUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
                GameFuseUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                GameFuseUser.CurrentUser.SetUsernameInternal(json.GetString("username"));
                GameFuseUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json.GetString("last_login")));
                GameFuseUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json.GetNumber("number_of_logins")));
                GameFuseUser.CurrentUser.SetAuthenticationTokenInternal(json.GetString("authentication_token"));
                GameFuseUser.CurrentUser.SetIDInternal(Convert.ToInt32(json.GetNumber("id")));
                GameFuseUser.CurrentUser.DownloadAttributes(true, callback); // Chain next request - download users attributes

            }
            else
            {
                Log("GameFuse Sign In Failure: " + email);

                GameFuseUtilities.HandleCallback(request, "User has been signed in successfully", callback);
            }

        }

        #endregion

        #region request: sign up
        public static void SignUp(string email, string password, string password_confirmation, string username, Action<string, bool> callback = null)
        {
            Instance.SignUpPrivate(email, password, password_confirmation, username, callback);
        }

        private void SignUpPrivate(string email, string password, string password_confirmation, string username, Action<string, bool> callback = null)
        {
            StartCoroutine(SignUpRoutine(email, password, password_confirmation, username, callback));
        }

        private IEnumerator SignUpRoutine(string email, string password, string password_confirmation, string username, Action<string, bool> callback = null)
        {
            Log("GameFuse Sign Up: " + email);

            if (GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before signing up users");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            form.AddField("password_confirmation", password_confirmation);
            form.AddField("username", username);

            form.AddField("game_id", GetGameId());
            form.AddField("game_token", GetGameToken());

            var request = UnityWebRequest.Post(baseURL + "/users", form);

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {

                Log("GameFuse Sign Up Success: " + email);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                GameFuseUser.CurrentUser.SetSignedInInternal();
                GameFuseUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
                GameFuseUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                GameFuseUser.CurrentUser.SetUsernameInternal(json.GetString("username"));
                GameFuseUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json.GetString("last_login")));
                GameFuseUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json.GetNumber("number_of_logins")));
                GameFuseUser.CurrentUser.SetAuthenticationTokenInternal(json.GetString("authentication_token"));
                GameFuseUser.CurrentUser.SetIDInternal(Convert.ToInt32(json.GetNumber("id")));
                GameFuseUser.CurrentUser.DownloadAttributes(true, callback); // Chain next request - download users attributes

            }
            else
            {
                Log("GameFuse Sign Up Failure: " + email);
                GameFuseUtilities.HandleCallback(request, "User could not sign up: " + request.error, callback);
            }

        }



        #endregion

        #region Leaderboard

        public void GetLeaderboard(int limit, bool onePerUser, string LeaderboardName, Action<string, bool> callback = null)
        {
            StartCoroutine(GetLeaderboardRoutine(limit, onePerUser, LeaderboardName, callback));
        }

        private IEnumerator GetLeaderboardRoutine(int limit, bool onePerUser, string LeaderboardName, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuse Get Leaderboard: " + limit.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GameFuseUser.CurrentUser.GetAuthenticationToken() + "&limit=" + limit.ToString() + "&one_per_user=" + onePerUser.ToString()+ "&leaderboard_name="+ LeaderboardName.ToString();
            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/games/" + GameFuse.GetGameId() + "/leaderboard_entries" + parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuse.Log("GameFuse Get Leaderboard Success: : " + limit.ToString());

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
        }
        #endregion


        #region Forgot Password

        public void SendPasswordResetEmail(string email, Action<string, bool> callback = null)
        {
            StartCoroutine(SendPasswordResetEmailRoutine(email, callback));
        }

        private IEnumerator SendPasswordResetEmailRoutine(string email, Action<string, bool> callback = null)
        {
            GameFuse.Log("GameFuse SendPasswordResetEmail: " + email.ToString());

            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before sending password resets");

            var parameters = "?game_token=" + GameFuse.GetGameToken() + "&game_id=" + GameFuse.GetGameId().ToString() + "&email=" + email.ToString();
            var request = UnityWebRequest.Get(GameFuse.GetBaseURL() + "/games/" + GameFuse.GetGameId().ToString() + "/forget_password" + parameters);
            request.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();
            Debug.Log(request);

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                GameFuseUtilities.HandleCallback(request, "Forgot password email sent!", callback);
            } else {
                GameFuseUtilities.HandleCallback(request, "Forgot password email failed to send!", callback);
            }

            
        }
        #endregion

    }

}



public class GameFuseException : Exception
{
    public GameFuseException()
    {
    }

    public GameFuseException(string message)
        : base(message)
    {
    }

    public GameFuseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}