using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
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
        
        /// <summary>
        /// Signs up a new user
        /// </summary>
        public async Task<SignInResponse> SignUpAsync(SignUpRequest request)
        {
            string url = $"{_baseUrl}/users";
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
        
        /// <summary>
        /// Sends a password reset email to the specified email address
        /// </summary>
        public async Task<bool> SendPasswordResetEmailAsync(string email, string gameId, string gameToken)
        {
            // The GameFuse API expects game_token and game_id as query parameters for this endpoint
            string url = $"{_baseUrl}/games/{gameId}/forget_password?game_token={gameToken}&game_id={gameId}&email={email}";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                try
                {
                    // For this endpoint, we just need to check if the request was successful
                    // We're using a simple response class to parse the success message
                    var response = await SendRequestAsync<Dictionary<string, object>>(webRequest);
                    return true;
                }
                catch (ApiException)
                {
                    throw;
                }
            }
        }
    }
}