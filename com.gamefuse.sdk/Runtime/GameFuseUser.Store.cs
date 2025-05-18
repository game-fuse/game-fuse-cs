using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Gets all items in the store.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of store items.</returns>
        public Task<IReadOnlyList<StoreItem>> GetStoreItemsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _storeService.GetStoreItemsAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a specific store item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The store item.</returns>
        public Task<StoreItem> GetStoreItemAsync(int itemId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _storeService.GetStoreItemAsync(itemId, cancellationToken);
        }

        /// <summary>
        /// Purchases a store item for the current user.
        /// </summary>
        /// <param name="itemId">The ID of the item to purchase.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated credit balance after the purchase.</returns>
        public async Task<CreditBalance> PurchaseItemAsync(int itemId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var result = await _storeService.PurchaseItemAsync(Id, itemId, cancellationToken);
            Credits = result.Credits; // Update the user's credit balance
            return result;
        }

        /// <summary>
        /// Gets all items purchased by the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of purchased store items.</returns>
        public Task<IReadOnlyList<StoreItem>> GetUserPurchasesAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _storeService.GetUserPurchasesAsync(Id, cancellationToken);
        }

        /// <summary>
        /// Gets the current user's credit balance.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The user's credit balance.</returns>
        public async Task<CreditBalance> GetCreditBalanceAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            var result = await _storeService.GetCreditBalanceAsync(Id, cancellationToken);
            Credits = result.Credits; // Update the user's credit balance
            return result;
        }

        /// <summary>
        /// Gets the current user's credit transaction history.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of credit transactions.</returns>
        public Task<IReadOnlyList<CreditTransaction>> GetCreditTransactionsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _storeService.GetCreditTransactionsAsync(Id, cancellationToken);
        }
    }
}