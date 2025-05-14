using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    public class LeaderboardService : AbstractService, ILeaderboardService
    {
        public LeaderboardService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        public async Task<MessageResponse> AddLeaderboardEntryAsync(int userId, string leaderboardName, int score)
        {
            return await AddLeaderboardEntryAsync(userId, leaderboardName, score, null);
        }

        public async Task<MessageResponse> AddLeaderboardEntryAsync(int userId, string leaderboardName, int score, object metadata)
        {
            if (string.IsNullOrEmpty(leaderboardName))
            {
                throw new ArgumentException("Leaderboard name is required", nameof(leaderboardName));
            }

            string url = $"{_baseUrl}/users/{userId}/add_leaderboard_entry";
            
            var requestData = new
            {
                leaderboard_name = leaderboardName,
                score = score,
                metadata = metadata
            };
            
            string jsonBody = SerializeRequest(requestData);
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<MessageResponse>(webRequest);
            }
        }

        public async Task<MessageResponse> ClearLeaderboardEntriesAsync(int userId)
        {
            string url = $"{_baseUrl}/users/{userId}/clear_my_leaderboard_entries";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST))
            {
                return await SendRequestAsync<MessageResponse>(webRequest);
            }
        }

        public async Task<LeaderboardEntriesResponse> GetUserLeaderboardEntriesAsync(int userId, int limit, string leaderboardName = null, bool onePerUser = false)
        {
            string url = $"{_baseUrl}/users/{userId}/leaderboard_entries?limit={limit}";
            
            if (!string.IsNullOrEmpty(leaderboardName))
            {
                url += $"&leaderboard_name={Uri.EscapeDataString(leaderboardName)}";
            }
            
            url += $"&one_per_user={onePerUser.ToString().ToLower()}";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<LeaderboardEntriesResponse>(webRequest);
            }
        }

        public async Task<LeaderboardEntriesResponse> GetGameLeaderboardEntriesAsync(int gameId, string leaderboardName, int limit)
        {
            if (string.IsNullOrEmpty(leaderboardName))
            {
                throw new ArgumentException("Leaderboard name is required", nameof(leaderboardName));
            }
            
            string url = $"{_baseUrl}/games/{gameId}/leaderboard_entries?leaderboard_name={Uri.EscapeDataString(leaderboardName)}&limit={limit}";
            
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<LeaderboardEntriesResponse>(webRequest);
            }
        }
    }
}