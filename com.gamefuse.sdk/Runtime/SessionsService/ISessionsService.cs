using System.Threading.Tasks;
namespace GameFuseCSharp
{
    public interface ISessionsService
    {
        Task<SignInResponse> SignInAsync(SignInRequest request);
    }
}
