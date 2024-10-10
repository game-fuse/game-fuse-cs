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
            using(UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, createStoreItemJson))
            {
                return await SendRequestAsync<CreateStoreItemResponse>(webRequest);
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