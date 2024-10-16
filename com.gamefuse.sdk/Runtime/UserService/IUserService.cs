using System.Threading.Tasks;
namespace GameFuseCSharp
{
    public interface IUserService: IServiceInitializable
    {
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);
    }
}
