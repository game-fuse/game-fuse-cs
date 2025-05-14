using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
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
            var gameRound = new GameRoundObject
            {
                GameUserId = gameUserId,
                GameType = "default"  // Providing required game_type
            };
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

        public async Task<MultiplayerGameRoundResponse> CreateMultiplayerGameRoundAsync(string gameType, List<GameRoundObject> playerRounds)
        {
            if (string.IsNullOrEmpty(gameType))
            {
                throw new ArgumentException("GameType is required for multiplayer game rounds", nameof(gameType));
            }

            if (playerRounds == null || playerRounds.Count == 0)
            {
                throw new ArgumentException("At least one player round is required", nameof(playerRounds));
            }

            // Create the multiplayer game round first
            var multiplayerRound = new GameRoundObject
            {
                GameType = gameType,
                Multiplayer = true
            };

            string url = $"{_baseUrl}/game_rounds";
            string jsonBody = SerializeRequest(multiplayerRound);

            GameRoundObject createdMultiplayerRound;
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                createdMultiplayerRound = await SendRequestAsync<GameRoundObject>(webRequest);
            }

            // Now create individual player rounds linked to the multiplayer round
            var createdPlayerRounds = new List<GameRoundObject>();
            foreach (var playerRound in playerRounds)
            {
                // Make sure we set the multiplayer game round ID to link them
                playerRound.MultiplayerGameRoundId = createdMultiplayerRound.Id;
                
                var createdRound = await CreateGameRoundAsync(playerRound);
                createdPlayerRounds.Add(createdRound);
            }

            // Get the complete multiplayer game round with rankings
            return await GetMultiplayerGameRoundAsync(createdMultiplayerRound.Id);
        }

        public async Task<GameRoundObject> UpdateGameRoundAsync(int gameRoundId, GameRoundObject gameRound)
        {
            string url = $"{_baseUrl}/game_rounds/{gameRoundId}";

            // First, get the existing game round to check if it's multiplayer
            GameRoundObject existingRound = await GetGameRoundAsync(gameRoundId);

            // Create update object with all allowed fields
            var updateData = new
            {
                // Don't include game_type if this is a multiplayer game
                game_type = existingRound.MultiplayerGameRoundId.HasValue ? null : gameRound.GameType,
                place = gameRound.Place,
                score = gameRound.Score,
                start_time = gameRound.StartTime,
                end_time = gameRound.EndTime,
                metadata = gameRound.Metadata
            };

            string jsonBody = SerializeRequest(updateData);

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

        public async Task<MultiplayerGameRoundResponse> GetMultiplayerGameRoundAsync(int multiplayerGameRoundId)
        {
            string url = $"{_baseUrl}/game_rounds/multiplayer_game_round/{multiplayerGameRoundId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<MultiplayerGameRoundResponse>(webRequest);
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