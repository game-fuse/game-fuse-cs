using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    public class UserService : AbstractService, IUserService
    {
        public UserService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        protected override void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        public async Task<SignInResponse> SignUpAsync(SignUpRequest request)
        {
            string url = $"{_baseUrl}/users";
            string jsonBody = JsonUtility.ToJson(request);
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<SignInResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.Log($"UserSerive SignUpAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
