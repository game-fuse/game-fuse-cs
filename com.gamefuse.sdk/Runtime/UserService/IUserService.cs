using System.Threading.Tasks;
namespace GameFuseCSharp
{
    public interface IUserService
    {
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);
    }
}
