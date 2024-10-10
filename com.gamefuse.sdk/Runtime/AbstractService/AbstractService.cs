using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public abstract class AbstractService : IServiceInitializable
    {
        protected string _baseUrl = "https://gamefuse.co/api/v3/test_suite";
        protected string _serviceKeyToken;
        protected string _serviceKeyName;
        protected const int TimeoutSeconds = 10;

        protected enum HttpVerbs
        {
            DELETE,
            GET,
            POST,
            PUT
        }

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

        protected UnityWebRequest CreateRequest(string url, HttpVerbs method, string jsonBody)
        {
            UnityWebRequest webRequest = CreateRequest(url, method);
            SetRequestBody(webRequest, jsonBody);
            return webRequest;
        }

        protected UnityWebRequest CreateRequest(string url, HttpVerbs method)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, method.ToString());
            SetRequestHeaders(webRequest);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        protected void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("service-key-token", _serviceKeyToken);
            webRequest.SetRequestHeader("service-key-name", _serviceKeyName);
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }
        protected void SetRequestBody(UnityWebRequest webRequest, string jsonBody)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        protected async Task<T> SendRequestAsync<T>(UnityWebRequest webRequest) where T : class, new()
        {
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"the raw json is: \n {jsonResponse}");
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
