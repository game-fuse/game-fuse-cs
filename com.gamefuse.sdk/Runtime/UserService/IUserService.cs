using System.Threading.Tasks;
namespace GameFuseCSharp
{
    public interface IUserService
    {
        Task<SignInResponse> SignUpAsync(SignUpRequest request);
    }
}
