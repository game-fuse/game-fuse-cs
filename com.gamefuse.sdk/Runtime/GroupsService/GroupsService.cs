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
            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GroupResponse>(webRequest);
            }
        }

        public async Task<GroupsResponse> GetAllGroupsAsync(bool withFullData = false)
        {
            string url = $"{_baseUrl}/groups";
            if (withFullData)
            {
                url += "?with_full_data=true";
            }

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
            string jsonBody = SerializeRequest(request);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                return await SendRequestAsync<GroupConnectionResponse>(webRequest);
            }
        }

        public async Task<GroupConnectionStatusResponse> AcceptGroupConnectionRequestAsync(int connectionId)
        {
            try
            {
                return await ManageGroupConnectionRequestAsync(connectionId, JoinRequestStatus.accepted.ToString());
            }
            catch (ApiException)
            {
                throw;
            }
        }

        public async Task<GroupConnectionStatusResponse> DeclineGroupConnectionRequestAsync(int connectionId)
        {
            try
            {
                return await ManageGroupConnectionRequestAsync(connectionId, JoinRequestStatus.declined.ToString());
            }
            catch (ApiException)
            {
                throw;
            }
        }

        private async Task<GroupConnectionStatusResponse> ManageGroupConnectionRequestAsync(int connectionId, string status)
        {
            string url = $"{_baseUrl}/group_connections/{connectionId}";
            var statusRequest = new GroupConnectionStatusRequest { Status = status };
            string jsonBody = SerializeRequest(statusRequest);

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

        public async Task<GroupAttributesResponse> AddGroupAttributesAsync(int groupId, GroupAttributesRequest attributesRequest)
        {
            string url = $"{_baseUrl}/groups/{groupId}/add_attribute";
            string jsonBody = SerializeRequest(attributesRequest);

            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.POST, jsonBody))
            {
                try
                {
                    return await SendRequestAsync<GroupAttributesResponse>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"Failed to add attributes to group {groupId}. Status: {ex.StatusCode}, Message: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<GroupAttribute> ModifyGroupAttributeAsync(int groupId, string key, string value)
        {
            string url = $"{_baseUrl}/groups/{groupId}/modify_attribute";

            var modifyRequest = new ModifyGroupAttributeRequest
            {
                Key = key,
                Value = value
            };

            string jsonBody = SerializeRequest(modifyRequest);
            using (UnityWebRequest webRequest = CreateRequest(url, HttpVerbs.PATCH, jsonBody))
            {
                try
                {
                   return await SendRequestAsync<GroupAttribute>(webRequest);
                }
                catch (ApiException ex)
                {
                    Debug.LogError($"Failed to modify attribute for group {groupId}. Status: {ex.StatusCode}, Message: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
