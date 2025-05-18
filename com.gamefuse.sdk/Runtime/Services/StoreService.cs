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
    }
}