using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    public class GameRoundsService : AbstractService, IGameRoundsService
    {
        public GameRoundsService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        public async Task<GameRoundObject> CreateGameRoundAsync(int gameUserId)
        {
            var gameRound = new GameRoundObject { GameUserId = gameUserId };
            return await CreateGameRoundAsync(gameRound);
        }

        public async Task<GameRoundObject> CreateGameRoundAsync(GameRoundObject gameRound)
        {
            if (gameRound.GameUserId == 0)
            {
                throw new ArgumentException("GameUserId is required for creating a game round", nameof(gameRound));
            }

            string url = $"{_baseUrl}/game_rounds";
            string jsonBody = SerializeRequest(gameRound);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GameRoundObject>(webRequest);
            }
        }

        public async Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, GameRoundObject gameRound)
        {
            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";
            string jsonBody = SerializeRequest(gameRound);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.PUT, jsonBody))
            {
                return await SendRequestAsync<GameRoundObject>(webRequest);
            }
        }

        public async Task<GameRoundObject> GetGameRoundAsync(int gameRoundId)
        {
            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GameRoundObject>(webRequest);
            }
        }

        public async Task<GameRoundsResponse> GetUserGameRoundsAsync(int userId)
        {
            string url = $"{_baseUrl}/game_rounds?user_id={userId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GameRoundsResponse>(webRequest);
            }
        }

        public async Task<MessageResponse> DeleteGameRoundAsync(int gameRoundId)
        {
            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.DELETE))
            {
                return await SendRequestAsync<MessageResponse>(webRequest);
            }
        }
    }
}