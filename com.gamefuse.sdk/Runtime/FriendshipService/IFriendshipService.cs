using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface IFriendshipService
    {
        Task<FriendRequestResponse> SendFriendRequestAsync(string username);
        Task<FriendshipStatusResponse> AcceptFriendRequestAsync(int friendshipId);
        Task<FriendshipStatusResponse> DeclineFriendRequestAsync(int friendshipId);
        Task<FriendshipStatusResponse> CancelFriendRequestAsync(int friendshipId);
        Task<FriendshipStatusResponse> UnfriendPlayerAsync(int userId);
        Task<FriendsResponse> GetFriendsAsync();
        Task<IncomingFriendRequestsResponse> GetIncomingFriendRequestsAsync();
        Task<OutgoingFriendRequestsResponse> GetOutgoingFriendRequestsAsync();

    }
}
