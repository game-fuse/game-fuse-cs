using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuseCSharp
{
    /// <summary>
    /// Extension class that adds modern async methods to GameFuseUser
    /// </summary>
    public static class GameFuseUserExtensions
    {
        private static IUserService GetUserService(this GameFuseUser user)
        {
            string baseUrl = GameFuse.GetBaseURL();
            string token = user.GetAuthenticationToken();
            return new UserService(baseUrl, token);
        }
        
        /// <summary>
        /// Signs out the current user by resetting all user properties
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        public static void SignOut(this GameFuseUser user)
        {
            GameFuse.Log("GameFuseUser SignOut");
            
            // Reset all user properties
            user.SetSignedInInternal(false);
            user.SetNumberOfLoginsInternal(0);
            user.SetAuthenticationTokenInternal(null);
            user.SetUsernameInternal(null);
            user.SetScoreInternal(0);
            user.SetCreditsInternal(0);
            user.SetIDInternal(0);
            
            // Clear collections
            user.ClearAttributes();
            user.ClearStoreItems();
            
            GameFuse.Log("GameFuseUser SignOut Success");
        }
        
        /// <summary>
        /// Adds credits to the current user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="amount">Amount of credits to add</param>
        /// <returns>The updated total credits</returns>
        public static async Task<int> AddCreditsAsync(this GameFuseUser user, int amount)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to add credits");
                
            GameFuse.Log($"GameFuseUser AddCreditsAsync: {amount}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new AddCreditsRequest { Credits = amount };
                var response = await userService.AddCreditsAsync(user.GetID(), request);
                
                user.SetCreditsInternal(response.Credits);
                GameFuse.Log($"GameFuseUser AddCreditsAsync Success: {response.Credits}");
                
                return response.Credits;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser AddCreditsAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Sets the credits for the current user to a specific amount asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="amount">The amount to set the credits to</param>
        /// <returns>The updated total credits</returns>
        public static async Task<int> SetCreditsAsync(this GameFuseUser user, int amount)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to set credits");
                
            GameFuse.Log($"GameFuseUser SetCreditsAsync: {amount}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new SetCreditsRequest { Credits = amount };
                var response = await userService.SetCreditsAsync(user.GetID(), request);
                
                user.SetCreditsInternal(response.Credits);
                GameFuse.Log($"GameFuseUser SetCreditsAsync Success: {response.Credits}");
                
                return response.Credits;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser SetCreditsAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Adds score to the current user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="amount">Amount of score to add</param>
        /// <returns>The updated total score</returns>
        public static async Task<int> AddScoreAsync(this GameFuseUser user, int amount)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to add score");
                
            GameFuse.Log($"GameFuseUser AddScoreAsync: {amount}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new AddScoreRequest { Score = amount };
                var response = await userService.AddScoreAsync(user.GetID(), request);
                
                user.SetScoreInternal(response.Score);
                GameFuse.Log($"GameFuseUser AddScoreAsync Success: {response.Score}");
                
                return response.Score;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser AddScoreAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Sets the score for the current user to a specific amount asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="amount">The amount to set the score to</param>
        /// <returns>The updated total score</returns>
        public static async Task<int> SetScoreAsync(this GameFuseUser user, int amount)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to set score");
                
            GameFuse.Log($"GameFuseUser SetScoreAsync: {amount}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new SetScoreRequest { Score = amount };
                var response = await userService.SetScoreAsync(user.GetID(), request);
                
                user.SetScoreInternal(response.Score);
                GameFuse.Log($"GameFuseUser SetScoreAsync Success: {response.Score}");
                
                return response.Score;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser SetScoreAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Gets user's attributes asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <returns>A dictionary of user attributes</returns>
        public static async Task<Dictionary<string, string>> GetAttributesAsync(this GameFuseUser user)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to get attributes");
                
            GameFuse.Log("GameFuseUser GetAttributesAsync");
            
            try
            {
                var userService = user.GetUserService();
                var response = await userService.GetAttributesAsync(user.GetID());
                
                var attributeDict = new Dictionary<string, string>();
                foreach (var attr in response.Attributes)
                {
                    attributeDict[attr.Key] = attr.Value;
                }
                
                GameFuse.Log($"GameFuseUser GetAttributesAsync Success: {response.Attributes.Count} attributes");
                
                return attributeDict;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser GetAttributesAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Sets a single attribute for the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="key">The attribute key</param>
        /// <param name="value">The attribute value</param>
        /// <returns>A dictionary of user attributes after the update</returns>
        public static async Task<Dictionary<string, string>> SetAttributeAsync(this GameFuseUser user, string key, string value)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to set attributes");
                
            GameFuse.Log($"GameFuseUser SetAttributeAsync: {key}={value}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new SetAttributeRequest { Key = key, Value = value };
                var response = await userService.SetAttributeAsync(user.GetID(), request);
                
                var attributeDict = new Dictionary<string, string>();
                foreach (var attr in response.Attributes)
                {
                    attributeDict[attr.Key] = attr.Value;
                }
                
                GameFuse.Log($"GameFuseUser SetAttributeAsync Success: {key}={value}");
                
                return attributeDict;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser SetAttributeAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Sets multiple attributes for the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="attributes">Dictionary of attributes to set</param>
        /// <returns>A dictionary of user attributes after the update</returns>
        public static async Task<Dictionary<string, string>> SetAttributesAsync(this GameFuseUser user, Dictionary<string, string> attributes)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to set attributes");
                
            GameFuse.Log($"GameFuseUser SetAttributesAsync: {attributes.Count} attributes");
            
            try
            {
                var userService = user.GetUserService();
                var request = new SetAttributesRequest();
                
                foreach (var kvp in attributes)
                {
                    request.Attributes.Add(new AttributeItem { Key = kvp.Key, Value = kvp.Value });
                }
                
                var response = await userService.SetAttributesAsync(user.GetID(), request);
                
                var attributeDict = new Dictionary<string, string>();
                foreach (var attr in response.Attributes)
                {
                    attributeDict[attr.Key] = attr.Value;
                }
                
                GameFuse.Log($"GameFuseUser SetAttributesAsync Success: {attributes.Count} attributes");
                
                return attributeDict;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser SetAttributesAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Removes an attribute from the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="key">The attribute key to remove</param>
        /// <returns>A dictionary of user attributes after the removal</returns>
        public static async Task<Dictionary<string, string>> RemoveAttributeAsync(this GameFuseUser user, string key)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to remove attributes");
                
            GameFuse.Log($"GameFuseUser RemoveAttributeAsync: {key}");
            
            try
            {
                var userService = user.GetUserService();
                var response = await userService.RemoveAttributeAsync(user.GetID(), key);
                
                var attributeDict = new Dictionary<string, string>();
                foreach (var attr in response.Attributes)
                {
                    attributeDict[attr.Key] = attr.Value;
                }
                
                GameFuse.Log($"GameFuseUser RemoveAttributeAsync Success: {key}");
                
                return attributeDict;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser RemoveAttributeAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Gets user's purchased store items asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <returns>A list of purchased store items</returns>
        public static async Task<List<GameFuseStoreItem>> GetStoreItemsAsync(this GameFuseUser user)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to get store items");
                
            GameFuse.Log("GameFuseUser GetStoreItemsAsync");
            
            try
            {
                var userService = user.GetUserService();
                var response = await userService.GetStoreItemsAsync(user.GetID());
                
                var storeItems = new List<GameFuseStoreItem>();
                foreach (var item in response.StoreItems)
                {
                    storeItems.Add(new GameFuseStoreItem(
                        item.Name,
                        item.Category,
                        item.Description,
                        item.Cost,
                        item.Id,
                        item.IconUrl
                    ));
                }
                
                user.SetCreditsInternal(response.Credits);
                GameFuse.Log($"GameFuseUser GetStoreItemsAsync Success: {storeItems.Count} items");
                
                return storeItems;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser GetStoreItemsAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Purchases a store item for the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="storeItemId">The ID of the store item to purchase</param>
        /// <returns>A list of purchased store items after the purchase</returns>
        public static async Task<List<GameFuseStoreItem>> PurchaseStoreItemAsync(this GameFuseUser user, int storeItemId)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to purchase store items");
                
            GameFuse.Log($"GameFuseUser PurchaseStoreItemAsync: {storeItemId}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new PurchaseStoreItemRequest { StoreItemId = storeItemId };
                var response = await userService.PurchaseStoreItemAsync(user.GetID(), request);
                
                var storeItems = new List<GameFuseStoreItem>();
                foreach (var item in response.StoreItems)
                {
                    storeItems.Add(new GameFuseStoreItem(
                        item.Name,
                        item.Category,
                        item.Description,
                        item.Cost,
                        item.Id,
                        item.IconUrl
                    ));
                }
                
                user.SetCreditsInternal(response.Credits);
                GameFuse.Log($"GameFuseUser PurchaseStoreItemAsync Success: {storeItemId}");
                
                return storeItems;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser PurchaseStoreItemAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Purchases a store item for the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="storeItem">The store item to purchase</param>
        /// <returns>A list of purchased store items after the purchase</returns>
        public static Task<List<GameFuseStoreItem>> PurchaseStoreItemAsync(this GameFuseUser user, GameFuseStoreItem storeItem)
        {
            return PurchaseStoreItemAsync(user, storeItem.GetId());
        }
        
        /// <summary>
        /// Removes a purchased store item from the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="storeItemId">The ID of the store item to remove</param>
        /// <param name="reimburse">Whether to reimburse the user with credits</param>
        /// <returns>A list of purchased store items after the removal</returns>
        public static async Task<List<GameFuseStoreItem>> RemoveStoreItemAsync(this GameFuseUser user, int storeItemId, bool reimburse)
        {
            if (GameFuse.GetGameId() == null)
                throw new GameFuseException("Please set up your game with GameFuse.SetUpGame before modifying users");

            if (user.GetID() <= 0)
                throw new GameFuseException("User must be signed in to remove store items");
                
            GameFuse.Log($"GameFuseUser RemoveStoreItemAsync: {storeItemId}");
            
            try
            {
                var userService = user.GetUserService();
                var request = new RemoveStoreItemRequest { StoreItemId = storeItemId, Reimburse = reimburse };
                var response = await userService.RemoveStoreItemAsync(user.GetID(), request);
                
                var storeItems = new List<GameFuseStoreItem>();
                foreach (var item in response.StoreItems)
                {
                    storeItems.Add(new GameFuseStoreItem(
                        item.Name,
                        item.Category,
                        item.Description,
                        item.Cost,
                        item.Id,
                        item.IconUrl
                    ));
                }
                
                user.SetCreditsInternal(response.Credits);
                GameFuse.Log($"GameFuseUser RemoveStoreItemAsync Success: {storeItemId}");
                
                return storeItems;
            }
            catch (ApiException ex)
            {
                GameFuse.Log($"GameFuseUser RemoveStoreItemAsync Failure: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Removes a purchased store item from the user asynchronously.
        /// </summary>
        /// <param name="user">The GameFuseUser instance</param>
        /// <param name="storeItem">The store item to remove</param>
        /// <param name="reimburse">Whether to reimburse the user with credits</param>
        /// <returns>A list of purchased store items after the removal</returns>
        public static Task<List<GameFuseStoreItem>> RemoveStoreItemAsync(this GameFuseUser user, GameFuseStoreItem storeItem, bool reimburse)
        {
            return RemoveStoreItemAsync(user, storeItem.GetId(), reimburse);
        }
    }
    
    
}