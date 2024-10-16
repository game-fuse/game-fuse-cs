using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

namespace GameFuseCSharp
{
    public class UserService : AbstractService, IUserService
    {

        protected override void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        public async Task<SignUpResponse> SignUpAsync(SignUpRequest request)
        {
            string url = $"{_baseUrl}/users";
            string jsonBody = JsonUtility.ToJson(request);

            Debug.Log($"Sending SignUp request to: {url}");
            Debug.Log($"Request body: {jsonBody}");

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<SignUpResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"SignUp failed. Status Code: {ex.StatusCode}, Error: {ex.Message}");
                    Debug.LogError($"Response body: {ex.ResponseBody}");
                    throw;
                }
            }
        }
    }
}
