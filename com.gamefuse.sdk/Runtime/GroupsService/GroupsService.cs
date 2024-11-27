using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    public class GroupsService : AbstractService, IGroupsService
    {
        public GroupsService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        public async Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request)
        {
            string url = $"{_baseUrl}/groups";
            string jsonBody = JsonUtility.ToJson(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GroupResponse>(webRequest);
            }
        }

        public async Task<GroupsResponse> GetAllGroupsAsync()
        {
            string url = $"{_baseUrl}/groups";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GroupsResponse>(webRequest);
            }
        }

        public async Task<GroupResponse> GetGroupDetailsAsync(int groupId)
        {
            string url = $"{_baseUrl}/groups/{groupId}";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GroupResponse>(webRequest);
            }
        }

        public async Task<GroupConnectionResponse> SendGroupConnectionRequestAsync(GroupConnectionRequest request)
        {
            string url = $"{_baseUrl}/group_connections";
            string jsonBody = JsonUtility.ToJson(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GroupConnectionResponse>(webRequest);
            }
        }

        public async Task<GroupConnectionStatusResponse> ManageGroupConnectionRequestAsync(int connectionId, string status)
        {
            string url = $"{_baseUrl}/group_connections/{connectionId}";
            string jsonBody = JsonUtility.ToJson(new { status });

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.PUT, jsonBody))
            {
                return await SendRequestAsync<GroupConnectionStatusResponse>(webRequest);
            }
        }

        public async Task<GroupAttributesResponse> GetGroupAttributesAsync(int groupId)
        {
            string url = $"{_baseUrl}/groups/{groupId}/attributes";

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.GET))
            {
                return await SendRequestAsync<GroupAttributesResponse>(webRequest);
            }
        }

        public async Task<GroupAttributesResponse> AddGroupAttributesAsync(int groupId, List<GroupAttributeRequest> attributes)
        {
            string url = $"{_baseUrl}/groups/{groupId}/add_attribute";
            string jsonBody = JsonUtility.ToJson(new { attributes });

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GroupAttributesResponse>(webRequest);
            }
        }
    }
}
