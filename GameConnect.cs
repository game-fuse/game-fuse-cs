using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Boomlagoon.JSON;
//using UnityEditor;

namespace GameConnectCSharp
{
    /// <summary>Class <c>GameConnect</c> is your connection with the GameConnect
    /// API.  Through this class you can connect to your apps, login users,
    /// create users.  When a user is signed in you can use GameConnectUser to 
    /// access your account, attributes and purchased store items.
    /// </summary>
    public class GameConnect : MonoBehaviour
    {

        static UnityWebRequestAsyncOperation request;

        #region instance vars
        private string id;
        private string token;
        private string name;
        private string description;
        private bool verboseLogging = false;
        private List<GameConnectStoreItem> store = new List<GameConnectStoreItem>();
        public List<GameConnectLeaderboardEntry> leaderboardEntries = new List<GameConnectLeaderboardEntry>();

        #endregion

        #region singleton management
        private static GameConnect _instance;
        public static GameConnect Instance { get { return _instance; } }
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
        // private static string baseURL = "https://gameconnect.io/api/v1";
        private static string baseURL = "http://localhost/api/v1";

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
            Log("GameConnect Setting Up Game: "+ gameId+": "+ token);
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
            Log("GameConnect Setting Up Game Sending Request: " + baseURL + "/games/verify?" + body);
            var request = UnityWebRequest.Get(baseURL + "/games/verify?" + body);
            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                Log("GameConnect Setting Up Game Recieved Request Success: " + gameId + ": " + token);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Instance.id = json.GetNumber("id").ToString();
                Instance.name = json.GetString("name");
                Instance.description = json.GetString("description");
                Instance.token = json.GetString("token");
                DownloadStoreItemsPrivate(callback);

            }
            else
            {
                Log("GameConnect Setting Up Game Recieved Request Failure: " + gameId + ": " + token);
                GameConnectUtilities.HandleCallback(request, "Game has failed to set up!", callback);
            }


        }

        private void DownloadStoreItemsPrivate(Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadStoreItemsRoutine(callback));
        }

        private IEnumerator DownloadStoreItemsRoutine(Action<string, bool> callback = null)
        {
            Log("GameConnect Downloading Store Items");
            var body = "game_id=" + id + "&game_token=" + token;
            var request = UnityWebRequest.Get(baseURL + "/games/store_items?" + body);
            if (GameConnectUser.CurrentUser.GetAuthenticationToken() != null)
                request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                Log("GameConnect Downloading Store Items Success");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var storeItems = json.GetArray("store_items");
                store.Clear();
                foreach (var storeItem in storeItems)
                {
                    store.Add(new GameConnectStoreItem(
                        storeItem.Obj.GetString("name"),
                        storeItem.Obj.GetString("category"),
                        storeItem.Obj.GetString("description"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("cost")),
                        Convert.ToInt32(storeItem.Obj.GetNumber("id"))));
                }


            }
            else
            {
                GameConnectUtilities.HandleCallback(request, "Game has failed to set up!", callback);
                Log("GameConnect Downloading Store Items FAiled");

            }

            GameConnectUtilities.HandleCallback(request, "Game has been set up!", callback);

        }

        public static List<GameConnectStoreItem> GetStoreItems()
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

            Log("GameConnect Sign In: " + email );

            if (GetGameId() == null)
                throw new GameConnectException("Please set up your game with PainLessAuth.SetUpGame before signing in users");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            form.AddField("game_id", GetGameId());

            var request = UnityWebRequest.Post(baseURL + "/sessions", form);

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                Log("GameConnect Sign In Success: " + email);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                GameConnectUser.CurrentUser.SetSignedInInternal();
                GameConnectUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
                GameConnectUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                GameConnectUser.CurrentUser.SetUsernameInternal(json.GetString("username"));
                GameConnectUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json.GetString("last_login")));
                GameConnectUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json.GetNumber("number_of_logins")));
                GameConnectUser.CurrentUser.SetAuthenticationTokenInternal(json.GetString("authentication_token"));
                GameConnectUser.CurrentUser.SetIDInternal(Convert.ToInt32(json.GetNumber("id")));
                GameConnectUser.CurrentUser.DownloadAttributes(true, callback); // Chain next request - download users attributes

            }
            else
            {
                Log("GameConnect Sign In Failure: " + email);

                GameConnectUtilities.HandleCallback(request, "User has been signed in successfully", callback);
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
            Log("GameConnect Sign Up: " + email);

            if (GetGameId() == null)
                throw new GameConnectException("Please set up your game with PainLessAuth.SetUpGame before signing up users");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            form.AddField("password_confirmation", password_confirmation);
            form.AddField("username", username);

            form.AddField("game_id", GetGameId());
            form.AddField("game_token", GetGameToken());

            var request = UnityWebRequest.Post(baseURL + "/users", form);

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {

                Log("GameConnect Sign Up Success: " + email);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                GameConnectUser.CurrentUser.SetSignedInInternal();
                GameConnectUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
                GameConnectUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                GameConnectUser.CurrentUser.SetUsernameInternal(json.GetString("username"));
                GameConnectUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json.GetString("last_login")));
                GameConnectUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json.GetNumber("number_of_logins")));
                GameConnectUser.CurrentUser.SetAuthenticationTokenInternal(json.GetString("authentication_token"));
                GameConnectUser.CurrentUser.SetIDInternal(Convert.ToInt32(json.GetNumber("id")));
                GameConnectUser.CurrentUser.DownloadAttributes(true, callback); // Chain next request - download users attributes

            }
            else
            {
                Log("GameConnect Sign Up Failure: " + email);
                GameConnectUtilities.HandleCallback(request, "User could not sign up: " + request.error, callback);
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
            GameConnect.Log("GameConnect Get Leaderboard: " + limit.ToString());

            if (GameConnect.GetGameId() == null)
                throw new GameConnectException("Please set up your game with GameConnect.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GameConnectUser.CurrentUser.GetAuthenticationToken() + "&limit=" + limit.ToString() + "&one_per_user=" + onePerUser.ToString()+ "&leaderboard_name="+ LeaderboardName.ToString();
            var request = UnityWebRequest.Get(GameConnect.GetBaseURL() + "/games/" + GameConnect.GetGameId() + "/leaderboard_entries" + parameters);
            request.SetRequestHeader("authentication_token", GameConnectUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameConnectUtilities.RequestIsSuccessful(request))
            {
                GameConnect.Log("GameConnect Get Leaderboard Success: : " + limit.ToString());

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
        #endregion

    }

}



public class GameConnectException : Exception
{
    public GameConnectException()
    {
    }

    public GameConnectException(string message)
        : base(message)
    {
    }

    public GameConnectException(string message, Exception inner)
        : base(message, inner)
    {
    }
}