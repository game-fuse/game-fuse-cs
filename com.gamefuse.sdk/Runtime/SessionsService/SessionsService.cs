using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    public class SessionsService : AbstractService, ISessionsService
    {
        public SessionsService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        protected override void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            string url = $"{_baseUrl}/sessions";
            string jsonBody = JsonUtility.ToJson(request);
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    SignInResponse response = await SendRequestAsync<SignInResponse>(webRequest);
                    _token = response.authentication_token;
                    return response;
                }
                catch (ApiException)
                {
                    throw;
                }
            }
        }
    }
}