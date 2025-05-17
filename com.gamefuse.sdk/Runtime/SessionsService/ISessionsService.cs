using System.Threading.Tasks;
namespace GameFuseCSharp
{
    public interface ISessionsService
    {
        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
        Task<SignInResponse> SignInAsync(SignInRequest request);
        
        /// <summary>
        /// Signs up a new user
        /// </summary>
        Task<SignInResponse> SignUpAsync(SignUpRequest request);
        
        /// <summary>
        /// Sends a password reset email to the specified email address
        /// </summary>
        Task<bool> SendPasswordResetEmailAsync(string email, string gameId, string gameToken);
    }
}
