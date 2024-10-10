using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface IFriendshipService: IServiceInitializable
    {
        Task<FriendRequestResponse> SendFriendRequestAsync(string username, string authToken);
        Task<FriendshipStatusResponse> UpdateFriendRequestStatusAsync(int friendshipId, string status, string authToken);
        Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId, string authToken);
        Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId, string authToken);
        Task<FriendshipDataResponse> GetFriendshipDataAsync(string authToken);
    }
}
