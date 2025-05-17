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

            var request = new
            {
                usernames = usernames,
                text = text
            };

            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<Chat>(webRequest);
            }
        }

        public async Task<Chat> CreateGroupChatAsync(int groupId, string text)
        {
            string url = $"{_baseUrl}/chats";

            var request = new
            {
                group_id = groupId,
                text = text
            };

            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<Chat>(webRequest);
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

            var request = new
            {
                chat_id = chatId,
                text = text
            };

            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<ChatMessage>(webRequest);
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
