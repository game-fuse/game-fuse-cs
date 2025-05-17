using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    public class ChatService : AbstractService, IChatService
    {
        public ChatService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
            Debug.Log($"ChatService created with baseUrl: {baseUrl}, token: {(string.IsNullOrEmpty(token) ? "NULL" : token.Substring(0, Mathf.Min(5, token.Length)) + "...")}");
        }
        
        protected override void SetRequestHeaders(UnityWebRequest webRequest)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                webRequest.SetRequestHeader("authentication_token", _token);
                Debug.Log($"ChatService setting auth token header: {_token.Substring(0, Mathf.Min(5, _token.Length))}...");
            }
            else
            {
                Debug.LogWarning("ChatService has no authentication token to set in request header");
            }
            
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        public async Task<GetChatsResponse> GetChatsAsync(int page = 1)
        {
            string url = $"{_baseUrl}/chats/page/{page}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GetChatsResponse>(webRequest);
            }
        }

        public async Task<Chat> CreateDirectChatAsync(string[] usernames, string text)
        {
            string url = $"{_baseUrl}/chats";

            var request = new CreateDirectChatRequest
            {
                Usernames = usernames,
                Text = text
            };

            string jsonBody = SerializeRequest(request);
            Debug.Log($"CreateDirectChatAsync - Request Body: {jsonBody}");
            Debug.Log($"CreateDirectChatAsync - Usernames: [{string.Join(", ", usernames)}], Text: {text}");
            Debug.Log($"CreateDirectChatAsync - Token: {_token?.Substring(0, Math.Min(5, _token?.Length ?? 0))}...");

            // Create the web request with authentication
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                // Double-check that auth header is set
                string existingToken = webRequest.GetRequestHeader("authentication_token");
                if (string.IsNullOrEmpty(existingToken) || !existingToken.Equals(_token))
                {
                    Debug.LogWarning("Authentication token header missing or mismatch - explicitly setting it");
                    webRequest.SetRequestHeader("authentication_token", _token);
                }
                
                try
                {
                    var response = await SendRequestAsync<CreateChatResponse>(webRequest, true);
                    Debug.Log($"CreateDirectChatAsync - Response received: {(response == null ? "null" : "not null")}");
                    Debug.Log($"CreateDirectChatAsync - Chat: {(response?.Chat == null ? "null" : "not null")}");
                    return response?.Chat;
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"CreateDirectChatAsync - API Exception: {ex.StatusCode}, {ex.Message}");
                    Debug.LogError($"CreateDirectChatAsync - Response Body: {ex.ResponseBody}");
                    throw;
                }
            }
        }

        public async Task<Chat> CreateGroupChatAsync(int groupId, string text)
        {
            string url = $"{_baseUrl}/chats";

            var request = new CreateGroupChatRequest
            {
                GroupId = groupId,
                Text = text,
                Usernames = null // Not needed for group chat
            };

            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                var response = await SendRequestAsync<CreateChatResponse>(webRequest);
                return response.Chat;
            }
        }

        public async Task<GetMessagesResponse> GetMessagesAsync(int chatId, int page = 1)
        {
            string url = $"{_baseUrl}/messages/page/{page}?chat_id={chatId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GetMessagesResponse>(webRequest);
            }
        }

        public async Task<ChatMessage> SendMessageAsync(int chatId, string text)
        {
            string url = $"{_baseUrl}/messages";

            var request = new SendMessageRequest
            {
                ChatId = chatId,
                Text = text
            };

            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                var response = await SendRequestAsync<SendMessageResponse>(webRequest);
                return response.Message;
            }
        }

        public async Task<MessageReadResponse> MarkMessageAsReadAsync(int messageId)
        {
            string url = $"{_baseUrl}/messages/{messageId}/mark_as_read";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST))
            {
                return await SendRequestAsync<MessageReadResponse>(webRequest);
            }
        }
    }
}
