using System;
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
            string url = $"{_baseUrl}/chats";

            // Create a proper request using our model class
            var request = new CreateDirectChatRequest
            {
                Usernames = usernames,
                Text = text
            };

            // Serialize using the proper settings
            string jsonBody = SerializeRequest(request);

            // Use the AbstractService pattern
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    // Log the request for debugging
                    Debug.Log($"CreateDirectChatAsync - URL: {url}");
                    Debug.Log($"CreateDirectChatAsync - Body: {jsonBody}");
                    Debug.Log($"CreateDirectChatAsync - Auth: {_token?.Substring(0, Mathf.Min(5, _token?.Length ?? 0))}...");
                    
                    // Send the request using the AbstractService
                    var response = await SendRequestAsync<CreateChatResponse>(webRequest);
                    
                    if (response != null)
                    {
                        // If we have participants in the chat_users field, set them in the Chat
                        if (response.Chat != null && response.ChatUsers != null)
                        {
                            response.Chat.Participants = response.ChatUsers.ToArray();
                        }
                        
                        return response.Chat;
                    }
                    else
                    {
                        Debug.LogError("Response is null after deserialization");
                        throw new ApiException(0, "Failed to deserialize chat response", "");
                    }
                }
                catch (ApiException)
                {
                    // Re-throw API exceptions
                    throw;
                }
                catch (Exception ex)
                {
                    // Wrap other exceptions
                    Debug.LogError($"Exception in CreateDirectChatAsync: {ex.Message}");
                    throw new ApiException(0, $"Exception creating chat: {ex.Message}", "");
                }
            }
        }

        public async Task<Chat> CreateGroupChatAsync(int groupId, string text)
        {
            string url = $"{_baseUrl}/chats";

            // Create a proper request using our model class
            var request = new CreateGroupChatRequest
            {
                GroupId = groupId,
                Text = text,
                Usernames = null // Usernames aren't needed for group chat
            };

            // Serialize using the proper settings
            string jsonBody = SerializeRequest(request);

            // Use the AbstractService pattern
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    // Log the request for debugging
                    Debug.Log($"CreateGroupChatAsync - URL: {url}");
                    Debug.Log($"CreateGroupChatAsync - Body: {jsonBody}");
                    
                    // Send the request using the AbstractService
                    var response = await SendRequestAsync<CreateChatResponse>(webRequest);
                    
                    if (response != null)
                    {
                        // If we have participants in the chat_users field, set them in the Chat
                        if (response.Chat != null && response.ChatUsers != null)
                        {
                            response.Chat.Participants = response.ChatUsers.ToArray();
                        }
                        
                        return response.Chat;
                    }
                    else
                    {
                        Debug.LogError("Response is null after deserialization");
                        throw new ApiException(0, "Failed to deserialize group chat response", "");
                    }
                }
                catch (ApiException)
                {
                    // Re-throw API exceptions
                    throw;
                }
                catch (Exception ex)
                {
                    // Wrap other exceptions
                    Debug.LogError($"Exception in CreateGroupChatAsync: {ex.Message}");
                    throw new ApiException(0, $"Exception creating group chat: {ex.Message}", "");
                }
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
