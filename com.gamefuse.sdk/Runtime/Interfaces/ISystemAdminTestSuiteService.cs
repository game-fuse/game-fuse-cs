using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface ISystemAdminTestSuiteService: IServiceInitializable
    {
        public void SetServiceKeyName(string serviceKeyName);
        Task<CreateGameResponse> CreateGameAsync();
        Task<CreateUserResponse> CreateUserAsync(int gameId, string username, string email);
        Task<CreateStoreItemResponse> CreateStoreItemAsync(int gameId, string name, string description, string category, int cost);
        Task<CleanUpResponse> CleanUpTestAsync(int gameId);
    }
}
