using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text;

namespace GameFuseCSharp
{

    public class SystemAdminTestSuiteService : ISystemAdminTestSuiteService
    {
        private string _baseUrl = "https://gamefuse.co/api/v3/test_suite";
        private string _serviceKeyToken;
        private string _serviceKeyName;

        private const int TimeoutSeconds = 10;

        public void Initialize(string token, string name)
        {
            _serviceKeyToken = token;
            _serviceKeyName = name;
        }

        public void Initialize(string baseUrl, string token, string name)
        {
            _baseUrl = baseUrl;
            Initialize(token, name);
        }

        public async Task<CreateGameResponse> CreateGameAsync()
        {
            string url = $"{_baseUrl}/create_game";
            using (UnityWebRequest webRequest = CreatePostRequest(url, string.Empty))
            {
                return await SendRequestAsync<CreateGameResponse>(webRequest);
            }
        }

        public async Task<CreateUserResponse> CreateUserAsync(int gameId, string username, string email)
        {
            string url = $"{_baseUrl}/create_user";
            CreateUserRequest createUserRequest = new CreateUserRequest
            {
                game_id = gameId,
                username = username,
                email = email
            };
            string createUserJson = JsonUtility.ToJson(createUserRequest);
            using(UnityWebRequest webRequest = CreatePostRequest(url, createUserJson))
            {
                return await SendRequestAsync<CreateUserResponse>(webRequest);
            }
        }

        public async Task<CreateStoreItemResponse> CreateStoreItemAsync(int gameId, string name, string description, string category, int cost)
        {
            string url = $"{_baseUrl}/create_store_item";
            CreateStoreItemRequest createStoreItemRequest = new CreateStoreItemRequest
            {
                game_id = gameId,
                name = name,
                description = description,
                category = category,
                cost = cost
            };
            string createStoreItemJson = JsonUtility.ToJson(createStoreItemRequest);
            using(UnityWebRequest webRequest = CreatePostRequest(url, createStoreItemJson))
            {
                return await SendRequestAsync<CreateStoreItemResponse>(webRequest);
            }
        }

        public async Task<CleanUpResponse> CleanUpTestAsync(int gameId)
        {
            string url = $"{_baseUrl}/clean_up_test";
            CleanUpGameRequest deleteGameRequest = new CleanUpGameRequest { game_id = gameId };
            string deleteGameJson = JsonUtility.ToJson(deleteGameRequest);
            using(UnityWebRequest unityWebRequest = CreateDeleteRequest(url, deleteGameJson))
            {
                return await SendRequestAsync<CleanUpResponse>(unityWebRequest);
            }
        }

        private UnityWebRequest CreatePostRequest(string url, string jsonBody)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            SetRequestHeaders(webRequest);
            SetRequestBody(webRequest, jsonBody);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        private UnityWebRequest CreateDeleteRequest(string url, string jsonBody)
        {
            UnityWebRequest webRequest = UnityWebRequest.Delete(url);
            SetRequestHeaders(webRequest);
            SetRequestBody(webRequest, jsonBody);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        private void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("service-key-token", _serviceKeyToken);
            webRequest.SetRequestHeader("service-key-name", _serviceKeyName);
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        private void SetRequestBody(UnityWebRequest webRequest, string jsonBody)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        private async Task<T> SendRequestAsync<T>(UnityWebRequest webRequest) where T : class, new()
        {
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                return JsonUtility.FromJson<T>(jsonResponse);
            }
            else
            {
                Debug.LogError($"Error: {webRequest.error}");
                return default(T);
            }
        }
    }
}