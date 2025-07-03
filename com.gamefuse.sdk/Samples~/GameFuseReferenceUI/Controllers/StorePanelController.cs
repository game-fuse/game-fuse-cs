using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFuse.UI.Controls;

namespace GameFuse.UI
{
    /// <summary>
    /// Controls the store panel - manages store items, purchasing, and inventory.
    /// Covers Phase 5 functionality: Store & Economy
    /// </summary>
    public class StorePanelController
    {
        // Events
        public static event Action<string> OnError;
        public static event Action<string> OnSuccess;
        public static event Action<GameFuseUser> OnUserDataUpdated;

        // UI Elements
        private VisualElement root;
        private StorePanel storePanel;
        private GameFuseUser currentUser;

        // Tab elements
        private Button availableTabButton;
        private Button inventoryTabButton;
        private VisualElement availableItemsPanel;
        private VisualElement inventoryPanel;

        // Available Items Elements
        private Button categoryFilterButton;
        private TextField storeSearchInput;
        private Label userCreditsDisplay;
        private Button availableRefreshButton;
        private VisualElement availableItemsGrid;
        private VisualElement availableEmptyState;

        // Inventory Elements
        private Label inventoryCount;
        private Button inventoryRefreshButton;
        private VisualElement inventoryItemsGrid;
        private VisualElement inventoryEmptyState;

        // Loading and Status
        private VisualElement loadingIndicator;
        private Label statusLabel;

        // Data Containers
        private List<StoreItem> availableItems = new List<StoreItem>();
        private List<StoreItem> filteredItems = new List<StoreItem>();
        private List<StoreItem> inventoryItems = new List<StoreItem>();
        private string currentCategory = "All Categories";
        private List<string> categories = new List<string> { "All Categories" };

        public StorePanelController(VisualElement panelContainer)
        {
            root = panelContainer;
            storePanel = root as StorePanel;
            
            // Handle both custom control and legacy implementation
            if (storePanel == null)
            {
                Debug.Log("StorePanelController: Using standard VisualElement container");
                // Attempt to find the StorePanel as a child element
                storePanel = root.Q<StorePanel>();
                
                if (storePanel == null)
                {
                    Debug.LogWarning("StorePanelController: Could not find StorePanel custom control. Item details functionality will be limited.");
                }
            }
            
            if (storePanel != null)
            {
                Debug.Log("StorePanelController: Using StorePanel custom control");
                storePanel.Initialize();
            }
            
            InitializeElements();
            SetupEventHandlers();
        }

        private void InitializeElements()
        {
            // Tab elements
            availableTabButton = root.Q<Button>("available-tab-button");
            inventoryTabButton = root.Q<Button>("inventory-tab-button");
            availableItemsPanel = root.Q<VisualElement>("available-items-panel");
            inventoryPanel = root.Q<VisualElement>("inventory-panel");

            // Available Items Elements
            categoryFilterButton = root.Q<Button>("category-filter");
            storeSearchInput = root.Q<TextField>("store-search-input");
            userCreditsDisplay = root.Q<Label>("user-credits-display");
            availableRefreshButton = root.Q<Button>("available-refresh-button");
            availableItemsGrid = root.Q<VisualElement>("available-items-grid");
            availableEmptyState = root.Q<VisualElement>("available-empty-state");

            // Inventory Elements
            inventoryCount = root.Q<Label>("inventory-count");
            inventoryRefreshButton = root.Q<Button>("inventory-refresh-button");
            inventoryItemsGrid = root.Q<VisualElement>("inventory-items-grid");
            inventoryEmptyState = root.Q<VisualElement>("inventory-empty-state");

            // Loading and Status
            loadingIndicator = root.Q<VisualElement>("store-loading");
            statusLabel = root.Q<Label>("store-status");
        }

