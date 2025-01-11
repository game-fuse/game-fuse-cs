using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Creates a new group with the current user as admin.
        /// </summary>
        /// <param name="request">Group creation parameters</param>
        /// <returns>Response containing the created group details</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.CreateGroupAsync(request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a list of all available groups.
        /// </summary>
        /// <param name="withFullData">If true, includes complete group details including members and admins</param>
        /// <returns>Response containing array of groups</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupResponse[]> GetAllGroupsAsync(bool withFullData = false)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                GroupsResponse response = await groupsService.GetAllGroupsAsync(withFullData);
                return response.Groups;
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets detailed information about a specific group.
        /// </summary>
        /// <param name="groupId">ID of the group to fetch details for</param>
        /// <returns>Response containing complete group information</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupResponse> GetGroupDetailsAsync(int groupId)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.GetGroupDetailsAsync(groupId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends a request to join a group or invites a user to join.
        /// </summary>
        /// <param name="request">Group connection request parameters</param>
        /// <returns>Response containing the connection details</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupConnectionResponse> SendGroupConnectionRequestAsync(GroupConnectionRequest request)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.SendGroupConnectionRequestAsync(request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Accepts a group connection request.
        /// </summary>
        /// <param name="connectionId">ID of the connection request to manage</param>
        /// <returns>Response confirming the status update</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupConnectionStatusResponse> AcceptGroupConnectionRequestAsync(int connectionId)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.AcceptGroupConnectionRequestAsync(connectionId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Accepts a group connection request.
        /// </summary>
        /// <param name="connectionId">ID of the connection request to manage</param>
        /// <returns>Response confirming the status update</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupConnectionStatusResponse> DeclineGroupConnectionRequestAsync(int connectionId)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.DeclineGroupConnectionRequestAsync(connectionId);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all attributes for a specific group.
        /// </summary>
        /// <param name="groupId">ID of the group to fetch attributes for</param>
        /// <returns>Response containing group attributes</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupAttributesResponse> GetGroupAttributesAsync(int groupId)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.GetGroupAttributesAsync(groupId);
            }
            catch (ApiException)
            {
                throw;
            }
        }



        /// <summary>
        /// Adds new attributes to a group.
        /// </summary>
        /// <param name="groupId">ID of the group to add attributes to</param>
        /// <param name="request">Attribute to add</param>
        /// <returns>Response containing updated group attributes</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupAttributesResponse> AddGroupAttributeAsync(int groupId, GroupAttributeRequest request)
        {

            GroupAttributesRequest attributesRequest = new GroupAttributesRequest();
            attributesRequest.Attributes = new GroupAttributeRequest[] { request };
            try
            {
                return await AddGroupAttributesAsync(groupId, attributesRequest);
            }catch(ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds new attributes to a group.
        /// </summary>
        /// <param name="groupId">ID of the group to add attributes to</param>
        /// <param name="request">Attributes to add</param>
        /// <returns>Response containing updated group attributes</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupAttributesResponse> AddGroupAttributesAsync(int groupId, GroupAttributesRequest request)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.AddGroupAttributesAsync(groupId, request);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        /// <summary>
        /// Modifies an existing group attribute.
        /// </summary>
        /// <param name="groupId">ID of the group containing the attribute</param>
        /// <param name="key">Key of the attribute to modify</param>
        /// <param name="value">New value for the attribute</param>
        /// <returns>Response containing updated group attributes</returns>
        /// <exception cref="ApiException">Thrown when request fails</exception>
        public async Task<GroupAttribute> ModifyGroupAttributeAsync(int groupId, string key, string value)
        {
            try
            {
                IGroupsService groupsService = new GroupsService(GameFuse.GetBaseURL(), authenticationToken);
                return await groupsService.ModifyGroupAttributeAsync(groupId, key, value);
            }
            catch (ApiException)
            {
                throw;
            }
        }
    }
}