using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public class FriendshipService : AbstractService, IFriendshipService
    {

       
        public async Task<FriendRequestResponse> SendFriendRequestAsync(string username)
        {
            string url = $"{_baseUrl}/friendships";
            string jsonBody = JsonUtility.ToJson(new FriendRequestData { username = username });

            using (UnityWebRequest webRequest = CreateRequest(url,HttpVerbs.POST,jsonBody))
            {
                return await SendRequestAsync<FriendRequestResponse>(webRequest);
            }
        }

        public async Task<FriendshipStatusResponse> UpdateFriendRequestStatusAsync(int friendshipId, string status)
        {
            string url = $"{_baseUrl}/friendships/{friendshipId}";
            string jsonBody = JsonUtility.ToJson(new FriendshipStatusData { status = status });

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.PUT, jsonBody))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        public async Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId)
        {
            string url = $"{_baseUrl}/friendships/{friendshipId}";

            using (UnityWebRequest webRequest = CreateRequest(url,HttpVerbs.DELETE))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        public async Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId)
        {
            string url = $"{_baseUrl}/unfriend?user_id={userId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.DELETE))
            {
                return await SendRequestAsync<FriendshipStatusResponse>(webRequest);
            }
        }

        public async Task<FriendshipDataResponse> GetFriendshipDataAsync()
        {
            string url = $"{_baseUrl}/friendships";

            using (UnityWebRequest webRequest = CreateRequest(url,HttpVerbs.GET))
            {
                return await SendRequestAsync<FriendshipDataResponse>(webRequest);
            }
        }
    }
}