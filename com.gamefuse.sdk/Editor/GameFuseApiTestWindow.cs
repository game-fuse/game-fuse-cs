// GameFuseApiTestWindow.cs
using GameFuse.Config;
using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Transport;
using Newtonsoft.Json; // For better JSON serialization
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameFuse.Editor
{
    public class GameFuseApiTestWindow : EditorWindow
    {
        private const string NullString = "(null)";
        private const float IndentWidth = 15f;

        private string _gameId;
        private string _gameApiKey;
        private string _userAuthToken; // This will be updated by Sign Up/Sign In
        private Vector2 _scrollPosition;
        private bool _isExecuting;
        private HttpStatusCode _lastStatusCode = HttpStatusCode.Unused; // Initialize
        private string _lastResponseJson;
        private string _lastErrorMessage;
        private bool _showResponse = true;
        private bool _showError = true; // Default to show error if any
        private ApiEndpoint _selectedEndpoint;
        private readonly Dictionary<string, string> _paramValues = new Dictionary<string, string>();
        private ITransport _transport; // This will be re-created or re-configured for calls

        // Define API structure (consider moving to a separate file if it grows large)
        #region API Definitions
        private static readonly List<ApiCategory> ApiCategories = new List<ApiCategory>
        {
            new ApiCategory("Authentication", new List<ApiEndpoint>
            {
                new ApiEndpoint("Sign Up", EndpointType.SignUp, new List<ApiParameter>
                {
                    new ApiParameter("email", "Email", true),
                    new ApiParameter("password", "Password", true),
                    new ApiParameter("username", "Username", true)
                }),
                new ApiEndpoint("Sign In", EndpointType.SignIn, new List<ApiParameter>
                {
                    new ApiParameter("emailOrUsername", "Email or Username", true),
                    new ApiParameter("password", "Password", true)
                }),
                new ApiEndpoint("Forgot Password", EndpointType.ForgotPassword, new List<ApiParameter>
                {
                    new ApiParameter("email", "Email", true)
                })
            }),
            // Add other categories and endpoints here as previously defined
            // For brevity, I'll omit repeating all of them.
            // Ensure your full list from the original file is here.
             new ApiCategory("User", new List<ApiEndpoint>
            {
                new ApiEndpoint("Get Current User (Me)", EndpointType.GetCurrentUser, null), // Requires Auth
                // Add other User endpoints...
            }),
            new ApiCategory("Messages", new List<ApiEndpoint> // Example for Chat/Message API
            {
                new ApiEndpoint("Create Direct Chat", EndpointType.CreateDirectChat, new List<ApiParameter>
                {
                    new ApiParameter("usernames", "Usernames (CSV)", true), // e.g., "user2,user3"
                    new ApiParameter("initialMessage", "Initial Message", true)
                }),
                // Add other Message endpoints...
            })
            // ... other categories ...
        };

        private class ApiCategory
        {
            public string Name { get; }
            public List<ApiEndpoint> Endpoints { get; }
            public bool IsExpanded { get; set; }
            public ApiCategory(string name, List<ApiEndpoint> endpoints) { Name = name; Endpoints = endpoints; IsExpanded = true; } // Default expanded
        }

        private class ApiEndpoint
        {
            public string Name { get; }
            public EndpointType Type { get; }
            public List<ApiParameter> Parameters { get; }
            public ApiEndpoint(string name, EndpointType type, List<ApiParameter> parameters) { Name = name; Type = type; Parameters = parameters; }
        }

        private class ApiParameter
        {
            public string Name { get; }
            public string DisplayName { get; }
            public bool Required { get; }
            public ApiParameter(string name, string displayName, bool required) { Name = name; DisplayName = displayName; Required = required; }
        }

        private enum EndpointType // Add all your endpoint types here
        {
            None, // Default
            SignUp, SignIn, ForgotPassword,
            GetCurrentUser, UpdateUser, UpdatePassword, SetUserAttribute, GetUserAttributes,
            // Game Rounds
            CreateGameRound, GetGameRound, GetUserGameRounds, UpdateGameRound, GetLeaderboard, GetUserRank,
            // Store
            GetStoreItems, GetStoreItem, PurchaseItem, GetUserPurchases, GetCreditBalance, GetCreditTransactions,
            // Friends
            GetFriends, SendFriendRequest, GetFriendRequests, AcceptFriendRequest, RejectFriendRequest, RemoveFriend, SearchUsers,
            // Groups
            CreateGroup, GetGroup, GetUserGroups, UpdateGroup, DeleteGroup, AddUserToGroup, RemoveUserFromGroup, SendJoinRequest, SearchGroups,
            // Messages (Added based on recent work)
            FetchPaginatedChats, CreateDirectChat, CreateGroupChat, FetchPaginatedMessages, SendMessageToChat, MarkMessageAsRead
            // Add more as needed
        }
        #endregion

        [MenuItem("Tools/GameFuse/API Test Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameFuseApiTestWindow>();
            window.titleContent = new GUIContent("GameFuse API Test");
            window.minSize = new Vector2(600, 700); // Increased height a bit
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
            // Transport is created on-demand in ExecuteApiCallInternal now to use latest auth token
        }

        private void LoadSettings()
        {
            var settings = GameFuseSettings.Settings; // Ensure this exists and is configured
            if (settings != null)
            {
                _gameId = settings.GameId;
                _gameApiKey = settings.GameApiKey;
            }
            else
            {
                Debug.LogWarning("GameFuseSettings not found. Please configure them via Assets > Create > GameFuse > Settings.");
            }
        }

        private void OnGUI()
        {
            DrawHeader();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            DrawCredentialsSection();
            DrawApiTestSection();
            DrawResponseSection();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            // ... (same as before) ...
            GUILayout.Space(10);
            EditorGUILayout.LabelField("GameFuse API Test Tool", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("Test GameFuse API calls. Sign Up/Sign In will update the User Auth Token.", MessageType.Info);
            GUILayout.Space(10);
        }

        private void DrawCredentialsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Credentials & Configuration", EditorStyles.boldLabel);
            _gameId = EditorGUILayout.TextField(new GUIContent("Game ID", "Found on your GameFuse.co dashboard."), _gameId);
            _gameApiKey = EditorGUILayout.TextField(new GUIContent("Game API Key", "Found on your GameFuse.co dashboard."), _gameApiKey);
            _userAuthToken = EditorGUILayout.TextField(new GUIContent("User Auth Token", "Automatically set after Sign Up/Sign In."), _userAuthToken);
            if (GUILayout.Button("Load from GameFuseSettings")) LoadSettings();
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawApiTestSection()
        {
            // ... (same as before - drawing categories, endpoints, parameters) ...
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("API Endpoint Selection", EditorStyles.boldLabel);
            foreach (var category in ApiCategories)
            {
                category.IsExpanded = EditorGUILayout.Foldout(category.IsExpanded, category.Name, true, EditorStyles.foldoutHeader);
                if (category.IsExpanded)
                {
                    EditorGUI.indentLevel++;
                    foreach (var endpoint in category.Endpoints)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(IndentWidth);
                        if (GUILayout.Toggle(_selectedEndpoint == endpoint, endpoint.Name, EditorStyles.radioButton))
                        {
                            if (_selectedEndpoint != endpoint) { _selectedEndpoint = endpoint; _paramValues.Clear(); }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.Space(5);
            if (_selectedEndpoint != null && _selectedEndpoint.Parameters != null && _selectedEndpoint.Parameters.Count > 0)
            {
                EditorGUILayout.LabelField("Parameters for: " + _selectedEndpoint.Name, EditorStyles.boldLabel);
                foreach (var param in _selectedEndpoint.Parameters)
                {
                    _paramValues.TryGetValue(param.Name, out string currentValue);
                    _paramValues[param.Name] = EditorGUILayout.TextField(param.DisplayName + (param.Required ? " *" : ""), currentValue ?? "");
                }
                GUILayout.Space(5);
            }
            GUI.enabled = !_isExecuting && _selectedEndpoint != null && ValidateParameters();
            if (GUILayout.Button(_isExecuting ? "Executing..." : "Execute API Call")) ExecuteApiCall();
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawResponseSection()
        {
            // ... (same as before - drawing status, error, response JSON) ...
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("API Response", EditorStyles.boldLabel);
            if (_isExecuting)
            {
                EditorGUILayout.LabelField("Executing...");
            }
            else
            {
                EditorGUILayout.LabelField("Status Code: " + (_lastStatusCode == HttpStatusCode.Unused ? "N/A" : _lastStatusCode.ToString() + " (" + (int)_lastStatusCode + ")"));
                if (!string.IsNullOrEmpty(_lastErrorMessage))
                {
                    _showError = EditorGUILayout.Foldout(_showError, "Error Details", true, EditorStyles.foldoutHeader);
                    if (_showError) EditorGUILayout.HelpBox(_lastErrorMessage, MessageType.Error);
                }
                if (!string.IsNullOrEmpty(_lastResponseJson))
                {
                    _showResponse = EditorGUILayout.Foldout(_showResponse, "Response JSON", true, EditorStyles.foldoutHeader);
                    if (_showResponse) EditorGUILayout.TextArea(_lastResponseJson, GUILayout.MinHeight(100), GUILayout.ExpandHeight(true));
                }
            }
            EditorGUILayout.EndVertical();
        }

        private bool ValidateParameters()
        {
            if (_selectedEndpoint == null || _selectedEndpoint.Parameters == null) return true;
            foreach (var param in _selectedEndpoint.Parameters)
            {
                if (param.Required && string.IsNullOrEmpty(GetParamValue(param.Name))) return false;
            }
            return true;
        }

        private void ExecuteApiCall() // UI event handler, calls the async version
        {
            if (_isExecuting) return;
            ExecuteApiCallInternal();
        }

        private async void ExecuteApiCallInternal() // Async void for top-level async from UI
        {
            if (_selectedEndpoint == null) return;

            _isExecuting = true;
            _lastResponseJson = "";
            _lastErrorMessage = "";
            _lastStatusCode = HttpStatusCode.Unused;
            Repaint(); // Show "Executing..."

            // Create a new transport instance for each call to ensure it uses the latest auth token if set
            _transport = new UnityWebRequestTransport();
            if (!string.IsNullOrEmpty(_userAuthToken))
            {
                _transport.SetAuthHeaderProvider(() => new Dictionary<string, string>
                {
                    ["authentication-token"] = _userAuthToken
                });
            }

            try
            {
                await ExecuteSelectedEndpointLogicAsync(); // Actual logic is here
                // If successful and no specific status code was caught as an error, assume 2xx.
                // The transport layer should throw GameFuseApiException which includes status code for errors.
                // If ExecuteSelectedEndpointLogicAsync completes without throwing, it implies success.
                // We might not have the exact success status code (e.g. 200 vs 201) unless the transport returns it.
                // For simplicity, if it doesn't throw an API exception, we might just show "OK" or not update _lastStatusCode from Unused.
                // Let's assume for now successful calls don't need to update _lastStatusCode here, only errors.
                // Or, methods in ExecuteSelectedEndpointLogicAsync could set it.
                // If an API call returns a response that includes status, parse it.
                // For now, if no exception, it's "successful enough" for the test tool.
                if (_lastStatusCode == HttpStatusCode.Unused) _lastStatusCode = HttpStatusCode.OK; // Default success
            }
            catch (GameFuseApiException apiEx)
            {
                _lastErrorMessage = $"API Error: {apiEx.Message}\nCode: {apiEx.ApiErrorCode}\nDetails: {apiEx.Message}";
                _lastStatusCode = apiEx.StatusCode;
                Debug.LogError($"GameFuse API Exception: {apiEx}");
            }
            catch (Exception ex)
            {
                _lastErrorMessage = $"Generic Error: {ex.Message}\nStackTrace: {ex.StackTrace}";
                _lastStatusCode = HttpStatusCode.InternalServerError; // Generic error
                Debug.LogError($"Exception during API call: {ex}");
            }
            finally
            {
                _isExecuting = false;
                Repaint(); // Update UI with response/error
            }
        }

        private async Task ExecuteSelectedEndpointLogicAsync()
        {
            // Reset for this call, successful execution will fill _lastResponseJson
            _lastResponseJson = "";
            _lastStatusCode = HttpStatusCode.Unused;


            switch (_selectedEndpoint.Type)
            {
                case EndpointType.SignUp:
                    var authServiceSignUp = new AuthService(_transport);
                    User signUpUser = await authServiceSignUp.SignUpAsync(
                        GetParamValue("email"), GetParamValue("password"), GetParamValue("username"),
                        _gameId, _gameApiKey);
                    _lastResponseJson = JsonConvert.SerializeObject(signUpUser, Formatting.Indented);
                    _userAuthToken = signUpUser.AuthenticationToken; // Update token
                    // No need to call UpdateAuthTokenInTransport as next call creates new transport
                    break;

                case EndpointType.SignIn:
                    var authServiceSignIn = new AuthService(_transport);
                    User signInUser = await authServiceSignIn.SignInAsync(
                        GetParamValue("emailOrUsername"), GetParamValue("password"),
                        _gameId, _gameApiKey);
                    _lastResponseJson = JsonConvert.SerializeObject(signInUser, Formatting.Indented);
                    _userAuthToken = signInUser.AuthenticationToken; // Update token
                    break;

                case EndpointType.ForgotPassword:
                    var authServiceForgot = new AuthService(_transport);
                    // ForgotPasswordAsync in service returns Task, not specific data for JSON
                    await authServiceForgot.ForgotPasswordAsync(GetParamValue("email"), _gameId, _gameApiKey);
                    _lastResponseJson = "{ \"message\": \"Forgot password request sent if email exists.\" }";
                    break;

                case EndpointType.GetCurrentUser:
                    if (string.IsNullOrEmpty(_userAuthToken)) { throw new InvalidOperationException("User not authenticated. Sign In or Sign Up first."); }
                    // CurrentUser is typically GameFuseUser.CurrentUser.Id when using the facade.
                    // For this direct tool, we need a way to get the current user's ID.
                    // A simple but fragile way is to assume it's part of the auth token, but that's an implementation detail.
                    // For a test tool, it might be acceptable to require user to input their ID or we parse from _userAuthToken if possible (not recommended for SDK).
                    // Let's assume we need the signed-in user ID. For now, if GameFuseUser.CurrentUser is available:
                    if (GameFuseUser.CurrentUser != null && GameFuseUser.CurrentUser.IsAuthenticated())
                    {
                        var userService = new UserService(_transport);
                        User currentUser = await userService.GetUserAsync(GameFuseUser.CurrentUser.Id);
                        _lastResponseJson = JsonConvert.SerializeObject(currentUser, Formatting.Indented);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot get current user: No GameFuseUser.CurrentUser available or not authenticated.");
                    }
                    break;

                case EndpointType.CreateDirectChat: // Example from recent additions
                    if (string.IsNullOrEmpty(_userAuthToken)) { throw new InvalidOperationException("User not authenticated."); }
                    var msgService = new MessageService(_transport);
                    var usernames = GetParamValue("usernames").Split(',').Select(s => s.Trim()).ToList();
                    var initialMsg = GetParamValue("initialMessage");
                    Chat createdChat = await msgService.CreateChatAsync(new CreateChatPayload { Usernames = usernames, Text = initialMsg });
                    _lastResponseJson = JsonConvert.SerializeObject(createdChat, Formatting.Indented);
                    break;

                // --- ADD CASES FOR ALL YOUR OTHER EndpointType VALUES HERE ---
                // Example:
                // case EndpointType.GetStoreItems:
                //     var storeService = new StoreService(_transport); // Needs gameId, gameToken, not user auth
                //     var itemsResponse = await storeService.GetAvailableStoreItemsAsync(_gameId, _gameApiKey);
                //     _lastResponseJson = JsonConvert.SerializeObject(itemsResponse, Formatting.Indented);
                //     break;

                default:
                    _lastErrorMessage = $"Endpoint '{_selectedEndpoint.Name}' (Type: {_selectedEndpoint.Type}) execution logic not implemented yet.";
                    _lastStatusCode = HttpStatusCode.NotImplemented;
                    _lastResponseJson = $"{{ \"error\": \"{_lastErrorMessage}\" }}";
                    break;
            }
        }

        private string GetParamValue(string paramName)
        {
            _paramValues.TryGetValue(paramName, out string value);
            return value == NullString ? null : value; // Allow intentional nulls if needed by API
        }

        // Generic GetParamValue<T> is removed for simplicity; cast/parse explicitly in ExecuteSelectedEndpointLogicAsync
        // This makes error handling for parsing more direct within each case.
    }
}