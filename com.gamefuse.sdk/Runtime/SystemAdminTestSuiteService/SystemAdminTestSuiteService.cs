using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text;

namespace GameFuseCSharp
{

    public class SystemAdminTestSuiteService : AbstractService, ISystemAdminTestSuiteService
    {
        private string _serviceKeyToken;
        private string _serviceKeyName;

        public SystemAdminTestSuiteService(string baseUrl, string token, string serviceKeyName)
        {
            _baseUrl = $"{baseUrl}/test_suite";
            _serviceKeyToken = token;
            _serviceKeyName = serviceKeyName;
        }

        protected override void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("service-key-token", _serviceKeyToken);
            webRequest.SetRequestHeader("service-key-name", _serviceKeyName);
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        public async Task<CreateGameResponse> CreateGameAsync()
        {
            string url = $"{_baseUrl}/create_game";
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST ,string.Empty))
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
            using(UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, createUserJson))
            {
                return await SendRequestAsync<CreateUserResponse>(webRequest);
            }
        }

        public async Task<CleanUpResponse> CleanUpTestAsync(int gameId)
        {
            string url = $"{_baseUrl}/clean_up_test";
            CleanUpGameRequest deleteGameRequest = new CleanUpGameRequest { game_id = gameId };
            string deleteGameJson = JsonUtility.ToJson(deleteGameRequest);
            using(UnityWebRequest unityWebRequest = CreateRequest(url,HttpVerbs.DELETE,deleteGameJson))
            {
                return await SendRequestAsync<CleanUpResponse>(unityWebRequest);
            }
        }
    }
}