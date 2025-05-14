using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

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
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    SignInResponse response = await SendRequestAsync<SignInResponse>(webRequest);
                    _token = response.AuthenticationToken;
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