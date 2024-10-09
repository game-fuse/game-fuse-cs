using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface ISystemAdminTestSuiteService
    {
        void Initialize(string token, string name);

        void Initialize(string baseUrl, string token, string name);
        Task<CreateGameResponse> CreateGameAsync();
        Task<CreateUserResponse> CreateUserAsync(int gameId, string username, string email);
        Task<CreateStoreItemResponse> CreateStoreItemAsync(int gameId, string name, string description, string category, int cost);
        Task<CleanUpResponse> CleanUpTestAsync(int gameId);
    }
}
