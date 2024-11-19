using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface ISystemAdminTestSuiteService
    {
        Task<CreateGameResponse> CreateGameAsync();
        Task<CreateUserResponse> CreateUserAsync(int gameId, string username, string email);
        Task<CleanUpResponse> CleanUpTestAsync(int gameId);
    }
}
