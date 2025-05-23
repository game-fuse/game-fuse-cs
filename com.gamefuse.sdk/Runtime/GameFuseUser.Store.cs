using GameFuse.Config;
using GameFuse.Models.Shared;
using GameFuse.Services;
using GameFuse.Transport;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        private string _gameId;
        private string _gameApiKey;

        private void LoadSettings()
        {
            var settings = GameFuseSettings.Settings;
            if (settings != null)
            {
                _gameId = settings.GameId;
                _gameApiKey = settings.GameApiKey;
            }
        }

        /// <summary>
        /// Gets all available store items for a specific game.
        /// This is a game-level call and does not require a signed-in user.
        /// </summary>
        /// <param name="gameId">The ID of the game. Found on your GameFuse.co dashboard.</param>
        /// <param name="gameToken">The API token of the game. Found on your GameFuse.co dashboard.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of available store items.</returns>
        public async Task<IReadOnlyList<StoreItem>> GetAvailableStoreItemsAsync(string gameId, string gameToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(gameId)) throw new System.ArgumentNullException(nameof(gameId));
            if (string.IsNullOrEmpty(gameToken)) throw new System.ArgumentNullException(nameof(gameToken));
            GameStoreItemsResponse response = await _storeService.GetAvailableStoreItemsAsync(gameId, gameToken, cancellationToken);
            return response?.StoreItems?.AsReadOnly() ?? (IReadOnlyList<StoreItem>)new List<StoreItem>().AsReadOnly();
        }

        public async Task<IReadOnlyList<StoreItem>> GetAvailableStoreItemsAsync(CancellationToken cancellationToken = default)
        {
            LoadSettings();
            if (string.IsNullOrEmpty(_gameId)) throw new System.ArgumentNullException(nameof(_gameId));
            if (string.IsNullOrEmpty(_gameApiKey)) throw new System.ArgumentNullException(nameof(_gameApiKey));
            GameStoreItemsResponse response = await _storeService.GetAvailableStoreItemsAsync(_gameId, _gameApiKey, cancellationToken);
            return response?.StoreItems?.AsReadOnly() ?? (IReadOnlyList<StoreItem>)new List<StoreItem>().AsReadOnly();
        }

        /// <summary>
        /// Purchases a store item for this authenticated user.
        /// Updates the user's internal credit balance and list of purchased items.
        /// </summary>
        /// <param name="storeItemId">The ID of the item to purchase.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UserStore object containing the updated credits and full list of purchased items.
        /// Returns null if the operation failed before an exception was thrown (e.g. unexpected API response).</returns>
        public async Task<UserStore> PurchaseStoreItemAsync(int storeItemId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated(); // User must be signed in

            UserStore purchaseResult = await _storeService.PurchaseStoreItemAsync(this.Id, storeItemId, cancellationToken);

            if (purchaseResult != null && purchaseResult.Credits != -1) // Check for our error indicator from service
            {
                // Update the GameFuseUser instance's internal state
                // Assumes _userData is the backing field for User properties.
                _userData.Credits = purchaseResult.Credits;
                _userData.GameUserStoreItems = purchaseResult.StoreItems; // User.GameUserStoreItems must be internally settable
            }
            // else: purchaseResult might be null or indicate an error from service layer

            return purchaseResult;
        }

        /// <summary>
        /// Removes a previously purchased store item for this authenticated user.
        /// Updates the user's internal credit balance and list of purchased items.
        /// </summary>
        /// <param name="storeItemId">The ID of the item to remove from purchases.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UserStore object containing the updated credits and remaining list of purchased items.
        /// Returns null or an error-indicating UserStore if the operation failed.</returns>
        public async Task<UserStore> RemoveStoreItemAsync(int storeItemId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated(); // User must be signed in

            UserStore removeResult = await _storeService.RemoveUserStoreItemAsync(this.Id, storeItemId, cancellationToken);

            if (removeResult != null && removeResult.Credits != -1) // Check for our error indicator
            {
                // Update the GameFuseUser instance's internal state
                _userData.Credits = removeResult.Credits;
                _userData.GameUserStoreItems = removeResult.StoreItems;
            }

            return removeResult;
        }

        /*
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
           // Credits = result.Credits; // Update the user's credit balance  TODO come back to this see if we need to update credits
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
            //Credits = result.Credits; // Update the user's credit balance TODO come back to this see if we need to update credits
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
        */
    }
}