        private void SetupEventHandlers()
        {
            // Tab navigation
            availableTabButton.clicked += () => SwitchTab("available");
            inventoryTabButton.clicked += () => SwitchTab("inventory");
            
            // Available Items functionality
            if (availableRefreshButton != null)
            {
                availableRefreshButton.clicked += RefreshAvailableItems;
            }

            if (storeSearchInput != null)
            {
                storeSearchInput.RegisterValueChangedCallback(evt => ApplyFilters());
            }

            if (categoryFilterButton != null)
            {
                categoryFilterButton.clicked += ShowCategoryMenu;
            }

            // Inventory functionality
            if (inventoryRefreshButton != null)
            {
                inventoryRefreshButton.clicked += RefreshInventory;
            }
        }

        private void SwitchTab(string tabName)
        {
            // Reset all tabs
            availableTabButton.RemoveFromClassList("active-tab");
            inventoryTabButton.RemoveFromClassList("active-tab");
            
            availableItemsPanel.style.display = DisplayStyle.None;
            inventoryPanel.style.display = DisplayStyle.None;
            
            // Set active tab
            switch (tabName)
            {
                case "available":
                    availableTabButton.AddToClassList("active-tab");
                    availableItemsPanel.style.display = DisplayStyle.Flex;
                    if (availableItems.Count == 0) // Only load if not already loaded
                    {
                        RefreshAvailableItems();
                    }
                    break;
                case "inventory":
                    inventoryTabButton.AddToClassList("active-tab");
                    inventoryPanel.style.display = DisplayStyle.Flex;
                    RefreshInventory();
                    break;
            }
        }

        public void SetCurrentUser(GameFuseUser user)
        {
            currentUser = user;
            if (currentUser != null)
            {
                UpdateCreditsDisplay();
                // Only refresh if we're on the available tab
                if (availableTabButton.ClassListContains("active-tab"))
                {
                    RefreshAvailableItems();
                }
            }
        }

        private void UpdateCreditsDisplay()
        {
            if (userCreditsDisplay != null && currentUser != null)
            {
                userCreditsDisplay.text = currentUser.Credits.ToString();
            }
        }

        #region Available Items

        private async void RefreshAvailableItems()
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                // Get all available store items
                var itemsList = await currentUser.GetAvailableStoreItemsAsync();
                var allItems = itemsList.ToList();
                
                // Get user's owned items to filter them out
                var userStore = await currentUser.GetUserStoreItemsAsync();
                var ownedItemIds = userStore?.StoreItems?.Select(item => item.Id).ToHashSet() ?? new HashSet<int>();
                
                // Filter out items the user already owns
                availableItems = allItems.Where(item => !ownedItemIds.Contains(item.Id)).ToList();
                
                // Extract unique categories from all items (not just available)
                ExtractCategories(allItems);
                
                // Apply filters and display
                ApplyFilters();
                
