using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface IFriendshipService
    {
        Task<FriendRequestResponse> SendFriendRequestAsync(string username);
        Task<FriendshipStatusResponse> UpdateFriendRequestStatusAsync(int friendshipId, string status);
        Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId);
        Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId);
        Task<FriendshipDataResponse> GetFriendshipDataAsync();
    }
}
