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
            try
            {
                string url = $"{_baseUrl}/chats";
                
                // Create the JSON directly instead of using the serializer
                string jsonBody = $"{{\"usernames\":[{string.Join(",", usernames.Select(u => $"\"{u}\""))}],\"text\":\"{text}\"}}";
                Debug.Log($"CreateDirectChatAsync - Request URL: {url}");
                Debug.Log($"CreateDirectChatAsync - Request Body: {jsonBody}");
                
                // Create a web request directly, bypassing the AbstractService methods that might be causing issues
                var webRequest = new UnityWebRequest(url, "POST");
                webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("authentication-token", _token);
                
                // Send the request directly
                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (webRequest.responseCode == 200 || webRequest.responseCode == 201)
                {
                    // Get the raw response text
                    var responseText = webRequest.downloadHandler.text;
                    Debug.Log($"Chat response: {responseText}");
                    
                    // Parse the response using a simple approach
                    var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
                    if (responseJson != null && responseJson.ContainsKey("chat"))
                    {
                        // Extract the chat JSON
                        var chatJson = JsonConvert.SerializeObject(responseJson["chat"]);
                        // Deserialize to Chat object
                        var chat = JsonConvert.DeserializeObject<Chat>(chatJson);
                        
                        // Get participants from chat_users if available
                        if (responseJson.ContainsKey("chat_users"))
                        {
                            var participantsJson = JsonConvert.SerializeObject(responseJson["chat_users"]);
                            var participants = JsonConvert.DeserializeObject<ChatParticipant[]>(participantsJson);
                            chat.Participants = participants;
                        }
                        
                        return chat;
                    }
                }
                
                // Handle error
                Debug.LogError($"Failed to create chat: {webRequest.responseCode}, {webRequest.error}");
                if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    Debug.LogError($"Response: {webRequest.downloadHandler.text}");
                }
                
                throw new ApiException(
                    webRequest.responseCode,
                    $"Failed to create chat: {webRequest.error}",
                    webRequest.downloadHandler.text
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in CreateDirectChatAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Chat> CreateGroupChatAsync(int groupId, string text)
        {
            try
            {
                string url = $"{_baseUrl}/chats";
                
                // Create the JSON directly
                string jsonBody = $"{{\"group_id\":{groupId},\"text\":\"{text}\"}}";
                Debug.Log($"CreateGroupChatAsync - Request URL: {url}");
                Debug.Log($"CreateGroupChatAsync - Request Body: {jsonBody}");
                
                // Create a web request directly
                var webRequest = new UnityWebRequest(url, "POST");
                webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("authentication-token", _token);
                
                // Send the request directly
                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (webRequest.responseCode == 200 || webRequest.responseCode == 201)
                {
                    // Get the raw response text
                    var responseText = webRequest.downloadHandler.text;
                    Debug.Log($"Group chat response: {responseText}");
                    
                    // Parse the response using a simple approach
                    var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
                    if (responseJson != null && responseJson.ContainsKey("chat"))
                    {
                        // Extract the chat JSON
                        var chatJson = JsonConvert.SerializeObject(responseJson["chat"]);
                        // Deserialize to Chat object
                        var chat = JsonConvert.DeserializeObject<Chat>(chatJson);
                        
                        // Get participants from chat_users if available
                        if (responseJson.ContainsKey("chat_users"))
                        {
                            var participantsJson = JsonConvert.SerializeObject(responseJson["chat_users"]);
                            var participants = JsonConvert.DeserializeObject<ChatParticipant[]>(participantsJson);
                            chat.Participants = participants;
                        }
                        
                        return chat;
                    }
                }
                
                // Handle error
                Debug.LogError($"Failed to create group chat: {webRequest.responseCode}, {webRequest.error}");
                if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    Debug.LogError($"Response: {webRequest.downloadHandler.text}");
                }
                
                throw new ApiException(
                    webRequest.responseCode,
                    $"Failed to create group chat: {webRequest.error}",
                    webRequest.downloadHandler.text
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in CreateGroupChatAsync: {ex.Message}");
                throw;
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
