using GameFuse.Config;
using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Services;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameFuse.Editor
{
    /// <summary>
    /// Unity Editor window for testing GameFuse API calls.
    /// </summary>
    public class GameFuseApiTestWindow : EditorWindow
    {
        private const string NullString = "(null)";
        private const float IndentWidth = 15f;

        private string _gameId;
        private string _gameApiKey;
        private string _userAuthToken;
        private Vector2 _scrollPosition;
        private bool _isExecuting;
        private HttpStatusCode _lastStatusCode;
        private string _lastResponseJson;
        private string _lastErrorMessage;
        private bool _showResponse = true;
        private bool _showError;
        private ApiEndpoint _selectedEndpoint;
        private Dictionary<string, string> _paramValues = new Dictionary<string, string>();
        private ITransport _transport;

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
            new ApiCategory("User", new List<ApiEndpoint>
            {
                new ApiEndpoint("Get Current User", EndpointType.GetCurrentUser, null),
                new ApiEndpoint("Update User", EndpointType.UpdateUser, new List<ApiParameter>
                {
                    new ApiParameter("username", "Username", false),
                    new ApiParameter("email", "Email", false)
                }),
                new ApiEndpoint("Update Password", EndpointType.UpdatePassword, new List<ApiParameter>
                {
                    new ApiParameter("currentPassword", "Current Password", true),
                    new ApiParameter("newPassword", "New Password", true)
                }),
                new ApiEndpoint("Set User Attribute", EndpointType.SetUserAttribute, new List<ApiParameter>
                {
                    new ApiParameter("key", "Key", true),
                    new ApiParameter("value", "Value", true)
                }),
                new ApiEndpoint("Get User Attributes", EndpointType.GetUserAttributes, null)
            }),
            new ApiCategory("Game Rounds", new List<ApiEndpoint>
            {
                new ApiEndpoint("Create Game Round", EndpointType.CreateGameRound, new List<ApiParameter>
                {
                    new ApiParameter("level", "Level", false),
                    new ApiParameter("customData", "Custom Data", false)
                }),
                new ApiEndpoint("Get Game Round", EndpointType.GetGameRound, new List<ApiParameter>
                {
                    new ApiParameter("gameRoundId", "Game Round ID", true)
                }),
                new ApiEndpoint("Get User Game Rounds", EndpointType.GetUserGameRounds, null),
                new ApiEndpoint("Update Game Round", EndpointType.UpdateGameRound, new List<ApiParameter>
                {
                    new ApiParameter("gameRoundId", "Game Round ID", true),
                    new ApiParameter("score", "Score", false),
                    new ApiParameter("customData", "Custom Data", false),
                    new ApiParameter("ended", "Ended", false)
                }),
                new ApiEndpoint("Get Leaderboard", EndpointType.GetLeaderboard, new List<ApiParameter>
                {
                    new ApiParameter("limit", "Limit", false)
                }),
                new ApiEndpoint("Get User Rank", EndpointType.GetUserRank, null)
            }),
            new ApiCategory("Store", new List<ApiEndpoint>
            {
                new ApiEndpoint("Get Store Items", EndpointType.GetStoreItems, null),
                new ApiEndpoint("Get Store Item", EndpointType.GetStoreItem, new List<ApiParameter>
                {
                    new ApiParameter("itemId", "Item ID", true)
                }),
                new ApiEndpoint("Purchase Item", EndpointType.PurchaseItem, new List<ApiParameter>
                {
                    new ApiParameter("itemId", "Item ID", true)
                }),
                new ApiEndpoint("Get User Purchases", EndpointType.GetUserPurchases, null),
                new ApiEndpoint("Get Credit Balance", EndpointType.GetCreditBalance, null),
                new ApiEndpoint("Get Credit Transactions", EndpointType.GetCreditTransactions, null)
            }),
            new ApiCategory("Friends", new List<ApiEndpoint>
            {
                new ApiEndpoint("Get Friends", EndpointType.GetFriends, null),
                new ApiEndpoint("Send Friend Request", EndpointType.SendFriendRequest, new List<ApiParameter>
                {
                    new ApiParameter("friendId", "Friend ID", true)
                }),
                new ApiEndpoint("Get Friend Requests", EndpointType.GetFriendRequests, null),
                new ApiEndpoint("Accept Friend Request", EndpointType.AcceptFriendRequest, new List<ApiParameter>
                {
                    new ApiParameter("friendshipId", "Friendship ID", true)
                }),
                new ApiEndpoint("Reject Friend Request", EndpointType.RejectFriendRequest, new List<ApiParameter>
                {
                    new ApiParameter("friendshipId", "Friendship ID", true)
                }),
                new ApiEndpoint("Remove Friend", EndpointType.RemoveFriend, new List<ApiParameter>
                {
                    new ApiParameter("friendId", "Friend ID", true)
                }),
                new ApiEndpoint("Search Users", EndpointType.SearchUsers, new List<ApiParameter>
                {
                    new ApiParameter("query", "Query", true)
                })
            }),
            new ApiCategory("Groups", new List<ApiEndpoint>
            {
                new ApiEndpoint("Create Group", EndpointType.CreateGroup, new List<ApiParameter>
                {
                    new ApiParameter("name", "Name", true),
                    new ApiParameter("groupType", "Group Type", true),
                    new ApiParameter("canAutoJoin", "Can Auto Join", true),
                    new ApiParameter("isInviteOnly", "Is Invite Only", true),
                    new ApiParameter("maxGroupSize", "Max Group Size", true),
                    new ApiParameter("searchable", "Searchable", true)
                }),
                new ApiEndpoint("Get Group", EndpointType.GetGroup, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true)
                }),
                new ApiEndpoint("Get User Groups", EndpointType.GetUserGroups, null),
                new ApiEndpoint("Update Group", EndpointType.UpdateGroup, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true),
                    new ApiParameter("name", "Name", false),
                    new ApiParameter("groupType", "Group Type", false),
                    new ApiParameter("canAutoJoin", "Can Auto Join", false),
                    new ApiParameter("isInviteOnly", "Is Invite Only", false),
                    new ApiParameter("maxGroupSize", "Max Group Size", false),
                    new ApiParameter("searchable", "Searchable", false)
                }),
                new ApiEndpoint("Delete Group", EndpointType.DeleteGroup, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true)
                }),
                new ApiEndpoint("Add User To Group", EndpointType.AddUserToGroup, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true),
                    new ApiParameter("userId", "User ID", true),
                    new ApiParameter("isAdmin", "Is Admin", false)
                }),
                new ApiEndpoint("Remove User From Group", EndpointType.RemoveUserFromGroup, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true),
                    new ApiParameter("userId", "User ID", true)
                }),
                new ApiEndpoint("Send Join Request", EndpointType.SendJoinRequest, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true)
                }),
                new ApiEndpoint("Search Groups", EndpointType.SearchGroups, new List<ApiParameter>
                {
                    new ApiParameter("query", "Query", true)
                })
            }),
            new ApiCategory("Messages", new List<ApiEndpoint>
            {
                new ApiEndpoint("Send Message", EndpointType.SendMessage, new List<ApiParameter>
                {
                    new ApiParameter("recipientId", "Recipient ID", true),
                    new ApiParameter("content", "Content", true)
                }),
                new ApiEndpoint("Get Conversation", EndpointType.GetConversation, new List<ApiParameter>
                {
                    new ApiParameter("otherUserId", "Other User ID", true)
                }),
                new ApiEndpoint("Get Conversations", EndpointType.GetConversations, null),
                new ApiEndpoint("Mark Message As Read", EndpointType.MarkMessageAsRead, new List<ApiParameter>
                {
                    new ApiParameter("messageId", "Message ID", true)
                }),
                new ApiEndpoint("Delete Message", EndpointType.DeleteMessage, new List<ApiParameter>
                {
                    new ApiParameter("messageId", "Message ID", true)
                }),
                new ApiEndpoint("Send Group Message", EndpointType.SendGroupMessage, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true),
                    new ApiParameter("content", "Content", true)
                }),
                new ApiEndpoint("Get Group Messages", EndpointType.GetGroupMessages, new List<ApiParameter>
                {
                    new ApiParameter("groupId", "Group ID", true)
                })
            })
        };

        [MenuItem("Tools/GameFuse/API Test Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameFuseApiTestWindow>();
            window.titleContent = new GUIContent("GameFuse API Test");
            window.minSize = new Vector2(600, 500);
            window.Show();
        }

        private void OnEnable()
        {
            _transport = new UnityWebRequestTransport();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = GameFuseSettings.Settings;
            if (settings != null)
            {
                _gameId = settings.GameId;
                _gameApiKey = settings.GameApiKey;
            }
        }

        private void OnGUI()
        {
            DrawHeader();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawCredentialsSection();
            DrawApiTestSection();
            DrawResponseSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("GameFuse API Test Tool", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("Use this tool to test GameFuse API calls directly from the Unity Editor.", MessageType.Info);
            GUILayout.Space(10);
        }

        private void DrawCredentialsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Credentials", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            _gameId = EditorGUILayout.TextField("Game ID", _gameId);
            _gameApiKey = EditorGUILayout.TextField("Game API Key", _gameApiKey);
            _userAuthToken = EditorGUILayout.TextField("User Auth Token", _userAuthToken);
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(_userAuthToken))
            {
                _transport.SetAuthHeaderProvider(() => new Dictionary<string, string>
                {
                    ["authentication-token"] = _userAuthToken
                });
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Load from GameFuseSettings"))
            {
                LoadSettings();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawApiTestSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("API Test", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // Draw API categories
            foreach (var category in ApiCategories)
            {
                category.IsExpanded = EditorGUILayout.Foldout(category.IsExpanded, category.Name, true);
                if (category.IsExpanded)
                {
                    EditorGUI.indentLevel++;
                    foreach (var endpoint in category.Endpoints)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(IndentWidth);
                        if (GUILayout.Toggle(_selectedEndpoint == endpoint, endpoint.Name, EditorStyles.radioButton))
                        {
                            if (_selectedEndpoint != endpoint)
                            {
                                _selectedEndpoint = endpoint;
                                _paramValues.Clear();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
            }

            GUILayout.Space(5);

            // Draw parameters for selected endpoint
            if (_selectedEndpoint != null && _selectedEndpoint.Parameters != null && _selectedEndpoint.Parameters.Count > 0)
            {
                EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                GUILayout.Space(5);

                foreach (var param in _selectedEndpoint.Parameters)
                {
                    if (!_paramValues.ContainsKey(param.Name))
                    {
                        _paramValues[param.Name] = "";
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(param.DisplayName + (param.Required ? " *" : ""), GUILayout.Width(150));
                    _paramValues[param.Name] = EditorGUILayout.TextField(_paramValues[param.Name]);
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(5);
            }

            // Execute button
            GUI.enabled = !_isExecuting && _selectedEndpoint != null && ValidateParameters();
            if (GUILayout.Button("Execute API Call"))
            {
                ExecuteApiCall();
            }
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawResponseSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Response", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (_isExecuting)
            {
                EditorGUILayout.LabelField("Executing...");
            }
            else
            {
                EditorGUILayout.LabelField("Status Code: " + _lastStatusCode);
                GUILayout.Space(5);

                if (!string.IsNullOrEmpty(_lastErrorMessage))
                {
                    _showError = EditorGUILayout.Foldout(_showError, "Error", true);
                    if (_showError)
                    {
                        EditorGUILayout.HelpBox(_lastErrorMessage, MessageType.Error);
                    }
                    GUILayout.Space(5);
                }

                if (!string.IsNullOrEmpty(_lastResponseJson))
                {
                    _showResponse = EditorGUILayout.Foldout(_showResponse, "Response JSON", true);
                    if (_showResponse)
                    {
                        EditorGUILayout.TextArea(_lastResponseJson, GUILayout.Height(200));
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        private bool ValidateParameters()
        {
            if (_selectedEndpoint == null || _selectedEndpoint.Parameters == null)
            {
                return true;
            }

            foreach (var param in _selectedEndpoint.Parameters)
            {
                if (param.Required && !_paramValues.ContainsKey(param.Name) || param.Required && string.IsNullOrEmpty(_paramValues[param.Name]))
                {
                    return false;
                }
            }

            return true;
        }

        private void ExecuteApiCall()
        {
            if (_selectedEndpoint == null)
            {
                return;
            }

            _isExecuting = true;
            _lastResponseJson = "";
            _lastErrorMessage = "";
            _lastStatusCode = HttpStatusCode.OK;

            Task.Run(async () => await ExecuteEndpointAsync(_selectedEndpoint))
                .ContinueWith(t =>
                {
                    _isExecuting = false;
                    if (t.IsFaulted && t.Exception != null)
                    {
                        _lastErrorMessage = t.Exception.InnerException?.Message ?? t.Exception.Message;
                        if (t.Exception.InnerException is GameFuseApiException apiEx)
                        {
                            _lastStatusCode = apiEx.StatusCode;
                        }
                    }
                });
        }

        private async Task ExecuteEndpointAsync(ApiEndpoint endpoint)
        {
            switch (endpoint.Type)
            {
                case EndpointType.SignUp:
                    var authService1 = new AuthService(_transport);
                    var signUpResult = await authService1.SignUpAsync(
                        GetParamValue("email"),
                        GetParamValue("password"),
                        GetParamValue("username"),
                        _gameId,
                        _gameApiKey
                    );
                    _lastResponseJson = JsonUtility.ToJson(signUpResult, true);
                    _userAuthToken = signUpResult.AuthenticationToken;
                    _transport.SetAuthHeaderProvider(() => new Dictionary<string, string>
                    {
                        ["authentication-token"] = _userAuthToken
                    });
                    break;

                case EndpointType.SignIn:
                    var authService2 = new AuthService(_transport);
                    var signInResult = await authService2.SignInAsync(
                        GetParamValue("emailOrUsername"),
                        GetParamValue("password"),
                        _gameId,
                        _gameApiKey
                    );
                    _lastResponseJson = JsonUtility.ToJson(signInResult, true);
                    _userAuthToken = signInResult.AuthenticationToken;
                    _transport.SetAuthHeaderProvider(() => new Dictionary<string, string>
                    {
                        ["authentication-token"] = _userAuthToken
                    });
                    break;

                case EndpointType.ForgotPassword:
                    var authService3 = new AuthService(_transport);
                    await authService3.ForgotPasswordAsync(
                        GetParamValue("email"),
                        _gameId,
                        _gameApiKey
                    );
                    _lastResponseJson = "{ \"success\": true }";
                    break;

                case EndpointType.GetCurrentUser:
                    var userService1 = new UserService(_transport);
                    var getCurrentUserResult = await userService1.GetUserAsync(
                        int.Parse(_userAuthToken.Split(':')[0])
                    );
                    _lastResponseJson = JsonUtility.ToJson(getCurrentUserResult, true);
                    break;

                // TODO: Add implementations for all other endpoint types

                default:
                    _lastResponseJson = "{ \"error\": \"Endpoint not implemented yet.\" }";
                    break;
            }
        }

        private string GetParamValue(string paramName)
        {
            if (_paramValues.TryGetValue(paramName, out string value))
            {
                return value == NullString ? null : value;
            }
            return null;
        }

        private T GetParamValue<T>(string paramName)
        {
            if (_paramValues.TryGetValue(paramName, out string value))
            {
                if (value == NullString)
                {
                    return default;
                }

                Type type = typeof(T);
                if (type == typeof(string))
                {
                    return (T)(object)value;
                }
                else if (type == typeof(int))
                {
                    return int.TryParse(value, out int result) ? (T)(object)result : default;
                }
                else if (type == typeof(bool))
                {
                    return bool.TryParse(value, out bool result) ? (T)(object)result : default;
                }
                else if (type == typeof(float))
                {
                    return float.TryParse(value, out float result) ? (T)(object)result : default;
                }
                else if (type == typeof(double))
                {
                    return double.TryParse(value, out double result) ? (T)(object)result : default;
                }
            }
            return default;
        }

        private class ApiCategory
        {
            public string Name { get; }
            public List<ApiEndpoint> Endpoints { get; }
            public bool IsExpanded { get; set; }

            public ApiCategory(string name, List<ApiEndpoint> endpoints)
            {
                Name = name;
                Endpoints = endpoints;
                IsExpanded = false;
            }
        }

        private class ApiEndpoint
        {
            public string Name { get; }
            public EndpointType Type { get; }
            public List<ApiParameter> Parameters { get; }

            public ApiEndpoint(string name, EndpointType type, List<ApiParameter> parameters)
            {
                Name = name;
                Type = type;
                Parameters = parameters;
            }
        }

        private class ApiParameter
        {
            public string Name { get; }
            public string DisplayName { get; }
            public bool Required { get; }

            public ApiParameter(string name, string displayName, bool required)
            {
                Name = name;
                DisplayName = displayName;
                Required = required;
            }
        }

        private enum EndpointType
        {
            SignUp,
            SignIn,
            ForgotPassword,
            GetCurrentUser,
            UpdateUser,
            UpdatePassword,
            SetUserAttribute,
            GetUserAttributes,
            CreateGameRound,
            GetGameRound,
            GetUserGameRounds,
            UpdateGameRound,
            GetLeaderboard,
            GetUserRank,
            GetStoreItems,
            GetStoreItem,
            PurchaseItem,
            GetUserPurchases,
            GetCreditBalance,
            GetCreditTransactions,
            GetFriends,
            SendFriendRequest,
            GetFriendRequests,
            AcceptFriendRequest,
            RejectFriendRequest,
            RemoveFriend,
            SearchUsers,
            CreateGroup,
            GetGroup,
            GetUserGroups,
            UpdateGroup,
            DeleteGroup,
            AddUserToGroup,
            RemoveUserFromGroup,
            SendJoinRequest,
            SearchGroups,
            SendMessage,
            GetConversation,
            GetConversations,
            MarkMessageAsRead,
            DeleteMessage,
            SendGroupMessage,
            GetGroupMessages
        }
    }
}