
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

namespace GameFuseCSharp
{

    public class FriendshipService
    {
        private string _baseUrl;
        private const int TimeoutSeconds = 10;

        private void Initialize(string baseUrl)
        {
            _baseUrl = baseUrl; // Replace with your actual base URL
        }

        public async Task<FriendRequestResponse> SendFriendRequestAsync(string username, string authToken)
        {
            string url = $"{_baseUrl}/friendships";
            string jsonBody = JsonUtility.ToJson(new FriendRequestData { username = username });

            using (UnityWebRequest webRequest = CreatePostRequest(url, jsonBody, authToken))
            {
                return await SendRequestAsync<FriendRequestResponse>(webRequest);
            }
        }

        public async Task<FriendshipStatusResponse> UpdateFriendRequestStatusAsync(int friendshipId, string status, string authToken)
        {
            string url = $"{_baseUrl}/friendships/{friendshipId}";
            string jsonBody = JsonUtility.ToJson(new FriendshipStatusData { status = status });

            using (UnityWebRequest webRequest = CreatePutRequest(url, jsonBody, authToken))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        public async Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId, string authToken)
        {
            string url = $"{_baseUrl}/friendships/{friendshipId}";

            using (UnityWebRequest webRequest = CreateDeleteRequest(url, authToken))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        public async Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId, string authToken)
        {
            string url = $"{_baseUrl}/unfriend?user_id={userId}";

            using (UnityWebRequest webRequest = CreateDeleteRequest(url, authToken))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        public async Task<FriendshipDataResponse> GetFriendshipDataAsync(string authToken)
        {
            string url = $"{_baseUrl}/friendships";

            using (UnityWebRequest webRequest = CreateGetRequest(url, authToken))
            {
                return await SendRequestAsync<FriendshipDataResponse>(webRequest);
            }
        }

        private UnityWebRequest CreateGetRequest(string url, string authToken)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("authentication-token", authToken);
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        private UnityWebRequest CreatePostRequest(string url, string jsonBody, string authToken)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("authentication-token", authToken);
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        private UnityWebRequest CreatePutRequest(string url, string jsonBody, string authToken)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "PUT");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("authentication-token", authToken);
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        private UnityWebRequest CreateDeleteRequest(string url, string authToken)
        {
            UnityWebRequest webRequest = UnityWebRequest.Delete(url);
            webRequest.SetRequestHeader("authentication-token", authToken);
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        private async Task<T> SendRequestAsync<T>(UnityWebRequest webRequest) where T : new()
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

    [System.Serializable]
    public class FriendRequestData
    {
        public string username;
    }

    [System.Serializable]
    public class FriendshipStatusData
    {
        public string status;
    }

    [System.Serializable]
    public class FriendRequestResponse
    {
        public string message;
        public int friendshipId;
    }

    [System.Serializable]
    public class FriendshipStatusResponse
    {
        public string message;
    }

    [System.Serializable]
    public class FriendshipDataResponse
    {
        public UserInfo[] friends;
        public FriendRequest[] outgoingFriendRequests;
        public FriendRequest[] incomingFriendRequests;
    }

    [System.Serializable]
    public class UserInfo
    {
        public int id;
        public string username;
        public string email;
        public string displayEmail;
        public int credits;
        public int score;
    }

    [System.Serializable]
    public class FriendRequest : UserInfo
    {
        public int friendshipId;
        public string requestedAt;
    }
}