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

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                var response = await SendRequestAsync<CreateChatResponse>(webRequest);
                return response.Chat;
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
