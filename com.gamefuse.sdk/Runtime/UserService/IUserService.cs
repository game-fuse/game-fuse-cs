using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFuseCSharp
{
    public interface IUserService
    {
        /// <summary>
        /// Signs up a new user
        /// </summary>
        Task<SignInResponse> SignUpAsync(SignUpRequest request);
        
        /// <summary>
        /// Adds credits to a user
        /// </summary>
        Task<UserCreditsResponse> AddCreditsAsync(int userId, AddCreditsRequest request);
        
        /// <summary>
        /// Sets a user's credits to a specific amount
        /// </summary>
        Task<UserCreditsResponse> SetCreditsAsync(int userId, SetCreditsRequest request);
        
        /// <summary>
        /// Adds score to a user
        /// </summary>
        Task<UserScoreResponse> AddScoreAsync(int userId, AddScoreRequest request);
        
        /// <summary>
        /// Sets a user's score to a specific amount
        /// </summary>
        Task<UserScoreResponse> SetScoreAsync(int userId, SetScoreRequest request);
        
        /// <summary>
        /// Gets a user's attributes
        /// </summary>
        Task<UserAttributesResponse> GetAttributesAsync(int userId);
        
        /// <summary>
        /// Sets a single attribute for a user
        /// </summary>
        Task<UserAttributesResponse> SetAttributeAsync(int userId, SetAttributeRequest request);
        
        /// <summary>
        /// Sets multiple attributes for a user in a single request
        /// </summary>
        Task<UserAttributesResponse> SetAttributesAsync(int userId, SetAttributesRequest request);
        
        /// <summary>
        /// Removes an attribute from a user
        /// </summary>
        Task<UserAttributesResponse> RemoveAttributeAsync(int userId, string attributeKey);
        
        /// <summary>
        /// Gets a user's purchased store items
        /// </summary>
        Task<UserStoreItemsResponse> GetStoreItemsAsync(int userId);
        
        /// <summary>
        /// Purchases a store item for a user
        /// </summary>
        Task<UserStoreItemsResponse> PurchaseStoreItemAsync(int userId, PurchaseStoreItemRequest request);
        
        /// <summary>
        /// Removes a store item from a user
        /// </summary>
        Task<UserStoreItemsResponse> RemoveStoreItemAsync(int userId, RemoveStoreItemRequest request);
    }
}
