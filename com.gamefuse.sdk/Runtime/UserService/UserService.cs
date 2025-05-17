using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

namespace GameFuseCSharp
{
    public class UserService : AbstractService, IUserService
    {
        public UserService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        
        public UserService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        protected override void SetRequestHeaders(UnityWebRequest webRequest)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                webRequest.SetRequestHeader("authentication-token", _token);
            }
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        public async Task<SignInResponse> SignUpAsync(SignUpRequest request)
        {
            string url = $"{_baseUrl}/users";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    var response = await SendRequestAsync<SignInResponse>(webRequest);
                    _token = response.AuthenticationToken;
                    return response;
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService SignUpAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserCreditsResponse> AddCreditsAsync(int userId, AddCreditsRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/add_credits";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserCreditsResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService AddCreditsAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserCreditsResponse> SetCreditsAsync(int userId, SetCreditsRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/set_credits";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserCreditsResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService SetCreditsAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserScoreResponse> AddScoreAsync(int userId, AddScoreRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/add_score";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserScoreResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService AddScoreAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserScoreResponse> SetScoreAsync(int userId, SetScoreRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/set_score";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserScoreResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService SetScoreAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserAttributesResponse> GetAttributesAsync(int userId)
        {
            string url = $"{_baseUrl}/users/{userId}/game_user_attributes";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                try
                {
                    return await SendRequestAsync<UserAttributesResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService GetAttributesAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserAttributesResponse> SetAttributeAsync(int userId, SetAttributeRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/add_game_user_attribute";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserAttributesResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService SetAttributeAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserAttributesResponse> SetAttributesAsync(int userId, SetAttributesRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/add_game_user_attribute";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserAttributesResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService SetAttributesAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserAttributesResponse> RemoveAttributeAsync(int userId, string attributeKey)
        {
            string encodedKey = UnityWebRequest.EscapeURL(attributeKey);
            string url = $"{_baseUrl}/users/{userId}/remove_game_user_attributes?game_user_attribute_key={encodedKey}";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                try
                {
                    return await SendRequestAsync<UserAttributesResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService RemoveAttributeAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserStoreItemsResponse> GetStoreItemsAsync(int userId)
        {
            string url = $"{_baseUrl}/users/{userId}/game_user_store_items";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                try
                {
                    return await SendRequestAsync<UserStoreItemsResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService GetStoreItemsAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserStoreItemsResponse> PurchaseStoreItemAsync(int userId, PurchaseStoreItemRequest request)
        {
            string url = $"{_baseUrl}/users/{userId}/purchase_game_user_store_item";
            string jsonBody = JsonConvert.SerializeObject(request, JsonSettings);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<UserStoreItemsResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService PurchaseStoreItemAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task<UserStoreItemsResponse> RemoveStoreItemAsync(int userId, RemoveStoreItemRequest request)
        {
            // The API is expecting a GET request with query parameters
            string url = $"{_baseUrl}/users/{userId}/remove_game_user_store_item?store_item_id={request.StoreItemId}&reimburse={request.Reimburse.ToString().ToLower()}";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                try
                {
                    return await SendRequestAsync<UserStoreItemsResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"UserService RemoveStoreItemAsync ApiException: \n Status Code: {ex.StatusCode} \n Message: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
