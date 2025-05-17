using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Boomlagoon.JSON;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        // Services
        private static ISessionsService _sessionsService;
        public static ISessionsService SessionsService 
        { 
            get 
            {
                if (_sessionsService == null)
                {
                    _sessionsService = new SessionsService(GetBaseURL());
                }
                return _sessionsService;
            }
        }

        static UnityWebRequestAsyncOperation request;

        #region instance vars
        private string id;
        private string token;
        private string _name;
        private string description;
        private bool verboseLogging = false;
        private List<GameFuseStoreItem> store = new List<GameFuseStoreItem>();
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
        private static string baseURL = "https://gamefuse.co/api/v3";
        //private static string baseURL = "http://localhost/api/v2";

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
            return Instance._name;
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
        public static void SetVerboseLogging(bool _verboseLogging)
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
        /// <summary>
        /// Sets up the game using the legacy callback-based API
        /// </summary>
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
                Log("GameFuse Setting Up Game Received Request Success: " + gameId + ": " + token);
                var data = request.downloadHandler.text;
                
                // Use Newtonsoft.Json to parse the response
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                
                // Set the game data
                Instance.id = json["id"].ToString();
                Instance._name = json["name"].ToString();
                Instance.description = json["description"].ToString();
                Instance.token = json["token"].ToString();

                // Process game variables
                Dictionary<string, string> gameVariables = new Dictionary<string, string>();
                var gameVariablesArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                    JsonConvert.SerializeObject(json["game_variables"]));
                    
                foreach (var item in gameVariablesArray) 
                {
                    string key = item["key"];
                    string value = item["value"];
                    gameVariables[key] = value;
                }
                Instance.gameVariables = gameVariables;
                DownloadStoreItemsPrivate(callback);
            }
            else
            {
                Log("GameFuse Setting Up Game Received Request Failure: " + gameId + ": " + token);
                GameFuseUtilities.HandleCallback(request, "Game has failed to set up!", callback);
            }
        }
        
        /// <summary>
        /// Sets up the game using modern async/await pattern
        /// </summary>
        public static async Task SetUpGameAsync(string gameId, string token, bool seedStore = false)
        {
            Log("GameFuse SetUpGameAsync: " + gameId + ": " + token);

            try
            {
                // Create the URL with query parameters
                string url = $"{baseURL}/games/verify?client_from_library=cs&game_id={gameId}&game_token={token}";
                if (seedStore) url += "&seed_store=true";
                
                // Create a web request
                UnityWebRequest webRequest = UnityWebRequest.Get(url);
                
                // Send the request
                var operation = webRequest.SendWebRequest();
                
                // Wait for the operation to complete
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (GameFuseUtilities.RequestIsSuccessful(webRequest))
                {
                    Log("GameFuse SetUpGameAsync Success: " + gameId + ": " + token);
                    
                    // Parse the response data
                    var data = webRequest.downloadHandler.text;
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    
                    // Set the game data
                    Instance.id = json["id"].ToString();
                    Instance._name = json["name"].ToString();
                    Instance.description = json["description"].ToString();
                    Instance.token = json["token"].ToString();
                    
                    // Process game variables
                    Dictionary<string, string> gameVariables = new Dictionary<string, string>();
                    var gameVariablesArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                        JsonConvert.SerializeObject(json["game_variables"]));
                    
                    foreach (var item in gameVariablesArray) 
                    {
                        string key = item["key"];
                        string value = item["value"];
                        gameVariables[key] = value;
                    }
                    Instance.gameVariables = gameVariables;
                    
                    // Download store items
                    await DownloadStoreItemsAsync();
                }
                else
                {
                    Log("GameFuse SetUpGameAsync Failure: " + gameId + ": " + token);
                    throw new ApiException(
                        webRequest.responseCode,
                        "Game has failed to set up!",
                        webRequest.downloadHandler.text
                    );
                }
                
                webRequest.Dispose();
            }
            catch (Exception ex)
            {
                Log($"GameFuse SetUpGameAsync Error: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Downloads store items asynchronously
        /// </summary>
        private static async Task DownloadStoreItemsAsync()
        {
            Log("GameFuse DownloadStoreItemsAsync");
            
            try
            {
                // Create the URL with query parameters
                string url = $"{baseURL}/games/store_items?game_id={GetGameId()}&game_token={GetGameToken()}";
                
                // Create a web request
                UnityWebRequest webRequest = UnityWebRequest.Get(url);
                
                // Add authentication header if user is signed in
                if (GameFuseUser.CurrentUser.GetAuthenticationToken() != null)
                {
                    webRequest.SetRequestHeader("authentication_token", GameFuseUser.CurrentUser.GetAuthenticationToken());
                }
                
                // Send the request
                var operation = webRequest.SendWebRequest();
                
                // Wait for the operation to complete
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (GameFuseUtilities.RequestIsSuccessful(webRequest))
                {
                    Log("GameFuse DownloadStoreItemsAsync Success");
                    
                    // Parse the response data
                    var data = webRequest.downloadHandler.text;
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    
                    // Process store items
                    var storeItemsRaw = JsonConvert.SerializeObject(json["store_items"]);
                    var storeItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(storeItemsRaw);
                    
                    Instance.store.Clear();
                    foreach (var storeItem in storeItems)
                    {
                        Instance.store.Add(new GameFuseStoreItem(
                            storeItem["name"].ToString(),
                            storeItem["category"].ToString(),
                            storeItem["description"].ToString(),
                            Convert.ToInt32(storeItem["cost"]),
                            Convert.ToInt32(storeItem["id"]),
                            storeItem["icon_url"].ToString()
                            )
                        );
                    }
                }
                else
                {
                    Log("GameFuse DownloadStoreItemsAsync Failure");
                    throw new ApiException(
                        webRequest.responseCode,
                        "Failed to download store items!",
                        webRequest.downloadHandler.text
                    );
                }
                
                webRequest.Dispose();
            }
            catch (Exception ex)
            {
                Log($"GameFuse DownloadStoreItemsAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Fetches game variables using the legacy callback-based API.
        /// </summary>
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
                Log("GameFuse Fetch Game Variables Received Request Success: " + gameId + ": " + token);
                var data = request.downloadHandler.text;
                
                // Use Newtonsoft.Json to parse the response
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                
                // Set the game data
                Instance.id = json["id"].ToString();
                Instance._name = json["name"].ToString();
                Instance.description = json["description"].ToString();
                Instance.token = json["token"].ToString();

                // Process game variables
                Dictionary<string, string> gameVariables = new Dictionary<string, string>();
                var gameVariablesArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                    JsonConvert.SerializeObject(json["game_variables"]));
                    
                foreach (var item in gameVariablesArray) 
                {
                    string key = item["key"];
                    string value = item["value"];
                    gameVariables[key] = value;
                }
                Instance.gameVariables = gameVariables;
                DownloadStoreItemsPrivate(callback);
            }
            else
            {
                Log("GameFuse Fetch Game Variables Received Request Failure: " + gameId + ": " + token);
                GameFuseUtilities.HandleCallback(request, "Game has failed to set up!", callback);
            }
            request.Dispose();
        }
        
        /// <summary>
        /// Fetches game variables using the modern async/await pattern.
        /// </summary>
        public static async Task FetchGameVariablesAsync(string gameId, string token)
        {
            Log("GameFuse FetchGameVariablesAsync: " + gameId + ": " + token);

            try
            {
                // Create the URL with query parameters
                string url = $"{baseURL}/games/fetch_game_variables?game_id={gameId}&game_token={token}";
                
                // Create a web request
                UnityWebRequest webRequest = UnityWebRequest.Get(url);
                
                // Send the request
                var operation = webRequest.SendWebRequest();
                
                // Wait for the operation to complete
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (GameFuseUtilities.RequestIsSuccessful(webRequest))
                {
                    Log("GameFuse FetchGameVariablesAsync Success: " + gameId + ": " + token);
                    
                    // Parse the response data using Newtonsoft.Json instead of Boomlagoon
                    var data = webRequest.downloadHandler.text;
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    
                    // Set the game data
                    Instance.id = json["id"].ToString();
                    Instance._name = json["name"].ToString();
                    Instance.description = json["description"].ToString();
                    Instance.token = json["token"].ToString();
                    
                    // Process game variables
                    Dictionary<string, string> gameVariables = new Dictionary<string, string>();
                    var gameVariablesArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(
                        JsonConvert.SerializeObject(json["game_variables"]));
                    
                    foreach (var item in gameVariablesArray) 
                    {
                        string key = item["key"];
                        string value = item["value"];
                        gameVariables[key] = value;
                    }
                    Instance.gameVariables = gameVariables;
                    
                    // Download store items
                    await DownloadStoreItemsAsync();
                }
                else
                {
                    Log("GameFuse FetchGameVariablesAsync Failure: " + gameId + ": " + token);
                    throw new ApiException(
                        webRequest.responseCode,
                        "Failed to fetch game variables!",
                        webRequest.downloadHandler.text
                    );
                }
                
                webRequest.Dispose();
            }
            catch (Exception ex)
            {
                Log($"GameFuse FetchGameVariablesAsync Error: {ex.Message}");
                throw;
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
                request.SetRequestHeader("authentication-token", GameFuseUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (GameFuseUtilities.RequestIsSuccessful(request))
            {
                Log("GameFuse Downloading Store Items Success");

                var data = request.downloadHandler.text;
                
                // Use Newtonsoft.Json to parse the response
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                store.Clear();
                
                // Process store items using Newtonsoft.Json
                var storeItemsRaw = JsonConvert.SerializeObject(json["store_items"]);
                var storeItemsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(storeItemsRaw);
                
                foreach (var storeItem in storeItemsList)
                {
                    store.Add(new GameFuseStoreItem(
                        storeItem["name"].ToString(),
                        storeItem["category"].ToString(),
                        storeItem["description"].ToString(),
                        Convert.ToInt32(storeItem["cost"]),
                        Convert.ToInt32(storeItem["id"]),
                        storeItem["icon_url"].ToString()
                        )
                    );
                }
            }
            else
            {
                GameFuseUtilities.HandleCallback(request, "Game has failed to set up!", callback);
                Log("GameFuse Downloading Store Items Failed");
            }

            GameFuseUtilities.HandleCallback(request, "Game has been set up!", callback);
            request.Dispose();
        }

        public static List<GameFuseStoreItem> GetStoreItems()
        {
            return Instance.store;
        }
        #endregion



        #region request: sign in
        /// <summary>
        /// Signs in a user with the legacy callback-based API
        /// </summary>
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
                
                // Use Newtonsoft.Json to parse the response
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                
                // Update the current user
                GameFuseUser.CurrentUser.SetSignedInInternal();
                GameFuseUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json["score"]));
                GameFuseUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json["credits"]));
                GameFuseUser.CurrentUser.SetUsernameInternal(json["username"].ToString());
                GameFuseUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json["last_login"].ToString()));
                GameFuseUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json["number_of_logins"]));
                GameFuseUser.CurrentUser.SetAuthenticationTokenInternal(json["authentication_token"].ToString());
                GameFuseUser.CurrentUser.SetIDInternal(Convert.ToInt32(json["id"]));
                
                // Chain next request - download users attributes
                GameFuseUser.CurrentUser.DownloadAttributes(true, callback);
            }
            else
            {
                Log("GameFuse Sign In Failure: " + email);
                GameFuseUtilities.HandleCallback(request, "User has been signed in successfully", callback);
            }
            request.Dispose();
        }
        
        /// <summary>
        /// Signs in a user using modern async/await pattern
        /// </summary>
        public static async Task<SignInResponse> SignInAsync(string email, string password)
        {
            Log("GameFuse SignInAsync: " + email);

            if (GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before signing in users");

            try
            {
                // Create the sign in request
                var request = new SignInRequest
                {
                    Email = email,
                    Password = password,
                    GameId = int.Parse(GetGameId()),
                    GameToken = GetGameToken()
                };

                // Send the request through the SessionsService
                var response = await SessionsService.SignInAsync(request);
                
                // Update the current user
                GameFuseUser.CurrentUser.SetSignedInInternal();
                GameFuseUser.CurrentUser.SetScoreInternal(response.Score);
                GameFuseUser.CurrentUser.SetCreditsInternal(response.Credits);
                GameFuseUser.CurrentUser.SetUsernameInternal(response.Username);
                GameFuseUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(response.LastLogin));
                GameFuseUser.CurrentUser.SetNumberOfLoginsInternal(response.NumberOfLogins);
                GameFuseUser.CurrentUser.SetAuthenticationTokenInternal(response.AuthenticationToken);
                GameFuseUser.CurrentUser.SetIDInternal(response.Id);
                
                // Download user attributes asynchronously
                await GameFuseUser.CurrentUser.GetAttributesAsync();
                
                Log("GameFuse SignInAsync Success: " + email);
                
                return response;
            }
            catch (ApiException ex)
            {
                Log($"GameFuse SignInAsync Failure: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region request: sign up
        /// <summary>
        /// Signs up a new user with the legacy callback-based API
        /// </summary>
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
                
                // Use Newtonsoft.Json to parse the response
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                
                // Update the current user
                GameFuseUser.CurrentUser.SetSignedInInternal();
                GameFuseUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json["score"]));
                GameFuseUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json["credits"]));
                GameFuseUser.CurrentUser.SetUsernameInternal(json["username"].ToString());
                GameFuseUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json["last_login"].ToString()));
                GameFuseUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json["number_of_logins"])); 
                GameFuseUser.CurrentUser.SetAuthenticationTokenInternal(json["authentication_token"].ToString());
                GameFuseUser.CurrentUser.SetIDInternal(Convert.ToInt32(json["id"]));
                
                // Chain next request - download users attributes
                GameFuseUser.CurrentUser.DownloadAttributes(true, callback);
                GameFuseUtilities.HandleCallback(request, "User Signed Up Successfully", callback);
            }
            else
            {
                Log("GameFuse Sign Up Failure: " + email);
                GameFuseUtilities.HandleCallback(request, "User could not sign up: " + request.error, callback);
            }
            request.Dispose();
        }

        /// <summary>
        /// Signs up a new user using modern async/await pattern
        /// </summary>
        public static async Task<SignInResponse> SignUpAsync(string email, string password, string passwordConfirmation, string username)
        {
            Log("GameFuse SignUpAsync: " + email);

            if (GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before signing up users");

            try
            {
                // Create the sign up request
                var request = new SignUpRequest
                {
                    Email = email,
                    Password = password,
                    PasswordConfirmation = passwordConfirmation,
                    Username = username,
                    GameId = int.Parse(GetGameId()),
                    GameToken = GetGameToken()
                };

                // Send the request through the SessionsService
                var response = await SessionsService.SignUpAsync(request);
                
                // Update the current user
                GameFuseUser.CurrentUser.SetSignedInInternal();
                GameFuseUser.CurrentUser.SetScoreInternal(response.Score);
                GameFuseUser.CurrentUser.SetCreditsInternal(response.Credits);
                GameFuseUser.CurrentUser.SetUsernameInternal(response.Username);
                GameFuseUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(response.LastLogin));
                GameFuseUser.CurrentUser.SetNumberOfLoginsInternal(response.NumberOfLogins);
                GameFuseUser.CurrentUser.SetAuthenticationTokenInternal(response.AuthenticationToken);
                GameFuseUser.CurrentUser.SetIDInternal(response.Id);
                
                // Download user attributes asynchronously
                await GameFuseUser.CurrentUser.GetAttributesAsync();
                
                Log("GameFuse SignUpAsync Success: " + email);
                
                return response;
            }
            catch (ApiException ex)
            {
                Log($"GameFuse SignUpAsync Failure: {ex.Message}");
                throw;
            }
        }
        #endregion

        // Leaderboard functionality has been moved to LeaderboardService.
        // Use LeaderboardService.GetGameLeaderboardEntriesAsync for game leaderboard data.
        // For user-specific entries, use GameFuseUser.GetGameLeaderboardEntriesAsync methods.


        #region Forgot Password
        /// <summary>
        /// Sends a password reset email using the legacy callback-based API
        /// </summary>
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
            request.Dispose();
        }
        
        /// <summary>
        /// Sends a password reset email using modern async/await pattern
        /// </summary>
        public static async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            Log("GameFuse SendPasswordResetEmailAsync: " + email);

            if (GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before sending password resets");

            try
            {
                // Send the request through the SessionsService
                bool success = await SessionsService.SendPasswordResetEmailAsync(email, GetGameId(), GetGameToken());
                
                Log("GameFuse SendPasswordResetEmailAsync Success: " + email);
                
                return success;
            }
            catch (ApiException ex)
            {
                Log($"GameFuse SendPasswordResetEmailAsync Failure: {ex.Message}");
                throw;
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