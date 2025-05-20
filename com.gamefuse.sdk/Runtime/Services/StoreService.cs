using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for store-related operations.
    /// </summary>
    public class StoreService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the StoreService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public StoreService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Gets all available store items for a specific game.
        /// This call does not require user authentication but uses gameId and gameToken.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="gameToken">The API token of the game.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a list of store items.</returns>
        public async Task<GameStoreItemsResponse> GetAvailableStoreItemsAsync(string gameId, string gameToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
            if (string.IsNullOrEmpty(gameToken)) throw new ArgumentNullException(nameof(gameToken));

            // API Path: GET /api/v3/games/store_items?game_id={gameId}&game_token={gameToken}
            string path = $"games/store_items?game_id={Uri.EscapeDataString(gameId)}&game_token={Uri.EscapeDataString(gameToken)}";

            // IMPORTANT: The ITransport instance used for this call in the facade (GameFuseStore)
            // should NOT automatically inject a user's authentication-token header.
            // The API doc for this endpoint does not list 'authentication-token' as a required header.
            var transport = new UnityWebRequestTransport();
            var response = await transport.GetAsync<GameStoreItemsResponse>(path, null, cancellationToken);

            if (response == null) return new GameStoreItemsResponse(); // Ensure non-null return
            response.StoreItems ??= new List<StoreItem>(); // Ensure internal list is not null

            return response;
        }

        /// <summary>
        /// Purchases a store item for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user making the purchase.</param>
        /// <param name="storeItemId">The ID of the store item to purchase.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UserStore object containing the updated credits and list of all purchased items by the user.</returns>
        public async Task<UserStore> PurchaseStoreItemAsync(int userId, int storeItemId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (storeItemId <= 0) throw new ArgumentOutOfRangeException(nameof(storeItemId), "Store Item ID must be positive.");

            var payload = new PurchaseItemPayload
            {
                StoreItemId = storeItemId
            };

            // API Path: POST /api/v3/users/{signedInUserId}/purchase_game_user_store_item
            string path = $"users/{userId}/purchase_game_user_store_item";

            var response = await _transport.PostAsync<PurchaseItemPayload, UserStore>(path, payload, null, cancellationToken);

            // Ensure StoreItems list is initialized even if API returns null for it (e.g., if it's the first purchase and list is omitted)
            if (response != null)
            {
                response.StoreItems ??= new List<StoreItem>();
            }
            else // Handle case where API might return completely null on error before exception
            {
                // Consider returning a specific error or an empty UserStore with an error indicator
                // For now, let's assume transport layer throws for HTTP errors, and this handles unexpected valid nulls.
                return new UserStore { Credits = -1 }; // Or throw custom exception
            }

            return response;
        }

        /// <summary>
        /// Removes/revokes a purchased store item from the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user from whom the item is removed.</param>
        /// <param name="storeItemId">The ID of the store item to remove.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UserStore object containing the updated credits and list of remaining purchased items.</returns>
        public async Task<UserStore> RemoveUserStoreItemAsync(int userId, int storeItemId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (storeItemId <= 0) throw new ArgumentOutOfRangeException(nameof(storeItemId), "Store Item ID must be positive.");

            // API Path: GET /api/v3/users/{signedInUserId}/remove_game_user_store_item?store_item_id={storeItemId}
            // Note: API doc uses GET for this "remove" operation.
            string path = $"users/{userId}/remove_game_user_store_item?store_item_id={storeItemId}";

            // No request body for this GET request.
            var response = await _transport.GetAsync<UserStore>(path, null, cancellationToken);

            if (response != null)
            {
                response.StoreItems ??= new List<StoreItem>();
            }
            else
            {
                return new UserStore { Credits = -1 }; // Error indicator
            }

            return response;
        }

        /*
        /// <summary>
        /// Gets all items in the store.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of store items.</returns>
        public async Task<IReadOnlyList<StoreItem>> GetStoreItemsAsync(CancellationToken cancellationToken = default)
        {
            var response = await _transport.GetAsync<List<StoreItem>>("store_items", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Gets a specific store item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The store item.</returns>
        public Task<StoreItem> GetStoreItemAsync(int itemId, CancellationToken cancellationToken = default)
        {
            if (itemId <= 0) throw new ArgumentOutOfRangeException(nameof(itemId), "Item ID must be positive.");
            
            return _transport.GetAsync<StoreItem>($"store_items/{itemId}", null, cancellationToken);
        }

        /// <summary>
        /// Purchases a store item for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="itemId">The ID of the item to purchase.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated credit balance after the purchase.</returns>
        public Task<CreditBalance> PurchaseItemAsync(int userId, int itemId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (itemId <= 0) throw new ArgumentOutOfRangeException(nameof(itemId), "Item ID must be positive.");
            
            var request = new Dictionary<string, int>
            {
                ["user_id"] = userId,
                ["store_item_id"] = itemId
            };
            
            return _transport.PostAsync<Dictionary<string, int>, CreditBalance>("purchases", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets all items purchased by a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of purchased store items.</returns>
        public async Task<IReadOnlyList<StoreItem>> GetUserPurchasesAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<StoreItem>>($"users/{userId}/purchases", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Gets a user's credit balance.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The user's credit balance.</returns>
        public Task<CreditBalance> GetCreditBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            return _transport.GetAsync<CreditBalance>($"users/{userId}/credits", null, cancellationToken);
        }

        /// <summary>
        /// Adds credits to a user's account.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="amount">The amount of credits to add.</param>
        /// <param name="description">A description of the transaction.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated credit balance.</returns>
        public Task<CreditBalance> AddCreditsAsync(int userId, int amount, string description, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException(nameof(description));
            
            var request = new Dictionary<string, object>
            {
                ["amount"] = amount,
                ["description"] = description
            };
            
            return _transport.PostAsync<Dictionary<string, object>, CreditBalance>($"users/{userId}/credits", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets a user's credit transaction history.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of credit transactions.</returns>
        public async Task<IReadOnlyList<CreditTransaction>> GetCreditTransactionsAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<CreditTransaction>>($"users/{userId}/credit_transactions", null, cancellationToken);
            return response.AsReadOnly();
        }
        */
    }
}