                ShowStatus("Store items refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load store items: {ex.Message}", true);
                OnError?.Invoke($"Failed to load store items: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ExtractCategories(List<StoreItem> items)
        {
            categories.Clear();
            categories.Add("All Categories");
            
            var uniqueCategories = items
                .Where(item => !string.IsNullOrEmpty(item.Category))
                .Select(item => item.Category)
                .Distinct()
                .OrderBy(cat => cat);
            
            categories.AddRange(uniqueCategories);
        }

        private void ShowCategoryMenu()
        {
            // In a real implementation, this would show a dropdown menu
            // For now, we'll cycle through categories
            int currentIndex = categories.IndexOf(currentCategory);
            currentIndex = (currentIndex + 1) % categories.Count;
            currentCategory = categories[currentIndex];
            
            if (categoryFilterButton != null)
            {
                categoryFilterButton.text = currentCategory;
            }
            
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var searchText = storeSearchInput?.value ?? "";
            
            filteredItems = availableItems.Where(item =>
            {
                // Category filter
                bool categoryMatch = currentCategory == "All Categories" || 
                                   item.Category == currentCategory;
                
                // Search filter
                bool searchMatch = string.IsNullOrWhiteSpace(searchText) ||
                                 item.Name.ToLower().Contains(searchText.ToLower()) ||
                                 (item.Description != null && item.Description.ToLower().Contains(searchText.ToLower()));
                
                return categoryMatch && searchMatch;
            }).ToList();
            
            DisplayAvailableItems();
        }

        private void DisplayAvailableItems()
        {
            // Clear existing items except empty state
            var children = availableItemsGrid.Children().ToList();
            foreach (var child in children)
            {
                if (child != availableEmptyState)
                {
                    availableItemsGrid.Remove(child);
                }
            }
            
            // Show/hide empty state
            if (filteredItems.Count == 0)
            {
                availableEmptyState.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                availableEmptyState.style.display = DisplayStyle.None;
            }
            
            // Add item cards
            foreach (var item in filteredItems)
            {
                var itemCard = CreateStoreItemCard(item, true);
                availableItemsGrid.Add(itemCard);
            }
        }

        #endregion

        #region Inventory

        private async void RefreshInventory()
        {
            if (currentUser == null) return;
            
            SetLoading(true);
            
            try
            {
                // Get the user's purchased store items from their profile
                inventoryItems = currentUser.GameUserStoreItems?.ToList() ?? new List<StoreItem>();
                DisplayInventoryItems();
                ShowStatus("Inventory refreshed", false);
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to load inventory: {ex.Message}", true);
                OnError?.Invoke($"Failed to load inventory: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void DisplayInventoryItems()
        {
            // Clear existing items except empty state
            var children = inventoryItemsGrid.Children().ToList();
            foreach (var child in children)
            {
                if (child != inventoryEmptyState)
                {
                    inventoryItemsGrid.Remove(child);
                }
            }
            
            // Update counter
            inventoryCount.text = inventoryItems.Count.ToString();
            
            // Show/hide empty state
            if (inventoryItems.Count == 0)
            {
                inventoryEmptyState.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                inventoryEmptyState.style.display = DisplayStyle.None;
            }
            
            // Add item cards
            foreach (var item in inventoryItems)
            {
                var itemCard = CreateStoreItemCard(item, false);
                inventoryItemsGrid.Add(itemCard);
            }
        }

        #endregion

        #region Item Card Creation

        private VisualElement CreateStoreItemCard(StoreItem item, bool isAvailable)
        {
            var card = new VisualElement();
            card.AddToClassList("store-item-card");
            
            // Item icon
            var icon = new VisualElement();
            icon.AddToClassList("item-icon");
            card.Add(icon);
            
            // Item info
            var info = new VisualElement();
            info.AddToClassList("item-info");
            
            var name = new Label(item.Name);
            name.AddToClassList("item-name");
            info.Add(name);
            
            if (!string.IsNullOrEmpty(item.Category))
            {
                var category = new Label(item.Category);
                category.AddToClassList("item-category");
                info.Add(category);
            }
            
            card.Add(info);
            
            // Actions section
            var actions = new VisualElement();
            actions.AddToClassList("item-actions");
            
            if (isAvailable)
            {
                // Price
                var price = new Label($"{item.Cost} Credits");
                price.AddToClassList("item-price");
                actions.Add(price);
                
                // View button
                var viewButton = new Button(() => HandleViewItem(item));
                viewButton.text = "View";
                viewButton.AddToClassList("item-button");
                viewButton.AddToClassList("primary");
                actions.Add(viewButton);
            }
            else
            {
                // Remove button for inventory items
                var removeButton = new Button(() => HandleRemoveItem(item));
                removeButton.text = "Remove";
                removeButton.AddToClassList("item-button");
                removeButton.AddToClassList("danger");
                actions.Add(removeButton);
            }
            
            card.Add(actions);
            
            return card;
        }

        #endregion

        #region Item Actions

        private void HandleViewItem(StoreItem item)
        {
            if (storePanel != null && currentUser != null)
            {
                storePanel.ShowItemDetailsPopup(item, currentUser.Credits, HandlePurchaseItem);
            }
        }

        private async void HandlePurchaseItem(StoreItem item)
        {
            if (currentUser == null || item == null) return;
            
            SetLoading(true);
            
            try
            {
                var userStore = await currentUser.PurchaseStoreItemAsync(item.Id);
                
                if (userStore != null && userStore.Credits != -1)
                {
                    // The purchase was successful - credits are already updated in the user object
                    UpdateCreditsDisplay();
                    
                    // Update inventory items
                    inventoryItems = userStore.StoreItems;
                    
                    // Notify about user data update
                    OnUserDataUpdated?.Invoke(currentUser);
                    
                    ShowStatus($"Purchased '{item.Name}' for {item.Cost} credits!", false);
                    OnSuccess?.Invoke($"Purchased '{item.Name}'!");
                    
                    // Refresh both lists
                    await Task.Delay(500); // Small delay to ensure server state is updated
                    RefreshAvailableItems();
                    RefreshInventory();
                }
                else
                {
                    ShowStatus("Purchase failed. Please try again.", true);
                    OnError?.Invoke("Purchase failed");
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to purchase item: {ex.Message}", true);
                OnError?.Invoke($"Failed to purchase item: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void HandleRemoveItem(StoreItem item)
        {
            if (currentUser == null || item == null) return;
            
            SetLoading(true);
            
            try
            {
                var userStore = await currentUser.RemoveStoreItemAsync(item.Id);
                
                if (userStore != null && userStore.Credits != -1)
                {
                    // The removal was successful - credits are already updated in the user object
                    UpdateCreditsDisplay();
                    
                    // Update inventory items
                    inventoryItems = userStore.StoreItems;
                    DisplayInventoryItems();
                    
                    // Notify about user data update
                    OnUserDataUpdated?.Invoke(currentUser);
                    
                    ShowStatus($"Removed '{item.Name}' from inventory", false);
                    OnSuccess?.Invoke($"Removed '{item.Name}' from inventory");
                }
                else
                {
                    ShowStatus("Failed to remove item. Please try again.", true);
                    OnError?.Invoke("Failed to remove item");
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to remove item: {ex.Message}", true);
                OnError?.Invoke($"Failed to remove item: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        #endregion

        private void SetLoading(bool loading)
        {
            if (loadingIndicator != null)
                loadingIndicator.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ShowStatus(string message, bool isError)
        {
            if (statusLabel == null) return;

            statusLabel.text = message;
            statusLabel.RemoveFromClassList("status-success");
            statusLabel.RemoveFromClassList("status-error");
            statusLabel.AddToClassList(isError ? "status-error" : "status-success");
            statusLabel.style.display = DisplayStyle.Flex;

            // Auto-hide after 3 seconds
            _ = Task.Delay(3000).ContinueWith(_ => {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (statusLabel != null)
                        statusLabel.style.display = DisplayStyle.None;
                });
            });
        }

        public void Reset()
        {
            currentUser = null;

            // Clear inputs
            if (storeSearchInput != null)
                storeSearchInput.value = "";
            
            // Reset category filter
            currentCategory = "All Categories";
            if (categoryFilterButton != null)
                categoryFilterButton.text = currentCategory;

            // Clear data collections
            availableItems.Clear();
            filteredItems.Clear();
            inventoryItems.Clear();
            categories.Clear();
            categories.Add("All Categories");

            // Reset UI displays
            DisplayAvailableItems();
            DisplayInventoryItems();

            // Reset to available tab
            SwitchTab("available");

            // Hide status
            if (statusLabel != null)
                statusLabel.style.display = DisplayStyle.None;

            // Clear credits display
            if (userCreditsDisplay != null)
                userCreditsDisplay.text = "0";

            SetLoading(false);
        }
    }
}