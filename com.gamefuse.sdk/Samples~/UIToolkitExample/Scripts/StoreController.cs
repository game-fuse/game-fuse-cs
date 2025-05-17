using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuseCSharp;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Handles store functionality including viewing, purchasing, and removing store items
    /// </summary>
    public class StoreController : BaseGameFuseUIController
    {
        // Game store items UI
        private Button getStoreItemsButton;
        private ScrollView gameStoreItemsScrollView;
        
        // User store items UI
        private TextField storeItemIdField;
        private Toggle reimburseToggle;
        private Button purchaseStoreItemButton;
        private Button removeStoreItemButton;
        private Button getPurchasedItemsButton;
        private ScrollView purchasedItemsScrollView;
        
        protected override void InitializeUI()
        {
            // Game store items
            getStoreItemsButton = rootElement.Q<Button>("get-store-items-button");
            gameStoreItemsScrollView = rootElement.Q<ScrollView>("game-store-items-scroll");
            
            // User store items
            storeItemIdField = rootElement.Q<TextField>("store-item-id");
            reimburseToggle = rootElement.Q<Toggle>("reimburse-toggle");
            purchaseStoreItemButton = rootElement.Q<Button>("purchase-store-item-button");
            removeStoreItemButton = rootElement.Q<Button>("remove-store-item-button");
            getPurchasedItemsButton = rootElement.Q<Button>("get-purchased-items-button");
            purchasedItemsScrollView = rootElement.Q<ScrollView>("purchased-items-scroll");
        }
        
        protected override void RegisterCallbacks()
        {
            if (getStoreItemsButton != null)
            {
                getStoreItemsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetStoreItemsClicked());
            }
            
            if (purchaseStoreItemButton != null)
            {
                purchaseStoreItemButton.RegisterCallback<ClickEvent>(async (evt) => await OnPurchaseStoreItemClicked());
            }
            
            if (removeStoreItemButton != null)
            {
                removeStoreItemButton.RegisterCallback<ClickEvent>(async (evt) => await OnRemoveStoreItemClicked());
            }
            
            if (getPurchasedItemsButton != null)
            {
                getPurchasedItemsButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetPurchasedItemsClicked());
            }
        }
        
        protected override void UnregisterCallbacks()
        {
            if (getStoreItemsButton != null)
            {
                getStoreItemsButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetStoreItemsClicked());
            }
            
            if (purchaseStoreItemButton != null)
            {
                purchaseStoreItemButton.UnregisterCallback<ClickEvent>(async (evt) => await OnPurchaseStoreItemClicked());
            }
            
            if (removeStoreItemButton != null)
            {
                removeStoreItemButton.UnregisterCallback<ClickEvent>(async (evt) => await OnRemoveStoreItemClicked());
            }
            
            if (getPurchasedItemsButton != null)
            {
                getPurchasedItemsButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetPurchasedItemsClicked());
            }
        }
        
        /// <summary>
        /// Gets and displays all store items available in the game
        /// </summary>
        private async Task OnGetStoreItemsClicked()
        {
            await ExecuteAsync(async () =>
            {
                // Get store items
                var storeItems = GFuse.GetStoreItems();
                
                // Clear current list
                ClearScrollView(gameStoreItemsScrollView);
                
                // Display store items
                if (storeItems != null && storeItems.Count > 0)
                {
                    foreach (var item in storeItems)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "ID", item.GetId().ToString() },
                            { "Name", item.GetName() },
                            { "Description", item.GetDescription() },
                            { "Cost", item.GetCost().ToString() }
                        };
                        
                        var itemElement = CreateListItem(item.GetName(), properties);
                        
                        // Add click handler to copy item ID for purchasing
                        itemElement.RegisterCallback<ClickEvent>((evt) => 
                        {
                            storeItemIdField.value = item.GetId().ToString();
                        });
                        
                        gameStoreItemsScrollView.Add(itemElement);
                    }
                    
                    LogMessage($"Retrieved {storeItems.Count} store items", LogType.Success);
                }
                else
                {
                    LogMessage("No store items found", LogType.Info);
                }
            });
        }
        
        /// <summary>
        /// Purchases a store item for the current user
        /// </summary>
        private async Task OnPurchaseStoreItemClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to purchase items", LogType.Error);
                return;
            }
            
            if (!int.TryParse(storeItemIdField.value, out int itemId))
            {
                LogMessage("Store Item ID must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Purchase the store item
                await GameFuseUser.CurrentUser.PurchaseStoreItemAsync(itemId);
                
                // Update UI
                await OnGetPurchasedItemsClicked();
                
                LogMessage($"Store item purchased successfully. Remaining credits: {GameFuseUser.CurrentUser.GetCredits()}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Removes a store item from the current user
        /// </summary>
        private async Task OnRemoveStoreItemClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to remove items", LogType.Error);
                return;
            }
            
            if (!int.TryParse(storeItemIdField.value, out int itemId))
            {
                LogMessage("Store Item ID must be a valid integer", LogType.Error);
                return;
            }
            
            bool reimburse = reimburseToggle != null && reimburseToggle.value;
            
            await ExecuteAsync(async () =>
            {
                // Remove the store item
                await GameFuseUser.CurrentUser.RemoveStoreItemAsync(itemId, reimburse);
                
                // Update UI
                await OnGetPurchasedItemsClicked();
                
                LogMessage($"Store item removed successfully. Credits: {GameFuseUser.CurrentUser.GetCredits()}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Gets and displays all store items purchased by the current user
        /// </summary>
        private async Task OnGetPurchasedItemsClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to view purchased items", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user's purchased items
                var storeItems = await GameFuseUser.CurrentUser.GetStoreItemsAsync();
                
                // Clear current items
                ClearScrollView(purchasedItemsScrollView);
                
                // Display purchased items
                if (storeItems != null && storeItems.Count > 0)
                {
                    foreach (var item in storeItems)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "ID", item.GetId().ToString() },
                            { "Name", item.GetName() },
                            { "Description", item.GetDescription() },
                            { "Cost", item.GetCost().ToString() }
                            // Note: Purchase date might need a different accessor if available
                        };
                        
                        var itemElement = CreateListItem(item.GetName(), properties);
                        
                        // Add click handler to copy item ID for removing
                        itemElement.RegisterCallback<ClickEvent>((evt) => 
                        {
                            storeItemIdField.value = item.GetId().ToString();
                        });
                        
                        purchasedItemsScrollView.Add(itemElement);
                    }
                    
                    LogMessage($"Retrieved {storeItems.Count} purchased items", LogType.Success);
                }
                else
                {
                    LogMessage("No purchased items found", LogType.Info);
                }
            });
        }
    }
}