using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface IGroupsService
    {
        Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request);
        Task<GroupsResponse> GetAllGroupsAsync();
        Task<GroupResponse> GetGroupDetailsAsync(int groupId);
        Task<GroupConnectionResponse> SendGroupConnectionRequestAsync(GroupConnectionRequest request);
        Task<GroupConnectionStatusResponse> ManageGroupConnectionRequestAsync(int connectionId, string status);
        Task<GroupAttributesResponse> GetGroupAttributesAsync(int groupId);
        Task<GroupAttributesResponse> AddGroupAttributesAsync(int groupId, List<GroupAttributeRequest> attributes);
    }
}
